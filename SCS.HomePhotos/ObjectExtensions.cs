namespace SCS.HomePhotos
{
    public static class ObjectExtensions
    {
        public static object GetProperty(this object value, string propertyName)
        {
            return value.GetType().GetProperty(propertyName).GetValue(value, null);
        }
    }
}
