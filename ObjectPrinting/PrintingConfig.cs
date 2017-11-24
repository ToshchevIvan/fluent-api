using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;


namespace ObjectPrinting
{
    //TODO RV(atolstov): Попробуй сделать данный класс Immutable.
    //          https://msdn.microsoft.com/ru-ru/library/system.collections.immutable(v=vs.111).aspx - пригодится
    //          Тогда ты всегда сможешь вынести логику "печати" в отдельный класс (Printer), а PrintingConfig передавать ему ввиде ссылки
    public class PrintingConfig<TOwner>
    {
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

<<<<<<< HEAD
<<<<<<< HEAD
        public string PrintToString(TOwner obj)
        {
            return new ObjectPrinter<TOwner>(this)
                .PrintToString(obj);
=======
=======
>>>>>>> 7d85c9f77ff8dd3079461c8959d0341a491c8f44
        //TODO RV(atolstov): Вот эту логику можно вынести в принтер
        private string PrintToString(object obj, int nestingLevel, string path) //TODO RV(atolstov): К имени такого метода стоит добавлять суффикс Internal
        {
            if (obj == null)
                return "null" + Environment.NewLine;

            var type = obj.GetType();

            var finalTypes = new[] //TODO RV(atolstov): А uint уже не finalType? Придумай другой способ определения non-user-defined классов
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
            };
            if (finalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;

            var properties = type.GetProperties() //TODO RV(atolstov): а почему поля игнорируются?
                .Where(p => !excludedTypes.Contains(p.PropertyType))
                .Where(p => !excludedProperties.Contains($"{path}.{p.Name}"));

            return type.Name + Environment.NewLine + 
                   PrintProperties(obj, properties, nestingLevel, path);
>>>>>>> 7d85c9f77ff8dd3079461c8959d0341a491c8f44
        }

        private static string GetMemberName<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var propInfo = ((MemberExpression) memberSelector.Body)
                .Member as PropertyInfo;
            return propInfo.Name;
        }
    }
}
