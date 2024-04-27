using UnityEngine;

namespace UICore
{
    /// <summary>
    ///   <para> UIForm is a base class for all UI elements - windows, buttons, widgets etc..</para>
    /// </summary>
    public class UIForm : MonoBehaviour //TODO Implement alpha and gradient settings.
    {
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
