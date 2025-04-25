using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractiveObjects
{
    public void GetFowardGridObject();
    public void MoveUnitToGridPostion(Unit unit);
    public void UnitStopGoingTo(object sender, EventArgs e);
    public void UnitStopGoingTo();
}
