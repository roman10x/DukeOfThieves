using System;
using UnityEngine;

namespace DukeOfThieves.Data
{
  [Serializable]
  public class Loot
  {
    [SerializeField]
    private int _value;

    public int Value => _value;
  }
}