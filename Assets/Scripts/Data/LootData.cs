using System;
using UnityEngine;

namespace DukeOfThieves.Data
{
  [Serializable]
  public class LootData
  {
    [SerializeField]
    private int _collected;
    [SerializeField]
    private LootPieceDataDictionary _lootPiecesOnScene = new LootPieceDataDictionary();
    
    public Action Changed;

    public void Collect(Loot loot)
    {
      _collected += loot.Value;
      Changed?.Invoke();
    }

    public void Add(int lootValue)
    {
      _collected += lootValue;
      Changed?.Invoke();
    }
  }
}