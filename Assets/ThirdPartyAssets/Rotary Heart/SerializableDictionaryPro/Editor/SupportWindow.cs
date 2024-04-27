using UnityEditor;

namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    public class SupportWindow : BaseSupportWindow
    {
        const string SUPPORT_FORUM = "https://forum.unity.com/threads/serializable-dictionary-pro.869377/";
        const string STORE_LINK = "https://assetstore.unity.com/packages/slug/150588";
        const string ASSET_NAME = "Serializable Dictionary Pro";
        const string VERSION = "3.3.9";

        protected override string SupportForum
        {
            get { return SUPPORT_FORUM; }
        }
        protected override string StoreLink
        {
            get { return STORE_LINK; }
        }
        protected override string AssetName
        {
            get { return ASSET_NAME; }
        }
        protected override string Version
        {
            get { return VERSION; }
        }

        [MenuItem("Tools/Rotary Heart/Serializable Dictionary/About")]
        public static void ShowWindow()
        {
            ShowWindow<SupportWindow>();
        }
    }
}