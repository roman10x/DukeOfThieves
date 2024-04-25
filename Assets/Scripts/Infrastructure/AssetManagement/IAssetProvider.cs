using System.Threading.Tasks;

using DukeOfThieves.Services;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DukeOfThieves.Infrastructure.AssetManagement
{
  public interface IAssetProvider:IService
  {
    Task<GameObject> Instantiate(string path, Vector3 at);
    Task<GameObject> Instantiate(string path);
    Task<T> Load<T>(AssetReference monsterDataPrefabReference) where T : class;
    void Cleanup();
    Task<T> Load<T>(string address) where T : class;
    void Initialize();
  }
}