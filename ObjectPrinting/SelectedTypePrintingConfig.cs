using System;
using System.Collections.Generic;


namespace ObjectPrinting
{
    //TODO RV(atolstov): наверное StringEntityPrintingConfig?
    // Почему же? Настраивается печать конкретного типа (TPropType), а не строки
    public class SelectedTypePrintingConfig<TOwner, TPropType> : SelectedEntityPrintingConfig<TOwner, TPropType>
    {
        internal SelectedTypePrintingConfig(IPrintingConfig<TOwner> printingConfig) : base(printingConfig)
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
