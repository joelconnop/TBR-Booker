using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Model.Entities;
using TBRBooker.Model.Enums;

namespace TBRBooker.Business
{
    public class PriceItemsBL
    {
        private static Dictionary<ProductIds, PriceItem> _productLookup;

        public static PriceItem Get(ProductIds productId)
        {
            if (_productLookup != null)
                return (PriceItem)_productLookup[productId].Clone();

            _productLookup = new Dictionary<ProductIds, PriceItem>();
            var products = new[]
{
    new { Id = ProductIds.ReptilePartyGoldCoast, Price = 300, Quantity = 1 },
    new { Id = ProductIds.ReptilePartyPlusGoldCoast, Price = 350, Quantity = 1 },
    new { Id = ProductIds.PremiumReptilePartyGoldCoast, Price = 450, Quantity = 1 },
    new { Id = ProductIds.ReptileShowGoldCoast, Price = 300, Quantity = 1 },
    new { Id = ProductIds.ReptileDisplayGoldCoast, Price = 640, Quantity = 1 },

    new { Id = ProductIds.ReptilePartyLogan, Price = 300, Quantity = 1 },
    new { Id = ProductIds.ReptilePartyPlusLogan, Price = 350, Quantity = 1 },
    new { Id = ProductIds.PremiumReptilePartyLogan, Price = 450, Quantity = 1 },
    new { Id = ProductIds.ReptileShowLogan, Price = 300, Quantity = 1 },
    new { Id = ProductIds.ReptileDisplayLogan, Price = 640, Quantity = 1 },

    new { Id = ProductIds.ReptilePartyBrisbane, Price = 300, Quantity = 1 },
    new { Id = ProductIds.ReptilePartyPlusBrisbane, Price = 350, Quantity = 1 },
    new { Id = ProductIds.PremiumReptilePartyBrisbane, Price = 450, Quantity = 1 },
    new { Id = ProductIds.ReptileShowBrisbane, Price = 300, Quantity = 1 },
    new { Id = ProductIds.ReptileDisplayBrisbane, Price = 640, Quantity = 1 },
        
    new { Id = ProductIds.AdditionalHours, Price = 100, Quantity = 0 },
    new { Id = ProductIds.AddCrocodile, Price = 50, Quantity = 1 },
    new { Id = ProductIds.AdditionalParticipants, Price = 6, Quantity = 0 },
    new { Id = ProductIds.AdditionalParticipantsPlus, Price = 10, Quantity = 0 },
    new { Id = ProductIds.AdditionalParticipantsPremium, Price = 12, Quantity = 0 },
    new { Id = ProductIds.PartyBags, Price = 5, Quantity = 0 },
    new { Id = ProductIds.BugCatcherUpgrade, Price = 60, Quantity = 1 },
    new { Id = ProductIds.BugCatcherSingle, Price = 10, Quantity = 0 },
    new { Id = ProductIds.Pinata, Price = 60, Quantity = 0 },
    new { Id = ProductIds.ShortDemonstrations, Price = 100, Quantity = 1 },
    new { Id = ProductIds.InteractiveEncounter, Price = 100, Quantity = 1 },
    new { Id = ProductIds.Parking, Price = 37, Quantity = 1 },
    new { Id = ProductIds.Discount, Price = -50, Quantity = 1 },
    new { Id = ProductIds.Other, Price = 0, Quantity = 1 },
    new { Id = ProductIds.NotSet, Price = 0, Quantity = 0 },
    new { Id = ProductIds.FetesAndFairs, Price = 370, Quantity = 1 },

    new { Id = ProductIds.NocturnalNatives, Price = 300, Quantity = 1 },
    new { Id = ProductIds.NightAtTheGardens, Price = 158, Quantity = 1 },
    new { Id = ProductIds.LittleNatureLovers, Price = 125, Quantity = 1 },
    new { Id = ProductIds.ScalyReptiles, Price = 290, Quantity = 1 },
    new { Id = ProductIds.GeneratorHire, Price = 100, Quantity = 1 }
};

            foreach (var product in products)
            {
                _productLookup.Add(product.Id, new PriceItem(product.Id, product.Price, product.Quantity));
            }

            return (PriceItem)_productLookup[productId].Clone();
        }

