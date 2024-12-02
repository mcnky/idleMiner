using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Shaft : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private ShaftMiner minerPrefab;
    [SerializeField] private Deposit depositPrefab;

    [Header("Locations")]
    [SerializeField] private Transform miningLocation;
    [SerializeField] private Transform depositLocation;
    [SerializeField] private Transform depositInstantiationPos;

    [Header("Manager")] 
    [SerializeField] private Transform managerPos;
    [SerializeField] private GameObject managerPrefab;
    
    public Transform MiningLocation => miningLocation;
    public Transform DepositLocation => depositLocation;
    public List<ShaftMiner> Miners => _miners;
    public Deposit CurrentDeposit { get; set; }
    public int ShaftID { get; set; }
    
    private GameObject _minersContainer;
    private List<ShaftMiner> _miners;
    private ShaftManagerLocation _shaftManagerLocation;
    
    private void Start()
    {
        _shaftManagerLocation = GetComponent<ShaftManagerLocation>();
        _miners = new List<ShaftMiner>();
        _minersContainer = new GameObject("Miners");
        CreateMiner();
        CreateDeposit();
    }

    public void CreateMiner()
    {
        ShaftMiner newMiner = Instantiate(minerPrefab, depositLocation.position, Quaternion.identity);
        newMiner.CurrentShaft = this;
        newMiner.transform.SetParent(_minersContainer.transform);
        newMiner.MoveMiner(miningLocation.position);

        if (_miners.Count > 0)
        {
            // 2nd miner and ....
            newMiner.CollectCapacity = _miners[0].CollectCapacity;
            newMiner.CollectPerSecond = _miners[0].CollectPerSecond;
            newMiner.MoveSpeed = _miners[0].MoveSpeed;
        }
        
        // Add new miner
        _miners.Add(newMiner);
    }

    public void CreateManager()    
    {
        GameObject shaftManager = Instantiate(managerPrefab, managerPos.position, Quaternion.identity);
        MineManager mineManager = shaftManager.GetComponent<MineManager>();
        mineManager.SetupManager(_shaftManagerLocation);
        _shaftManagerLocation.MineManager = mineManager;
    }

    private void CreateDeposit()
    {
        CurrentDeposit = Instantiate(depositPrefab, depositInstantiationPos.position, Quaternion.identity);
        CurrentDeposit.transform.SetParent(depositInstantiationPos);
    }

    private void ShaftBoost(Shaft shaft, ShaftManagerLocation shaftManager)
    {
        if (shaft == this)
        {
            switch (shaftManager.Manager.BoostType)
            {
                case BoostType.Movement:
                    foreach (ShaftMiner miner in _miners)
                    {
                        ManagersController.Instance.RunMovementBoost(miner,
                            shaftManager.Manager.boostDuration, shaftManager.Manager.boostValue);
                    }
                    break;
                case BoostType.Loading:
                    foreach (ShaftMiner miner in _miners)
                    {
                        ManagersController.Instance.RunLoadingBoost(miner,
                            shaftManager.Manager.boostDuration, shaftManager.Manager.boostValue);
                    }
                    break;
            }
        }
    }
    
    private void OnEnable()
    {
        ShaftManagerLocation.OnBoost += ShaftBoost;
    }

    private void OnDisable()
    {
        ShaftManagerLocation.OnBoost -= ShaftBoost;
    }
}
