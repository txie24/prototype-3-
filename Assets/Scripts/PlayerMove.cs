using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("movement")]
    public float moveSpeed = 5f;

    [Header("look")]
    public float mouseSensitivity = 150f;
    public bool lockCursor = true;

    Rigidbody rb;
    CharacterController controller;
    Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (FreezeManager.IsFrozen)
        {
            if (animator) animator.speed = 0f;
            return;
        }
        else
        {
            if (animator) animator.speed = 1f;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(0f, mouseX, 0f);

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 fwd = transform.forward; fwd.y = 0f; fwd.Normalize();
        Vector3 right = transform.right; right.y = 0f; right.Normalize();

        Vector3 move = fwd * v + right * h;
        if (move.sqrMagnitude > 1f) move.Normalize();

        if (controller)
        {
            controller.Move(move * moveSpeed * Time.deltaTime);
        }
        else if (rb)
        {
            rb.MovePosition(rb.position + move * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void FixedUpdate()
    {
        if (FreezeManager.IsFrozen && rb && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
