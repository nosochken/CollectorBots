using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Storage : MonoBehaviour
{
    public event Action Changed;

    public int AmountOfCollectable { get; private set; }

    private void Start()
    {
        Changed?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ICollectable collectable))
        {
            AmountOfCollectable++;
            Changed?.Invoke();
        }
    }

    public void UseResources(int amount)
    {
        AmountOfCollectable -= amount;
        Changed?.Invoke();
    }
}