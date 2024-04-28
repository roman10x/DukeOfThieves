using System;

namespace DukeOfThieves.Services
{
    public interface ILevelSessionDataService : IService
    {
        int CollectedLootValue { get; }
        int CurrentLevelIndex { get; }
        void StartSession(int levelIndex);
        void CleanupProgress();
        void AddLootInfo(int lootValueAmount);
    }
}