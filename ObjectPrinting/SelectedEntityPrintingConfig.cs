using System;


namespace ObjectPrinting
{
    public abstract class SelectedEntityPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        protected readonly IPrintingConfig<TOwner> PrintingConfig;

        internal SelectedEntityPrintingConfig(IPrintingConfig<TOwner> printingConfig)
        {
            PrintingConfig = printingConfig;
        }

        public abstract PrintingConfig<TOwner> Using(Func<TPropType, string> print);
        
        IPrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.ParentConfig => PrintingConfig;
    }


    public interface IPropertyPrintingConfig<TOwner, TPropType>
    {
        IPrintingConfig<TOwner> ParentConfig { get; }
    }
}
