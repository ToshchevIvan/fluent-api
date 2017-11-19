﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;


namespace ObjectPrinting
{
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

        private string PrintToString(object obj, int nestingLevel, string path)
        {
            if (obj == null)
                return "null" + Environment.NewLine;

            var type = obj.GetType();

            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
            };
            if (finalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;

            var builder = new StringBuilder();
            builder.AppendLine(type.Name);
            var indentation = new string('\t', nestingLevel + 1);
            var properties = type.GetProperties()
                .Where(p => !excludedTypes.Contains(p.PropertyType))
                .Where(p => !excludedProperties.Contains($"{path}.{p.Name}"));
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
