using System.Collections.Generic;
using UnityEngine;

namespace DukeOfThieves.StaticData
{
  [CreateAssetMenu(fileName = "LevelData", menuName = "Static Data/Level")]
  public class LevelStaticData : ScriptableObject
  {
    public string LevelKey;
    public Vector3 InitialHeroPosition;
  }
}