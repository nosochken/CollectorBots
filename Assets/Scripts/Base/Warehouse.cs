using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Warehouse : MonoBehaviour
{
    public event Action Replenished;

    public int AmountOfCollectable { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IWarehouseable warehouseable))
        {
            AmountOfCollectable++;
            Replenished?.Invoke();
        }
    }
}