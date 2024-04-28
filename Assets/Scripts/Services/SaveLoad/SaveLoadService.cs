using DukeOfThieves.Data;
using DukeOfThieves.Infrastructure;
using UnityEngine;

namespace DukeOfThieves.Services
{
  public class SaveLoadService : ISaveLoadService
  {
    private const string ProgressKey = "Progress";
    
    private readonly IPersistentProgressService _progressService;
    private readonly IGameFactory _gameFactory;

    public SaveLoadService(IPersistentProgressService progressService, IGameFactory gameFactory)
    {
      _progressService = progressService;
      _gameFactory = gameFactory;
    }

    public void SaveProgress()
    {
      foreach (ISavedProgress progressWriter in _gameFactory.ProgressWriters)
        progressWriter.UpdateProgress(_progressService.Progress);

      var savedJson = _progressService.Progress.ToJson();
      PlayerPrefs.SetString(ProgressKey, savedJson);
    }

    public PlayerProgress LoadProgress()
    {
      var playerProgress = PlayerPrefs.GetString(ProgressKey);
      return playerProgress?
        .ToDeserialized<PlayerProgress>();
    }
  }
}