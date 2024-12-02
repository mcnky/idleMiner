using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject warehouseMinerPrefab;
    
    [Header("Extras")]
    [SerializeField] private Deposit elevatorDeposit;
    [SerializeField] private Transform elevatorLocation;
    [SerializeField] private Transform warehouseDepositLocation;
    [SerializeField] private List<WarehouseMiner> miners;

    public List<WarehouseMiner> Miners => miners;
    
    private void Start()
    {
        miners = new List<WarehouseMiner>();
        AddMiner();
    }

    public void AddMiner()
    {
        GameObject newMiner = Instantiate(warehouseMinerPrefab, warehouseDepositLocation.position, Quaternion.identity);
        WarehouseMiner miner = newMiner.GetComponent<WarehouseMiner>();
        miner.ElevatorDeposit = elevatorDeposit;
        miner.ElevatorDepositLocation = elevatorLocation;
        miner.WarehouseLocation = warehouseDepositLocation;
        
        miners.Add(miner);
    }

    private void WarehouseMinerBoost(WarehouseManagerLocation warehouseManager)
    {
        switch (warehouseManager.Manager.BoostType)
        {
            case BoostType.Movement:
                foreach (WarehouseMiner miner in Miners)
                {
                    ManagersController.Instance.RunMovementBoost(miner,
                        warehouseManager.Manager.boostDuration, warehouseManager.Manager.boostValue);
                }
                break;
            case BoostType.Loading:
                foreach (WarehouseMiner miner in Miners)
                {
                    ManagersController.Instance.RunLoadingBoost(miner,
                        warehouseManager.Manager.boostDuration, warehouseManager.Manager.boostValue);
                }
                break;
        }
    }
    
    private void OnEnable()
    {
        WarehouseManagerLocation.OnBoost += WarehouseMinerBoost;
    }

    private void OnDisable()
    {
        WarehouseManagerLocation.OnBoost -= WarehouseMinerBoost;
    }
}
