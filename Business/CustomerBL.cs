using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Model.DTO;
using TBRBooker.Model.Entities;

namespace TBRBooker.Business
{
    public class CustomerBL
    {


        public static List<ExistingCustomerDTO> SearchCustomers(string searchTerm)
        {
            var directory = DBBox.GetCustomerDirectory();
            var matches = new List<ExistingCustomerDTO>();

            foreach (var c in directory)
            {
                string cname = c.DirectoryName.Replace(" from ", " ");
                if (cname.ToLower().Contains(searchTerm.ToLower()))
                    matches.Add(c);
            }

            return matches;
        }

    }
}
