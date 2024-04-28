namespace DukeOfThieves.Services
{
    public interface ILevelSessionDatService : IService
    {
        void CleanupProgress();
        void AddLootInfo(int lootValueAmount);
    }
}