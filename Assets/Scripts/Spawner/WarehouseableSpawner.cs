using UnityEngine;

public class WarehouseableSpawner<T> : Spawner<T> where T : MonoBehaviour, ISpawnable<T>, IWarehouseable { }