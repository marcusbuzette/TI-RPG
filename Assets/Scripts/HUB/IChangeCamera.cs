using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChangeCamera
{
    public void EnterOnThisCamera();

    public void BackToMainCameraHUB();

    //What will be the action once you enter this function?
    public void DoSomething();
}
