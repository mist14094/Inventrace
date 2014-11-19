//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.Caching;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.FullSearch;
using AdvantShop.Helpers;
using AdvantShop.Helpers.CsvHelper;
using AdvantShop.SaasData;
using AdvantShop.Statistic;
using CsvHelper;
using Resources;

namespace Admin
{
    public partial class ImportCSV : AdvantShopAdminPage
    {
        private readonly string _filePath;
        private readonly string _fullPath;
        private bool _hasHeadrs;
        private Dictionary<string, int> FieldMapping = new Dictionary<string, int>();
        private readonly List<ProductFields.Fields> _mustRequiredFfield;
		private const string StrFileName = "importCSV";
        private const string StrFileExt = ".csv";

        protected ImportCSV()
        {
            _filePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(_filePath);
			_mustRequiredFfield = new List<ProductFields.Fields>();

            foreach (var item in Directory.GetFiles(_filePath).Where(f => f.Contains(StrFileName)).OrderByDescending(x => x).Where(item => item.Contains(StrFileName)))
            {
                _fullPath = item;
                break;
            }

            if (!string.IsNullOrWhiteSpace(_fullPath)) return;

            var extStrFileName = (StrFileName + StrFileExt).FileNamePlusDate();
            _fullPath = _filePath + extStrFileName;
        }

        private void MsgErr(bool clean)
        {
            if (clean)
            {
                lblError.Visible = false;
                lblError.Text = string.Empty;
            }
            else
            {
                lblError.Visible = false;
            }
        }

        private void MsgErr(string messageText)
        {
            lblError.Visible = true;
            lblError.Text = @"<br/>" + messageText;
        }

        protected void btnAction_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(lblError.Text))
                return;

            if (!FieldMapping.ContainsKey(ProductFields.GetStringNameByEnum(ProductFields.Fields.Sku)) && !FieldMapping.ContainsKey(ProductFields.GetStringNameByEnum(ProductFields.Fields.Name)))
            {
                MsgErr(Resource.Admin_ImportCsv_SelectNameOrSKU);
                return;
            }

            divAction.Visible = false;
            choseDiv.Visible = false;
            if (!File.Exists(_fullPath)) return;
            try
            {
                if (CommonStatistic.IsRun) return;
                _hasHeadrs = Request["hasheadrs"] == "true";
                CommonStatistic.Init();
                CommonStatistic.IsRun = true;
                CommonStatistic.CurrentProcess = Request.Url.PathAndQuery;
                CommonStatistic.CurrentProcessName = Resource.Admin_ImportXLS_CatalogUpload;
                linkCancel.Visible = true;
                MsgErr(true);
                lblRes.Text = string.Empty;
                CommonStatistic.ThreadImport = new Thread(Process) { IsBackground = true };
                CommonStatistic.ThreadImport.Start();
                OutDiv.Visible = true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

        }

