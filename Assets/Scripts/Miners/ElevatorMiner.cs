using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorMiner : BaseMiner
{
    [SerializeField] private Elevator elevator;
    
    private int _currentShaftIndex = -1;
    private Deposit _currentDeposit;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            MoveToNextLocation();
        }
    }

    public void MoveToNextLocation()
    {
        _currentShaftIndex++; // 0

        Shaft currentShaft = ShaftManager.Instance.Shafts[_currentShaftIndex];
        Vector2 nextPos = currentShaft.DepositLocation.position;
        Vector2 fixedPos = new Vector2(transform.position.x, nextPos.y);

        _currentDeposit = currentShaft.CurrentDeposit;
        MoveMiner(fixedPos);
    }

    protected override void CollectGold()
    {
        if (!_currentDeposit.CanCollectGold() 
            && _currentDeposit != null
            && _currentShaftIndex == ShaftManager.Instance.Shafts.Count - 1)
        {
            _currentShaftIndex = -1;
            ChangeGoal();
            Vector3 elevatorDepositPos = new Vector3(transform.position.x, elevator.DepositLocation.position.y);
            MoveMiner(elevatorDepositPos);
            return;
        }

        int amountToCollect = _currentDeposit.CollectGold(this);
        float collectTime = amountToCollect / CollectPerSecond;
        OnLoading?.Invoke(this, collectTime);
        StartCoroutine(IECollect(amountToCollect, collectTime));
    }

    protected override IEnumerator IECollect(int collectGold, float collectTime)
    {
        yield return new WaitForSeconds(collectTime);

        CurrentGold = collectGold;
        _currentDeposit.RemoveGold(collectGold);
        yield return new WaitForSeconds(0.5f);

        if (CurrentGold == CollectCapacity || _currentShaftIndex == ShaftManager.Instance.Shafts.Count - 1)
        {
            _currentShaftIndex = -1;
            ChangeGoal();
            Vector3 elevatorDepositPos = new Vector3(transform.position.x, elevator.DepositLocation.position.y);
            MoveMiner(elevatorDepositPos);
        }
        else
        {
            MoveToNextLocation();
        }
    }

    protected override void DepositGold()
    {
        if (CurrentGold <= 0)
        {
            _currentShaftIndex = -1;
            ChangeGoal();
            MoveToNextLocation();
            return;
        }

        float depositTime = CurrentGold / CollectPerSecond;
        OnLoading?.Invoke(this, depositTime);
        StartCoroutine(IEDeposit(CurrentGold, depositTime));
    }
    
    protected override IEnumerator IEDeposit(int goldCollected, float depositTime)
    { 
        yield return new WaitForSeconds(depositTime);
        
        elevator.ElevatorDeposit.DepositGold(CurrentGold);
        CurrentGold = 0;
        _currentShaftIndex = -1;
        
        // Update goal and move to next location
        ChangeGoal();
        MoveToNextLocation();
    }

    private void ElevatorBoost(ElevatorManagerLocation elevatorManager)
    {
        switch (elevatorManager.Manager.BoostType)
        {
            case BoostType.Movement:
                ManagersController.Instance.RunMovementBoost(this, 
                    elevatorManager.Manager.boostDuration, elevatorManager.Manager.boostValue);
                break;
                
            case BoostType.Loading:
                ManagersController.Instance.RunLoadingBoost(this, elevatorManager.Manager.boostDuration, elevatorManager.Manager.boostValue);
                break;
        }
    }
    
    private void OnEnable()
    {
        ElevatorManagerLocation.OnBoost += ElevatorBoost;
    }

    private void OnDisable()
    {
        ElevatorManagerLocation.OnBoost -= ElevatorBoost;
    }
}
