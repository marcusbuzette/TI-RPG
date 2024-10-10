using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TrocarCamera : MonoBehaviour
{
    public CinemachineVirtualCamera camera1;
    public CinemachineVirtualCamera camera2;

    private bool camera1Ativa = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            camera1Ativa = !camera1Ativa;

            camera1.gameObject.SetActive(camera1Ativa);
            camera2.gameObject.SetActive(!camera1Ativa);
        }
    }
}