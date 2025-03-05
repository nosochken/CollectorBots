using UnityEngine;

public class BasePlacementHandler : MonoBehaviour
{
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private InputReader _inputReader;

    private Flag _flag;
    private Base _currentBase;

    private void OnEnable()
    {
        _inputReader.WasRaycastHit += ReadRaycastHitData;
    }

    private void Start()
    {
        _flag = Instantiate(_flagPrefab);
        _flag.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _inputReader.WasRaycastHit -= ReadRaycastHitData;
    }

    private void ReadRaycastHitData(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent(out Base selectedBase))
        {
            if (_currentBase == null && selectedBase.CanCreateNewBase)
            {
                _currentBase = selectedBase;
                _currentBase.NewBaseCreated += OnNewBaseCreated;

                _currentBase.OnWasChosen();
            }
        }
        else if (hit.collider.TryGetComponent<BasePlane>(out _))
        {
            if (_currentBase != null)
            {
                if (_flag.isActiveAndEnabled == false)
                {
                    _flag.SetActive();
                    _currentBase.CreateNewBase(_flag);
                }

                _flag.transform.position = hit.point;
            }

        }
    }

    private void OnNewBaseCreated()
    {
        _flag.SetInactive();
        _currentBase.NewBaseCreated -= OnNewBaseCreated;
        _currentBase = null;
    }
}