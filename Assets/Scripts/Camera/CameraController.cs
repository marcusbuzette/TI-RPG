using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using System;
using static UnityEngine.EventSystems.EventTrigger;
using TMPro;
using Unity.Mathematics;

public class CameraController : MonoBehaviour {

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
    private CinemachineImpulseSource cinemachineImpulseSource;

    [Space, Header("Limitador de movimento"), SerializeField]
    private Transform topLimit, bottomLimit, rightLimit, leftLimit;

    [Space, Header("Zoom em combate"), SerializeField]
    private float zoomDuration = 0.3f;

    [Space, Header("Camera Vibra��o"), SerializeField]
    public float
        shakeDuration = 0.3f,
        shakeAmplitude = 2f,
        shakeFrequency = 2f;

    private CinemachineBasicMultiChannelPerlin noise;
    private Coroutine shakeCoroutine;

    private bool exploreMoviment;
    private bool lockMoviment = false, stopMove = true;
    [Space, SerializeField] private Transform playerUnit;
    public int movimentArroundPlayerArea = 2;

    float distanceBeforeMoving, distanceAfterMoving;

    private Coroutine zoomCoroutine;

    Vector3 unitTurnPos;

    void Start() {
        LevelGrid.Instance.OnGameModeChanged += ChangeMovimentMode;
        SetGameMode();

        if (LevelGrid.Instance.IsInBattleMode()) FOV(50);
        else FOV(60);

        TurnSystem.Instance.onTurnChange += CheckIsPlayerTurn;
        UnitActionSystem.Instance.OnUnitMovedInExploreMode += GoToPositionUnitPos;
        UnitActionSystem.Instance.OnSelectedUnitChanged += SetSelectedUnit;
        UnitActionSystem.Instance.OnActionStarted += FollowPlayerOnAction;
        BaseAction.OnAnyActionCompleted += PlayerStopAction;

        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        noise = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
        TurnSystem.Instance.SetCameraController(this);
    }
    void Update() {
        if (TurnSystem.Instance.IsPlayerTurn() && lockMoviment) {
            lockMoviment = MoveTo(playerUnit.position);
        }
        else if (TurnSystem.Instance.IsPlayerTurn() && !lockMoviment && !stopMove) {
            if (playerUnit != null) transform.position = playerUnit.position;
            else playerUnit = UnitActionSystem.Instance.GetSelectedUnit().transform;
        }

        if (TurnSystem.Instance.IsPlayerTurn() && !lockMoviment) {
            if (stopMove) Movement();
            if (exploreMoviment) FollowPlayerUnit();
        }
        else if (!TurnSystem.Instance.IsPlayerTurn() && lockMoviment) {
            lockMoviment = MoveTo(unitTurnPos);
        }
        else if (!TurnSystem.Instance.IsPlayerTurn() && !lockMoviment) {
            if (TurnSystem.Instance.GetTurnUnit() != null) {
                transform.position = TurnSystem.Instance.GetTurnUnit().GetWorldPosition();
            }
        }

        Zoom();
        Rotation();
    }

    void Movement() {
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

    void Rotation() {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.Q)) {
            rotationVector.y = +1f;
        }
        if (Input.GetKey(KeyCode.E)) {
            rotationVector.y = -1f;
        }

