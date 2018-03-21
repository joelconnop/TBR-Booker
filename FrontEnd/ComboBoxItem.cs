using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBRBooker.Base;

namespace TBRBooker.FrontEnd
{
    public class ComboBoxItem
    {
        private readonly Enum Val;

        public ComboBoxItem(Enum val)
        {
            Val = val;
        }

        public T GetValue<T>()
        {
            //drama for invalid selections. Want the NotSet option
            if (Val is T)
            {
                return (T)Convert.ChangeType(Val, typeof(T));
            }
            throw new Exception($"Combo Box value {Val} could not be converted to {typeof(T).ToString()}.");
        }

        public override string ToString()
        {
            return EnumHelper.GetEnumDescription(Val);
        }

        public static void InitComboBox(ComboBox cb, Type enumType, Enum startingValue,
            List<Enum> excludeList = null)
        {
            cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cb.AutoCompleteSource = AutoCompleteSource.ListItems;

            cb.BeginUpdate();
            cb.Items.Clear();

            int i = 0;
            foreach (Enum e in Enum.GetValues(enumType))
            {
                if (excludeList == null || !excludeList.Contains(e))
                {

                    cb.Items.Add(new ComboBoxItem(e));
                    if (e.Equals(startingValue))
                        cb.SelectedIndex = i;
                    i++;
                }
            }

            cb.EndUpdate();
        }

        public static T GetSelected<T>(ComboBox cb)
        {
            //drama for invalid selections. Want the NotSet option
            if (cb.SelectedItem == null || !(cb.SelectedItem is ComboBoxItem))
            {
                //throw new Exception($"Combo Box value {SelectedVal} could not be converted to {typeof(T).ToString()}.");
                T defaultVal = default(T);
                if (defaultVal.ToString().Equals("NotSet"))
                    return defaultVal;
                foreach (Enum e in Enum.GetValues(typeof(T)))
                {
                    if (e.ToString().Equals("NotSet"))
                        defaultVal = (T)Convert.ChangeType(e, typeof(T));
                }
                return defaultVal;
            }

            return ((ComboBoxItem)cb.SelectedItem).GetValue<T>();
        }

        public static bool ManuallySelectItem<T>(ComboBox cb, T item)
        {
            foreach (var i in cb.Items)
            {
                var cbi = i as ComboBoxItem;
                if (cbi.GetValue<T>().Equals(item))
                {
                    cb.SelectedItem = i;
                    return true;
                }
            }
            return false;
        }


    }
}
