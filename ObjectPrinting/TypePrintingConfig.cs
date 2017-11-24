using System;
using System.Collections.Generic;


namespace ObjectPrinting
{
    public class TypePrintingConfig<TOwner, TPropType> : SelectedEntityPrintingConfig<TOwner, TPropType>
    {
        internal TypePrintingConfig(PrintingConfig<TOwner> printingConfig) : base(printingConfig)
        {
        }

        public override PrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            return new PrintingConfig<TOwner>(
                PrintingConfig.ExcludedTypes,
                PrintingConfig.CustomTypePrinters.Add(typeof(TPropType), print),
                PrintingConfig.ExcludedMembers,
                PrintingConfig.CustomMemberPrinters
            );
        }
    }
}
