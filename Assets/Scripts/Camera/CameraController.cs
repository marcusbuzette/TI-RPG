using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using System;

public class CameraController : MonoBehaviour
{

    private const float MAX_FOLLOW_Y_OFFSET = 12F;
    private const float MIN_FOLLOW_Y_OFFSET = 2F;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private float 
        normalSpeed = 6,
        sprintSpeed = 12;

    [SerializeField] float speed = 6f;
    float ZoomSpeed = 20f;
    [SerializeField] float RotationSpeed = 100f;

    private Vector3 targetFollowOffset;
    private CinemachineTransposer cinemachineTransposer;

    [Header("Limitador de movimento"), SerializeField]
    private Transform topLimit, bottomLimit, rightLimit, leftLimit;

    private bool freeMoviment;
    private Transform playerUnit;

    void Start()
    {
        LevelGrid.Instance.OnGameModeChanged += ChangeMovimentMode;
        SetGameMode();

        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
        TurnSystem.Instance.SetCameraController(this);
    }
    void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn()) {
            Zoom();
            Rotation();

            if (freeMoviment) Movement();
            else FolowPlayerUnit();
        }
        else {
            transform.position = TurnSystem.Instance.GetTurnUnit().GetWorldPosition();
        }
    }

    void Movement()
    {
        Vector3 InputMoveDir = new Vector3(0, 0, 0);

        if (transform.position.z <= topLimit.position.z) {
            if (transform.position.z >= bottomLimit.position.z) {
                if (transform.position.x >= leftLimit.position.x) {
                    if (transform.position.x <= rightLimit.position.x) {
                        if (Input.GetKey(KeyCode.W)) {
                            InputMoveDir.z = +1f;
                        }
                        if (Input.GetKey(KeyCode.S)) {
                            InputMoveDir.z = -1f;
                        }
                        if (Input.GetKey(KeyCode.A)) {
                            InputMoveDir.x = -1f;
                        }
                        if (Input.GetKey(KeyCode.D)) {
                            InputMoveDir.x = +1f;
                        }
                    }
                    else transform.position = new Vector3(rightLimit.position.x, transform.position.y, transform.position.z);
                }
                else transform.position = new Vector3(leftLimit.position.x, transform.position.y, transform.position.z);
            }
            else transform.position = new Vector3(transform.position.x, transform.position.y, bottomLimit.position.z);
        }
        else transform.position = new Vector3(transform.position.x, transform.position.y, topLimit.position.z);

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            speed = sprintSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            speed = normalSpeed;
        }

        Vector3 moveVector = transform.forward * InputMoveDir.z + transform.right * InputMoveDir.x;
        transform.position += moveVector * speed * Time.deltaTime;
    }

    void FolowPlayerUnit() {
        if(playerUnit == null) { playerUnit = UnitActionSystem.Instance.GetSelectedUnit().transform; }
        transform.position = playerUnit.position;
    }

    void Rotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.Q))
        {
            rotationVector.y = +1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotationVector.y = -1f;
        }

        transform.eulerAngles += rotationVector * RotationSpeed * Time.deltaTime;
    }

    void Zoom()
    {
        float zoomAmount = 1f;
        if (Input.mouseScrollDelta.y > 0)
        {
            targetFollowOffset.y -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFollowOffset.y += zoomAmount;
        }
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * ZoomSpeed);
    }

    //Place the camera in anywhere requested
    public void GoToPosition(Vector3 position) {
        transform.position = new Vector3(position.x, 0, position.z);
    }

    public void ChangeMovimentMode(object sender, EventArgs e) {
        freeMoviment = !freeMoviment;
        playerUnit = null;

        var battleZone = LevelGrid.Instance.GetCurrentBattleZone();
        var zone = LevelGrid.Instance.GetCurrentSquaredZone(battleZone);

        var startGrid = new GridPosition(zone.startX - 1, zone.startZ - 1, zone.floor, battleZone);
        var endGrid = new GridPosition(zone.endX + 1, zone.endZ + 1, zone.floor, battleZone);

        topLimit.position = new Vector3(
            LevelGrid.Instance.GetWorldPosition(startGrid).x,
            0 ,
            LevelGrid.Instance.GetWorldPosition(endGrid).z);

        bottomLimit.position = new Vector3(
            LevelGrid.Instance.GetWorldPosition(endGrid).x,
            0,
            LevelGrid.Instance.GetWorldPosition(startGrid).z);
    }

    private void SetGameMode() {
        if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE) freeMoviment = false;
        else freeMoviment = true;
    }
}
