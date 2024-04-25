using DukeOfThieves.Data;

namespace DukeOfThieves.Services
{
  public interface ISavedProgressReader
  {
    void LoadProgress(PlayerProgress progress);
  }
}