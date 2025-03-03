using UnityEngine;

public class Plane : MonoBehaviour
{
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    
    public Vector3 GetRandomSpawnPoint(float y)
    {
        Vector3 min =_collider.bounds.min + Vector3.one;
        Vector3 max = _collider.bounds.max - Vector3.one;
        
        float randomX = Random.Range(min.x, max.x);
        float randomZ = Random.Range(min.z, max.z);
        
        return new Vector3(randomX, y, randomZ);
    }
}
