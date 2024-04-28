namespace UICore
{
    /// <summary>
    ///   <para> Collection of window prefab keys. Key must named same as addressable key</para>
    /// After creating new key connect window prefab to this window key in NorasArk/Windows Storage Editor
    /// </summary>
    public enum WindowKeys
    {
        Empty = 0,
        MainMenuWindow = 1,
        GameHud = 10,
        StartLevelPopUp = 20, 
        PausePopUp = 30,
        TapToStartPopUp = 40
    }
}