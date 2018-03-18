using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Base
{
    public static class EnumHelper
    {

        // your enum->string method (I just decluttered it a bit :))
        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return ((DescriptionAttribute)attributes[0]).Description;
            else
                return ReplaceCamelCaseWithSpace(value.ToString(), false);
        }

        // the method to go from string->enum
        public static T GetEnumFromDescription<T>(string stringValue)
            where T : struct
        {
            foreach (object e in Enum.GetValues(typeof(T)))
                if (GetEnumDescription((Enum)e).Equals(stringValue))
                    return (T)e;
            throw new ArgumentException("No matching enum value found.");
        }

        // and a method to get a list of string values - no KeyValuePair needed
        public static IEnumerable<string> GetEnumDescriptions(Type enumType)
        {
            var strings = new Collection<string>();
            foreach (Enum e in Enum.GetValues(enumType))
                strings.Add(GetEnumDescription(e));
            return strings;
        }

        /// <summary>
        /// Replaces the camel case with space. if expectAbbreviations = true, and the input contains 2 capital letters together, it won't put the space in (basically)
        /// </summary>
        /// <param name="combinedString">The combined string.</param>
        /// <param name="expectAbbreviations">if set to <c>true</c> [expect abbreviations].</param>
        /// <returns>Turn camel case to normal string by adding space before each capital letter.</returns>
        public static string ReplaceCamelCaseWithSpace(string combinedString, bool expectAbbreviations)
        {
            var strBuilder = new StringBuilder();
            bool lastCharWasUpper = false;
            string currentAbbreviation = string.Empty;
            bool skipSpace = false;
            if (combinedString != null)
            {
                for (int cnt = 0; cnt <= combinedString.Length - 1; cnt++)
                {
                    if (char.IsUpper(combinedString[cnt]))
                    {
                        if (!expectAbbreviations)
                        {
                            strBuilder.Append(" ");
                            strBuilder.Append(combinedString[cnt]);

                            //last time through - must append it
                        }
                        else if (cnt == combinedString.Length - 1)
                        {
                            if (!lastCharWasUpper && cnt > 0)
                            {
                                strBuilder.Append(" ");
                            }
                            else if (!string.IsNullOrWhiteSpace(currentAbbreviation))
                            {
                                strBuilder.Append(currentAbbreviation);
                            }
                            strBuilder.Append(combinedString[cnt]);
                        }
                        else
                        {
                            if (!lastCharWasUpper && cnt > 0 && !skipSpace)
                            {
                                strBuilder.Append(" ");
                                skipSpace = true;
                            }
                            else if (skipSpace)
                            {
                                skipSpace = false;
                            }
                            lastCharWasUpper = true;
                            currentAbbreviation += combinedString[cnt];
                        }
                    }
                    else if (lastCharWasUpper)
                    {
                        strBuilder.Append(currentAbbreviation.Substring(0, currentAbbreviation.Length - 1));
                        if (!skipSpace)
                        {
                            strBuilder.Append(" ");
                        }
                        skipSpace = false;
                        strBuilder.Append(currentAbbreviation.ToCharArray()[currentAbbreviation.Length - 1]);
                        strBuilder.Append(combinedString[cnt]);
                        currentAbbreviation = string.Empty;
                        lastCharWasUpper = false;
                    }
                    else
                    {
                        strBuilder.Append(combinedString[cnt]);
                    }
                }
            }

            return strBuilder.ToString().Trim();
        }
    }
}
