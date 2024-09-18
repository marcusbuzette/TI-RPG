using UnityEngine;


public class Unit : MonoBehaviour {

    public float moveSpeed = 4f;
    public float rotateSpeed = 4f;
    public float stopDistance = .1f;
    private Vector3 targetPosition;

    [SerializeField] private GridTest gridTest; //Temporario

    private void Update() {
        if (Vector3.Distance(targetPosition, transform.position) > stopDistance) {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        }

        if (Input.GetMouseButtonDown(0)) {
            Move(gridTest.MoveInGrid());
        }
    }

    private void Move(Vector3 targetPosition) {
        this.targetPosition = targetPosition;
        Debug.Log("move: " + targetPosition);
    }
}
