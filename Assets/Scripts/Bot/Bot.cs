using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BotMovement), typeof(Collector))]
public class Bot : MonoBehaviour
{
    [SerializeField] private Base _basePrefab; 
    private BotMovement _movement;
    private Collector _collector;

    private Vector3 _placeOfRestPosition;
    private Vector3 _deliveryPosition;

    private Coroutine _coroutine;

    public event Action<ICollectable> Delivered;
    public event Action<Bot> BaseCreated;
    

    public bool IsWorking { get; private set; }

    private void Awake()
    {
        _movement = GetComponent<BotMovement>();
        _collector = GetComponent<Collector>();
        
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _collector.CollectibleDelivered += OnDelivered;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsWorking)
        {
            if (_collector.TryCollectTarget(collision))
            {
                StopCoroutine(_coroutine);

                _coroutine = StartCoroutine(_movement.GoTo(_deliveryPosition, () =>
                {
                    _collector.PutInRightPlace();
                    IsWorking = false;

                    _coroutine = StartCoroutine(_movement.GoTo(_placeOfRestPosition, () =>
                    {
                        StopCoroutine(_coroutine);
                        gameObject.SetActive(false);
                    }));

                }));
            }
        }
    }

    private void OnDisable()
    {
        _collector.CollectibleDelivered -= OnDelivered;
    }

    public void Init(Vector3 placeOfRestPosition)
    {
        _placeOfRestPosition = placeOfRestPosition;
        transform.position = new Vector3(
            _placeOfRestPosition.x, transform.position.y, _placeOfRestPosition.z);
    }

    public void DeliverTo(Vector3 deliveryPosition, Vector3 collectablePosition)
    {
        gameObject.SetActive(true);
        
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        IsWorking = true;

        _deliveryPosition = deliveryPosition;
        _collector.SetTargetPosition(collectablePosition);

        _coroutine = StartCoroutine(_movement.GoTo(collectablePosition));
    }
    
    public void CreateNewBase(Vector3 position)
    {
        //gameObject.SetActive(true);
    }

    private void OnDelivered(ICollectable collectable)
    {
        Delivered?.Invoke(collectable);
    }
}