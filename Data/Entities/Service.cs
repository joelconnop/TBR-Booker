using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Model.Enums;

namespace TBRBooker.Model.Entities
{

    public class Service
    {
        public ServiceTypes ServiceType { get; set; }
        
        public bool AddCrocodile { get; set; }
        public string SpecificAnimalsToCome { get; set; }

        public List<PriceItem> PriceItems { get; set; }

        public decimal TotalPrice => PriceItems?.Sum(x => x.Total) ?? 0;

        public int Pax { get; set; }

        public Party Party { get; set; }

        public override string ToString()
        {
            if (Party != null)
                return Party.ToString();
            return ServiceType.ToString();
        }

        public string GetParticularsText()
        {
            if (Party != null)
                return Party.GetParticularsText();
            return "";
        }

    }

    public class Party
    {
        //redundant with the PriceItem (PriceItemsBL.WhichPartyPackage())
        public PartyPackages Package { get; set; }
        public string BirthdayName { get; set; }
        public int BirthdayAge { get; set; }

        public Party()
        {
            BirthdayName = "";
        }

        public override string ToString()
        {
            return "Reptile " + Package.ToString();
        }

        public string GetParticularsText()
        {
            string birthday = BirthdayName;
            if (!string.IsNullOrEmpty(birthday))
            {
                if (BirthdayAge > 0)
                    return birthday + ", turning " + BirthdayAge.ToString();
                return birthday;
            }
            else if (BirthdayAge > 0)
                return BirthdayAge.ToString();
            return "";
        }
    }

}
