using UnityEngine;

public class CollectableSpawner<T> : Spawner<T> where T : MonoBehaviour, ISpawnable<T>, ICollectable { }