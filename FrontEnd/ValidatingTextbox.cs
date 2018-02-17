using System.Windows.Forms;
using System;
using System.Text.RegularExpressions;
using System.Text;

namespace TBRBooker.FrontEnd
{

    public class ValidatingTextbox
    {
        public enum TextBoxValidationType
        {
            NotSet,
            EmailAddress,
            Name,
            AddressBasic,
            IntegerPositive,
            IntegerZeroPlus,
            IntegerAny,
            DoublePositive,
            DoubleNegative,
            DoubleAny,
            Username,
            FilenameLong,
            Filename,
            PhoneNumberNational,
            PhoneNumberMobile,
            NZBankAccountNumber,
            DollarAmountPositive,
            DollarAmountAny,
            FilenameForUpload,
            Percentage,
            GeneralDatabase,
            Filepath,
            Website,
            PhoneNumberAny,
            LongDatabase
        }

        private enum CharacterCase
        {
            Any,
            AllLower,
            AllUpper
        }

        private TextBox _tb;
        public TextBoxValidationType ValidationType;
        private IWin32Window _owner;

        public bool isValidatingEnabled = true;
        private const int USERNAME_MAX_LENGTH = 20;
        private const int FILENAME_MAX_LENGTH = 64;
        private const int FILENAME_LONG_MAX_LENGTH = 128;
        private const int FILEPATH_MAX_LENGTH = 260;

        public ValidatingTextbox(IWin32Window owner, TextBox textbox, TextBoxValidationType validation)
        {
            ValidationType = validation;
            _tb = textbox;
            _tb.TextChanged += new EventHandler(Textbox_Change);
            _owner = owner;
        }

        public void Textbox_Change(object sender, EventArgs e)
        {
            //_tb.OnTextChanged(e);
            if (!isValidatingEnabled)
            {
                return;
            }

            try
            {
                switch (ValidationType)
                {
                    case TextBoxValidationType.NotSet:
                        break;  //no validation

                    case TextBoxValidationType.IntegerAny:
                        validateInteger();
                        break;

                    case TextBoxValidationType.IntegerZeroPlus:
                        validateInteger();
                        if (!_tb.Text.Equals(""))
                        {
                            int num = int.Parse(_tb.Text);
                            num = Math.Abs(num);
                            _tb.Text = num.ToString();
                        }
                        break;

                    case TextBoxValidationType.IntegerPositive:
                        validateInteger();
                        if (!_tb.Text.Equals(""))
                        {
                            if (_tb.Text.Equals("0"))
                            {
                                _tb.Text = "1";
                            }

                            _tb.Text = Math.Abs(int.Parse(_tb.Text)).ToString();
                        }
                        break;

                    case TextBoxValidationType.EmailAddress:
                        validateEmail();
                        break;

                    case TextBoxValidationType.Name:
                        validateName();
                        break;

                    case TextBoxValidationType.Username:
                        validateText("_", "", true, true, false, false, CharacterCase.AllLower, USERNAME_MAX_LENGTH);
                        break;

                    case TextBoxValidationType.Filename:
                        validateText(" !@#$%^&(){}[]-_=+-", "", true, true, false, false, CharacterCase.Any, FILENAME_MAX_LENGTH);
                        break;

                    case TextBoxValidationType.FilenameLong:
                        validateText(" !@#$%^&(){}[]-_=+-", "", true, true, false, false, CharacterCase.Any, FILENAME_LONG_MAX_LENGTH);
                        break;

                    case TextBoxValidationType.Filepath:
                        validateText(" \\/!@#$%^&(){}[]-_=+-", "", true, true, false, false, CharacterCase.Any, FILEPATH_MAX_LENGTH);
                        break;

                    case TextBoxValidationType.PhoneNumberNational:
                    case TextBoxValidationType.PhoneNumberMobile:
                    case TextBoxValidationType.PhoneNumberAny:
                        validatePhoneNumber();
                        break;

                    case TextBoxValidationType.DollarAmountPositive:
                        validateDollarAmount(false);
                        break;

                    case TextBoxValidationType.DollarAmountAny:
                        validateDollarAmount(true);
                        break;

                    case TextBoxValidationType.DoubleAny:
                        validateDoubleAmount(true, false);
                        break;

                    case TextBoxValidationType.DoublePositive:
                    case TextBoxValidationType.Percentage:
                        validateDoubleAmount(false, false);
                        if (ValidationType == TextBoxValidationType.Percentage)
                        {
                            if (!_tb.Text.Equals(string.Empty) && double.Parse(_tb.Text) > 100)
                            {
                                _tb.Text = "100";
                            }
                        }
                        break;

                    case TextBoxValidationType.DoubleNegative:
                        validateDoubleAmount(true, true);
                        break;

                    case TextBoxValidationType.AddressBasic:
                        validateAddressBasic();
                        break;

                    case TextBoxValidationType.FilenameForUpload:
                        validateText(" (){}[]-_=-", "", true, true, false, false, CharacterCase.Any, FILENAME_MAX_LENGTH);
                        break;

                    case TextBoxValidationType.GeneralDatabase:
                        validateText("", "\"';", true, true, true, true, CharacterCase.Any, 45);
                        break;

                    case TextBoxValidationType.LongDatabase:
                        validateText("", "\"';", true, true, true, true, CharacterCase.Any, 255);
                        break;
                }
            }
            catch (Exception ex)
            {
                isValidatingEnabled = false;    //shut down this validator
                ErrorHandler.HandleError(_owner, "Validating Text Box for " + _tb.Name, ex, true);
            }
        }

