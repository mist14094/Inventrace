//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;

namespace AdvantShop.Shipping
{
    public class Edost : IShippingMethod
    {
        private const string Url = "http://www.edost.ru/edost_calc_kln.php";
        public string ShopId { get; set; }
        public string Password { get; set; }
        public bool Insurance { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public float TotalPrice { get; set; }
        public string CityTo { get; set; }
        public string Zip { get; set; }
        public float Rate { get; set; }

        public Edost(Dictionary<string, string> parameters)
        {
            ShopId = parameters.ElementOrDefault(EdostTemplate.ShopId);
            Password = parameters.ElementOrDefault(EdostTemplate.Password);
            Rate = parameters.ElementOrDefault(EdostTemplate.Rate).TryParseFloat();
        }

        /// <summary>
        /// Don't use this for Edost
        /// </summary>
        /// <returns></returns>
        public float GetRate()
        {
            throw new Exception("GetRate method isnot avalible for Edost");
        }

        public List<ShippingOption> GetShippingOptions()
        {
            try
            {
                ServicePointManager.Expect100Continue = false;
                var request = WebRequest.Create(Url);
                request.Method = "POST";
                string postData = GetParam();
                byte[] byteArray = Encoding.GetEncoding("windows-1251").GetBytes(postData);

                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/x-www-form-urlencoded";
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;
                // Get the request stream.
                using (Stream dataStream = request.GetRequestStream())
                {
                    // Write the data to the request stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Close();
                }
                using (var response = request.GetResponse())
                {
                    // Get the stream containing all content returned by the requested server.
                    using (var dataStream = response.GetResponseStream())
                    {
                        if (dataStream == null) return new List<ShippingOption>();
                        // Open the stream using a StreamReader for easy access.
                        using (var reader = new StreamReader(dataStream))
                        {
                            // Read the content fully up to the end.
                            string responseFromServer = reader.ReadToEnd();
                            return ParseAnswer(responseFromServer);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return new List<ShippingOption>();
            }
        }

        private string GetParam()
        {
            float len = ShoppingCart.Sum(item => item.Amount * item.Offer.Product.Size.Split('|').Count() > 0 ? item.Offer.Product.Size.Split('|')[0].TryParseFloat() * item.Amount : 0);
            float width = ShoppingCart.Sum(item => item.Amount * item.Offer.Product.Size.Split('|').Count() > 1 ? item.Offer.Product.Size.Split('|')[1].TryParseFloat() * item.Amount : 0);
            float height = ShoppingCart.Sum(item => item.Amount * item.Offer.Product.Size.Split('|').Count() > 2 ? item.Offer.Product.Size.Split('|')[2].TryParseFloat() * item.Amount : 0);

            var a = new StringBuilder();
            a.Append("to_city=" + CityTo);
            a.Append("&zip=" + Zip);
            a.Append("&weight=" + ShoppingCart.TotalShippingWeight.ToString("F3"));
            a.Append("&strah=" + (Rate != 0 ? TotalPrice / Rate : TotalPrice).ToString("F2"));
            a.Append("&id=" + ShopId);
            a.Append("&p=" + Password);
            a.Append("&ln=" + (len > 0 ? len: 10));
            a.Append("&wd=" + (width > 0 ? width : 10));
            a.Append("&hg=" + (height > 0 ? height : 10));
            return a.ToString();
        }

        private List<ShippingOption> ParseAnswer(string responseFromServer)
        {
            var shippingOptions = new List<ShippingOption>();
            if (String.IsNullOrEmpty(responseFromServer))
                return shippingOptions;

            using (var sr = new StringReader(responseFromServer))
            using (var reader = new XmlTextReader(sr))
            {
                bool flagStartTagStat = false;
                bool flagStartTagTarif = false;
                bool flagStartTagPrice = false;
                bool flagStartTagPriceCash = false;
                bool flagStartTagTransfer = false;
                bool flagStartTagDay = false;
                bool flagStartTagCompany = false;
                bool flagStartTagName = false;
                bool flagStartTagPickpointmap = false;


                string price = string.Empty;
                string priceCash = string.Empty;
                string transfer = string.Empty;
                string company = string.Empty;
                string name = string.Empty;
                string day = string.Empty;
                string pickpointmap = string.Empty;


                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "stat")
                                flagStartTagStat = true;

                            if (reader.Name == "tarif")
                                flagStartTagTarif = true;

                            if (reader.Name == "price")
                                flagStartTagPrice = true;

                            if (reader.Name == "pricecash")
                                flagStartTagPriceCash = true;

                            if (reader.Name == "transfer")
                                flagStartTagTransfer = true;
                            if (reader.Name == "pickpointmap")
                                flagStartTagPickpointmap = true;

                            if (reader.Name == "day")
                                flagStartTagDay = true;

                            if (reader.Name == "company")
                                flagStartTagCompany = true;

                            if (reader.Name == "name")
                                flagStartTagName = true;
                            break;
                        case XmlNodeType.Text:
                            if (flagStartTagStat)
                            {
                                flagStartTagStat = false;
                                if (reader.Value != "1")
                                {
                                    GetErrorEdost(reader.Value);
                                    return shippingOptions;
                                }
                            }
                            if (flagStartTagTarif && flagStartTagPrice)
                                price = reader.Value;

                            if (flagStartTagTarif && flagStartTagPriceCash)
                                priceCash = reader.Value;

                            if (flagStartTagTarif && flagStartTagTransfer)
                                transfer = reader.Value;

                            if (flagStartTagTarif && flagStartTagPickpointmap)
                                pickpointmap = reader.Value;

                            if (flagStartTagTarif && flagStartTagDay)
                                day = reader.Value;

                            if (flagStartTagTarif && flagStartTagCompany)
                                company = reader.Value;

                            if (flagStartTagTarif && flagStartTagName)
                                name = reader.Value;

                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "stat")
                                flagStartTagStat = false;

                            if (reader.Name == "tarif")
                            {
                                flagStartTagTarif = false;
                                float rate = price.Replace(".", ",").TryParseFloat() * Rate;
                                float cash = priceCash.Replace(".", ",").TryParseFloat() * Rate;
                                float cashTransfer = transfer.Replace(".", ",").TryParseFloat() * Rate;

                                var shippingOption = new ShippingOption
                                             {
                                                 Name = company + (string.IsNullOrWhiteSpace(name) ? string.Empty : " (" + name + ")"),
                                                 Rate = rate,
                                                 DeliveryTime = day,
                                                 // extension ��������� ������ ��� ��������
                                                 Extend = priceCash.IsNotEmpty() || transfer.IsNotEmpty() || pickpointmap.IsNotEmpty() ? GetExtended(rate, cash, cashTransfer, pickpointmap) : null
                                             };
                                if (String.IsNullOrEmpty(pickpointmap))
                                    shippingOptions.Add(shippingOption);
                                else
                                    shippingOptions.Insert(0, shippingOption);

                                price = string.Empty;
                                priceCash = string.Empty;
                                transfer = string.Empty;
                                company = string.Empty;
                                name = string.Empty;
                                day = string.Empty;
                                pickpointmap = string.Empty;
                            }

                            if (reader.Name == "price")
                                flagStartTagPrice = false;

                            if (reader.Name == "pricecash")
                                flagStartTagPriceCash = false;

                            if (reader.Name == "transfer")
                                flagStartTagTransfer = false;

                            if (reader.Name == "pickpointmap")
                                flagStartTagPickpointmap = false;

                            if (reader.Name == "day")
                                flagStartTagDay = false;

                            if (reader.Name == "company")
                                flagStartTagCompany = false;

                            if (reader.Name == "name")
                                flagStartTagName = false;
                            break;
                    }
                }
            }
            return shippingOptions;
        }

