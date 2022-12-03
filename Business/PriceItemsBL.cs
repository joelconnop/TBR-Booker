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

            _productLookup.Add(ProductIds.ReptilePartyGoldCoast,
                new PriceItem(ProductIds.ReptilePartyGoldCoast, 250, 1));
            _productLookup.Add(ProductIds.ReptilePartyPlusGoldCoast,
new PriceItem(ProductIds.ReptilePartyPlusGoldCoast, 280, 1));
            _productLookup.Add(ProductIds.PremiumReptilePartyGoldCoast,
new PriceItem(ProductIds.PremiumReptilePartyGoldCoast, 400, 1));
            _productLookup.Add(ProductIds.ReptileShowGoldCoast,
new PriceItem(ProductIds.ReptileShowGoldCoast, 280, 1));
            _productLookup.Add(ProductIds.ReptileDisplayGoldCoast,
new PriceItem(ProductIds.ReptileDisplayGoldCoast, 600, 1));
            _productLookup.Add(ProductIds.RoamingReptilesGoldCoast,
new PriceItem(ProductIds.RoamingReptilesGoldCoast, 200, 1));
            _productLookup.Add(ProductIds.PythonAppearanceGoldCoast,
new PriceItem(ProductIds.PythonAppearanceGoldCoast, 150, 1));

            _productLookup.Add(ProductIds.ReptilePartyLogan,
                new PriceItem(ProductIds.ReptilePartyLogan, 250, 1));
            _productLookup.Add(ProductIds.ReptilePartyPlusLogan,
new PriceItem(ProductIds.ReptilePartyPlusLogan, 280, 1));
            _productLookup.Add(ProductIds.PremiumReptilePartyLogan,
new PriceItem(ProductIds.PremiumReptilePartyLogan, 400, 1));
            _productLookup.Add(ProductIds.ReptileShowLogan,
new PriceItem(ProductIds.ReptileShowLogan, 280, 1));
            _productLookup.Add(ProductIds.ReptileDisplayLogan,
new PriceItem(ProductIds.ReptileDisplayLogan, 600, 1));
            _productLookup.Add(ProductIds.RoamingReptilesLogan,
new PriceItem(ProductIds.RoamingReptilesLogan, 250, 1));
            _productLookup.Add(ProductIds.PythonAppearanceLogan,
new PriceItem(ProductIds.PythonAppearanceLogan, 200, 1));

            _productLookup.Add(ProductIds.ReptilePartyBrisbane,
                new PriceItem(ProductIds.ReptilePartyBrisbane, 250, 1));
            _productLookup.Add(ProductIds.ReptilePartyPlusBrisbane,
new PriceItem(ProductIds.ReptilePartyPlusBrisbane, 280, 1));
            _productLookup.Add(ProductIds.PremiumReptilePartyBrisbane,
new PriceItem(ProductIds.PremiumReptilePartyBrisbane, 400, 1));
            _productLookup.Add(ProductIds.ReptileShowBrisbane,
new PriceItem(ProductIds.ReptileShowBrisbane, 280, 1));
            _productLookup.Add(ProductIds.ReptileDisplayBrisbane,
new PriceItem(ProductIds.ReptileDisplayBrisbane, 600, 1));
            _productLookup.Add(ProductIds.RoamingReptilesBrisbane,
new PriceItem(ProductIds.RoamingReptilesBrisbane, 250, 1));
            _productLookup.Add(ProductIds.PythonAppearanceBrisbane,
new PriceItem(ProductIds.PythonAppearanceBrisbane, 200, 1));

            _productLookup.Add(ProductIds.AdditionalHours,
new PriceItem(ProductIds.AdditionalHours, 100, 0));
            _productLookup.Add(ProductIds.AddCrocodile,
                new PriceItem(ProductIds.AddCrocodile, 50, 1));
            _productLookup.Add(ProductIds.AdditionalParticipants,
                new PriceItem(ProductIds.AdditionalParticipants, 6, 0));
            _productLookup.Add(ProductIds.AdditionalParticipantsPlus,
    new PriceItem(ProductIds.AdditionalParticipantsPlus, 10, 0));
            _productLookup.Add(ProductIds.AdditionalParticipantsPremium,
new PriceItem(ProductIds.AdditionalParticipantsPremium, 12, 0));
            _productLookup.Add(ProductIds.PartyBags,
new PriceItem(ProductIds.PartyBags, 5, 0));
            _productLookup.Add(ProductIds.BuggyLollyJarUpgrade,
new PriceItem(ProductIds.BuggyLollyJarUpgrade, 65, 0));
            _productLookup.Add(ProductIds.BuggyLollyJarSingle,
new PriceItem(ProductIds.BuggyLollyJarUpgrade, 8, 0));
            _productLookup.Add(ProductIds.Pinata,
new PriceItem(ProductIds.Pinata, 60, 0));
            _productLookup.Add(ProductIds.ShortDemonstrations,
new PriceItem(ProductIds.ShortDemonstrations, 100, 1));
            _productLookup.Add(ProductIds.InteractiveEncounter,
new PriceItem(ProductIds.InteractiveEncounter, 50, 1));
            _productLookup.Add(ProductIds.Parking,
new PriceItem(ProductIds.Parking, 38, 1));
            _productLookup.Add(ProductIds.Discount,
new PriceItem(ProductIds.Discount, -50, 1));
            _productLookup.Add(ProductIds.BotanicGardens, new PriceItem(ProductIds.BotanicGardens, 150, 1));
            _productLookup.Add(ProductIds.Other, new PriceItem(ProductIds.Other, 0, 1));
            _productLookup.Add(ProductIds.NotSet, new PriceItem(ProductIds.NotSet, 0, 0, ""));
            _productLookup.Add(ProductIds.FetesAndFairs,
new PriceItem(ProductIds.FetesAndFairs, 350, 1));
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
                case ServiceTypes.Other:
                    return null;
                default:
                    throw new Exception($"Unknown Service '{service}'.");

            }

            return Get((ProductIds)(regionX * 7 + serviceX));

        }

        public static bool IsAService(ProductIds product)
        {
            return product <= ProductIds.PythonAppearanceBrisbane;
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
