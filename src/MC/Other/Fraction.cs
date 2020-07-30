using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;

namespace MC.Other
{
    [Serializable]
    public class Fraction
    {

        // +----------------------------------------------+
        // | Properties                                    |
        // +----------------------------------------------+
         
        public double Numerator
        {
            get
            {
                return _numerator;
            }
            set
            {
                _numerator = value;
            }
        }
        private double _numerator;

        public double Denominator
        {
            get
            {
                return _denominator;
            }
            set
            {
                if (value != 0)
                {
                    _denominator = value;
                }
                else
                {
                    Error("Only Chuck Norris can divide by zero!");
                }
            }
        }
        private double _denominator;


        // +----------------------------------------------+
        // | Constructors                                  |
        // +----------------------------------------------+

        public Fraction()
        {
            try
            {
                Initialize(0);
            }
            catch (Exception ex)
            {
                Error("Invalid fraction: " + ex.Message);
                Numerator = 0;
                Denominator = 1;
            }
        }

        public Fraction(int i)
        {
            Initialize((double)i);
        }

        public Fraction(double d)
        {
            try
            {
                Initialize(d);
            }
            catch (Exception ex)
            {
                Error("Invalid fraction: " + ex.Message);
                Numerator = 0;
                Denominator = 1;
            }
        }

        public Fraction(Fraction fraction)
        {
            try
            {
                Initialize(fraction.Numerator, fraction.Denominator);
            }
            catch (Exception ex)
            {
                Error("Invalid fraction: " + ex.Message);
                Numerator = 0;
                Denominator = 1;
            }
        }

        public Fraction(double numerator, double denominator)
        {
            try
            {
                Initialize(numerator, denominator);
            }
            catch (Exception ex)
            {
                Error("Invalid fraction: " + ex.Message);
                Numerator = 0;
                Denominator = 1;
            }
        }

        public Fraction(string s)
        {
            try
            {
                s = s.Replace(",", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);
                s = s.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);

                string[] str = s.Split('/');
                for (int i = 0; i < str.GetLength(0); i++)
                {
                    str[i] = str[i].Replace("(", "");
                    str[i] = str[i].Replace(")", "");
                    str[i] = str[i].Replace("[", "");
                    str[i] = str[i].Replace("]", "");
                    str[i] = str[i].Replace("{", "");
                    str[i] = str[i].Replace("}", "");

                    if (str[i].Contains("+"))
                    {
                        str[i] = Convert.ToString(Convert.ToDouble(str[i].Split('+')[0]) + Convert.ToDouble(str[i].Split('+')[1]));
                    }
                    if (str[i].Contains("-") && str[i].Split('-')[0] != "")
                    {
                        str[i] = Convert.ToString(Convert.ToDouble(str[i].Split('-')[0]) - Convert.ToDouble(str[i].Split('-')[1]));
                    }
                    if (str[i].Contains("*"))
                    {
                        str[i] = Convert.ToString(Convert.ToDouble(str[i].Split('*')[0]) * Convert.ToDouble(str[i].Split('*')[1]));
                    }
                    if (str[i].Contains("/"))
                    {
                        str[i] = Convert.ToString(Convert.ToDouble(str[i].Split('/')[0]) / Convert.ToDouble(str[i].Split('/')[1]));
                    }
                }
                if (str.GetLength(0) == 1)
                {                    
                    Initialize(Convert.ToDouble(str[0]));
                }
                else if (str.GetLength(0) == 2)
                {
                    Initialize(Convert.ToDouble(str[0]), Convert.ToDouble(str[1]));
                }
                else
                {
                    Error("Invalid fraction");
                }
            }
            catch (Exception ex)
            {
                Error("Invalid fraction: " + ex.Message);
                Numerator = 0;
                Denominator = 1;
            }
        }


        // +----------------------------------------------+
        // | Calculations                                  |
        // +----------------------------------------------+

        private void Initialize(double d)
        {
            if (d % 1 == 0)
            {
                Numerator = d;
                Denominator = 1;
            }
            else
            {
                double tmp = d;
                double multiple = 1;

                while (tmp.ToString().Contains("E-"))
                {
                    tmp *= 10;
                    multiple *= 10;
                }

                while (tmp.ToString().Contains("E+"))
                {
                    tmp /= 10;
                    multiple /= 10;
                }

                int decIndex = tmp.ToString().Length - 1 - tmp.ToString().IndexOf('.');
                Numerator = tmp * 10 * decIndex;
                Denominator = multiple * 10 * decIndex;
            }            
            Reduce();
        }

        private void Initialize(double numerator, double denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
            Reduce();
        }

