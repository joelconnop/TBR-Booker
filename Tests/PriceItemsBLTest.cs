using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Business;
using TBRBooker.Model.Enums;

namespace Tests
{

    [TestClass]
    public class PriceItemsBLTest
    {

        [TestMethod]
        public void PriceItemsBL_GetBaseService_1()
        {
            var result = PriceItemsBL.GetBaseItem(LocationRegions.GoldCoastCentral,
                ServiceTypes.Display, PartyPackages.NotSet);

            result.ProductId.ShouldBe(ProductIds.ReptileDisplayGoldCoast);
        }

        [TestMethod]
        public void PriceItemsBL_GetBaseService_2()
        {
            var result = PriceItemsBL.GetBaseItem(LocationRegions.Logan,
                ServiceTypes.ReptileParty, PartyPackages.PremiumParty);

            result.ProductId.ShouldBe(ProductIds.PremiumReptilePartyLogan);
        }

        [TestMethod]
        public void PriceItemsBL_GetBaseService_3()
        {
            var result = PriceItemsBL.GetBaseItem(LocationRegions.BeyondBrisbane,
                ServiceTypes.ReptileShow, PartyPackages.NotSet);

            result.ProductId.ShouldBe(ProductIds.ReptileShowBrisbane);
        }

    }
}
