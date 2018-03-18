using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Model.Enums;

namespace TBRBooker.Model.Entities
{
    /// <summary>
    /// doesn't have its own table, just have a list of these off booking
    /// </summary>
    public class Payment
    {

        public readonly DateTime Date;
        public readonly decimal Amount;
        public readonly PaymentMethods Method;

        public Payment(DateTime date, decimal amount, PaymentMethods method)
        {
            Date = date;
            Amount = amount;
            Method = method;
        }

        public override string ToString()
        {
            return $"{Date.ToString("d")}: {Amount.ToString("C")} ({Method.ToString()})";
        }

    }
}
