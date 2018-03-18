using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Base;
using TBRBooker.Model.Enums;

namespace TBRBooker.Model.Entities
{
    public class PriceItem : ICloneable
    {
        public ProductIds ProductId { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal Total => UnitPrice * Quantity;

        public PriceItem()
        {
            //for deserialisation (somehow wasn't needed?)
        }

        public PriceItem(ProductIds productId, decimal unitPrice, int quantity, string overrideDescription = null)
        {
            ProductId = productId;
            if (overrideDescription != null)
                Description = overrideDescription;
            else
                Description = EnumHelper.GetEnumDescription(ProductId);
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        public object Clone()
        {
            return new PriceItem(ProductId, UnitPrice, Quantity, Description);
        }

        public override bool Equals(Object obj)
        {
            var other = (PriceItem)obj;
            return ProductId == other.ProductId && Description.Equals(other.Description)
                && UnitPrice == other.UnitPrice && Quantity == other.Quantity;
        }

        /// <summary>
        /// no real reason for this, just to make compiler happy
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 2003319287;
            hashCode = hashCode * -1521134295 + ProductId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            hashCode = hashCode * -1521134295 + UnitPrice.GetHashCode();
            hashCode = hashCode * -1521134295 + Quantity.GetHashCode();
            hashCode = hashCode * -1521134295 + Total.GetHashCode();
            return hashCode;
        }
    }
    
}
