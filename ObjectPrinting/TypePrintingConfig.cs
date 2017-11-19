using System;
using System.Collections.Generic;


namespace ObjectPrinting
{
    public class TypePrintingConfig<TOwner, TPropType> : SelectedEntityPrintingConfig<TOwner, TPropType>
    {
        private readonly Dictionary<Type, Delegate> customTypePrinters;
        
        internal TypePrintingConfig(PrintingConfig<TOwner> printingConfig, 
            Dictionary<Type, Delegate> customTypePrinters) : base(printingConfig)
        {
            this.customTypePrinters = customTypePrinters;
        }
        
        public override PrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            customTypePrinters.Add(typeof(TPropType), print);
            return PrintingConfig;
        }
    }
}
