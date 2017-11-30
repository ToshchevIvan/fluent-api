using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace ObjectPrinting
{
    public class ObjectPrinter
    {
        public static PrintingConfig<TOwner> For<TOwner>()
        {
            return new PrintingConfig<TOwner>();
        }
    }

    public class ObjectPrinter<TOwner>
    {
        private readonly IPrintingConfig<TOwner> config;

        public ObjectPrinter(IPrintingConfig<TOwner> config)
        {
            this.config = config;
        }

        public string PrintToString(TOwner obj)
        {
            return InternalPrintToString(obj, 0);
        }

        private string InternalPrintToString(object obj, int nestingLevel)
        {
            if (obj == null)
                return "null" + Environment.NewLine;
            var type = obj.GetType();
            if (type.IsPrimitive || type.IsSystemType())
                return obj + Environment.NewLine;

            var members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
                .Where(m => !config.ExcludedMembers.Contains(m.Name))
                .Select(MemberInfoHelper.Help)
                .Where(m => !config.ExcludedTypes.Contains(m.MemberType));

            var builder = new StringBuilder();
            builder.AppendLine(type.Name);
            var indentation = new string('\t', nestingLevel + 1);
            foreach (var member in members)
            {
                builder.Append(indentation);
                builder.Append(member.Name);
                builder.Append(" = ");
                var value = member.GetValue(obj);
                var printer = GetMemberSerializer(member);
                if (printer != null)
                    builder.AppendLine(printer.DynamicInvoke(value).ToString());
                else
                    builder.Append(InternalPrintToString(value, nestingLevel + 1));
            }

            return builder.ToString();
        }

        private Delegate GetMemberSerializer(IMemberInfoHelper helper)
        {
            if (config.CustomMemberPrinters.TryGetValue(helper.Name, out var printer) ||
                config.CustomTypePrinters.TryGetValue(helper.MemberType, out printer))
                return printer;
            return null;
        }
    }
}
