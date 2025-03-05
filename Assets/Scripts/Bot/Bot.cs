using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BotMovement), typeof(Collector))]
[RequireComponent(typeof(BaseCreator))]
public class Bot : MonoBehaviour
{
    [SerializeField] private float _positionY = 1.17f;

    private BotMovement _movement;
    private Collector _collector;
    private BaseCreator _baseCreator;

    private Vector3 _placeOfRestPosition;
    private Vector3 _deliveryPosition;

    private Coroutine _coroutine;

    public event Action<Bot> DeliveredBy;
    public event Action<Bot> NotDeliveredBy;
    public event Action<Bot> BaseCreatedBy;

    public bool IsWorking { get; private set; }

    private void Awake()
    {
        _movement = GetComponent<BotMovement>();
        _collector = GetComponent<Collector>();
        _baseCreator = GetComponent<BaseCreator>();

        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsWorking)
        {
            if (_collector.TryCollectTarget(collision))
            {
                if (_coroutine != null)
                    StopCoroutine(_coroutine);

                _coroutine = StartCoroutine(ExecuteDeliverySequence());
            }
        }
    }

    public void Init(Vector3 placeOfRestPosition)
    {
        _placeOfRestPosition = placeOfRestPosition;
        transform.position = new Vector3(
            _placeOfRestPosition.x, _positionY, _placeOfRestPosition.z);
    }

    public void DeliverTo(Vector3 deliveryPosition, Vector3 collectablePosition)
    {
        Work();

        _deliveryPosition = deliveryPosition;
        _collector.SetTargetPosition(collectablePosition);

        _coroutine = StartCoroutine(ReachCollectablePosition(collectablePosition));
    }

    public void CreateNewBase(Vector3 newBasePosition)
    {
        Work();

        _coroutine = StartCoroutine(ExecuteCreatorSequence(newBasePosition));
    }

    public void SuspendWork()
    {
        IsWorking = false;

        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _collector.PutInRightPlace();
    }

    private void Work()
    {
        gameObject.SetActive(true);

        if (_coroutine != null)
            StopCoroutine(_coroutine);

        IsWorking = true;
    }

    private IEnumerator FinishWork()
    {
        IsWorking = false;

        yield return _movement.GoTo(_placeOfRestPosition);

        gameObject.SetActive(false);
    }

    private IEnumerator ExecuteDeliverySequence()
    {
        yield return _movement.GoTo(_deliveryPosition);

        _collector.PutInRightPlace();
        DeliveredBy.Invoke(this);

        _coroutine = StartCoroutine(FinishWork());
    }

    private IEnumerator ExecuteCreatorSequence(Vector3 newBasePosition)
    {
        yield return _movement.GoTo(newBasePosition);

        Base newBase = _baseCreator.Create(newBasePosition);
        BaseCreatedBy?.Invoke(this);

        newBase.Init(this);

        _coroutine = StartCoroutine(FinishWork());
    }

    private IEnumerator ReachCollectablePosition(Vector3 collectablePosition)
    {
        yield return _movement.GoTo(collectablePosition);

        NotDeliveredBy?.Invoke(this);
        _coroutine = StartCoroutine(FinishWork());
    }
}