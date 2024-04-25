using System;
using UnityEngine;

namespace DukeOfThieves.Data
{
  [Serializable]
  public class PositionOnLevel
  {
    [SerializeField]
    private string _level;

    public string Level => _level;
    public PositionOnLevel(string initialLevel)
    {
      _level = initialLevel;
    }
  }
}