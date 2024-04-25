using System.Collections.Generic;
using System.Threading.Tasks;
using DukeOfThieves.Services;
using UnityEngine;

namespace DukeOfThieves.Infrastructure
{
    public interface IGameFactory: IService
    {
        List<ISavedProgressReader> ProgressReaders { get; }
        List<ISavedProgress> ProgressWriters { get; }
        Task<GameObject> CreateHero(Vector3 at);
        
        void Cleanup();
        Task WarmUp();
    }
}