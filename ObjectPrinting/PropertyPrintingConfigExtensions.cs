using System;
using System.Globalization;


namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtensions
    {
        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> config)
        {
            return config(ObjectPrinter.For<T>()).PrintToString(obj);
        }

        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(
            this PropertyPrintingConfig<TOwner, string> propConfig, int maxLen)
        {
            return propConfig.Using(s => s.Substring(0, Math.Min(s.Length, maxLen)));
        }
        
        public static PrintingConfig<TOwner> Using<TOwner>(this SelectedEntityPrintingConfig<TOwner, int> propConfig, 
            CultureInfo culture)
        {
            return propConfig.Using(x => x.ToString(culture));
        }
        
        public static PrintingConfig<TOwner> Using<TOwner>(this SelectedEntityPrintingConfig<TOwner, long> propConfig, 
            CultureInfo culture)
        {
            return propConfig.Using(x => x.ToString(culture));
        }

        public static PrintingConfig<TOwner> Using<TOwner>(this SelectedEntityPrintingConfig<TOwner, float> propConfig, 
            CultureInfo culture)
        {
            return propConfig.Using(x => x.ToString(culture));
        }

        public static PrintingConfig<TOwner> Using<TOwner>(this SelectedEntityPrintingConfig<TOwner, double> propConfig, 
            CultureInfo culture)
        {
            return propConfig.Using(x => x.ToString(culture));
        }
        
        // uint, ulong, short, etc.
    }
}
