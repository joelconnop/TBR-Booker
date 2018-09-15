using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBRBooker.Base;

namespace TBRBooker.FrontEnd
{
    public class Styles
    {

        private static Color DefaultMainColour = Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
        private static Color DefaultContrastColour = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));

        private static Color UserMainColour;
        private static Color UserContrastColour;

        public static Color MainColour()
        {
            if (!UserMainColour.Name.Equals("0"))
                return UserMainColour;
            else
                return DefaultMainColour;
        }

        public static Color ContrastnColour()
        {
            if (!UserContrastColour.Name.Equals("0"))
                return UserContrastColour;
            else
                return DefaultContrastColour;
        }

        public static void InitStyles(string userMainColour, string userContrastColour)
        {
            try
            {
                if (!string.IsNullOrEmpty(userMainColour))
                {
                    var col = ColorTranslator.FromHtml(userMainColour);
                    if (!col.Name.Equals("0") && !col.Equals(DefaultMainColour))
                        UserMainColour = col;
                }
            }
            catch
            {
                // leave mainColour un-initialised
            }

            try
            {
                if (!string.IsNullOrEmpty(userContrastColour))
                {
                    var col = ColorTranslator.FromHtml(userContrastColour);
                    if (!col.Name.Equals("0") && !col.Equals(DefaultContrastColour))
                        UserContrastColour = col;
                }
            }
            catch
            {
                // leave contrastColour un-initialised
            }
        }

        public static void SetColours(Control control)
        {
            try
            {
                if (control.ForeColor.Equals(DefaultMainColour) && !UserMainColour.IsEmpty)
                    control.ForeColor = UserMainColour;
                else if (control.ForeColor.Equals(DefaultContrastColour) && !UserContrastColour.IsEmpty)
                    control.ForeColor = UserContrastColour;

                if (control.BackColor.Equals(DefaultMainColour) && !UserMainColour.IsEmpty)
                    control.BackColor = UserMainColour;
                else if (control.BackColor.Equals(DefaultContrastColour) && !UserContrastColour.IsEmpty)
                    control.BackColor = UserContrastColour;

                var tabbed = control as TabControl;
                if (tabbed != null)
                {
                    foreach (var tab in tabbed.TabPages)
                    {
                        SetColours((Control)tab);
                    }
                }

                foreach (var c in control.Controls)
                {
                    var child = c as Control;
                    if (child != null)
                        SetColours(child);
                }
            }
            catch
            {
                // risky code, but do nothing for now on error as its not important
                return;
            }
        }

        private static void SetColours(List<Control> controls)
        {

        }



    }
}
