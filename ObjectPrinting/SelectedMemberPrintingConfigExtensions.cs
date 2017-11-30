using System;
using System.Globalization;


namespace ObjectPrinting
{
    public static class SelectedMemberPrintingConfigExtensions
    {
        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> config)
        {
            return config(ObjectPrinter.For<T>()).PrintToString(obj);
        }

        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(
            this SelectedEntityPrintingConfig<TOwner, string> propConfig, int maxLen)
        {
            return propConfig.Using(s => s.Substring(0, Math.Min(s.Length, maxLen)));
        }

        public static PrintingConfig<TOwner> Using<TOwner, TPropType>(this SelectedEntityPrintingConfig<TOwner, TPropType> propConfig, 
            CultureInfo culture) where TPropType : IFormattable
        {
            const string defaultFormat = "G";
            return propConfig.Using(x => x.ToString(defaultFormat, culture));
        }

        //TODO RV: where T : IFormattable (�� ����� � ToString � ��������� :) ). �� ��� ��� 
        public static PrintingConfig<TOwner> Using<TOwner, TPropType>(this SelectedEntityPrintingConfig<TOwner, TPropType> propConfig,
            string format, IFormatProvider provider) where TPropType : IFormattable
        {
            return propConfig.Using(x => x.ToString(format, provider));
        }
        
        // uint, ulong, short, etc.
    }
}
