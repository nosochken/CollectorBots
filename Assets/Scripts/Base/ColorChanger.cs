using UnityEngine;

[RequireComponent(typeof(Material))]
public class ColorChanger : MonoBehaviour
{
    [SerializeField] private Base _base;

    private Renderer _renderer;
    private Color _defaultColor;

    private void OnEnable()
    {
        _base.WasChosen += BecomeChosen;
        _base.NewBaseCreated += BecomeOrdinary;
    }

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _defaultColor = _renderer.material.color;
    }

    private void OnDisable()
    {
        _base.WasChosen -= BecomeChosen;
        _base.NewBaseCreated -= BecomeOrdinary;
    }

    private void BecomeChosen()
    {
        _renderer.material.color = Color.blue;
    }

    private void BecomeOrdinary()
    {
        _renderer.material.color = _defaultColor;
    }
}