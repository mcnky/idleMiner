using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    #region Inspector

    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private TextMeshProUGUI panelTitle;
    [SerializeField] private GameObject[] stats;
    [SerializeField] private Image panelIcon;

    [Header("Button Colors")] [SerializeField]
    private Color buttonDisabledColor;

    [SerializeField] private Color buttonEnabledColor;

    [Header("Buttons")] [SerializeField] private GameObject[] upgradeButtons;

    [Header("Text")] [SerializeField] private TextMeshProUGUI upgradeCost;
    [SerializeField] private TextMeshProUGUI currentStat1;
    [SerializeField] private TextMeshProUGUI currentStat2;
    [SerializeField] private TextMeshProUGUI currentStat3;
    [SerializeField] private TextMeshProUGUI currentStat4;
    [SerializeField] private TextMeshProUGUI stat1Title;
    [SerializeField] private TextMeshProUGUI stat2Title;
    [SerializeField] private TextMeshProUGUI stat3Title;
    [SerializeField] private TextMeshProUGUI stat4Title;

    [Header("Upgraded")] [SerializeField] private TextMeshProUGUI stat1Upgraded;
    [SerializeField] private TextMeshProUGUI stat2Upgraded;
    [SerializeField] private TextMeshProUGUI stat3Upgraded;
    [SerializeField] private TextMeshProUGUI stat4Upgraded;

    [Header("Images")] [SerializeField] private Image stat1Icon;
    [SerializeField] private Image stat2Icon;
    [SerializeField] private Image stat3Icon;
    [SerializeField] private Image stat4Icon;

    [Header("Shaft Icons")] [SerializeField]
    private Sprite shaftMinerIcon;

    [SerializeField] private Sprite minerIcon;
    [SerializeField] private Sprite walkingIcon;
    [SerializeField] private Sprite miningIcon;
    [SerializeField] private Sprite workerCapacityIcon;

    [Header("Elevator Icon")] [SerializeField]
    private Sprite elevatorMinerIcon;

    [SerializeField] private Sprite loadIcon;
    [SerializeField] private Sprite movementIcon;
    [SerializeField] private Sprite loadingIcon;

    [Header("Warehouse Icon")] [SerializeField]
    private Sprite transportersIcon;

    [SerializeField] private Sprite transportationIcon;
    [SerializeField] private Sprite warehouseLoadingIcon;
    [SerializeField] private Sprite warehouseWalkingIcon;

    #endregion

    public int TimesToUpgrade { get; set; }

    private Shaft _selectedShaft;
    private Warehouse _currentWarehouse;
    private ShaftUpgrade _selectedShaftUpgrade;
    private BaseUpgrade _currentUpgrade;

    private void Start()
    {
        ActivateButton(0);
        TimesToUpgrade = 1;
    }

    public void Upgrade()
    {
        if (GoldManager.Instance.CurrentGold >= _currentUpgrade.UpgradeCost)
        {
            _currentUpgrade.Upgrade(TimesToUpgrade);
            if (_currentUpgrade is ShaftUpgrade)
            {
                UpdateUpgradePanel(_currentUpgrade);
            }

            if (_currentUpgrade is ElevatorUpgrade)
            {
                UpdateElevatorPanel(_currentUpgrade);
            }

            if (_currentUpgrade is WarehouseUpgrade)
            {
                UpdateWarehousePanel(_currentUpgrade);
            }
        }
    }

    public void OpenUpgradePanel(bool status)
    {
        upgradePanel.SetActive(status);
    }

    #region Upgrade Buttons

    public void UpgradeX1()
    {
        ActivateButton(0);
        TimesToUpgrade = 1;
        upgradeCost.text = $"{_currentUpgrade.UpgradeCost}";
    }

    public void UpgradeX10()
    {
        ActivateButton(1);
        TimesToUpgrade = CanUpgradeManyTimes(10, _currentUpgrade) ? 10 : 0;
        upgradeCost.text = GetUpgradeCost(10, _currentUpgrade).ToString();
    }

    public void UpgradeX50()
    {
        ActivateButton(2);
        TimesToUpgrade = CanUpgradeManyTimes(50, _currentUpgrade) ? 50 : 0;
        upgradeCost.text = GetUpgradeCost(50, _currentUpgrade).ToString();
    }

    public void UpgradeMax()
    {
        ActivateButton(3);
        TimesToUpgrade = CalculateUpgradeCount(_currentUpgrade);
        upgradeCost.text = GetUpgradeCost(TimesToUpgrade, _currentUpgrade).ToString();
    }

    #endregion

    #region Help Methods

    public void ActivateButton(int buttonIndex)
    {
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            upgradeButtons[i].GetComponent<Image>().color = buttonDisabledColor;
        }

        upgradeButtons[buttonIndex].GetComponent<Image>().color = buttonEnabledColor;
        upgradeButtons[buttonIndex].transform.DOPunchPosition(transform.localPosition +
                                                              new Vector3(0f, -5f, 0f), 0.5f).Play();
    }

    private int GetUpgradeCost(int amount, BaseUpgrade upgrade)
    {
        int cost = 0;
        int upgradeCost = (int) upgrade.UpgradeCost;
        for (int i = 0; i < amount; i++)
        {
            cost += upgradeCost;
            upgradeCost *= (int) upgrade.UpgradeCostMultiplier;
        }

        return cost;
    }

    public bool CanUpgradeManyTimes(int upgradeAmount, BaseUpgrade upgrade)
    {
        int count = CalculateUpgradeCount(upgrade);
        if (count > upgradeAmount)
        {
            return true;
        }

        return false;
    }

    public int CalculateUpgradeCount(BaseUpgrade upgrade)
    {
        int count = 0;
        int currentGold = GoldManager.Instance.CurrentGold;
        int upgradeCost = (int) upgrade.UpgradeCost;

        for (int i = currentGold; i >= 0; i -= upgradeCost)
        {
            count++;
            upgradeCost *= (int) upgrade.UpgradeCostMultiplier;
        }

        return count;
    }

    #endregion

    #region Update Warehouse Panel

    private void UpdateWarehousePanel(BaseUpgrade upgrade)
    {
        panelTitle.text = $"Warehouse Level {upgrade.CurrentLevel}";
        upgradeCost.text = $"{upgrade.UpgradeCost}";
        
        // Update Icons
        stat1Icon.sprite = transportersIcon;
        stat2Icon.sprite = transportationIcon;
        stat3Icon.sprite = warehouseLoadingIcon;
        stat4Icon.sprite = warehouseWalkingIcon;

        // Update Stats Title
        stat1Title.text = "Transporters";
        stat2Title.text = "Transportation";
        stat3Title.text = "Loading Speed";
        stat4Title.text = "Walking Speed";
        
        // Update Current Values
        currentStat1.text = $"{_currentWarehouse.Miners.Count}";
        currentStat2.text = $"{_currentWarehouse.Miners[0].CollectCapacity}";
        currentStat3.text = $"{_currentWarehouse.Miners[0].CollectPerSecond}";
        currentStat4.text = $"{_currentWarehouse.Miners[0].MoveSpeed}";
        
        // Update miners count Upgraded
        if ((upgrade.CurrentLevel + 1) % 10 == 0)
        {
            stat1Upgraded.text = $"+1";
        }
        else
        {
            stat1Upgraded.text = $"+0";
        }
        
        // Update Transportation upgraded
        int collectCapacity = _currentWarehouse.Miners[0].CollectCapacity;
        float collectCapacityMTP = upgrade.CollectCapacityMultiplier;
        int collectCapacityAdded = Mathf.Abs(collectCapacity - (collectCapacity * (int) collectCapacityMTP));
        stat2Upgraded.text = $"+{collectCapacityAdded}";
        
        // Update loading Speed Upgraded
        float currentLoadSpeed = _currentWarehouse.Miners[0].CollectPerSecond;
        float currentLoadSpeedMTP = upgrade.CollectPerSecondMultiplier;
        int loadSpeedAdded = (int) Mathf.Abs(currentLoadSpeed - (currentLoadSpeed * currentLoadSpeedMTP));
        stat3Upgraded.text = $"+{loadSpeedAdded}";
        
        // Update move speed upgraded
        float walkSpeed = _currentWarehouse.Miners[0].MoveSpeed;
        float walkSpeedMTP = upgrade.MoveSpeedMultiplier;
        int walkSpeedAdded = (int) Mathf.Abs(walkSpeed - (walkSpeed * walkSpeedMTP));
        if ((upgrade.CurrentLevel + 1) % 10 == 0)
        {
            stat4Upgraded.text = $"+{walkSpeedAdded}/s";
        }
        else
        {
            stat4Upgraded.text = $"+0/s";
        }
    }

    #endregion
    
    #region Update Elevator Panel

    public void UpdateElevatorPanel(BaseUpgrade upgrade)
    {
        ElevatorMiner miner = upgrade.Elevator.Miner;
        panelTitle.text = $"Elevator Level {upgrade.CurrentLevel}";
        
        // Update Stats Icons
        stats[3].SetActive(false);
        panelIcon.sprite = elevatorMinerIcon;
        stat1Icon.sprite = loadIcon;
        stat2Icon.sprite = movementIcon;
        stat3Icon.sprite = loadingIcon;
        
        // Update Stats Titles
        stat1Title.text = "Load";
        stat2Title.text = "Movement Speed";
        stat3Title.text = "Loading Speed";
        
        // Update current Stats
        currentStat1.text = $"{miner.CollectCapacity}";
        currentStat2.text = $"{miner.MoveSpeed}";
        currentStat3.text = $"{miner.CollectPerSecond}";
        
        // Update load value upgraded
        int currentCollect = miner.CollectCapacity;
        int collecMTP = (int) upgrade.CollectCapacityMultiplier;
        int load = Mathf.Abs(currentCollect - (currentCollect * collecMTP));
        stat1Upgraded.text = $"{load}";
        
        // Update move speed Upgraded
        float currentMovespeed = miner.MoveSpeed;
        float moveSpeedMTP = upgrade.MoveSpeedMultiplier;
        float moveSpeedAdded = Mathf.Abs(currentMovespeed - (currentMovespeed * moveSpeedMTP));
        if ((upgrade.CurrentLevel + 1) % 10 == 0)
        {
            stat2Upgraded.text = $"+{moveSpeedAdded}/s";
        }
        
        // Update new loading speed Added
        float loadingSpeed = miner.CollectPerSecond;
        float loadingSpeedMTP = upgrade.CollectPerSecondMultiplier;
        float loadingAdded = Mathf.Abs(loadingSpeed - (loadingSpeed * loadingSpeedMTP));
        stat3Upgraded.text = $"+{loadingAdded}/s";
    }

    #endregion
    
    #region Update Shaft Panel

    private void UpdateUpgradePanel(BaseUpgrade upgrade)
    {
        panelTitle.text = $"Mine Shaft {_selectedShaft.ShaftID + 1} Level {upgrade.CurrentLevel}";

        upgradeCost.text = $"{upgrade.UpgradeCost}";
        currentStat1.text = $"{_selectedShaft.Miners.Count}";
        currentStat2.text = $"{_selectedShaft.Miners[0].MoveSpeed}";
        currentStat3.text = $"{_selectedShaft.Miners[0].CollectPerSecond}";
        currentStat4.text = $"{_selectedShaft.Miners[0].CollectCapacity}";
        
        // Update Stats Icons
        stats[3].SetActive(true);
        panelIcon.sprite = shaftMinerIcon;
        stat1Icon.sprite = minerIcon;
        stat2Icon.sprite = walkingIcon;
        stat3Icon.sprite = miningIcon;
        stat4Icon.sprite = workerCapacityIcon;
        
        // Update Stats Titles
        stat1Title.text = "Miners";
        stat2Title.text = "Walking Speed";
        stat3Title.text = "Mining Speed";
        stat4Title.text = "Worker Capacity";
        
        // Upgrade worker capacity
        // 200 => 400 === 200 - 400 -> -200
        int collectCapacity = _selectedShaft.Miners[0].CollectCapacity;
        float collectCapacityMTP = upgrade.CollectCapacityMultiplier;
        int collectCapacityAdded = Mathf.Abs(collectCapacity - (collectCapacity * (int) collectCapacityMTP));
        stat4Upgraded.text = $"+{collectCapacityAdded}";
        
        // Upgrade load speed
        float currentLoadSpeed = _selectedShaft.Miners[0].CollectPerSecond;
        float currentLoadSpeedMTP = upgrade.CollectPerSecondMultiplier;
        int loadSpeedAdded = (int) Mathf.Abs(currentLoadSpeed - (currentLoadSpeed * currentLoadSpeedMTP));
        stat3Upgraded.text = $"+{loadSpeedAdded}";
        
        // Upgrade move speed
        float walkSpeed = _selectedShaft.Miners[0].MoveSpeed;
        float walkSpeedMTP = upgrade.MoveSpeedMultiplier;
        int walkSpeedAdded = (int) Mathf.Abs(walkSpeed - (walkSpeed * walkSpeedMTP));
        if ((upgrade.CurrentLevel + 1) % 10 == 0)
        {
            stat2Upgraded.text = $"+{walkSpeedAdded}/s";
        }
        else
        {
            stat2Upgraded.text = $"+0/s";
        }
        
        // Upgrade Miner count
        if ((upgrade.CurrentLevel + 1) % 10 == 0)
        {
            stat1Upgraded.text = $"+1";
        }
        else
        {
            stat1Upgraded.text = $"+0";
        }
    }

    
    
    #endregion

    #region Events

    private void ShaftUpgradeRequest(Shaft shaft, ShaftUpgrade shaftUpgrade)
    {
        List<Shaft> shaftList = ShaftManager.Instance.Shafts;
        for (int i = 0; i < shaftList.Count; i++)
        {
            if (shaft.ShaftID == shaftList[i].ShaftID)
            {
                _selectedShaft = shaftList[i];
                _selectedShaftUpgrade = shaftList[i].GetComponent<ShaftUpgrade>();
            }
        }

        _currentUpgrade = shaftUpgrade;
        OpenUpgradePanel(true);
        UpdateUpgradePanel(_selectedShaftUpgrade);
    }
    
    private void ElevatorUpgradeRequest(ElevatorUpgrade elevatorUpgrade)
    {
        _currentUpgrade = elevatorUpgrade;
        OpenUpgradePanel(true);
        UpdateElevatorPanel(elevatorUpgrade);
    }
    
    private void WarehouseUpgradeRequest(Warehouse warehouse, WarehouseUpgrade warehouseUpgrade)
    {
        _currentUpgrade = warehouseUpgrade;
        _currentWarehouse = warehouse;
        OpenUpgradePanel(true);
        UpdateWarehousePanel(warehouseUpgrade);
    }
    
    private void OnEnable()
    {
        ShaftUI.OnUpgradeRequest += ShaftUpgradeRequest;
        ElevatorUI.OnUpgradeRequest += ElevatorUpgradeRequest;
        WarehouseUI.OnUpgradeRequest += WarehouseUpgradeRequest;
    }

    private void OnDisable()
    {
        ShaftUI.OnUpgradeRequest -= ShaftUpgradeRequest;
        ElevatorUI.OnUpgradeRequest -= ElevatorUpgradeRequest;
        WarehouseUI.OnUpgradeRequest -= WarehouseUpgradeRequest;
    }

    #endregion
}
