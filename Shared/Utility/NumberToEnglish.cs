/*
   Author  : Nguyen Phuoc Vinh
   Email   : vinhnp_it@daico-furniture.com
   Date    : 28/01/2011
   Company :  Dai Co   
 */
using System;

namespace DaiCo.Shared.Utility
{
  /// <summary>
  /// Change Currency into words
  /// </summary>
  public static class NumberToEnglish
  {
    /// <summary>
    /// Change Numeric To Words
    /// </summary>
    /// <param name="numb">number in double</param>
    /// <returns>number in words</returns>
    public static string ChangeNumericToWords(double numb)
    {
      String num = numb.ToString();
      return ChangeToWords(num, false);
    }

    /// <summary>
    /// Change Currency To Words
    /// </summary>
    /// <param name="numb">Currency in string</param>
    /// <returns>Currence in words</returns>
    public static string ChangeCurrencyToWords(string numb)
    {
      return ChangeToWords(numb, true);
    }

    /// <summary>
    /// Change Numeric To Words
    /// </summary>
    /// <param name="numb">Numeric in string</param>
    /// <returns>Numeric in words</returns>
    public static string ChangeNumericToWords(string numb)
    {
      return ChangeToWords(numb, false);
    }

    /// <summary>
    /// Change Currency To Words
    /// </summary>
    /// <param name="numb">Currency in double</param>
    /// <returns>Currence in words</returns>
    public static string ChangeCurrencyToWords(double numb)
    {
      return ChangeToWords(numb.ToString(), true);
    }

