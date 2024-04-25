using DukeOfThieves.Data;

namespace DukeOfThieves.Services
{
  public class PersistentProgressService : IPersistentProgressService
  {
    public PlayerProgress Progress { get; set; }
  }
}