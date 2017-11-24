using System;
using System.Reflection;


namespace ObjectPrinting
{
    internal interface IMemberInfoHelper
    {
        Type MemberType { get; }
        string Name { get; }
        object GetValue(object instance);
    }
    
    internal static class MemberInfoHelper
    {
        internal static IMemberInfoHelper Help(MemberInfo info)
        {
            switch (info.MemberType)
            {
                case MemberTypes.Field:
                    return new FieldHelper {Info = (FieldInfo) info};
                case MemberTypes.Property:
                    return new PropertyHelper {Info = (PropertyInfo) info};
                default:
                    throw new ArgumentException();
            }
        }
        
        private class FieldHelper : IMemberInfoHelper
        {
            internal FieldInfo Info;
            public string Name => Info.Name;
            public Type MemberType => Info.FieldType;
            public object GetValue(object instance) => Info.GetValue(instance);
        }
        
        private class PropertyHelper : IMemberInfoHelper
        {
            internal PropertyInfo Info;
            public string Name => Info.Name;
            public Type MemberType => Info.PropertyType;
            public object GetValue(object instance) => Info.GetValue(instance);
        }
    }
}