    /// <summary>
    /// Change To Words
    /// </summary>
    /// <param name="numb"></param>
    /// <param name="isCurrency"></param>
    /// <returns></returns>
    private static string ChangeToWords(string numb, bool isCurrency)
    {
      String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
      String endStr = (isCurrency) ? ("Only") : ("");
      try
      {
        int decimalPlace = numb.IndexOf(".");
        if (decimalPlace > 0)
        {
          wholeNo = numb.Substring(0, decimalPlace);
          points = numb.Substring(decimalPlace + 1);
          if (Convert.ToInt32(points) > 0)
          {
            andStr = (isCurrency) ? ("and") : ("point");// just to separate whole numbers from points/cents
            endStr = (isCurrency) ? ("Cents " + endStr) : ("");
            pointStr = TranslateCents(points);
          }
        }
        val = String.Format("{0} {1}{2} {3}", TranslateWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
      }
      catch {; }
      return val;
    }

    /// <summary>
    /// Translate Whole Number
    /// </summary>
    /// <param name="number">very char in string source</param>
    /// <returns>numice in word</returns>
    private static string TranslateWholeNumber(string number)
    {
      string word = "";
      try
      {
        bool beginsZero = false;//tests for 0XX
        bool isDone = false;//test if already translated
        double dblAmt = (Convert.ToDouble(number));
        //if ((dblAmt > 0) && number.StartsWith("0"))
        if (dblAmt > 0)
        {//test for zero or digit zero in a nuemric
          beginsZero = number.StartsWith("0");
          int numDigits = number.Length;
          int pos = 0;//store digit grouping
          String place = "";//digit grouping name:hundres,thousand,etc...
          switch (numDigits)
          {
            case 1://ones' range
              word = Ones(number);
              isDone = true;
              break;
            case 2://tens' range
              word = Tens(number);
              isDone = true;
              break;
            case 3://hundreds' range
              pos = (numDigits % 3) + 1;
              place = " Hundred ";
              break;
            case 4://thousands' range
            case 5:
            case 6:
              pos = (numDigits % 4) + 1;
              place = " Thousand ";
              break;
            case 7://millions' range
            case 8:
            case 9:
              pos = (numDigits % 7) + 1;
              place = " Million ";
              break;
            case 10://Billions's range
              pos = (numDigits % 10) + 1;
              place = " Billion ";
              break;
            //add extra case options for anything above Billion...
            default:
              isDone = true;
              break;
          }
          if (!isDone)
          {//if transalation is not done, continue...(Recursion comes in now!!)
            if (number.Substring(0, pos).Equals("0"))
            {
              place = string.Empty;
            }
            word = TranslateWholeNumber(number.Substring(0, pos)) + place + TranslateWholeNumber(number.Substring(pos));
            //check for trailing zeros
            if (beginsZero) word = " and " + word.Trim();
          }
          //ignore digit grouping names
          if (word.Trim().Equals(place.Trim())) word = "";
        }
      }
      catch {; }
      return word.Trim();
    }

    /// <summary>
    /// Number bigger then 10
    /// </summary>
    /// <param name="digit">digit in string</param>
    /// <returns>digit in word</returns>
    private static string Tens(string digit)
    {
      int digt = Convert.ToInt32(digit);
      String name = null;
      switch (digt)
      {
        case 10:
          name = "Ten";
          break;
        case 11:
          name = "Eleven";
          break;
        case 12:
          name = "Twelve";
          break;
        case 13:
          name = "Thirteen";
          break;
        case 14:
          name = "Fourteen";
          break;
        case 15:
          name = "Fifteen";
          break;
        case 16:
          name = "Sixteen";
          break;
        case 17:
          name = "Seventeen";
          break;
        case 18:
          name = "Eighteen";
          break;
        case 19:
          name = "Nineteen";
          break;
        case 20:
          name = "Twenty";
          break;
        case 30:
          name = "Thirty";
          break;
        case 40:
          name = "Forty";
          break;
        case 50:
          name = "Fifty";
          break;
        case 60:
          name = "Sixty";
          break;
        case 70:
          name = "Seventy";
          break;
        case 80:
          name = "Eighty";
          break;
        case 90:
          name = "Ninety";
          break;
        default:
          if (digt > 0)
          {
            name = Tens(digit.Substring(0, 1) + "0") + " " + Ones(digit.Substring(1));
          }
          break;
      }
      return name;
    }

    /// <summary>
    /// Digit in unit
    /// </summary>
    /// <param name="digit"></param>
    /// <returns></returns>
    private static string Ones(string digit)
    {
      int digt = Convert.ToInt32(digit);
      String name = "";
      switch (digt)
      {
        case 1:
          name = "One";
          break;
        case 2:
          name = "Two";
          break;
        case 3:
          name = "Three";
          break;
        case 4:
          name = "Four";
          break;
        case 5:
          name = "Five";
          break;
        case 6:
          name = "Six";
          break;
        case 7:
          name = "Seven";
          break;
        case 8:
          name = "Eight";
          break;
        case 9:
          name = "Nine";
          break;
      }
      return name;
    }

    /// <summary>
    /// Digit in cent
    /// </summary>
    /// <param name="cents"></param>
    /// <returns></returns>
    private static string TranslateCents(string cents)
    {
      String cts = "", digit = "", engOne = "";
      for (int i = 0; i < cents.Length; i++)
      {
        digit = cents[i].ToString();
        if (digit.Equals("0"))
        {
          engOne = "Zero";
        }
        else
        {
          engOne = Ones(digit);
        }
        cts += " " + engOne;
      }
      return cts;
    }

    /// <summary>
    /// Change US Dollar To Words
    /// </summary>
    /// <param name="numb">Currency in string</param>
    /// <returns>Currence in words</returns>
    public static string ChangeUSDollarToWords(string numb)
    {
      bool isCurrency = true;
      String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
      String endStr = (isCurrency) ? ("Only") : ("");
      try
      {
        int decimalPlace = numb.IndexOf(".");
        if (decimalPlace > 0)
        {
          wholeNo = numb.Substring(0, decimalPlace);
          points = numb.Substring(decimalPlace + 1).PadRight(2, '0');
          if (Convert.ToInt32(points) > 0)
          {
            andStr = (isCurrency) ? ("and") : ("point");// just to separate whole numbers from points/cents
            endStr = (isCurrency) ? ("Cents " + endStr) : ("");
          }
        }
        string wholeNoStr = TranslateWholeNumber(wholeNo).Trim().Substring(0, 1) + TranslateWholeNumber(wholeNo).Trim().Substring(1).ToLower();
        string pointsStr = points.Length > 0 ? " " + TranslateWholeNumber(points).Trim().ToLower() : string.Empty;
        val = String.Format("{0} US dollars {1}{2} {3}", wholeNoStr, andStr, pointsStr, endStr.ToLower());
      }
      catch {; }
      return val;
    }
  }
}

