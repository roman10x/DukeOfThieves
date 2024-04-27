namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class DefaultKeyAttribute : System.Attribute
    {
        public object DefaultValue { get; }
        
        public DefaultKeyAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}