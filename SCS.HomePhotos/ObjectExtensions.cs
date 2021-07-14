namespace SCS.HomePhotos
{
    /// <summary>
    /// Object class extension methods.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets an object property value.
        /// </summary>
        /// <param name="value">The target object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property value.</returns>
        public static object GetProperty(this object value, string propertyName)
        {
            return value.GetType().GetProperty(propertyName).GetValue(value, null);
        }
    }
}