        public string TextForDatabase()
        {
            switch (ValidationType)
            {
                case TextBoxValidationType.DollarAmountAny:
                case TextBoxValidationType.DollarAmountPositive:
                    if (_tb.Text.Length > 1)
                    {
                        decimal dval = 0;
                        if (decimal.TryParse(_tb.Text, out dval))
                        {
                            return dval.ToString();
                        }
                        else
                        {
                            return "0";
                        }
                    }
                    else
                    {
                        return "0";
                    }

                case TextBoxValidationType.DoubleAny:
                case TextBoxValidationType.DoubleNegative:
                case TextBoxValidationType.DoublePositive:
                case TextBoxValidationType.Percentage:
                    double val = 0;
                    if (double.TryParse(_tb.Text.Trim(), out val))
                    {
                        return val.ToString();
                    }
                    else
                    {
                        return "0.0";
                    }

                case TextBoxValidationType.IntegerAny:
                case TextBoxValidationType.IntegerPositive:
                case TextBoxValidationType.IntegerZeroPlus:
                    var ival = 0;
                    if (int.TryParse(_tb.Text.Trim(), out ival))
                    {
                        return ival.ToString();
                    }
                    else
                    {
                        return "0";
                    }

                case TextBoxValidationType.EmailAddress:
                    string pattern = "^[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9][\\w\\.-]*@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$";
                    Match emailAddressMatch = Regex.Match(_tb.Text, pattern);
                    if (emailAddressMatch.Success)
                    {
                        return _tb.Text;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case TextBoxValidationType.Website:
                    string epattern = "^((http(s?))\\://)?(www\\.)([a-zA-Z].)[a-zA-Z0-9\\-\\.]+\\.(com|edu|gov|mil|net|org|biz|info|name|museum|us|ca|uk|au|nz)(\\:[0-9]+)*(/($|[a-zA-Z0-9\\.\\,\\;\\?\\'\\\\\\+&amp;%\\$#\\=~_\\-]+))*$";
                    Match urlAddressMatch = Regex.Match(_tb.Text, epattern);
                    if (urlAddressMatch.Success)
                    {
                        return _tb.Text;
                    }
                    else
                    {
                        return string.Empty;
                    }

                default:
                    //this old carefulness for the database is not needed
                    //validateText("", "\"';", true, true, true, true, CharacterCase.Any, 255);
                    return _tb.Text;
            }
        }

        public bool IsTextValid()
        {
            return _tb.Text.Length == TextForDatabase().Length;
        }

        private void removeInvalidInput()
        {
            throw new Exception("what is this meant to do? call validateText instead");
            //int cursorPos;
            //cursorPos = _tb.SelectionStart - 1;
            //_tb.Text = _tb.Text.Remove(cursorPos, cursorPos + 1);
            //_tb.SelectionStart = Math.Min(cursorPos, _tb.TextLength);
        }

        private void validateText(string specificAllowed, string specificBanned, bool lettersAllowed, bool digitsAllowed, bool symbolsAllowed, bool whitespaceAllowed, CharacterCase charCase, int maxLength)
        {
            string tempStr = "";
            char tempChar;
            int cursorPos = _tb.SelectionStart;
            bool allowed = true;
            for (int i = 0; i <= _tb.Text.Length - 1; i++)
            {
                tempChar = _tb.Text[i];
                var charStr = tempChar.ToString();
                if (char.IsLetter(tempChar) & !lettersAllowed)
                {
                    allowed = false;
                }
                else if (char.IsDigit(tempChar) & !digitsAllowed)
                {
                    allowed = false;
                }
                else if (char.IsWhiteSpace(tempChar) & !whitespaceAllowed)
                {
                    allowed = false;
                }
                else if (!(char.IsLetter(tempChar) | char.IsNumber(tempChar) | char.IsWhiteSpace(tempChar)) & !symbolsAllowed)
                {
                    allowed = false;
                }

                if (specificAllowed.Contains(charStr))
                {
                    allowed = true;
                }

                if (specificBanned.Contains(charStr))
                {
                    allowed = false;
                }

                if (char.IsLetter(tempChar))
                {
                    switch (charCase)
                    {
                        case CharacterCase.AllLower:
                            tempChar = char.ToLower(tempChar);
                            break;
                        case CharacterCase.AllUpper:
                            tempChar = char.ToUpper(tempChar);
                            break;
                    }
                }

                if (allowed & i < maxLength)
                {
                    tempStr += charStr;
                }
                else
                {
                    cursorPos -= 1;
                }

                allowed = true;
            }

            _tb.Text = tempStr;
            _tb.SelectionStart = Math.Max(1, Math.Min(cursorPos, tempStr.Length));
        }

        private void validateInteger()
        {
            int tempInt;
            if (string.IsNullOrEmpty(_tb.Text) || int.TryParse(_tb.Text, out tempInt))
            {
                return;
            }
            else
            {
                validateText("-", "", false, true, false, false, CharacterCase.Any, 11);
            }
        }

        private void validateName()
        {
            //string[] nameParts;
            StringBuilder fullname = new StringBuilder();
            //int cursorPos = 0;
            if (_tb.Text.Equals(""))
            {
                return;
            }

            isValidatingEnabled = false;
            validateText("-.'&,", "", true, false, false, true, CharacterCase.Any, 256);
            //cursorPos = _tb.SelectionStart;
            //nameParts = FormattingUtils.GetStringTokens(_tb.Text, new char[]{' ', '-'}, true);
            //foreach (string nameStr in nameParts)
            //{
            //    fullname.Append(nameStr.Substring(0, 1).ToUpper);
            //    if (nameStr.Length > 1)
            //    {
            //        fullname.Append(nameStr.Substring(1).ToLower);
            //    }
            //}

            //_tb.Text = fullname.ToString();
            // _tb.SelectionStart = Math.Min(cursorPos, _tb.Text.Length);
            isValidatingEnabled = true;
        }

        private void validateAddressBasic()
        {
            //string[] nameParts;
            StringBuilder fullAddress = new StringBuilder();
            //int cursorPos = 0;
            if (_tb.Text.Equals(""))
            {
                return;
            }

            isValidatingEnabled = false;
            validateText("-'./,", "", true, true, false, true, CharacterCase.Any, 256);
            //cursorPos = _tb.SelectionStart;
            //nameParts = FormattingUtils.GetStringTokens(_tb.Text, new char[]{' ', '-'}, true);
            //foreach (string nameStr in nameParts)
            //{
            //    fullAddress.Append(nameStr.Substring(0, 1).ToUpper);
            //    if (nameStr.Length > 1)
            //    {
            //        fullAddress.Append(nameStr.Substring(1).ToLower);
            //    }
            //}

            //_tb.Text = fullAddress.ToString();
            //_tb.SelectionStart = Math.Min(cursorPos, _tb.Text.Length);
            isValidatingEnabled = true;
        }

        private void validateEmail()
        {
            string localPart, domain;
            int cursorPos = _tb.SelectionStart;
            isValidatingEnabled = false;
            if (_tb.Text.Contains("@"))
            {
                localPart = _tb.Text.Substring(0, _tb.Text.IndexOf("@"));
                domain = _tb.Text.Substring(_tb.Text.IndexOf("@") + 1);
                _tb.Text = localPart;
                validateText(".!#$%&'*+-/=?^_`{|}~", "", true, true, false, false, CharacterCase.Any, 64);
                localPart = _tb.Text;
                _tb.Text = domain;
                validateText(".-_", "", true, true, false, false, CharacterCase.Any, 254);
                domain = _tb.Text;
                _tb.Text = localPart + "@" + domain;
                validateText("@.!#$%&'*+-/=?^_`{|}~", "", true, true, false, false, CharacterCase.Any, 254);
            }
            else
            {
                validateText(".!#$%&'*+-/=?^_`{|}~", "", true, true, false, false, CharacterCase.Any, 64);
            }

            _tb.SelectionStart = Math.Min(cursorPos, _tb.Text.Length);
            isValidatingEnabled = true;
        }

        private void validatePhoneNumber()
        {
            StringBuilder output = new StringBuilder();
            int cursorPos = 0;
            bool isMaintainCursorPos = false;
            //TextBoxValidationType tempValidationType = ValidationType;
            string backup = _tb.Text;
            if (_tb.SelectionStart < _tb.Text.Length)
            {
                isMaintainCursorPos = true;
                cursorPos = _tb.SelectionStart;
            }

            isValidatingEnabled = false;

            if (_tb.Text.Length <= 1)
                ValidationType = TextBoxValidationType.PhoneNumberAny;

            if (ValidationType == TextBoxValidationType.PhoneNumberAny)
            {
                validateText("+()", "", false, true, false, true, CharacterCase.Any, 20);
            }
            else if (ValidationType == TextBoxValidationType.PhoneNumberMobile | ValidationType == TextBoxValidationType.PhoneNumberNational)
            {
                validateText("", "", false, true, false, false, CharacterCase.Any, 20);
            }
            else
            {
                throw new Exception("Unexpected validation type '" + ValidationType.ToString() + "' while validating a phone number.");
            }

            if (ValidationType == TextBoxValidationType.PhoneNumberAny & _tb.Text.Length >= 2)
            {
                if (_tb.Text.StartsWith("04"))
                {
                    ValidationType = TextBoxValidationType.PhoneNumberMobile;
                }
                else if (_tb.Text.StartsWith("0") & !_tb.Text.StartsWith("00"))
                {
                    ValidationType = TextBoxValidationType.PhoneNumberNational;
                }
                else
                {
                    _tb.Text = backup;
                    return;
                }
            }

            switch (ValidationType)
            {
                case TextBoxValidationType.PhoneNumberAny:
                    output.Append(_tb.Text);
                    if (!isMaintainCursorPos)
                    {
                        cursorPos += _tb.Text.Length;
                    }
                    break;

                case TextBoxValidationType.PhoneNumberMobile:
                    if (!isMaintainCursorPos)
                    {
                        cursorPos += 5;
                    }
                    //append the first 4 digits of a mobile number
                    if (appendChunk(output, 0, 4, "", "", "04"))
                    {
                        if (!isMaintainCursorPos)
                        {
                            cursorPos += 4;
                        }

                        //append the 5-7 digits of a mobile number
                        if (appendChunk(output, 4, 3, " ", ""))
                        {
                            if (!isMaintainCursorPos)
                            {
                                cursorPos += 3;
                            }

                            //append the final 3 digits of a mobile number
                            appendChunk(output, 7, 3, " ", "");
                        }
                    }
                    break;

                case TextBoxValidationType.PhoneNumberNational:

                    if (!isMaintainCursorPos)
                    {
                        cursorPos += 5;
                    }

                    if (appendChunk(output, 0, 2, "(", ")"))
                    {
                        if (!isMaintainCursorPos)
                        {
                            cursorPos += 5;
                        }

                        if (appendChunk(output, 2, 4, " ", ""))
                        {
                            if (!isMaintainCursorPos)
                            {
                                cursorPos += 4;
                            }

                            appendChunk(output, 6, 4, " ", "");
                        }
                    }
                    break;

                default:
                    throw new Exception("Validation type not configured for validating phone numbers: " + ValidationType.ToString() + ".");
            }

            _tb.Text = output.ToString();
            _tb.SelectionStart = Math.Min(cursorPos, _tb.Text.Length);
            isValidatingEnabled = true;
        }

        private void validateDoubleAmount(bool negativeAllowed, bool alwaysNegative)
        {
            int cursorPos = _tb.SelectionStart;
            if (_tb.Text.Equals(""))
            {
                return;
            }

            isValidatingEnabled = false;
            int decPointPos = -1;
            if (_tb.Text.Contains("."))
            {
                decPointPos = _tb.Text.IndexOf(".");
            }

            if (_tb.Text.Substring(0, 1).Equals("-") & negativeAllowed)
            {
                alwaysNegative = true;
            }

            validateText("", "", false, true, false, false, CharacterCase.Any, 100);
            if (alwaysNegative)
            {
                _tb.Text = "-" + _tb.Text;
            }

            if (decPointPos > -1)
            {
                string firstHalf = "";
                string secondhalf = "";
                if (decPointPos > 0)
                {
                    firstHalf = _tb.Text.Substring(0, decPointPos);
                }

                if (decPointPos < _tb.Text.Length)
                {
                    secondhalf = _tb.Text.Substring(decPointPos);
                }

                _tb.Text = firstHalf + "." + secondhalf;
            }

            _tb.SelectionStart = Math.Max(2, Math.Min(cursorPos, _tb.Text.Length));
            isValidatingEnabled = true;
        }

        private void validateDollarAmount(bool negativeAllowed)
        {
            int cursorPos = _tb.SelectionStart;
            StringBuilder output = new StringBuilder();
            int dollars = 0;
            bool isNegative = false;
            isValidatingEnabled = false;
            if (negativeAllowed & _tb.Text.Contains("-") & !_tb.Text.Contains("+"))
            {
                isNegative = true;
            }
            else if (negativeAllowed & _tb.Text.Contains("-") & _tb.Text.Contains("+"))
            {
                cursorPos = Math.Max(0, cursorPos - 2);
            }

            validateText(".", "", false, true, false, false, CharacterCase.Any, 9);
            if (isNegative)
            {
                output.Append("-");
            }

            // output.Append("$");
            if (_tb.Text.Contains("."))
            {
                if (_tb.Text.IndexOf(".") > 0)
                {
                    dollars = int.Parse(_tb.Text.Substring(0, _tb.Text.IndexOf(".")));
                }

                output.Append(dollars);
                output.Append(".");
                if (_tb.Text.Length > _tb.Text.IndexOf(".") + 2)
                {
                    output.Append(_tb.Text.Substring(_tb.Text.IndexOf(".") + 1, 2));
                }
                else if (_tb.Text.Length > _tb.Text.IndexOf(".") + 1)
                {
                    output.Append(_tb.Text.Substring(_tb.Text.IndexOf(".") + 1, 1));
                    output.Append("0");
                }
                else
                {
                    output.Append("00");
                }
            }
            else
            {
                if (_tb.Text.Equals(""))
                {
                    if (!isNegative)
                    {
                        output.Append("0");
                    }
                }
                else
                {
                    output.Append(_tb.Text);
                }

                output.Append(".00");
            }

            _tb.Text = output.ToString();
            _tb.SelectionStart = Math.Max(1, Math.Min(cursorPos, _tb.Text.Length));
            isValidatingEnabled = true;
        }

        private bool appendChunk(StringBuilder output, int startIndex, int length, string preSeperator, string postSeperator, string fixedValue = "", int fixedValueStart = 0)
        {
            string tempStr = "";
            fixedValueStart += startIndex;
            output.Append(preSeperator);
            if (_tb.Text.Length > startIndex)
            {
                tempStr = _tb.Text.Substring(startIndex, Math.Min(_tb.Text.Length - startIndex, length));
                if (fixedValue.Length <= length & fixedValue.Length > 0 & fixedValueStart < _tb.Text.Length)
                {
                    int fixedValueFinish = Math.Min(_tb.Text.Length - fixedValueStart, fixedValueStart + fixedValue.Length);
                    tempStr = tempStr.Remove(fixedValueStart, fixedValueFinish - fixedValueStart);
                    output.Append(tempStr.Insert(fixedValueStart, fixedValue.Substring(0, fixedValueFinish)));
                }
                else
                {
                    output.Append(tempStr);
                }
            }

            if (_tb.Text.Length > startIndex + length)
            {
                output.Append(postSeperator);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}