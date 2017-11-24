using System;
using System.Collections.Generic;


namespace ObjectPrinting
{
    public class MemberPrintingConfig<TOwner, TPropType> : SelectedEntityPrintingConfig<TOwner, TPropType>
    {
        private readonly string propertyName;

        internal MemberPrintingConfig(PrintingConfig<TOwner> printingConfig, string propertyName) :
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
