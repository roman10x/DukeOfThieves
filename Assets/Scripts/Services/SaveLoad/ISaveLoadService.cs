using DukeOfThieves.Data;

namespace DukeOfThieves.Services
{
  public interface ISaveLoadService : IService
  {
    void SaveProgress();
    PlayerProgress LoadProgress();
  }
}