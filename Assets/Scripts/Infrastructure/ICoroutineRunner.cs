using System.Collections;
using UnityEngine;

namespace DukeOfThieves.Infrastructure
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}