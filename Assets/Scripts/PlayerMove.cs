using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

    }
}
