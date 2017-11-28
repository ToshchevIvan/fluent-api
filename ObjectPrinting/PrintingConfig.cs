using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;


namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        //TODO RV(atolstov): почему public а не private?
        //  стоило бы сюда добавить либо явную реализацию интерфейса, содержащего эти свойства
        //  либо перенести в данный класс базовые методы, возвращающие его измененные копии
        //  иначе список подсказок VS будет засоряться лишними полями
        public ImmutableHashSet<Type> ExcludedTypes { get; }
        public ImmutableDictionary<Type, Delegate> CustomTypePrinters { get; }
        public ImmutableHashSet<string> ExcludedMembers { get; }
        public ImmutableDictionary<string, Delegate> CustomMemberPrinters { get; }

        public PrintingConfig()
        {
            ExcludedTypes = ImmutableHashSet.Create<Type>();
            CustomTypePrinters = ImmutableDictionary.Create<Type, Delegate>();
            ExcludedMembers = ImmutableHashSet.Create<string>();
            CustomMemberPrinters = ImmutableDictionary.Create<string, Delegate>();
        }

        public PrintingConfig(ImmutableHashSet<Type> excludedTypes,
            ImmutableDictionary<Type, Delegate> customTypePrinters,
            ImmutableHashSet<string> excludedMembers,
            ImmutableDictionary<string, Delegate> customMemberPrinters)
        {
            ExcludedTypes = excludedTypes;
            CustomTypePrinters = customTypePrinters;
            ExcludedMembers = excludedMembers;
            CustomMemberPrinters = customMemberPrinters;
        }

        public PrintingConfig<TOwner> Excluding<TPropType>()
        {
            return new PrintingConfig<TOwner>(
                    ExcludedTypes.Add(typeof(TPropType)),
                    CustomTypePrinters,
                    ExcludedMembers,
                    CustomMemberPrinters
                );
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var memberName = GetMemberName(memberSelector);
            return new PrintingConfig<TOwner>(
                ExcludedTypes,
                CustomTypePrinters,
                ExcludedMembers.Add(memberName),
                CustomMemberPrinters
            );
        }

        public TypePrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new TypePrintingConfig<TOwner, TPropType>(this);
        }

        public MemberPrintingConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var memberName = GetMemberName(memberSelector);
            return new MemberPrintingConfig<TOwner, TPropType>(this, memberName);
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
