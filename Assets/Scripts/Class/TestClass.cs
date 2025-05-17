using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestClass : IClasses
{
    [SerializeField] private List<BaseAction> baseActions;

    public override void SetActions() {
        /*baseActions.Add(this.AddComponent<HitAction>());
        baseActions.Add(this.AddComponent<MoveAction>());
        baseActions.Add(this.AddComponent<>());*/
    }
}
