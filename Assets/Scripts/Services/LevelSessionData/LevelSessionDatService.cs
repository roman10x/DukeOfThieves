namespace DukeOfThieves.Services
{
    public class LevelSessionDatService : ILevelSessionDatService
    {
        private int _currentlyCollectedLootAmount = 0;
        public void CleanupProgress()
        {
            _currentlyCollectedLootAmount = 0;
        }

        public void AddLootInfo(int lootValueAmount)
        {
            _currentlyCollectedLootAmount += lootValueAmount;
        }
    }
}