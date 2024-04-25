using System;
using UnityEngine;

namespace DukeOfThieves.Data
{
  [Serializable]
  public class LootPieceData
  {
    [SerializeField]
    private Vector3Data _position;
    [SerializeField]
    private Loot _loot;

    public LootPieceData(Vector3Data position, Loot loot)
    {
      _position = position;
      _loot = loot;
    }
  }
}