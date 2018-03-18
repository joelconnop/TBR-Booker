using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Model.Entities
{

    /// <summary>
    /// doesn't have its own table, just have a list of these off booking
    /// </summary>
    public class Followup : ICloneable
    {
        public DateTime FollowupDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Purpose { get; set; }
        public string CompleteNote { get; set; }
        public bool IsConfirmationCall { get; set; }

        public object Clone()
        {
            return new Followup()
            {
                FollowupDate = FollowupDate,
                CompletedDate = CompletedDate,
                Purpose = Purpose,
                CompleteNote = CompleteNote,
                IsConfirmationCall = IsConfirmationCall
            };
        }

        public bool IsOutstanding()
        {
            return !CompletedDate.HasValue;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = (Followup)obj;

            //null check, either both null or neither null
            if (CompletedDate.HasValue != other.CompletedDate.HasValue)
                return false;
            if (string.IsNullOrEmpty(CompleteNote) != string.IsNullOrEmpty(other.CompleteNote))
                return false;


            return FollowupDate.Ticks == other.FollowupDate.Ticks
                && (!CompletedDate.HasValue || 
                    CompletedDate.Value.Ticks == other.CompletedDate.Value.Ticks)
                && Purpose.Equals(other.Purpose)
                && (string.IsNullOrEmpty(CompleteNote) || CompleteNote.Equals(other.CompleteNote))
                && IsConfirmationCall == other.IsConfirmationCall;
        }

        /// <summary>
        /// no real reason for this, just to make compiler happy
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 2003319287;
            hashCode = hashCode * -1521134295 + FollowupDate.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Purpose);
            hashCode = hashCode * -1521134295 + IsConfirmationCall.GetHashCode();
            if (CompletedDate.HasValue)
            {
                hashCode = hashCode * -1521134295 + CompletedDate.Value.Ticks.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CompleteNote);
            }
            return hashCode;
        }
    }
}
