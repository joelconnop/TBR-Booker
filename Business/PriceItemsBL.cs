﻿using System;
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
            var products = new[]
{
    new { Id = ProductIds.ReptilePartyGoldCoast, Price = 250, Quantity = 1 },
    new { Id = ProductIds.ReptilePartyPlusGoldCoast, Price = 280, Quantity = 1 },
    new { Id = ProductIds.PremiumReptilePartyGoldCoast, Price = 400, Quantity = 1 },
    new { Id = ProductIds.ReptileShowGoldCoast, Price = 280, Quantity = 1 },
    new { Id = ProductIds.ReptileDisplayGoldCoast, Price = 600, Quantity = 1 },
    new { Id = ProductIds.RoamingReptilesGoldCoast, Price = 200, Quantity = 1 },
    new { Id = ProductIds.PythonAppearanceGoldCoast, Price = 150, Quantity = 1 },

    new { Id = ProductIds.ReptilePartyLogan, Price = 250, Quantity = 1 },
    new { Id = ProductIds.ReptilePartyPlusLogan, Price = 280, Quantity = 1 },
    new { Id = ProductIds.PremiumReptilePartyLogan, Price = 400, Quantity = 1 },
    new { Id = ProductIds.ReptileShowLogan, Price = 280, Quantity = 1 },
    new { Id = ProductIds.ReptileDisplayLogan, Price = 600, Quantity = 1 },
    new { Id = ProductIds.RoamingReptilesLogan, Price = 250, Quantity = 1 },
    new { Id = ProductIds.PythonAppearanceLogan, Price = 200, Quantity = 1 },

    new { Id = ProductIds.ReptilePartyBrisbane, Price = 250, Quantity = 1 },
    new { Id = ProductIds.ReptilePartyPlusBrisbane, Price = 280, Quantity = 1 },
    new { Id = ProductIds.PremiumReptilePartyBrisbane, Price = 400, Quantity = 1 },
    new { Id = ProductIds.ReptileShowBrisbane, Price = 280, Quantity = 1 },
    new { Id = ProductIds.ReptileDisplayBrisbane, Price = 600, Quantity = 1 },
    new { Id = ProductIds.RoamingReptilesBrisbane, Price = 250, Quantity = 1 },
    new { Id = ProductIds.PythonAppearanceBrisbane, Price = 200, Quantity = 1 },

    new { Id = ProductIds.AdditionalHours, Price = 100, Quantity = 0 },
    new { Id = ProductIds.AddCrocodile, Price = 50, Quantity = 1 },
    new { Id = ProductIds.AdditionalParticipants, Price = 6, Quantity = 0 },
    new { Id = ProductIds.AdditionalParticipantsPlus, Price = 10, Quantity = 0 },
    new { Id = ProductIds.AdditionalParticipantsPremium, Price = 12, Quantity = 0 },
    new { Id = ProductIds.PartyBags, Price = 5, Quantity = 0 },
    new { Id = ProductIds.BuggyLollyJarUpgrade, Price = 65, Quantity = 0 },
    new { Id = ProductIds.BuggyLollyJarSingle, Price = 8, Quantity = 0 },
    new { Id = ProductIds.Pinata, Price = 60, Quantity = 0 },
    new { Id = ProductIds.ShortDemonstrations, Price = 100, Quantity = 1 },
    new { Id = ProductIds.InteractiveEncounter, Price = 50, Quantity = 1 },
    new { Id = ProductIds.Parking, Price = 38, Quantity = 1 },
    new { Id = ProductIds.Discount, Price = -50, Quantity = 1 },
    new { Id = ProductIds.BotanicGardens, Price = 150, Quantity = 1 },
    new { Id = ProductIds.Other, Price = 0, Quantity = 1 },
    new { Id = ProductIds.NotSet, Price = 0, Quantity = 0 },
    new { Id = ProductIds.FetesAndFairs, Price = 350, Quantity = 1 }
};

            foreach (var product in products)
            {
                _productLookup.Add(product.Id, new PriceItem(product.Id, product.Price, product.Quantity));
            }
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
