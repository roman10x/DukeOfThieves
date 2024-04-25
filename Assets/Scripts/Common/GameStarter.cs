#if UNITY_EDITOR
using UnityEditor;

namespace DukeOfThieves.Common
{
    public class GameStarter
    {
        [MenuItem("DoT/Start Game", priority = -2)]
        public static void StartGame()
        {
            EditorApplication.isPlaying = true;
            EditorApplication.OpenScene("Assets/Scenes/InitialScene.unity");
        }
    }
}
#endif