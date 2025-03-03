using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner<T> : MonoBehaviour where T : MonoBehaviour, ISpawnable<T>
{
    [SerializeField] private T _prefab;
    //[SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private Plane _plane;
    [SerializeField, Min(1)] private float _spawnDelay;

    [SerializeField, Min(1)] private int _poolCapacity;
    [SerializeField, Min(1)] private int _poolMaxSize;

    private ObjectPool<T> _pool;
    
    private int amount;

    private void Awake()
    {
        _pool = new ObjectPool<T>(
        createFunc: () => Create(),
        actionOnGet: (spawnable) => ActOnGet(spawnable),
        actionOnRelease: (spawnable) => spawnable.gameObject.SetActive(false),
        actionOnDestroy: (spawnable) => ActOnDestroy(spawnable),
        collectionCheck: true,
        defaultCapacity: _poolCapacity,
        maxSize: _poolMaxSize);
    }

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private T Create()
    {
        T spawnable = Instantiate(_prefab);
        spawnable.ReadyToSpawn += ReturnToPool;

        return spawnable;
    }

    private void ActOnGet(T spawnable)
    {
        amount++;
        spawnable.name +=amount;
        
        DetermineSpawnPosition(spawnable);
        spawnable.gameObject.SetActive(true);
    }

    private void ActOnDestroy(T spawnable)
    {
        spawnable.ReadyToSpawn -= ReturnToPool;
        Destroy(spawnable.gameObject);
    }

    private IEnumerator Spawn()
    {
        WaitForSeconds wait = new WaitForSeconds(_spawnDelay);

        while (isActiveAndEnabled)
        {
            _pool.Get();
            yield return wait;
        }
    }

    private void ReturnToPool(T spawnable)
    {
        _pool.Release(spawnable);
    }

    private void DetermineSpawnPosition(T spawnable)
    {
        Vector3 spawnPoint = _plane.GetRandomSpawnPoint(0.2f);
        //int point = Random.Range(0, _spawnPoints.Count);
        //spawnable.transform.position = _spawnPoints[point].position;
        spawnable.transform.position = spawnPoint;
    }
}