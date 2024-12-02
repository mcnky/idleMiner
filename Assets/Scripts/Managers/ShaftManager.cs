using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaftManager : Singleton<ShaftManager>
{
    [SerializeField] private Shaft shaftPrefab;
    [SerializeField] private float newShaftYPosition;
    [SerializeField] private int newShaftCost = 500;

    [Header("Shaft")]
    [SerializeField] private List<Shaft> shafts;

    public List<Shaft> Shafts => shafts;
    public int NewShaftCost => newShaftCost;

    private int _currentShaftIndex;

    private void Start()
    {
        shafts[0].ShaftID = 0;
    }

    public void AddShaft()
    {
        Transform lastShaft = shafts[_currentShaftIndex].transform;
        Shaft newShaft = Instantiate(shaftPrefab, lastShaft.position, Quaternion.identity);
        newShaft.transform.localPosition = new Vector3(lastShaft.position.x, lastShaft.position.y - newShaftYPosition, lastShaft.position.z);

        _currentShaftIndex++;

        newShaft.ShaftID = _currentShaftIndex;
        shafts.Add(newShaft);
    }
}
