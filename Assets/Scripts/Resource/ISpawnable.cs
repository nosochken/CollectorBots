using System;
using UnityEngine;

public interface ISpawnable<T> where T : MonoBehaviour, ISpawnable<T>
{
    public event Action<T> ReadyToSpawn;
}