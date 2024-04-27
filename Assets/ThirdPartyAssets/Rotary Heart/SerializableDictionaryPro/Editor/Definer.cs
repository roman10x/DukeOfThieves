using System.Collections.Generic;
using UnityEditor;

namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    [InitializeOnLoad]
    public class SerializableDictionaryProDefiner : Definer
    {
        static SerializableDictionaryProDefiner()
        {
            List<string> defines = new List<string>(1)
            {
                "RH_SerializedDictionaryPro"
            };
            
            if (string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath("0e6b03a8ca9bd434fae7398312521818")))
            {
                RemoveDefines(new List<string>(1)
                {
                    "RH_SerializedDictionary"
                });
            }
            
            ApplyDefines(defines);
        }
    }
}