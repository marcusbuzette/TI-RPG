using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{

    private const float MAX_FOLLOW_Y_OFFSET = 12F;
    private const float MIN_FOLLOW_Y_OFFSET = 2F;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] float speed = 6f;
    float ZoomSpeed = 20f;
    [SerializeField] float RotationSpeed = 100f;

    private Vector3 targetFollowOffset;
    private CinemachineTransposer cinemachineTransposer;

    void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }
    void Update()
    {
        Movement();
        Rotation();
        Zoom();
    }

    void Movement()
    {
        Vector3 InputMoveDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            InputMoveDir.z = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            InputMoveDir.z = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            InputMoveDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            InputMoveDir.x = +1f;
        }

        Vector3 moveVector = transform.forward * InputMoveDir.z + transform.right * InputMoveDir.x;
        transform.position += moveVector * speed * Time.deltaTime;
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
}
