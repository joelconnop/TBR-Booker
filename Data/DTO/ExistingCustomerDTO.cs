using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Model.DTO
{
    public class ExistingCustomerDTO
    {
        public readonly string CustomerId;
        public readonly string DirectoryName;

        public ExistingCustomerDTO(string id, string name)
        {
            CustomerId = id;
            DirectoryName = name;
        }
    }
}
