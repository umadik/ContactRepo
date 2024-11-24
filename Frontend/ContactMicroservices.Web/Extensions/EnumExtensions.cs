namespace ContactMicroservices.Web.Extensions
{
    public static class EnumExtensions
    {
        public static T? GetAttribute<T>(this Enum enumValue) where T : Attribute
        {
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
                if (attributes != null && attributes.Length > 0)
                {
                    return (T)attributes[0];
                }
            }
            return null;
        }
    }
}