        public static ShippingOptionEx GetExtended(float bacePice, float pricecash, float transfer, string pickpointmap)
        {
            if (!string.IsNullOrEmpty(pickpointmap))
                return new ShippingOptionEx
                           {
                               Pickpointmap = pickpointmap,
                               Type = ExtendedType.Pickpoint
                           };

            else
            {
                return new ShippingOptionEx
                    {
                        BasePrice = bacePice,
                        PriceCash = pricecash,
                        Transfer = transfer,
                        Type = ExtendedType.CashOnDelivery
                    };
            }
        }

        //1 - �����
        //2 - ������ � ������� ������������
        //3 - �������� ������ �������� (������ ��� �������������)
        //4 - �������� ������� ���������
        //5 - �������� ����� ��� ������
        //6 - ���������� ������ ������� ��������
        //7 - �� ������ �������� �������� � ���������� ��������
        //8 - ������ ������� �� ��������
        //9 - �������� ����� �������� �� ����
        //11 - �� ������ ���
        //12 - �� ������ ������ �������� (������ ��� �������������)
        private static void GetErrorEdost(string str)
        {
            try
            {
                switch (str)
                {
                    case "2":
                        throw new Exception("������ � ������� ������������");

                    case "3":
                        throw new Exception("�������� ������ �������� (������ ��� �������������)");

                    case "4":
                        throw new Exception("�������� ������� ���������");

                    case "5":
                        throw new Exception("�������� ����� ��� ������");

                    case "6":
                        throw new Exception("���������� ������ ������� ��������");

                    case "7":
                        throw new Exception("�� ������ �������� �������� � ���������� ��������");

                    case "8":
                        throw new Exception("������ ������� �� ��������");

                    case "9":
                        throw new Exception("�������� ����� �������� �� ����");

                    case "11":
                        throw new Exception("�� ������ ���");

                    case "12":
                        throw new Exception("�� ������ ������ �������� (������ ��� �������������)");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, false);
            }
        }

    }
}