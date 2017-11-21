using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;


namespace ObjectPrinting
{
    //TODO RV(atolstov): Попробуй сделать данный класс Immutable.
    //          https://msdn.microsoft.com/ru-ru/library/system.collections.immutable(v=vs.111).aspx - пригодится
    //          Тогда ты всегда сможешь вынести логику "печати" в отдельный класс (Printer), а PrintingConfig передавать ему ввиде ссылки
    public class PrintingConfig<TOwner>
    {
        private readonly HashSet<Type> excludedTypes = new HashSet<Type>();

        private readonly Dictionary<Type, Delegate> customTypePrinters =
            new Dictionary<Type, Delegate>();

        private readonly HashSet<string> excludedProperties = new HashSet<string>();

        private readonly Dictionary<string, Delegate> customPropertyPrinters =
            new Dictionary<string, Delegate>();

        public TypePrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new TypePrintingConfig<TOwner, TPropType>(this, customTypePrinters);
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var propName = GetMemberPath(memberSelector);
            return new PropertyPrintingConfig<TOwner, TPropType>(this, propName, customPropertyPrinters);
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            excludedProperties.Add(GetMemberPath(memberSelector));
            return this;
        }

        public PrintingConfig<TOwner> Excluding<TPropType>()
        {
            excludedTypes.Add(typeof(TPropType));
            return this;
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0, obj.GetType().Name);
        }

        public static string GetMemberPath<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            return typeof(TOwner).Name + "." + string.Join(".",
                       GetMembersOnPath(memberSelector.Body as MemberExpression)
                           .Select(m => m.Member.Name)
                           .Reverse());
        }

        private static IEnumerable<MemberExpression> GetMembersOnPath(MemberExpression expression)
        {
            while (expression != null)
            {
                yield return expression;
                expression = expression.Expression as MemberExpression;
            }
        }

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
        }

        private string PrintProperties(object obj, IEnumerable<PropertyInfo> properties, int nestingLevel, string path)
        {
            var builder = new StringBuilder();
            var indentation = new string('\t', nestingLevel + 1);
            foreach (var propertyInfo in properties)
            {
                var propertyPath = $"{path}.{propertyInfo.Name}";
                builder.Append(indentation + propertyInfo.Name + " = ");
                if (customPropertyPrinters.TryGetValue(propertyPath, out var printer) ||
                    customTypePrinters.TryGetValue(propertyInfo.PropertyType, out printer))
                {
                    builder.AppendLine(printer.DynamicInvoke(propertyInfo.GetValue(obj)).ToString());
                }
                else
                {
                    builder.Append(PrintToString(propertyInfo.GetValue(obj),
                        nestingLevel + 1, propertyPath));
                }
            }
            return builder.ToString();
        }
    }
}
