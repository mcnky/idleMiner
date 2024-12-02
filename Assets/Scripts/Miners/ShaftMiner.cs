using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaftMiner : BaseMiner
{
    public Shaft CurrentShaft { get; set; }
    
    private int miningAnimationParametor = Animator.StringToHash("Mining");
    private int walkingAnimationParametor = Animator.StringToHash("Walking");
    
    public override void MoveMiner(Vector3 newPosition)
    {
        base.MoveMiner(newPosition);
        _animator.SetTrigger(walkingAnimationParametor);
    }

    protected override void CollectGold()
    {
        float collectTime = CollectCapacity / CollectPerSecond;
        _animator.SetTrigger(miningAnimationParametor);
        OnLoading?.Invoke(this, collectTime);
        StartCoroutine(IECollect(CollectCapacity, collectTime));
    }

    protected override IEnumerator IECollect(int collectGold ,float collectTime)
    {
        yield return new WaitForSeconds(collectTime);

        CurrentGold = collectGold;
        ChangeGoal();
        RotateMiner(-1);
        MoveMiner(CurrentShaft.DepositLocation.position);
    }

    protected override void DepositGold()
    {
        // Add CurrentGold to the Deposit (Class)
        CurrentShaft.CurrentDeposit.DepositGold(CurrentGold);
        
        // Update some Values
        CurrentGold = 0;
        ChangeGoal();
        RotateMiner(1);
        MoveMiner(CurrentShaft.MiningLocation.position);
    }
}
