using System;


namespace ObjectPrinting
{
    public static class TypeExtensions
    {
        public static bool IsSystemType(this Type type) => 
            type.Assembly == typeof(object).Assembly;
    }
}
