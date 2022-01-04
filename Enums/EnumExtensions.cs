using System.ComponentModel;
using System.Reflection;

namespace MovieProMVC.Enums
{
    public static class EnumExtensionsMethods
    {
        public static string GetDescription(this Enum enumValue)
        {
            var enumType = enumValue.GetType();
            var memberInfo = enumType.GetMember(enumValue.ToString()).First();

            if (memberInfo != null)
            {
                var displayAttribute = memberInfo.GetCustomAttribute<DescriptionAttribute>();

                if (displayAttribute != null)
                {
                    return displayAttribute.Description;
                }
            }

            return enumValue.ToString();
        }
    }
}
