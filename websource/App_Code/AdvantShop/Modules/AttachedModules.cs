//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using AdvantShop.Modules.Interfaces;

namespace AdvantShop.Modules
{
    public class AttachedModules
    {
        private static List<Type> _allModules;

        public enum EModuleType
        {
            All,
            ProcessPhoto,
            RenderIntoHtml,
            OrderChanged,
            SMS,
            MyAccountControls,
            DetailsModule,
            OrderConfirmation,
            ClientPage,
            SendMails,
            RenderIntoOrderConfiramtionHtml,
            ProductTabs,
            ShoppingCartPopup,
            UrlRewrite,
	    RenderIntoShoppingCart
        }

        public static void LoadModules()
        {
            _allModules = new List<Type>(System.Reflection.Assembly.GetExecutingAssembly().GetTypes().
                  Where(item => item.Namespace == "AdvantShop.Modules" && item.GetInterface("AdvantShop.Modules.Interfaces.IModule") == typeof(IModule)).ToList());
        }

        public static List<Type> GetModules(EModuleType moduleType)
        {
            if (_allModules == null)
                return null;

            var resultList = new List<Type>();
            switch (moduleType)
            {
                case EModuleType.All:
                    resultList = _allModules;
                    break;
                case EModuleType.ProcessPhoto:
                    resultList =
                        _allModules.Where(
                            item =>
                            item.IsClass &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.IProcessPhoto") == typeof(IProcessPhoto)).ToList();

                    break;
                case EModuleType.RenderIntoHtml:
                    resultList =
                        _allModules.Where(
                            item =>
                            item.IsClass &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.IRenderIntoHtml") == typeof(IRenderIntoHtml)).ToList();
                    break;
                case EModuleType.OrderChanged:
                    resultList =
                        _allModules.Where(
                            item =>
                            item.IsClass &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.IOrderChanged") == typeof(IOrderChanged)).ToList();
                    break;
                case EModuleType.SMS:
                    resultList =
                        _allModules.Where(
                            item =>
                            item.IsClass &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.IModuleSms") == typeof(IModuleSms)).ToList();
                    break;
                case EModuleType.MyAccountControls:
                    resultList =
                        _allModules.Where(
                            item =>
                            item.IsClass &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.IMyAccountControls") == typeof(IMyAccountControls)).ToList();
                    break;
                case EModuleType.DetailsModule:
                    resultList =
                    _allModules.Where(
                        item =>
                        item.IsClass &&
                        item.GetInterface("AdvantShop.Modules.Interfaces.IModuleDetails") == typeof(IModuleDetails)).ToList();
                    break;
                case EModuleType.OrderConfirmation:
                    resultList =
                        _allModules.Where(
                            item =>
                            item.IsClass &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.IOrderConfirmation") == typeof(IOrderConfirmation)).ToList();
                    break;
                case EModuleType.ClientPage:
                    resultList =
                        _allModules.Where(
                            item =>
                            item.IsClass &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.IClientPageModule") == typeof(IClientPageModule)).ToList();
                    break;
                case EModuleType.SendMails:
                    resultList =
                        _allModules.Where(
                            item =>
                            item.IsClass &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.ISendMails") == typeof(ISendMails)).ToList();
                    break;
                case EModuleType.RenderIntoOrderConfiramtionHtml:
                    resultList =
                        _allModules.Where(
                            item =>
                            item.IsClass &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.IOrderRenderIntoHtml") == typeof(IOrderRenderIntoHtml)).ToList();
                    break;
                case EModuleType.ProductTabs:
                    resultList =
                        _allModules.Where(
                            item =>
                            item.IsClass &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.IProductTabs") == typeof(IProductTabs)).ToList();
                    break;
                case EModuleType.ShoppingCartPopup:
                    resultList =
                        _allModules.Where(
                            item =>
                            item.IsClass &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.IShoppingCartPopup") == typeof(IShoppingCartPopup)).ToList();
                    break;
               case EModuleType.UrlRewrite:
                    resultList =
                        _allModules.Where(
                            item =>
                            item.IsClass &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.IModuleUrlRewrite") == typeof(IModuleUrlRewrite)).ToList();
                    break;
	      case EModuleType.RenderIntoShoppingCart:
                    resultList =
                        _allModules.Where(
                            item =>
                            item.IsClass &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.IRenderIntoShoppingCart") == typeof(IRenderIntoShoppingCart)).ToList();
                    break;
            }
            return resultList;
        }
    }
}