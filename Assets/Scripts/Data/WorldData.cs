using System;
using UnityEngine;

namespace DukeOfThieves.Data
{
  [Serializable]
  public class WorldData
  {
    [SerializeField]
    private PositionOnLevel _positionOnLevel;
    [SerializeField]
    private LootData _lootData;

    public PositionOnLevel PositionOnLevel => _positionOnLevel;

    public WorldData(string initialLevel)
    {
      _positionOnLevel = new PositionOnLevel(initialLevel);
      _lootData = new LootData();
    }
  }
}