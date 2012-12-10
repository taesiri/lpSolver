using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace LinearProgramming.Helper
{
    public static class Helpers
    {
        public static string GetDescription<T>(this object enummerationValue) where T : struct
        {
            Type type = enummerationValue.GetType();
            if (!type.IsEnum) throw new InvalidDataException("EnummerationValue must be of Enum Type");

            MemberInfo[] memberInfo = type.GetMember(enummerationValue.ToString());
            if (memberInfo.Length > 0)

            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof (DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute) attrs[0]).Description;
                }
            }
            return enummerationValue.ToString();
        }
    }
}