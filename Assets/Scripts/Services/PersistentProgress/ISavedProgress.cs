using DukeOfThieves.Data;

namespace DukeOfThieves.Services
{
  public interface ISavedProgress : ISavedProgressReader
  {
    void UpdateProgress(PlayerProgress progress);
  }
}