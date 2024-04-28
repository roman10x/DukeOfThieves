using System;

namespace DukeOfThieves.Services
{
    public class LevelSessionDataService : ILevelSessionDataService
    {
        private int _currentLevelIndex = 0;
        private int _currentlyCollectedLootAmount = 0;
        
        public int CollectedLootValue => _currentlyCollectedLootAmount;
        public int CurrentLevelIndex => _currentLevelIndex;
        
        public static Action<int> OnLootCollected;

        public void StartSession(int levelIndex)
        {
           _currentLevelIndex = levelIndex;
        }

        public void CleanupProgress()
        {
            _currentlyCollectedLootAmount = 0;
        }

        public void AddLootInfo(int lootValueAmount)
        {
            _currentlyCollectedLootAmount += lootValueAmount;
            OnLootCollected?.Invoke(_currentlyCollectedLootAmount);
        }
    }
}