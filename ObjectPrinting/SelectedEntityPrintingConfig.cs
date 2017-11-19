using System;


namespace ObjectPrinting
{
    public abstract class SelectedEntityPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        protected readonly PrintingConfig<TOwner> PrintingConfig;

        internal SelectedEntityPrintingConfig(PrintingConfig<TOwner> printingConfig)
        {
            PrintingConfig = printingConfig;
        }

        public abstract PrintingConfig<TOwner> Using(Func<TPropType, string> print);
        
        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.ParentConfig => PrintingConfig;
    }


    public interface IPropertyPrintingConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> ParentConfig { get; }
    }
}
