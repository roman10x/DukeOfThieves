using DukeOfThieves.Data;

namespace DukeOfThieves.Services
{
  public interface IPersistentProgressService : IService
  {
    PlayerProgress Progress { get; set; }
  }
}