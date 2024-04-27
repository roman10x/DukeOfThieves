namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class IdAttribute : System.Attribute
    {
        public string Id { get; }

        /// <summary>
        /// Serializable field name for the property id
        /// </summary>
        /// <param name="id">Field name</param>
        public IdAttribute(string id)
        {
            Id = id;
        }
    }
}