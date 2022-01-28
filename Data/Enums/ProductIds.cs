using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Model.Enums
{
    public enum ProductIds
    {
        //if changing any of the GC/Bris/Logan enum order, also see PriceItemsBL.GetBaseItem
        ReptilePartyGoldCoast,
        ReptilePartyPlusGoldCoast,
        PremiumReptilePartyGoldCoast,
        ReptileShowGoldCoast,
        ReptileDisplayGoldCoast,
        RoamingReptilesGoldCoast,
        PythonAppearanceGoldCoast,
        ReptilePartyLogan,
        ReptilePartyPlusLogan,
        PremiumReptilePartyLogan,
        ReptileShowLogan,
        ReptileDisplayLogan,
        RoamingReptilesLogan,
        PythonAppearanceLogan,
        ReptilePartyBrisbane,
        ReptilePartyPlusBrisbane,
        PremiumReptilePartyBrisbane,
        ReptileShowBrisbane,
        ReptileDisplayBrisbane,
        RoamingReptilesBrisbane,
        PythonAppearanceBrisbane,
        Other,
        AdditionalHours,
        AddCrocodile,
        AdditionalParticipants,
        AdditionalParticipantsPlus,
        AdditionalParticipantsPremium,
        PartyBags,
        ShortDemonstrations,
        InteractiveEncounter,
        Parking,
        Discount,
        NotSet,
        BotanicGardens,
        BuggyLollyJarUpgrade,
        BuggyLollyJarSingle,
        Pinata,
        FetesAndFairs
    }
}
