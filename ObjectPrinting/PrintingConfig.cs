using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;


namespace ObjectPrinting
{
    public interface IPrintingConfig<TOwner>
    {
        ImmutableHashSet<Type> ExcludedTypes { get; }
        ImmutableDictionary<Type, Delegate> CustomTypePrinters { get; }
        ImmutableHashSet<string> ExcludedMembers { get; }
        ImmutableDictionary<string, Delegate> CustomMemberPrinters { get; }
    }
    
    public class PrintingConfig<TOwner> : IPrintingConfig<TOwner>
    {
        //TODO RV(atolstov): почему public а не private?
        //  стоило бы сюда добавить либо явную реализацию интерфейса, содержащего эти свойства
        //  либо перенести в данный класс базовые методы, возвращающие его измененные копии
        //  иначе список подсказок VS будет засоряться лишними полями
        ImmutableHashSet<Type> IPrintingConfig<TOwner>.ExcludedTypes => excludedTypes;
        ImmutableDictionary<Type, Delegate> IPrintingConfig<TOwner>.CustomTypePrinters => customTypePrinters;
        ImmutableHashSet<string> IPrintingConfig<TOwner>.ExcludedMembers => excludedMembers;
        ImmutableDictionary<string, Delegate> IPrintingConfig<TOwner>.CustomMemberPrinters => customMemberPrinters;

        private readonly ImmutableHashSet<Type> excludedTypes;
        private readonly ImmutableDictionary<Type, Delegate> customTypePrinters;
        private readonly ImmutableHashSet<string> excludedMembers;
        private readonly ImmutableDictionary<string, Delegate> customMemberPrinters;

        public PrintingConfig()
        {
            excludedTypes = ImmutableHashSet.Create<Type>();
            customTypePrinters = ImmutableDictionary.Create<Type, Delegate>();
            excludedMembers = ImmutableHashSet.Create<string>();
            customMemberPrinters = ImmutableDictionary.Create<string, Delegate>();
        }

        public PrintingConfig(ImmutableHashSet<Type> excludedTypes,
            ImmutableDictionary<Type, Delegate> customTypePrinters,
            ImmutableHashSet<string> excludedMembers,
            ImmutableDictionary<string, Delegate> customMemberPrinters)
        {
            this.excludedTypes = excludedTypes;
            this.customTypePrinters = customTypePrinters;
            this.excludedMembers = excludedMembers;
            this.customMemberPrinters = customMemberPrinters;
        }

        public PrintingConfig<TOwner> Excluding<TPropType>()
        {
            return new PrintingConfig<TOwner>(
                    excludedTypes.Add(typeof(TPropType)),
                    customTypePrinters,
                    excludedMembers,
                    customMemberPrinters
                );
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var memberName = GetMemberName(memberSelector);
            return new PrintingConfig<TOwner>(
                excludedTypes,
                customTypePrinters,
                excludedMembers.Add(memberName),
                customMemberPrinters
            );
        }

        public SelectedTypePrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new SelectedTypePrintingConfig<TOwner, TPropType>(this);
        }

        public SelectedMemberPrintingConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var memberName = GetMemberName(memberSelector);
            return new SelectedMemberPrintingConfig<TOwner, TPropType>(this, memberName);
        }

        public string PrintToString(TOwner obj)
        {
            return new ObjectPrinter<TOwner>(this)
                .PrintToString(obj);
        }

        private static string GetMemberName<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var propInfo = ((MemberExpression) memberSelector.Body)
                .Member as PropertyInfo;
            return propInfo.Name;
        }
    }
}
