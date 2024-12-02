using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ShaftUI : MonoBehaviour
{
    public static Action<Shaft, ShaftUpgrade> OnUpgradeRequest;
    
    [Header("Buttons")] 
    [SerializeField] private GameObject buyNewShaftButton;
    
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI currentGoldTMP;
    [SerializeField] private TextMeshProUGUI currentLevelTMP;

    private Shaft _shaft;
    private ShaftUpgrade _shaftUpgrade;
    
    private void Start()
    {
        _shaftUpgrade = GetComponent<ShaftUpgrade>();
        _shaft = GetComponent<Shaft>();
    }

    private void Update()
    {
        currentGoldTMP.text = _shaft.CurrentDeposit.CurrentGold.ToString();
    }

    public void BuyNewShaft()
    {
        if (GoldManager.Instance.CurrentGold > ShaftManager.Instance.NewShaftCost)
        {
            GoldManager.Instance.RemoveGold(ShaftManager.Instance.NewShaftCost);
            ShaftManager.Instance.AddShaft();
            buyNewShaftButton.SetActive(false);
        }
    }

    public void UpgradeRequest()
    {
        OnUpgradeRequest?.Invoke(_shaft, _shaftUpgrade);
    }
    
    private void UpgradeShaft(BaseUpgrade upgrade, int currentLevel)
    {
        if (upgrade == _shaftUpgrade)
        {
            currentLevelTMP.text = $"Level\n{currentLevel}";
        }
    }
    
    private void OnEnable()
    {
        ShaftUpgrade.OnUpgrade += UpgradeShaft;
    }

    private void OnDisable()
    {
        ShaftUpgrade.OnUpgrade -= UpgradeShaft;
    }
}
