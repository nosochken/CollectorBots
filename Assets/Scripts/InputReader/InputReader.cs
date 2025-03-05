using System;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    private const int LeftMouseButton = 0;

    [SerializeField] private float _maxDistance = 40f;

    public event Action<RaycastHit> WasRaycastHit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(LeftMouseButton))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance))
                WasRaycastHit?.Invoke(hit);
        }
    }
}