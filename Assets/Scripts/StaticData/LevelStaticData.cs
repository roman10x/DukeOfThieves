using System.Collections.Generic;
using UnityEngine;

namespace DukeOfThieves.StaticData
{
  [CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level")]
  public class LevelStaticData : ScriptableObject
  {
    public string LevelKey;
    public Vector2 InitialHeroPosition;
    public GameObject GamePrefab;
    public Sprite MenuImage;
    public AudioClip Music;
  }
}