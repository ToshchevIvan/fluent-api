using System;
using System.Collections.Generic;


namespace ObjectPrinting
{
    public class SelectedMemberPrintingConfig<TOwner, TPropType> : SelectedEntityPrintingConfig<TOwner, TPropType>
    {
        private readonly string propertyName;

        internal SelectedMemberPrintingConfig(IPrintingConfig<TOwner> printingConfig, string propertyName) :
            base(printingConfig)
        {
            this.propertyName = propertyName;
        }

        public override PrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            return new PrintingConfig<TOwner>(
                PrintingConfig.ExcludedTypes,
                PrintingConfig.CustomTypePrinters,
                PrintingConfig.ExcludedMembers,
                PrintingConfig.CustomMemberPrinters.Add(propertyName, print)
            );
        }
    }
}