        private void Reduce()
        {
            if (Denominator != 1)
            {
                while (Numerator % 1 != 0 || Denominator % 1 != 0)
                {
                    Numerator *= 10;
                    Denominator *= 10;
                }

                if (Numerator == 0)
                {
                    Denominator = 1;
                }
                else
                {
                    double GCD = GreatestCommonDivisor(Numerator, Denominator);
                    Numerator /= GCD;
                    Denominator /= GCD;
                }

                if (Denominator < 0)
                {
                    Numerator = -Numerator;
                    Denominator = -Denominator;
                }
            }
        }

        private double GreatestCommonDivisor(double d1, double d2)
        {
            d1 = Math.Abs(d1);
            d2 = Math.Abs(d2);
            
            while (d1 != 0)
            {            
                if (d1 < d2)
                {
                    double tmp = d1;
                    d1 = d2;
                    d2 = tmp;
                }
                d1 %= d2;
            }
            return d2;
        }

        
        // +----------------------------------------------+
        // | Output                                        |
        // +----------------------------------------------+
        
        public override string ToString()
        {
            try
            {
                if (Denominator == 1)
                {
                    return Numerator.ToString();
                }
                else
                {
                    return Numerator.ToString() + "/" + Denominator.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogLow("Error converting Fraction to string. " + ex.Message);
                return "0";
            }
        }

        public string ToPython()
        {
            try
            {
            if (Denominator == 1)
            {
                return Numerator.ToString() + ".";
            }
            else
            {
                return Numerator.ToString() + "./" + Denominator.ToString() + ".";
            }
            }
            catch (Exception ex)
            {
                Logger.LogLow("Error converting Fraction to Python. " + ex.Message);
                return "0.";
            }
        }

        public decimal ToDecimal()
        {
            return (decimal)Numerator / (decimal)Denominator;
        }

        public string ToDecimalString()
        {
            return ToDouble().ToString();
        }

        public string ToExcelString()
        {
            if (Denominator == 1)
            {
                return Numerator.ToString();
            }
            else
            {
                return "=" + Numerator.ToString() + "/" + Denominator.ToString();
            }
        }

        public double ToDouble()
        {
            try
            {
                if (Denominator != 0)
                {
                    return Numerator / Denominator;
                }
                else
                {
                    return double.NaN;
                }
            }
            catch (Exception ex)
            {
                Logger.LogLow("Error converting Fraction to double. " + ex.Message);
                return 0;
            }
        }


        // +----------------------------------------------+
        // | Operators                                     |
        // +----------------------------------------------+

        public static Fraction operator +(Fraction f1, Fraction f2)
        {
            return new Fraction((f1.Denominator * f2.Numerator) + (f1.Numerator * f2.Denominator), f1.Denominator * f2.Denominator);
        }

        public static Fraction operator +(double d, Fraction f)
        {
            Fraction tmp = new Fraction(d);
            return new Fraction((tmp.Denominator * f.Numerator) + (tmp.Numerator * f.Denominator), tmp.Denominator * f.Denominator);
        }

        public static Fraction operator +(Fraction f, double d)
        {
            Fraction tmp = new Fraction(d);
            return new Fraction((f.Denominator * tmp.Numerator) + (f.Numerator * tmp.Denominator), f.Denominator * tmp.Denominator);
        }

        public static Fraction operator -(Fraction f1, Fraction f2)
        {
            return new Fraction((f1.Denominator * f2.Numerator) - (f1.Numerator * f2.Denominator), f1.Denominator * f2.Denominator);
        }

        public static Fraction operator -(double d, Fraction f)
        {
            Fraction tmp = new Fraction(d);
            return new Fraction((tmp.Denominator * f.Numerator) - (tmp.Numerator * f.Denominator), tmp.Denominator * f.Denominator);
        }

        public static Fraction operator -(Fraction f, double d)
        {
            Fraction tmp = new Fraction(d);
            return new Fraction((f.Denominator * tmp.Numerator) - (f.Numerator * tmp.Denominator), f.Denominator * tmp.Denominator);
        }
        
        public static Fraction operator *(Fraction f1, Fraction f2)
        {
            return new Fraction(f1.Numerator * f2.Numerator, f1.Denominator * f2.Denominator);
        }

        public static Fraction operator *(double d, Fraction f)
        {
            Fraction tmp = new Fraction(d);
            return new Fraction(tmp.Numerator * f.Numerator, tmp.Denominator * f.Denominator);
        }

        public static Fraction operator *(Fraction f, double d)
        {
            Fraction tmp = new Fraction(d);
            return new Fraction(f.Numerator * tmp.Numerator, f.Denominator * tmp.Denominator);
        }

        public static Fraction operator /(Fraction f1, Fraction f2)
        {
            return new Fraction(f1.Numerator * f2.Denominator, f1.Denominator * f2.Numerator);
        }

        public static Fraction operator /(double d, Fraction f)
        {
            Fraction tmp = new Fraction(d);
            return new Fraction(tmp.Numerator * f.Denominator, tmp.Denominator * f.Numerator);
        }

        public static Fraction operator /(Fraction f, double d)
        {
            Fraction tmp = new Fraction(d);
            return new Fraction(f.Numerator * tmp.Denominator, f.Denominator * tmp.Numerator);
        }

        public static bool operator ==(Fraction f1, Fraction f2)
        {
            return ((object)f1 == null || (object)f2 == null) ? false : f1.ToDouble() == f2.ToDouble();
        }

        public static bool operator ==(double d, Fraction f)
        {
            return (object)f == null ? false : f.ToDouble() == d;
        }

        public static bool operator ==(Fraction f, double d)
        {
            return (object)f == null ? false : f.ToDouble() == d;
        }

        public static bool operator !=(Fraction f1, Fraction f2)
        {
            return ((object)f1 == null || (object)f2 == null) ? true : f1.ToDouble() != f2.ToDouble();
        }

        public static bool operator !=(double d, Fraction f)
        {
            return (object)f == null ? true : f.ToDouble() != d;
        }

        public static bool operator !=(Fraction f, double d)
        {
            return (object)f == null ? true : f.ToDouble() != d;
        }

        public static bool operator <=(Fraction f1, Fraction f2)
        {
            return f1.ToDouble() <= f2.ToDouble();
        }

        public static bool operator <=(double d, Fraction f)
        {
            return f.ToDouble() <= d;
        }

        public static bool operator <=(Fraction f, double d)
        {
            return f.ToDouble() <= d;
        }

        public static bool operator >=(Fraction f1, Fraction f2)
        {
            return f1.ToDouble() >= f2.ToDouble();
        }

        public static bool operator >=(double d, Fraction f)
        {
            return f.ToDouble() >= d;
        }

        public static bool operator >=(Fraction f, double d)
        {
            return f.ToDouble() >= d;
        }
        
        public static bool operator <(Fraction f1, Fraction f2)
        {
            return f1.ToDouble() < f2.ToDouble();
        }

        public static bool operator <(double d, Fraction f)
        {
            return f.ToDouble() < d;
        }

        public static bool operator <(Fraction f, double d)
        {
            return f.ToDouble() < d;
        }

        public static bool operator >(Fraction f1, Fraction f2)
        {
            return f1.ToDouble() > f2.ToDouble();
        }

        public static bool operator >(double d, Fraction f)
        {
            return f.ToDouble() > d;
        }

        public static bool operator >(Fraction f, double d)
        {
            return f.ToDouble() > d;
        }
        

        // +----------------------------------------------+
        // | Other                                         |
        // +----------------------------------------------+

        private void Error(string str)
        {
            MessageBox.Show(str, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public override bool Equals(object obj)
        {
            try
            {
                Fraction frac = (Fraction)obj;
                return ToDouble() == frac.ToDouble();
            }
            catch (Exception ex)
            {
                Logger.LogLow(ex.Message);
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (int)(ToDouble() * 1000000);
        }

        /// <summary>
        /// Test if a certain object can be converted to a fraction
        /// </summary>        
        public static bool IsFraction(object o)
        {
            bool fracOk;
            try
            {
                string s = Convert.ToString(o);

                s = s.Replace(",", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);
                s = s.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);

                string[] str = s.Split('/');
                for (int i = 0; i < str.GetLength(0); i++)
                {
                    str[i] = str[i].Replace("(", "");
                    str[i] = str[i].Replace(")", "");
                    str[i] = str[i].Replace("[", "");
                    str[i] = str[i].Replace("]", "");
                    str[i] = str[i].Replace("{", "");
                    str[i] = str[i].Replace("}", "");

                    if (str[i].Contains("+"))
                    {
                        str[i] = Convert.ToString(Convert.ToDouble(str[i].Split('+')[0]) + Convert.ToDouble(str[i].Split('+')[1]));
                    }
                    if (str[i].Contains("-") && str[i].Split('-')[0] != "")
                    {
                        str[i] = Convert.ToString(Convert.ToDouble(str[i].Split('-')[0]) - Convert.ToDouble(str[i].Split('-')[1]));
                    }
                    if (str[i].Contains("*"))
                    {
                        str[i] = Convert.ToString(Convert.ToDouble(str[i].Split('*')[0]) * Convert.ToDouble(str[i].Split('*')[1]));
                    }
                    if (str[i].Contains("/"))
                    {
                        str[i] = Convert.ToString(Convert.ToDouble(str[i].Split('/')[0]) / Convert.ToDouble(str[i].Split('/')[1]));
                    }
                }
                if (str.GetLength(0) == 1)
                {
                    double d = Convert.ToDouble(str[0]);
                }
                else if (str.GetLength(0) == 2)
                {
                    double d1 = Convert.ToDouble(str[0]);
                    double d2 = Convert.ToDouble(str[1]);
                }

                fracOk = true;
            }
            catch (Exception ex)
            {
                fracOk = false;
                Logger.LogLow(ex.Message);
            }
            return fracOk;
        }

    }
}