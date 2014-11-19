//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Helpers.CsvHelper;
using AdvantShop.SaasData;
using AdvantShop.Statistic;
using Resources;

namespace Admin
{
    public partial class ExportCsv : AdvantShopAdminPage
    {
        private readonly string _strFilePath;
        private string _strFullPath;
        public string NotDoPost = string.Empty;
        private Separators.SeparatorsEnum _separator;
        private Encodings.EncodingsEnum _encoding;
        protected List<ProductFields.Fields> FieldMapping = new List<ProductFields.Fields>();
        private const string StrFileName = "products";
        private const string StrFileExt = ".csv";
        protected string ExtStrFileName;

        public ExportCsv()
        {

            _strFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(_strFilePath);
            foreach (var item in Directory.GetFiles(_strFilePath).Where(f => f.Contains(StrFileName)))
            {
                if (item.Contains(StrFileName))
                {
                    _strFullPath = item;
                    ExtStrFileName = Path.GetFileName(item);
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(_strFullPath))
            {
                ExtStrFileName = (StrFileName + StrFileExt).FileNamePlusDate();
                _strFullPath = _strFilePath + ExtStrFileName;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            CommonHelper.DisableBrowserCache();
            choseDiv.Visible = !CommonStatistic.IsRun;
            divAction.Visible = !CommonStatistic.IsRun;
            divbtnAction.Visible = !CommonStatistic.IsRun;

            LoadFirstProduct();
            var tbl = new Table() { ID = "tblValues" };
            var ddlRow = new TableRow { ID = "ddlRow" };
            var lblRow = new TableRow { ID = "lblRow", BackColor = ColorTranslator.FromHtml("#EFF0F2") };
            var cellM = new TableCell { ID = "cellM" };
            cellM.Attributes.Add("style", "vertical-align:top; width:150px");
            cellM.Controls.Add(new Label { Text = Resource.Admin_ExportCsv_Column });
            ddlRow.Cells.Add(cellM);

            var cellL = new TableCell { ID = "cellL" };
            cellL.Attributes.Add("style", "vertical-align:top; width:150px");
            cellL.Controls.Add(new Label { Text = Resource.Admin_ExportCsv_SampleOfData });
            var div4 = new Panel { Width = 110 };
            cellL.Controls.Add(div4);
            lblRow.Cells.Add(cellL);

            foreach (var item in Enum.GetValues(typeof(ProductFields.Fields)))
            {
                var enumTemp = (ProductFields.Fields)item;
                var enumIntStringTemp = enumTemp.ConvertIntString();
                // none and photo in export by default no need
                if (enumTemp == ProductFields.Fields.None || enumTemp == ProductFields.Fields.Sorting) continue;
                var cell = new TableCell { ID = "cell" + enumIntStringTemp };
                cell.Attributes.Add("style", "vertical-align:top");
                var ddl = new DropDownList { ID = "ddlType" + ((int)item).ToString(), Width = 150 };
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.None), ProductFields.Fields.None.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Sku), ProductFields.Fields.Sku.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Name), ProductFields.Fields.Name.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.ParamSynonym), ProductFields.Fields.ParamSynonym.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Category), ProductFields.Fields.Category.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Enabled), ProductFields.Fields.Enabled.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Price), ProductFields.Fields.Price.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.PurchasePrice), ProductFields.Fields.PurchasePrice.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Amount), ProductFields.Fields.Amount.ConvertIntString()));

                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.MultiOffer), ProductFields.Fields.MultiOffer.ConvertIntString()));

                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Unit), ProductFields.Fields.Unit.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Discount), ProductFields.Fields.Discount.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.ShippingPrice), ProductFields.Fields.ShippingPrice.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Weight), ProductFields.Fields.Weight.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Size), ProductFields.Fields.Size.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.BriefDescription), ProductFields.Fields.BriefDescription.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Description), ProductFields.Fields.Description.ConvertIntString()));

                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Title), ProductFields.Fields.Title.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.H1), ProductFields.Fields.H1.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.MetaKeywords), ProductFields.Fields.MetaKeywords.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.MetaDescription), ProductFields.Fields.MetaDescription.ConvertIntString()));
                
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Photos), ProductFields.Fields.Photos.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Markers), ProductFields.Fields.Markers.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Properties), ProductFields.Fields.Properties.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Producer), ProductFields.Fields.Producer.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.OrderByRequest), ProductFields.Fields.OrderByRequest.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.SalesNote), ProductFields.Fields.SalesNote.ConvertIntString()));

                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Related), ProductFields.Fields.Related.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.Alternative), ProductFields.Fields.Alternative.ConvertIntString()));
                ddl.Items.Add(new ListItem(ProductFields.GetDisplayNameByEnum(ProductFields.Fields.CustomOption), ProductFields.Fields.CustomOption.ConvertIntString()));

                if (!string.IsNullOrEmpty(Request["state"]) && Request["state"].ToLower() == "select")
                    ddl.SelectedValue = enumIntStringTemp;
                var lb = new Label
                    {
                        ID = "lbProduct" + ((int)item).ToString(),
                        Text = ddlProduct.Items.Count > 0 && Request["state"] == "select"
                                   ? ddlProduct.Items.FindByValue(enumIntStringTemp) != null ? ddlProduct.Items.FindByValue(enumIntStringTemp).Text : string.Empty
                                   : string.Empty
                    };
                lb.Attributes.Add("style", "display:block");
                ddl.Attributes.Add("onchange", string.Format("Change(this)"));
                cell.Controls.Add(ddl);
                ddlRow.Cells.Add(cell);
                var cellLbl = new TableCell { ID = "cellLbl" + enumIntStringTemp };
                cellLbl.Controls.Add(lb);
                lblRow.Cells.Add(cellLbl);
            }

            tbl.Rows.Add(ddlRow);
            tbl.Rows.Add(lblRow);
            choseDiv.Controls.Add(tbl);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveExcel)
            {
                mainDiv.Visible = false;
                notInTariff.Visible = true;
            }

            //hrefAgaint.Visible = false;
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_ExportExcel_Title));

            if (!IsPostBack)
            {
                ddlEncoding.Items.Clear();
                ddlEncoding.Items.Add(new ListItem(Encodings.EncodingsEnum.Windows1251.ToString(), Encodings.EncodingsEnum.Windows1251.ConvertIntString(), true));
                ddlEncoding.Items.Add(new ListItem(Encodings.EncodingsEnum.Utf8.ToString(), Encodings.EncodingsEnum.Utf8.ConvertIntString()));
                ddlEncoding.Items.Add(new ListItem(Encodings.EncodingsEnum.Utf16.ToString(), Encodings.EncodingsEnum.Utf16.ConvertIntString()));
                ddlEncoding.Items.Add(new ListItem(Encodings.EncodingsEnum.Koi8R.ToString(), Encodings.EncodingsEnum.Koi8R.ConvertIntString()));
                ddlSeparetors.Items.Clear();
                ddlSeparetors.Items.Add(new ListItem(Resource.Admin_ImportCsv_Semicolon, Separators.SeparatorsEnum.SemicolonSeparated.ConvertIntString(), true));
                ddlSeparetors.Items.Add(new ListItem(Resource.Admin_ImportCsv_Comma, Separators.SeparatorsEnum.CommaSeparated.ConvertIntString()));
                ddlSeparetors.Items.Add(new ListItem(Resource.Admin_ImportCsv_Tab, Separators.SeparatorsEnum.TabSeparated.ConvertIntString()));
            }

            if (choseDiv.FindControl("tblValues") != null && IsPostBack)
            {
                var cells = ((TableRow)choseDiv.FindControl("ddlRow")).Cells;
                foreach (TableCell item in cells)
                {
                    var element = item.Controls.OfType<DropDownList>().FirstOrDefault();
                    if (element == null) continue;

                    if (item.Controls.OfType<DropDownList>().First().SelectedValue == ProductFields.Fields.None.ConvertIntString()) continue;

                    if (!FieldMapping.Contains((ProductFields.Fields)SQLDataHelper.GetInt(item.Controls.OfType<DropDownList>().First().SelectedValue)))
                        FieldMapping.Add((ProductFields.Fields)SQLDataHelper.GetInt((item.Controls.OfType<DropDownList>().First().SelectedValue)));//, cells.GetCellIndex(item));
                    else
                    {
                        MsgErr(string.Format(Resource.Admin_ImportCsv_DuplicateMessage, item.Controls.OfType<DropDownList>().First().SelectedItem.Text));
                        return;
                    }
                }

                if (ChbCategorySort.Checked)
                {
                    var ind = FieldMapping.IndexOf(ProductFields.Fields.Category);
                    if (ind > 0)
                        FieldMapping.Insert(ind + 1, ProductFields.Fields.Sorting);
                    else
                        FieldMapping.Add(ProductFields.Fields.Sorting);
                }
            }
            if (FieldMapping.Count == 0 && IsPostBack)
            {
                MsgErr(Resource.Admin_ExportCsv_ListEmpty);
                return;
            }
            MsgErr(true);
            OutDiv.Visible = CommonStatistic.IsRun;
            linkCancel.Visible = CommonStatistic.IsRun;
        }

        private void LoadFirstProduct()
        {
            var product = ProductService.GetFirstProduct();
            if (product == null) return;
            foreach (var item in Enum.GetValues(typeof(ProductFields.Fields)))
            {
                if ((ProductFields.Fields)item == ProductFields.Fields.None)
                    ddlProduct.Items.Add(new ListItem { Text = @"-", Value = ProductFields.Fields.None.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Sku)
                    ddlProduct.Items.Add(new ListItem { Text = product.ArtNo, Value = ProductFields.Fields.Sku.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Name)
                    ddlProduct.Items.Add(new ListItem { Text = product.Name.HtmlEncode(), Value = ProductFields.Fields.Name.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.ParamSynonym)
                    ddlProduct.Items.Add(new ListItem { Text = product.UrlPath, Value = ProductFields.Fields.ParamSynonym.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Category)
                    ddlProduct.Items.Add(new ListItem { Text = CategoryService.GetCategory(product.CategoryID).Name, Value = ProductFields.Fields.Category.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Enabled)
                    ddlProduct.Items.Add(new ListItem { Text = product.Enabled ? "+" : "-", Value = ProductFields.Fields.Enabled.ConvertIntString() });

                if (product.Offers.Count > 0)
                {
                    if ((ProductFields.Fields)item == ProductFields.Fields.Price)
                        ddlProduct.Items.Add(new ListItem { Text = product.Offers[0].Price.ToString("F2"), Value = ProductFields.Fields.Price.ConvertIntString() });

                    if ((ProductFields.Fields)item == ProductFields.Fields.PurchasePrice)
                        ddlProduct.Items.Add(new ListItem { Text = product.Offers[0].SupplyPrice.ToString("F2"), Value = ProductFields.Fields.PurchasePrice.ConvertIntString() });

                    if ((ProductFields.Fields)item == ProductFields.Fields.Amount)
                        ddlProduct.Items.Add(new ListItem { Text = product.Offers[0].Amount.ToString("F2"), Value = ProductFields.Fields.Amount.ConvertIntString() });

                    if ((ProductFields.Fields)item == ProductFields.Fields.MultiOffer)
                    {
                        ddlProduct.Items.Add(new ListItem
                            {
                                Text =
                                    product.Offers.Select(
                                        offer =>
                                        "[" + offer.ArtNo + ":" + (offer.Size != null ? offer.Size.SizeName : "null") + ":" + (offer.Color != null ? offer.Color.ColorName : "null") + ":" + offer.Price +
                                        ":" + offer.SupplyPrice + ":" + offer.Amount + "]").AggregateString(','),
                                Value = ProductFields.Fields.MultiOffer.ConvertIntString()
                            });
                    }

                }

                if ((ProductFields.Fields)item == ProductFields.Fields.Unit)
                    ddlProduct.Items.Add(new ListItem { Text = product.Unit, Value = ProductFields.Fields.Unit.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Discount)
                    ddlProduct.Items.Add(new ListItem { Text = product.Discount.ToString("F2"), Value = ProductFields.Fields.Discount.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.ShippingPrice)
                    ddlProduct.Items.Add(new ListItem { Text = product.ShippingPrice.ToString("F2"), Value = ProductFields.Fields.ShippingPrice.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Weight)
                    ddlProduct.Items.Add(new ListItem { Text = product.Weight.ToString("F2"), Value = ProductFields.Fields.Weight.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Size)
                    ddlProduct.Items.Add(new ListItem { Text = product.Size.Replace("|", " x "), Value = ProductFields.Fields.Size.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.BriefDescription)
                    ddlProduct.Items.Add(new ListItem { Text = product.BriefDescription.Reduce(20).HtmlEncode(), Value = ProductFields.Fields.BriefDescription.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Description)
                    ddlProduct.Items.Add(new ListItem { Text = product.Description.Reduce(20).HtmlEncode(), Value = ProductFields.Fields.Description.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Title)
                    ddlProduct.Items.Add(new ListItem { Text = product.Meta.Title.Reduce(20), Value = ProductFields.Fields.Title.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.H1)
                    ddlProduct.Items.Add(new ListItem { Text = product.Meta.H1.Reduce(20), Value = ProductFields.Fields.H1.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.MetaKeywords)
                    ddlProduct.Items.Add(new ListItem { Text = product.Meta.MetaKeywords.Reduce(20), Value = ProductFields.Fields.MetaKeywords.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.MetaDescription)
                    ddlProduct.Items.Add(new ListItem { Text = product.Meta.MetaDescription.Reduce(20), Value = ProductFields.Fields.MetaDescription.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Photos)
                    ddlProduct.Items.Add(new ListItem { Text = PhotoService.PhotoToString(product.ProductPhotos), Value = ProductFields.Fields.Photos.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Markers)
                    ddlProduct.Items.Add(new ListItem { Text = ProductService.MarkersToString(product), Value = ProductFields.Fields.Markers.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Properties)
                    ddlProduct.Items.Add(new ListItem { Text = PropertyService.PropertiesToString(product.ProductPropertyValues).HtmlEncode(), Value = ProductFields.Fields.Properties.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Producer)
                    ddlProduct.Items.Add(new ListItem { Text = BrandService.BrandToString(product.BrandId), Value = ProductFields.Fields.Producer.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.OrderByRequest)
                    ddlProduct.Items.Add(new ListItem { Text = product.AllowPreOrder ? "+" : "-", Value = ProductFields.Fields.OrderByRequest.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.SalesNote)
                    ddlProduct.Items.Add(new ListItem { Text = product.SalesNote, Value = ProductFields.Fields.SalesNote.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Related)
                    ddlProduct.Items.Add(new ListItem { Text = ProductService.LinkedProductToString(product.ProductId, RelatedType.Related), Value = ProductFields.Fields.Related.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.Alternative)
                    ddlProduct.Items.Add(new ListItem { Text = ProductService.LinkedProductToString(product.ProductId, RelatedType.Alternative), Value = ProductFields.Fields.Alternative.ConvertIntString() });

                if ((ProductFields.Fields)item == ProductFields.Fields.CustomOption)
                    ddlProduct.Items.Add(new ListItem { Text = CustomOptionsService.CustomOptionsToString(CustomOptionsService.GetCustomOptionsByProductId(product.ProductId)), Value = ProductFields.Fields.CustomOption.ConvertIntString() });
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            if (lError.Visible) return;
            if (CommonStatistic.IsRun) return;

            divAction.Visible = false;
            divbtnAction.Visible = false;
            choseDiv.Visible = false;
            _separator = (Separators.SeparatorsEnum)SQLDataHelper.GetInt(ddlSeparetors.SelectedValue);
            _encoding = (Encodings.EncodingsEnum)SQLDataHelper.GetInt(ddlEncoding.SelectedValue);

            CommonStatistic.Init();
            CommonStatistic.IsRun = true;
            CommonStatistic.CurrentProcess = Request.Url.PathAndQuery;
            CommonStatistic.CurrentProcessName = Resource.Admin_ExportExcel_CatalogDownload;
            linkCancel.Visible = true;
            OutDiv.Visible = true;
            btnDownload.Visible = false;
            try
            {
                // Directory
                foreach (var file in Directory.GetFiles(_strFilePath).Where(f => f.Contains(StrFileName)).ToList())
                {
                    FileHelpers.DeleteFile(file);
                }

                ExtStrFileName = (StrFileName + StrFileExt).FileNamePlusDate();
                _strFullPath = _strFilePath + ExtStrFileName;
                FileHelpers.CreateDirectory(_strFilePath);
                CommonStatistic.TotalRow = ProductService.GetProductsCount();
                CommonStatistic.ThreadImport = new Thread(Save) { IsBackground = true };
                CommonStatistic.ThreadImport.Start();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                MsgErr(ex.Message);
            }

            //Trial.TrackEvent(Trial.TrialEvents.MakeCSVExport, "");
        }

        protected void Save()
        {
            CsvExport.SaveProductsToCsv(_strFullPath, _encoding, _separator, FieldMapping);
            CommonStatistic.IsRun = false;
        }

        protected void linkCancel_Click(object sender, EventArgs e)
        {
            if (!CommonStatistic.ThreadImport.IsAlive) return;
            CommonStatistic.IsRun = false;
            CommonStatistic.Init();
            //hrefAgaint.Visible = true;
            linkCancel.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //divSomeMessage.Visible = !hrefAgaint.Visible;
            divSomeMessage.Visible = !CommonStatistic.IsRun;

            if (CommonStatistic.IsRun)
            {
                ltLink.Text = string.Empty;
                return;
            }
            if (File.Exists(_strFullPath))
            {
                var f = new FileInfo(_strFullPath);
                const double size = 0;
                double sizeM = (double)f.Length / 1048576; //1024 * 1024

                string sizeMesage;
                if ((int)sizeM > 0)
                {
                    sizeMesage = ((int)sizeM) + " MB";
                }
                else
                {
                    double sizeK = (double)f.Length / 1024;
                    if ((int)sizeK > 0)
                    {
                        sizeMesage = ((int)sizeK) + " KB";
                    }
                    else
                    {
                        sizeMesage = ((int)size) + " B";
                    }
                }

                var temp = @"<a href='" + UrlService.GetAbsoluteLink("price_temp/" + ExtStrFileName) + @"' {0}>" +
                           Resource.Admin_ExportExcel_DownloadFile + @"</a>";
                //hrefLink.Text = string.Format(temp, "style='color: white; text-decoration: none;'");

                //spanMessage.Text
                var t = @"<span> " + Resource.Admin_ExportExcel_FileSize + @": " + sizeMesage + @"</span>" + @"<span>, " + AdvantShop.Localization.Culture.ConvertDate(File.GetLastWriteTime(_strFullPath)) + @"</span>";
                ltLink.Text = string.Format(temp, "") + t;
            }
            else
            {
                //hrefLink.Text = "#";
                //spanMessage.Text = @"<span>" + Resource.Admin_ExportExcel_NotExistDownloadFile + @"</span>";
                ltLink.Text = @"<span>" + Resource.Admin_ExportExcel_NotExistDownloadFile + @"</span>";
            }
        }

        private void MsgErr(bool clean)
        {
            if (clean)
            {
                lError.Visible = false;
                lError.Text = string.Empty;
            }
            else
            {
                lError.Visible = false;
            }
        }

        private void MsgErr(string messageText)
        {
            lError.Visible = true;
            lError.Text = @"<br/>" + messageText;
        }
    }
}