        private static void LogInvalidData(string message)
        {
            CommonStatistic.WriteLog(message);
            CommonStatistic.TotalErrorRow++;
            CommonStatistic.RowPosition++;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            MsgErr(true);
            divStart.Visible = !CommonStatistic.IsRun && (string.IsNullOrEmpty(Request["action"]));
            divAction.Visible = !CommonStatistic.IsRun && (Request["action"] == "start");

            choseDiv.Visible = !CommonStatistic.IsRun;

            OutDiv.Visible = CommonStatistic.IsRun;
            linkCancel.Visible = CommonStatistic.IsRun;

            if (CommonStatistic.IsRun || (Request["action"] != "start")) return;
            if (!File.Exists(_fullPath)) return;

            var tbl = new Table() { ID = "tblValues" };
            var namesRow = new TableRow { ID = "namesRow", BackColor = System.Drawing.ColorTranslator.FromHtml("#0D76B8") };
            var firstValRow = new TableRow { ID = "firstValsRow" };
            var ddlRow = new TableRow { ID = "ddlRow" };

            var firstCell = new TableCell { Width = 200, BackColor = System.Drawing.Color.White, };
            firstCell.Controls.Add(new Label { Text = Resource.Admin_ImportCsv_Column, CssClass = "firstColumn" });
            var div1 = new Panel { CssClass = "arrow_left_bg" };
            div1.Controls.Add(new Panel { CssClass = "arrow_right_bg" });
            firstCell.Controls.Add(div1);


            var secondCell = new TableCell { Width = 200 };
            secondCell.Controls.Add(new Label { Text = Resource.Admin_ImportCsv_FistLineInTheFile, CssClass = "firstColumn" });
            var div2 = new Panel { CssClass = "arrow_left_bg_two" };
            div2.Controls.Add(new Panel { CssClass = "arrow_right_bg" });
            secondCell.Controls.Add(div2);

            var firdCell = new TableCell { Width = 200 };
            firdCell.Controls.Add(new Label { Text = Resource.Admin_ImportCsv_DataType, CssClass = "firstColumn" });
            var div3 = new Panel { CssClass = "arrow_left_bg_free" };
            div3.Controls.Add(new Panel { CssClass = "arrow_right_bg" });
            firdCell.Controls.Add(div3);
            var div4 = new Panel { Width = 200 };
            firdCell.Controls.Add(div4);

            namesRow.Cells.Add(firstCell);
            firstValRow.Cells.Add(secondCell);
            ddlRow.Cells.Add(firdCell);

            _hasHeadrs = chbHasHeadrs.Checked;
            using (var csv = new CsvReader(new StreamReader(_fullPath, Encodings.GetEncoding())))
            {
                csv.Configuration.Delimiter = Separators.GetCharSeparator();
                csv.Configuration.BufferSize = 0x1000;
                csv.Configuration.HasHeaderRecord = false;

                csv.Read();

                if (_hasHeadrs && csv.CurrentRecord.HasDuplicates())
                {
                    var strFileds = string.Empty;
                    foreach (var item in csv.CurrentRecord.Duplicates())
                    {
                        strFileds += "\"" + item + "\",";
                    }
                    MsgErr(Resource.Admin_ImportCsv_DuplicateHeader + strFileds.Trim(','));
                    btnAction.Visible = false;
                }

                for (int i = 0; i < csv.CurrentRecord.Length; i++)
                {
                    var cell = new TableCell { ID = "cell" + i.ToString() };
                    var lb = new Label();
                    bool flagMustReqField = false;
                    if (Request["hasheadrs"].ToLower() == "true")
                    {
                        var tempCsv = (csv[i].Length > 50 ? csv[i].Substring(0, 49) : csv[i]).Replace("*", "");
                        if (_mustRequiredFfield.Any(item => ProductFields.GetStringNameByEnum(item).Replace("*", "") == tempCsv.ToLower()))
                        {
                            flagMustReqField = true;
                        }
                        lb.Text = tempCsv;
                    }
                    else
                    {
                        lb.Text = Resource.Admin_ImportCsv_Empty;
                    }
                    lb.ForeColor = System.Drawing.Color.White;
                    cell.Controls.Add(lb);

                    if (flagMustReqField)
                    {
                        var lbl = new Label
                            {
                                Text = @"*",
                                ForeColor = System.Drawing.Color.Red
                            };
                        cell.Controls.Add(lbl);
                    }

                    namesRow.Cells.Add(cell);

                    cell = new TableCell() { Width = 150 };
                    var ddl = new DropDownList { ID = "ddlType" + i.ToString(), Width = 150 };
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.None), ProductFields.GetStringNameByEnum(ProductFields.Fields.None)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Sku), ProductFields.GetStringNameByEnum(ProductFields.Fields.Sku)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Name), ProductFields.GetStringNameByEnum(ProductFields.Fields.Name).Trim('*')));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.ParamSynonym), ProductFields.GetStringNameByEnum(ProductFields.Fields.ParamSynonym)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Category), ProductFields.GetStringNameByEnum(ProductFields.Fields.Category).Trim('*')));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Enabled), ProductFields.GetStringNameByEnum(ProductFields.Fields.Enabled).Trim('*')));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Price), ProductFields.GetStringNameByEnum(ProductFields.Fields.Price).Trim('*')));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.PurchasePrice), ProductFields.GetStringNameByEnum(ProductFields.Fields.PurchasePrice)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Amount), ProductFields.GetStringNameByEnum(ProductFields.Fields.Amount)));

                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.MultiOffer), ProductFields.GetStringNameByEnum(ProductFields.Fields.MultiOffer)));

                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Unit), ProductFields.GetStringNameByEnum(ProductFields.Fields.Unit)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Discount), ProductFields.GetStringNameByEnum(ProductFields.Fields.Discount)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.ShippingPrice), ProductFields.GetStringNameByEnum(ProductFields.Fields.ShippingPrice)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Weight), ProductFields.GetStringNameByEnum(ProductFields.Fields.Weight)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Size), ProductFields.GetStringNameByEnum(ProductFields.Fields.Size)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.BriefDescription), ProductFields.GetStringNameByEnum(ProductFields.Fields.BriefDescription)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Description), ProductFields.GetStringNameByEnum(ProductFields.Fields.Description)));

                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Title), ProductFields.GetStringNameByEnum(ProductFields.Fields.Title)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.H1), ProductFields.GetStringNameByEnum(ProductFields.Fields.H1)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.MetaKeywords), ProductFields.GetStringNameByEnum(ProductFields.Fields.MetaKeywords)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.MetaDescription), ProductFields.GetStringNameByEnum(ProductFields.Fields.MetaDescription)));
                    

                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Photos), ProductFields.GetStringNameByEnum(ProductFields.Fields.Photos)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Markers), ProductFields.GetStringNameByEnum(ProductFields.Fields.Markers)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Properties), ProductFields.GetStringNameByEnum(ProductFields.Fields.Properties)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Producer), ProductFields.GetStringNameByEnum(ProductFields.Fields.Producer)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.OrderByRequest), ProductFields.GetStringNameByEnum(ProductFields.Fields.OrderByRequest)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.SalesNote), ProductFields.GetStringNameByEnum(ProductFields.Fields.SalesNote)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Sorting), ProductFields.GetStringNameByEnum(ProductFields.Fields.Sorting)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Related), ProductFields.GetStringNameByEnum(ProductFields.Fields.Related)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Alternative), ProductFields.GetStringNameByEnum(ProductFields.Fields.Alternative)));
                    ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.CustomOption), ProductFields.GetStringNameByEnum(ProductFields.Fields.CustomOption)));

                    ddl.SelectedValue = lb.Text.Replace("*", "").Trim().ToLower();
                    cell.Controls.Add(ddl);
                    ddlRow.Cells.Add(cell);
                }

                csv.Read();
                if (csv.CurrentRecord != null)
                    for (int i = 0; i < csv.CurrentRecord.Length; i++)
                    {
                        var cell = new TableCell();
                        if (csv[i] == null)
                            cell.Controls.Add(new Label { Text = string.Empty });
                        else
                            cell.Controls.Add(new Label { Text = csv[i].Length > 50 ? csv[i].Substring(0, 49).HtmlEncode() : csv[i].HtmlEncode() });
                        firstValRow.Cells.Add(cell);
                    }
            }
            tbl.Rows.Add(namesRow);
            tbl.Rows.Add(firstValRow);
            tbl.Rows.Add(ddlRow);
            choseDiv.Controls.Add(tbl);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_ImportXLS_Title));

            if ((SaasDataService.IsSaasEnabled) && (!SaasDataService.CurrentSaasData.HaveExcel))
            {
                mainDiv.Visible = false;
                notInTariff.Visible = true;
            }

            if (!IsPostBack)
            {
                if (divStart.Visible)
                {
                    ddlEncoding.Items.Clear();
                    ddlEncoding.Items.Add(new ListItem(Encodings.EncodingsEnum.Windows1251.ToString(), Encodings.EncodingsEnum.Windows1251.ConvertIntString(), true));
                    ddlEncoding.Items.Add(new ListItem(Encodings.EncodingsEnum.Utf8.ToString(), Encodings.EncodingsEnum.Utf8.ConvertIntString()));
                    ddlEncoding.Items.Add(new ListItem(Encodings.EncodingsEnum.Utf16.ToString(), Encodings.EncodingsEnum.Utf16.ConvertIntString()));
                    ddlEncoding.Items.Add(new ListItem(Encodings.EncodingsEnum.Koi8R.ToString(), Encodings.EncodingsEnum.Koi8R.ConvertIntString()));
                    ddlEncoding.SelectedValue = Encodings.CsvEnconing.ConvertIntString();

                    ddlSeparetors.Items.Clear();
                    ddlSeparetors.Items.Add(new ListItem(Resource.Admin_ImportCsv_Semicolon, Separators.SeparatorsEnum.SemicolonSeparated.ConvertIntString(), true));
                    ddlSeparetors.Items.Add(new ListItem(Resource.Admin_ImportCsv_Comma, Separators.SeparatorsEnum.CommaSeparated.ConvertIntString()));
                    ddlSeparetors.Items.Add(new ListItem(Resource.Admin_ImportCsv_Tab, Separators.SeparatorsEnum.TabSeparated.ConvertIntString()));
                    ddlSeparetors.SelectedValue = Separators.CsvSeparator.ConvertIntString();
                }
            }

            if (choseDiv.FindControl("tblValues") != null && IsPostBack)
            {
                short index = 0;
                var cells = ((TableRow)choseDiv.FindControl("ddlRow")).Cells;
                foreach (TableCell item in cells)
                {
                    var element = item.Controls.OfType<DropDownList>().FirstOrDefault();
                    if (element == null) continue;

                    if (element.SelectedValue != ProductFields.GetStringNameByEnum(ProductFields.Fields.None))
                    {
                        if (!FieldMapping.ContainsKey(element.SelectedValue))
                            FieldMapping.Add(element.SelectedValue, index);
                        else
                        {
                            MsgErr(string.Format(Resource.Admin_ImportCsv_DuplicateMessage, element.SelectedItem.Text));//  "Duplicate field");
                            return;
                        }
                    }
                    index++;
                }
            }

            if (!btnAction.Visible || !IsPostBack) return;

            foreach (var item in _mustRequiredFfield)
                if (!FieldMapping.ContainsKey(ProductFields.GetStringNameByEnum(item).Trim('*')))
                {
                    MsgErr(string.Format(Resource.Admin_ImportCsv_NotChoice, ProductFields.GetDisplayNameByEnum(item)));
                    return;
                }
            //MsgErr(true);
        }

        private void Process()
        {
            if (chboxDisableProducts.Checked)
            {
                ProductService.DisableAllProducts();
            }

            CommonStatistic.TotalRow = GetRowCount();

            var somePostProcessing = FieldMapping.ContainsKey(ProductFields.GetStringNameByEnum(ProductFields.Fields.Related)) ||
                                     FieldMapping.ContainsKey(ProductFields.GetStringNameByEnum(ProductFields.Fields.Alternative));

            if (somePostProcessing)
            {
                CommonStatistic.TotalRow *= 2;
            }

            ProcessRow(false);
            if (somePostProcessing && CommonStatistic.IsRun)
                ProcessRow(true);

            CategoryService.RecalculateProductsCountManual();
            CategoryService.SetCategoryHierarchicallyEnabled(0);
            CommonStatistic.IsRun = false;
            LuceneSearch.CreateAllIndexInBackground();
            CacheManager.Clean();
            FileHelpers.DeleteFilesFromImageTempInBackground();
            FileHelpers.DeleteFile(_fullPath);
        }

        private long GetRowCount()
        {
            long count = 0;
            using (var csv = new CsvReader(new StreamReader(_fullPath, Encodings.GetEncoding())))
            {
                csv.Configuration.Delimiter = Separators.GetCharSeparator();
                csv.Configuration.HasHeaderRecord = _hasHeadrs;
                while (csv.Read())
                    count++;
            }
            return count;
        }

        private void ProcessRow(bool onlyPostProcess)
        {
            if (!File.Exists(_fullPath)) return;
            using (var csv = new CsvReader(new StreamReader(_fullPath, Encodings.GetEncoding())))
            {
                csv.Configuration.Delimiter = Separators.GetCharSeparator();
                csv.Configuration.HasHeaderRecord = _hasHeadrs;

                while (csv.Read())
                {
                    if (!CommonStatistic.IsRun)
                    {
                        csv.Dispose();
                        FileHelpers.DeleteFile(_fullPath);
                        return;
                    }
                    try
                    {
                        var productInStrings = PrepareRow(csv);
                        if (productInStrings == null) continue;

                        if (!onlyPostProcess)
                            ImportProduct.UpdateInsertProduct(productInStrings);
                        else
                            ImportProduct.PostProcess(productInStrings);

                    }
                    catch (Exception ex)
                    {
                        MsgErr(ex.Message + " at csv");
                        Debug.LogError(ex);
                    }
                }
            }
        }

        private Dictionary<ProductFields.Fields, string> PrepareRow(CsvReader csv)
        {
            // Step by rows
            var productInStrings = new Dictionary<ProductFields.Fields, string>();

            string nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Sku);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.Sku, SQLDataHelper.GetString(csv[FieldMapping[nameField]]));
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Name).Trim('*');
            if (FieldMapping.ContainsKey(nameField))
            {
                var name = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                if (!string.IsNullOrEmpty(name))
                {
                    productInStrings.Add(ProductFields.Fields.Name, name);
                }
                else
                {
                    LogInvalidData(string.Format(Resource.Admin_ImportCsv_CanNotEmpty, ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Name), CommonStatistic.RowPosition + 2));
                    return null;
                }
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Enabled).Trim('*');
            if (FieldMapping.ContainsKey(nameField))
            {
                string enabled = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                productInStrings.Add(ProductFields.Fields.Enabled, enabled);
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Discount);
            if (FieldMapping.ContainsKey(nameField))
            {
                var discount = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                if (string.IsNullOrEmpty(discount))
                    discount = "0";
                float tmp;
                if (float.TryParse(discount, out  tmp))
                {
                    productInStrings.Add(ProductFields.Fields.Discount, tmp.ToString());
                }
                else if (float.TryParse(discount, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp))
                {
                    productInStrings.Add(ProductFields.Fields.Discount, tmp.ToString());
                }
                else
                {
                    LogInvalidData(string.Format(Resource.Admin_ImportCsv_MustBeNumber, ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Discount), CommonStatistic.RowPosition + 2));
                    return null;
                }
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Weight);
            if (FieldMapping.ContainsKey(nameField))
            {
                var weight = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                if (string.IsNullOrEmpty(weight))
                    weight = "0";
                float tmp;
                if (float.TryParse(weight, out  tmp))
                {
                    productInStrings.Add(ProductFields.Fields.Weight, tmp.ToString());
                }
                else if (float.TryParse(weight, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp))
                {
                    productInStrings.Add(ProductFields.Fields.Weight, tmp.ToString());
                }
                else
                {
                    LogInvalidData(string.Format(Resource.Admin_ImportCsv_MustBeNumber, ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Weight), CommonStatistic.RowPosition + 2));
                    return null;
                }
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Size);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.Size, SQLDataHelper.GetString(csv[FieldMapping[nameField]]));
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.BriefDescription);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.BriefDescription, SQLDataHelper.GetString(csv[FieldMapping[nameField]]));
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Description);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.Description, SQLDataHelper.GetString(csv[FieldMapping[nameField]]));
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.SalesNote);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.SalesNote, Convert.ToString(csv[FieldMapping[nameField]]));
            }



            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.MultiOffer);
            if (FieldMapping.ContainsKey(nameField))
            {
                var multiOffer = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                if (multiOffer.IsNotEmpty())
                {
                    productInStrings.Add(ProductFields.Fields.MultiOffer, multiOffer);
                }
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Price).Trim('*');
            if (FieldMapping.ContainsKey(nameField))
            {
                var price = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                if (string.IsNullOrEmpty(price))
                    price = "0";
                float tmp;
                if (float.TryParse(price, out  tmp))
                {
                    productInStrings.Add(ProductFields.Fields.Price, tmp.ToString());
                }
                else if (float.TryParse(price, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp))
                {
                    productInStrings.Add(ProductFields.Fields.Price, tmp.ToString());
                }
                else
                {
                    LogInvalidData(string.Format(Resource.Admin_ImportCsv_MustBeNumber, ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Price), CommonStatistic.RowPosition + 2));
                    return null;
                }
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.PurchasePrice);
            if (FieldMapping.ContainsKey(nameField))
            {
                var sypplyprice = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                if (string.IsNullOrEmpty(sypplyprice))
                    sypplyprice = "0";
                float tmp;
                if (float.TryParse(sypplyprice, out  tmp))
                {
                    productInStrings.Add(ProductFields.Fields.PurchasePrice, tmp.ToString());
                }
                else if (float.TryParse(sypplyprice, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp))
                {
                    productInStrings.Add(ProductFields.Fields.PurchasePrice, tmp.ToString());
                }
                else
                {
                    LogInvalidData(string.Format(Resource.Admin_ImportCsv_MustBeNumber, ProductFields.GetDisplayNameByEnum(ProductFields.Fields.PurchasePrice), CommonStatistic.RowPosition + 2));
                    return null;
                }
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Amount);
            if (FieldMapping.ContainsKey(nameField))
            {
                var amount = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                if (string.IsNullOrEmpty(amount))
                    amount = "0";
                int tmp;
                if (int.TryParse(amount, out  tmp))
                {
                    productInStrings.Add(ProductFields.Fields.Amount, amount);
                }
                else
                {
                    LogInvalidData(string.Format(Resource.Admin_ImportCsv_MustBeNumber, ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Amount), CommonStatistic.RowPosition + 2));
                    return null;
                }
            }


            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.ShippingPrice);
            if (FieldMapping.ContainsKey(nameField))
            {
                var shippingPrice = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                if (string.IsNullOrEmpty(shippingPrice))
                    shippingPrice = "0";
                float tmp;
                if (float.TryParse(shippingPrice, out  tmp))
                {
                    productInStrings.Add(ProductFields.Fields.ShippingPrice, tmp.ToString());
                }
                else if (float.TryParse(shippingPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp))
                {
                    productInStrings.Add(ProductFields.Fields.ShippingPrice, tmp.ToString());
                }
                else
                {
                    LogInvalidData(string.Format(Resource.Admin_ImportCsv_MustBeNumber, ProductFields.GetDisplayNameByEnum(ProductFields.Fields.ShippingPrice), CommonStatistic.RowPosition + 2));
                    return null;
                }
            }


            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Unit);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.Unit, SQLDataHelper.GetString(csv[FieldMapping[nameField]]));
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.ParamSynonym);
            if (FieldMapping.ContainsKey(nameField))
            {
                string rewurl = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                productInStrings.Add(ProductFields.Fields.ParamSynonym, rewurl);
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Title);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.Title, SQLDataHelper.GetString(csv[FieldMapping[nameField]]));
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.H1);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.H1, SQLDataHelper.GetString(csv[FieldMapping[nameField]]));
            }


            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.MetaKeywords);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.MetaKeywords, SQLDataHelper.GetString(csv[FieldMapping[nameField]]));
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.MetaDescription);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.MetaDescription, SQLDataHelper.GetString(csv[FieldMapping[nameField]]));
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Photos);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.Photos, SQLDataHelper.GetString(csv[FieldMapping[nameField]]));
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Markers);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.Markers, SQLDataHelper.GetString(csv[FieldMapping[nameField]]));
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Properties);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.Properties, SQLDataHelper.GetString(csv[FieldMapping[nameField]]));
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Producer);
            if (FieldMapping.ContainsKey(nameField))
            {
                productInStrings.Add(ProductFields.Fields.Producer, SQLDataHelper.GetString(csv[FieldMapping[nameField]]));
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.OrderByRequest);
            if (FieldMapping.ContainsKey(nameField))
            {
                string orderbyrequest = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                productInStrings.Add(ProductFields.Fields.OrderByRequest, orderbyrequest);
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Category).Trim('*');
            if (FieldMapping.ContainsKey(nameField))
            {
                var parentCategory = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                if (!string.IsNullOrEmpty(parentCategory))
                {
                    productInStrings.Add(ProductFields.Fields.Category, parentCategory);
                }
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Sorting);
            if (FieldMapping.ContainsKey(nameField))
            {
                string sorting = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                productInStrings.Add(ProductFields.Fields.Sorting, sorting);
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Related);
            if (FieldMapping.ContainsKey(nameField))
            {
                string sorting = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                productInStrings.Add(ProductFields.Fields.Related, sorting);
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.Alternative);
            if (FieldMapping.ContainsKey(nameField))
            {
                string sorting = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                productInStrings.Add(ProductFields.Fields.Alternative, sorting);
            }

            nameField = ProductFields.GetStringNameByEnum(ProductFields.Fields.CustomOption);
            if (FieldMapping.ContainsKey(nameField))
            {
                string customOption = SQLDataHelper.GetString(csv[FieldMapping[nameField]]);
                productInStrings.Add(ProductFields.Fields.CustomOption, customOption);
            }

            return productInStrings;
        }

        protected void linkCancel_Click(object sender, EventArgs e)
        {
            if (!CommonStatistic.ThreadImport.IsAlive) return;
            CommonStatistic.IsRun = false;
            hlDownloadImportLog.Attributes.CssStyle["display"] = "inline";
        }

        protected void btnSaveSettings_Click(object sender, EventArgs e)
        {
            if (!FileUpload.HasFile) return;
            var temp = _filePath + (StrFileName + StrFileExt).FileNamePlusDate();
            FileUpload.SaveAs(temp);
            if (!File.Exists(temp)) return;

            Encodings.CsvEnconing = (Encodings.EncodingsEnum)SQLDataHelper.GetInt(ddlEncoding.SelectedValue);
            Separators.CsvSeparator = (Separators.SeparatorsEnum)SQLDataHelper.GetInt(ddlSeparetors.SelectedValue);
            Response.Redirect("ImportCSV.aspx?action=start&hasheadrs=" + chbHasHeadrs.Checked.ToString().ToLower());
        }
    }
}