using TMPro;
using UnityEngine;

public class DisplayBaseStatistic : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourcesAmountInBase;
    [SerializeField] private TextMeshProUGUI _botsAmountInBase;
    
    private Base _base;

    private void Awake()
    {
        _base = GetComponentInParent<Base>();
    }

    private void OnEnable()
    {
        _base.StorageChanged += OnStorageChanged;
        _base.BotsAmountChanged += OnBotsAmountChanged;
    }

    private void OnDisable()
    {
        _base.StorageChanged -= OnStorageChanged;
        _base.BotsAmountChanged -= OnBotsAmountChanged;
    }
    
    private void OnStorageChanged(int amount)
    {
        Display(_resourcesAmountInBase, amount);
    }
    
    private void OnBotsAmountChanged(int amount)
    {
        Display(_botsAmountInBase, amount);
    }

    private void Display(TextMeshProUGUI text, int amount)
    {
        text.text = amount.ToString();
    }
}