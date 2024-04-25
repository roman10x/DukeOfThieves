using UnityEngine;

namespace DukeOfThieves.Infrastructure
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField]
        private GameBootstrapper _bootstrapperPrefab;

        private void Awake()
        {
            Instantiate(_bootstrapperPrefab);
        }
    }
}