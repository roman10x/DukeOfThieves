﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DukeOfThieves.Services;
using DukeOfThieves.StaticData;
using UnityEngine;

namespace DukeOfThieves.Infrastructure
{
    public interface IGameFactory: IService
    {
        List<ISavedProgressReader> ProgressReaders { get; }
        List<ISavedProgress> ProgressWriters { get; }
        Task<GameObject> CreateHero(Vector2 at);
        void CreateLevel(Action<bool> onLeveLFinished);
        LevelStaticData PrepareLevel(int index);
        void Cleanup();
        void StopGame();
        Task WarmUp(Action onWarmed);
        void SetLevelLayoutController(LevelLayoutController layoutController);
    }
}