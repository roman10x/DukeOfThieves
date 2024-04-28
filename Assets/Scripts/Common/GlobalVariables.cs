using DukeOfThieves.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace DukeOfThieves.Common
{
    public static class GlobalVariables
    {
        //Serializer Settings 
        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.None
        };
    }
}