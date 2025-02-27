using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Storage : MonoBehaviour
{
    public event Action Replenished;

    public int AmountOfCollectable { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ICollectable collectable))
        {
            AmountOfCollectable++;
            Replenished?.Invoke();
        }
    }
}