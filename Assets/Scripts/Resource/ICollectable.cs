using UnityEngine;

public interface ICollectable
{
    public Vector3 Position { get; }

    public void BePickUp(Transform collector, float holdDistance);

    public void BePlaced();
}