        public static PriceItem GetBaseItem(LocationRegions region, ServiceTypes service, 
            PartyPackages partyPackage = PartyPackages.NotSet)
        {
            //if (region == LocationRegions.NotSet)
            //    throw new Exception("Region not set for GetBaseItem.");

            int regionX = 0;
            int serviceX = 0;

            switch (region)
            {
                case LocationRegions.GoldCoastSouth:
                case LocationRegions.GoldCoastNorth:
                case LocationRegions.GoldCoastHinterland:
                case LocationRegions.GoldCoastCentral:
                case LocationRegions.NotSet:
                    regionX = 0;
                    break;
                case LocationRegions.Logan:
                    regionX = 1;
                    break;
                case LocationRegions.Brisbane:
                case LocationRegions.BeyondBrisbane:
                case LocationRegions.Ipswich:
                    regionX = 2;
                    break;
                default:
                    throw new Exception($"Unknown location region '{region}'.");
            }

            switch (service)
            {
                case ServiceTypes.ReptileParty:
                    switch (partyPackage)
                    {
                        case PartyPackages.Party:
                        case PartyPackages.NotSet:
                            serviceX = 0;
                            break;
                        case PartyPackages.PartyPlus:
                            serviceX = 1;
                            break;
                        case PartyPackages.PremiumParty:
                            serviceX = 2;
                            break;
                        default:
                            throw new Exception($"Unknown Party Package '{partyPackage}'.");
                    }
                    break;
                case ServiceTypes.ReptileShow:
                    serviceX = 3;
                    break;
                case ServiceTypes.Display:
                    serviceX = 4;
                    break;
                case ServiceTypes.RoamingReptiles:
                    serviceX = 5;
                    break;
                case ServiceTypes.PythonAppearance:
                    serviceX = 6;
                    break;
                case ServiceTypes.FetesAndFairs:
                    return Get(ProductIds.FetesAndFairs);
                case ServiceTypes.LittleNatureLovers:
                    return Get(ProductIds.LittleNatureLovers);
                case ServiceTypes.Other:
                case ServiceTypes.GuidedWalk:
                    return null;
                default:
                    throw new Exception($"Unknown Service '{service}'.");

            }

            return Get((ProductIds)(regionX * 7 + serviceX));

        }

        public static bool IsAService(ProductIds product)
        {
            switch (product)
            {
                case ProductIds.ReptilePartyGoldCoast:
                case ProductIds.ReptilePartyPlusGoldCoast:
                case ProductIds.PremiumReptilePartyGoldCoast:
                case ProductIds.ReptileShowGoldCoast:
                case ProductIds.ReptileDisplayGoldCoast:
                case ProductIds.RoamingReptilesGoldCoast:
                case ProductIds.PythonAppearanceGoldCoast:
                case ProductIds.ReptilePartyLogan:
                case ProductIds.ReptilePartyPlusLogan:
                case ProductIds.PremiumReptilePartyLogan:
                case ProductIds.ReptileShowLogan:
                case ProductIds.ReptileDisplayLogan:
                case ProductIds.RoamingReptilesLogan:
                case ProductIds.PythonAppearanceLogan:
                case ProductIds.ReptilePartyBrisbane:
                case ProductIds.ReptilePartyPlusBrisbane:
                case ProductIds.PremiumReptilePartyBrisbane:
                case ProductIds.ReptileShowBrisbane:
                case ProductIds.ReptileDisplayBrisbane:
                case ProductIds.RoamingReptilesBrisbane:
                case ProductIds.PythonAppearanceBrisbane:
                case ProductIds.FetesAndFairs:
                case ProductIds.LittleNatureLovers:
                    return true;
                case ProductIds.Other:
                case ProductIds.AdditionalHours:
                case ProductIds.AddCrocodile:
                case ProductIds.AdditionalParticipants:
                case ProductIds.AdditionalParticipantsPlus:
                case ProductIds.AdditionalParticipantsPremium:
                case ProductIds.PartyBags:
                case ProductIds.ShortDemonstrations:
                case ProductIds.InteractiveEncounter:
                case ProductIds.Parking:
                case ProductIds.Discount:
                case ProductIds.NotSet:
                case ProductIds.BuggyLollyJarUpgrade:
                case ProductIds.BuggyLollyJarSingle:
                case ProductIds.Pinata:
                case ProductIds.GeneratorHire:
                case ProductIds.BugCatcherUpgrade:
                case ProductIds.BugCatcherSingle:
                    return false;
                case ProductIds.BotanicGardens:
                case ProductIds.NocturnalNatives:
                case ProductIds.NightAtTheGardens:
                case ProductIds.ScalyReptiles:
                    // these are services but all fall under the general 'guided walks'
                    return false;
                default:
                    throw new Exception($"Undefined if {product} is a service.");
            }
        }

        public static PartyPackages WhichPartyPackage(ProductIds product)
        {
            switch (product)
            {
                case ProductIds.ReptilePartyBrisbane:
                case ProductIds.ReptilePartyGoldCoast:
                case ProductIds.ReptilePartyLogan:
                    return PartyPackages.Party;
                case ProductIds.ReptilePartyPlusBrisbane:
                case ProductIds.ReptilePartyPlusGoldCoast:
                case ProductIds.ReptilePartyPlusLogan:
                    return PartyPackages.PartyPlus;
                case ProductIds.PremiumReptilePartyBrisbane:
                case ProductIds.PremiumReptilePartyGoldCoast:
                case ProductIds.PremiumReptilePartyLogan:
                    return PartyPackages.PremiumParty;
                default:
                    return PartyPackages.NotSet;
            }
        }
    }
}
