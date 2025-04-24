using UnityEngine;

public class MagicCircleRotation : MonoBehaviour
{
    public float rotationSpeed = 30f; // Velocidade de rotação

    void Update()
    {
        transform.Find("MagicCircle").Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}