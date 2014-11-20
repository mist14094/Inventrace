using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using InventraceBLogic;
using InventraceConstants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InventraceUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            InventraceBLogic.Store bl = new Store();
            bl.StoreName = "Pranavi Enterprises, Tirichi";
            bl.StoreDesc = "Dealer for All kinds of Lens etc..";
            bl.AddressLine1 = "474, CTH Road";
            bl.AddressLine2 = "Ambattur";
            bl.City = "Chennai";
            bl.ZipCode = "600 053";
            bl.State = "Tamil Nadu";
            bl.StoreId = 1;
            bl.UpdateStore(bl);

            List < Store > dfList = bl.GetStores();

        }

    }
}