        transform.eulerAngles += rotationVector * RotationSpeed * Time.deltaTime;
    }

    void Zoom() {
        float zoomAmount = 1f;
        if (Input.mouseScrollDelta.y > 0) {
            targetFollowOffset.y -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0) {
            targetFollowOffset.y += zoomAmount;
        }
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * ZoomSpeed);
    }

    //Place the camera in anywhere requested
    public void GoToPosition(Vector3 position) {
        unitTurnPos = position;
        lockMoviment = true;
    }

    public void GoToPositionUnitPos(object sender, EventArgs e) {
        lockMoviment = true;
        stopMove = false;
        playerUnit = UnitActionSystem.Instance.GetSelectedUnit()?.transform;
        BaseAction.OnAnyActionCompleted += UnitStopMove;
    }

    public void ChangeMovimentMode(object sender, EventArgs e) {
        exploreMoviment = !exploreMoviment;

        if (LevelGrid.Instance.IsInBattleMode()) FOV(50);
        else FOV(60);

        playerUnit = null;

        var battleZone = LevelGrid.Instance.GetCurrentBattleZone();
        var zone = LevelGrid.Instance.GetCurrentSquaredZone(battleZone);

        var startGrid = new GridPosition(zone.startX - 1, zone.startZ - 1, zone.floor, battleZone);
        var endGrid = new GridPosition(zone.endX + 1, zone.endZ + 1, zone.floor, battleZone);

        topLimit.position = new Vector3(
            LevelGrid.Instance.GetWorldPosition(startGrid).x,
            0,
            LevelGrid.Instance.GetWorldPosition(endGrid).z);

        bottomLimit.position = new Vector3(
            LevelGrid.Instance.GetWorldPosition(endGrid).x,
            0,
            LevelGrid.Instance.GetWorldPosition(startGrid).z);
    }

    private void FollowPlayerUnit() {

        if (playerUnit == null) { playerUnit = UnitActionSystem.Instance.GetSelectedUnit().transform; }

        topLimit.position = new Vector3(
            playerUnit.position.x - movimentArroundPlayerArea,
            0,
            playerUnit.position.z + movimentArroundPlayerArea);

        bottomLimit.position = new Vector3(
            playerUnit.position.x + movimentArroundPlayerArea,
            0,
            playerUnit.position.z - movimentArroundPlayerArea);
    }

    private void SetGameMode() {
        if (LevelGrid.Instance.GetGameMode() == LevelGrid.GameMode.EXPLORE) {
            exploreMoviment = true;
        }
        else exploreMoviment = false;
    }

    private void UnitStopMove(object sender, EventArgs e) {
        if ((sender as BaseAction).GetUnit().transform == playerUnit) {
            stopMove = true;
        }
    }

    public void LockCameraOnSelectedUnit(Unit selectedUnit) {
        playerUnit = selectedUnit.transform;
        lockMoviment = true;
    }

    private bool MoveTo(Vector3 target) {
        Vector3 moveDir = (target - transform.position).normalized;

        distanceBeforeMoving = Vector3.Distance(transform.position, target);

        transform.position += moveDir * 20 * Time.deltaTime;

        distanceAfterMoving = Vector3.Distance(transform.position, target);

        if (distanceBeforeMoving < distanceAfterMoving) {
            return false;
        }
        return true;
    }

    public void SetSelectedUnit(object sender, EventArgs e) {
        playerUnit = UnitActionSystem.Instance.GetSelectedUnit()?.transform;
    }

    public void FollowPlayerOnAction(object sender, EventArgs e) {
        if (TurnSystem.Instance.IsPlayerTurn() && LevelGrid.Instance.IsInBattleMode()) {
            FOV(40);
            lockMoviment = true;
            stopMove = false;
        }
    }

    public void PlayerStopAction(object sender, EventArgs e) {
        if (TurnSystem.Instance.IsPlayerTurn() && LevelGrid.Instance.IsInBattleMode()) {
            FOV(50);
            lockMoviment = false;
            stopMove = true;
        }
    }

    public void CheckIsPlayerTurn(object sender, EventArgs e) {
        if (!TurnSystem.Instance.IsPlayerTurn() && LevelGrid.Instance.IsInBattleMode()) {
            FOV(50);
            lockMoviment = false;
            stopMove = true;
        }
    }

    public void FOV(float _fov) {
        StartZoom(_fov);
    }

    private void StartZoom(float targetFOV) {
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(ZoomRoutine(targetFOV));
    }

    private IEnumerator ZoomRoutine(float targetFOV) {
        float startFOV = cinemachineVirtualCamera.m_Lens.FieldOfView;
        float elapsed = 0f;

        while (elapsed < zoomDuration) {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomDuration;
            cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            yield return null;
        }

        cinemachineVirtualCamera.m_Lens.FieldOfView = targetFOV;
    }

    public void Shake() {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine() {
        if (noise == null) yield break;

        noise.m_AmplitudeGain = shakeAmplitude;
        noise.m_FrequencyGain = shakeFrequency;

        yield return new WaitForSeconds(shakeDuration);

        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
    }
    private void OnDestroy() {
        TurnSystem.Instance.onTurnChange -= CheckIsPlayerTurn;
        UnitActionSystem.Instance.OnUnitMovedInExploreMode -= GoToPositionUnitPos;
        UnitActionSystem.Instance.OnSelectedUnitChanged -= SetSelectedUnit;
        UnitActionSystem.Instance.OnActionStarted -= FollowPlayerOnAction;
        BaseAction.OnAnyActionCompleted -= PlayerStopAction;
        BaseAction.OnAnyActionCompleted -= UnitStopMove;
        LevelGrid.Instance.OnGameModeChanged -= ChangeMovimentMode;
    }
}