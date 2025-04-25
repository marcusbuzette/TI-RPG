using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] private GameObject actionCamera;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

        HideActionCamera();
    }

    private void ShowActionCamera()
    {
        actionCamera.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCamera.SetActive(false);
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        Vector3 cameraCharacterHeight = Vector3.up * 1.7f;

        switch (sender)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();


                Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition().normalized);

                float ShoulderOffsetAmount = 0.15f;
                Vector3 ShoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * ShoulderOffsetAmount;

                Vector3 actionCameraPositionShoot =
                    shooterUnit.GetWorldPosition() + cameraCharacterHeight + ShoulderOffset + (shootDir * -0.3f);

                actionCamera.transform.position = actionCameraPositionShoot;
                actionCamera.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);
                int shootDamage = shootAction.GetDamage();

                if (targetUnit.GetHealthPoints() - shootDamage <= 0) ShowActionCamera();
                break;
  case HitAction hitAction:
    // Debug.Log("Entrou no Case HitAction");
    Unit hitterUnit = hitAction.GetUnit();
    Unit targetUnitH = hitAction.GetTargetUnit();

    // Debug.Log($"Health before hit: {targetUnitH.GetHealthPoints()}, Damage: {hitAction.GetDamage()}");

    Vector3 hitDir = (targetUnitH.GetWorldPosition() - hitterUnit.GetWorldPosition()).normalized;

    Vector3 middlePoint = (hitterUnit.GetWorldPosition() + targetUnitH.GetWorldPosition()) / 2f;

    float cameraDistance = 10f;
    Vector3 sideDir = Quaternion.Euler(0, 90, 0) * hitDir;

    Vector3 actionCameraPositionHit = middlePoint + cameraCharacterHeight + (sideDir * cameraDistance);

    actionCamera.transform.position = actionCameraPositionHit;
    actionCamera.transform.LookAt(middlePoint + cameraCharacterHeight);

    int hitDamage = hitAction.GetDamage();

    // Debug.Log($"Health after hit: {targetUnitH.GetHealthPoints() - hitDamage}");

    if (targetUnitH.GetHealthPoints() - hitDamage <= 0)
    {
        Debug.Log("Showing Action Camera for HitAction");
        ShowActionCamera();
    }
    break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
            case HitAction hitAction:
                HideActionCamera();
                break;
        }
    }

    private void OnDestroy()
    {
        BaseAction.OnAnyActionStarted -= BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted -= BaseAction_OnAnyActionCompleted;
    }
}
