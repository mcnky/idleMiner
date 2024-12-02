﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ManagersController : Singleton<ManagersController>
{
    [SerializeField] private ManagerCard managerCardPrefab;
    [SerializeField] private int initialManagerCost = 100;
    [SerializeField] private int managerCostMultiplier = 3;

    [Header("Assing Manager UI")] 
    [SerializeField] private Image managerIcon;
    [SerializeField] private Image boostIcon;
    [SerializeField] private TextMeshProUGUI managerName;
    [SerializeField] private TextMeshProUGUI managerLevel;
    [SerializeField] private TextMeshProUGUI boostEffect;
    [SerializeField] private TextMeshProUGUI boostDescription;

    [SerializeField] private TextMeshProUGUI managerPanelTitle;
    [SerializeField] private Transform managersContainer;
    [SerializeField] private GameObject managerPanel; 
    [SerializeField] private GameObject assignedManagerPanel;
    [SerializeField] private List<Manager> managerList;

    public BaseManagerLocation CurrentManagerLocation { get; set; }
    public int NewManagerCost { get; set; }

    private List<ManagerCard> _assignedManagerCards;
    private Camera _camera;
    
    private void Start()
    {
        _assignedManagerCards = new List<ManagerCard>();
        _camera = Camera.main;
        NewManagerCost = initialManagerCost;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
            {
                if (hitInfo.transform.GetComponent<MineManager>() != null)
                {
                    CurrentManagerLocation = hitInfo.transform.GetComponent<MineManager>().Location;
                    OpenPanel(true);
                }
            }
        }
    }

    #region Boosts

    public void RunMovementBoost(BaseMiner miner, float duration, float value)
    {
        StartCoroutine(IEMovementBoost(miner, duration, value));
    }

    public void RunLoadingBoost(BaseMiner miner, float duration, float value)
    {
        StartCoroutine(IELoadingBoost(miner, duration, value));
    }
    
    private IEnumerator IEMovementBoost(BaseMiner miner, float duration, float value)
    {
        float startSpeed = miner.MoveSpeed;
        miner.MoveSpeed *= value;
        yield return new WaitForSeconds(duration);
        miner.MoveSpeed = startSpeed;
    }

    private IEnumerator IELoadingBoost(BaseMiner miner, float duration, float value)
    {
        float startValue = miner.CollectPerSecond;
        miner.CollectPerSecond *= value;
        yield return new WaitForSeconds(duration);
        miner.CollectPerSecond = startValue;
    }
    
    #endregion
    
    public void UnassignManager()
    {
        RestoreManagerCard(CurrentManagerLocation.Manager);
        CurrentManagerLocation.Manager = null;
        UpdateAssignedManagerInfo(CurrentManagerLocation);
    }
    
    public void HireManager()
    {
        if (GoldManager.Instance.CurrentGold >= NewManagerCost)
        {
            // Create card
            ManagerCard card = Instantiate(managerCardPrefab, managersContainer);
            
            // Get Random Manager
            int randomIndex = Random.Range(0, managerList.Count);
            Manager randomManager = managerList[randomIndex];
            card.SetupManagerCard(randomManager);
            
            managerList.RemoveAt(randomIndex);

            GoldManager.Instance.RemoveGold(NewManagerCost);
            NewManagerCost *= managerCostMultiplier;
        }
    }

    public void UpdateAssignedManagerInfo(BaseManagerLocation managerLocation)
    {
        if (managerLocation.Manager != null)
        {
            managerIcon.sprite = managerLocation.Manager.managerIcon;
            boostIcon.sprite = managerLocation.Manager.boostIcon;
            managerName.text = managerLocation.Manager.managerName;
            managerLevel.text = managerLocation.Manager.ManagerLevel.ToString();
            boostEffect.text = managerLocation.Manager.boostDuration.ToString();
            boostDescription.text = managerLocation.Manager.boostDescription;
            managerLocation.UpdateBoostIcon();
            assignedManagerPanel.SetActive(true);
        }
        else
        {
            managerIcon.sprite = null;
            boostIcon.sprite = null;
            managerName.text = null;
            managerLevel.text = null;
            boostEffect.text = null;
            boostDescription.text = null;
            assignedManagerPanel.SetActive(false);
        }
    }
    
    public void AddAssignedManagerCard(ManagerCard card)
    {
        _assignedManagerCards.Add(card);
    }
    
    public void OpenPanel(bool value)
    {
        managerPanel.SetActive(value);
        if (value)
        {
            managerPanelTitle.text = CurrentManagerLocation.LocationTitle;
            UpdateAssignedManagerInfo(CurrentManagerLocation);
        }
    }

    private void RestoreManagerCard(Manager manager)
    {
        ManagerCard managerCard = null;
        for (int i = 0; i < _assignedManagerCards.Count; i++)
        {
            if (_assignedManagerCards[i].Manager.managerName == manager.managerName)
            {
                _assignedManagerCards[i].gameObject.SetActive(true);
                managerCard = _assignedManagerCards[i];
            }
        }

        if (managerCard != null)
        {
            _assignedManagerCards.Remove(managerCard);
        }
    }
}
