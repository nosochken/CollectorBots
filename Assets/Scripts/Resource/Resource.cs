using System;
using UnityEngine;

public class Resource : MonoBehaviour, ICollectable, IWarehouseable, ISpawnable<Resource>
{
    public event Action<Resource> ReadyToSpawn;

    public Vector3 Position => transform.position;

    public void BePickUp(Transform collector, float holdDistance)
    {
        transform.SetParent(collector);
        transform.localPosition = new Vector3(0f, 0f, holdDistance);
    }

    public void BePlaced()
    {
        transform.SetParent(null);
        ReadyToSpawn?.Invoke(this);
    }
}