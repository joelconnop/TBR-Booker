using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Model
{
    public class Address : ICloneable
    {
        public Address()
        {
            Street = Suburb = State = PostCode = string.Empty;
        }

        public Address(string venueName, string street, string suburb, string state, string postcode)
        {
            VenueName = venueName;
            Street = street;
            Suburb = suburb;
            State = state;
            PostCode = postcode;
        }

        public string VenueName { get; set; }
        public string Street { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public bool IsSet()
        {
            return (!string.IsNullOrEmpty(Street) &&
                !string.IsNullOrEmpty(Suburb) &&
                !string.IsNullOrEmpty(State) &&
                !string.IsNullOrEmpty(PostCode));
        }

        public string ToString(bool includePostCode = true)
        {
            string addrStr = string.Empty;

            if (!string.IsNullOrEmpty(VenueName))
            {
                addrStr += VenueName + ", ";
            }
            if (!string.IsNullOrEmpty(Street))
            {
                addrStr += (Street + ", ");
            }
            if (!string.IsNullOrEmpty(Suburb))
            {
                addrStr += (Suburb + ", ");
            }
            if (!string.IsNullOrEmpty(State))
            {
                addrStr += (State + ", ");
            }
            if (!string.IsNullOrEmpty(PostCode))
            {
                addrStr += (PostCode);
            }

            return addrStr.Trim().TrimEnd(',');
        }

        public override string ToString()
        {
            return ToString(true);
        }

    }

}
