using System;
using System.Collections.Generic;


namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TPropType> : SelectedEntityPrintingConfig<TOwner, TPropType>
    {
        private readonly string propertyName;
        private readonly Dictionary<string, Delegate> customPropertyPrinters;
        
        internal PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig, string propertyName, 
            Dictionary<string, Delegate> customPropertyPrinters) : base(printingConfig)
        {
            this.propertyName = propertyName;
            this.customPropertyPrinters = customPropertyPrinters;
        }

        public override PrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            customPropertyPrinters.Add(propertyName, print);
            return PrintingConfig;
        }
    }
}
