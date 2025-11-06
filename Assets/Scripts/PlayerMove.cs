using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMove : MonoBehaviour
{
    [Header("movement")]
    public float moveSpeed = 5f;

    [Header("look")]
    public float mouseSensitivity = 150f;
    public bool lockCursor = true;

    Rigidbody rb;                 // optional fallback only
    CharacterController controller; // optional fallback only
    Animator animator;
    NavMeshAgent agent;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // Let our own yaw control rotation.
        agent.updateRotation = false;

        // Agent speed is irrelevant when using Move(), but set for clarity.
        agent.speed = moveSpeed;
        if (agent.stoppingDistance < 0.05f) agent.stoppingDistance = 0.05f;
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
        // Freeze: stop animation and agent stepping.
        if (FreezeManager.IsFrozen)
        {
            if (animator) animator.speed = 0f;
            if (agent) agent.isStopped = true;
            return;
        }
        else
        {
            if (animator) animator.speed = 1f;
            if (agent) agent.isStopped = false;
        }

        // Mouse-look yaw
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(0f, mouseX, 0f);

        // WASD input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Build a planar move vector relative to current yaw
        Vector3 fwd = transform.forward; fwd.y = 0f; fwd.Normalize();
        Vector3 right = transform.right; right.y = 0f; right.Normalize();

        Vector3 move = fwd * v + right * h;
        if (move.sqrMagnitude > 1f) move.Normalize();

        // Prefer NavMesh movement; fall back if we’re off-mesh or missing agent.
        Vector3 step = move * moveSpeed * Time.deltaTime;

        if (agent && agent.isOnNavMesh)
        {
            // Slide along NavMesh edges, respect obstacles.
            agent.Move(step);
        }
        else if (controller)
        {
            controller.Move(step);
        }
        else if (rb)
        {
            rb.MovePosition(rb.position + step);
        }
        else
        {
            transform.Translate(step, Space.World);
        }
    }

    void FixedUpdate()
    {
        // If a non-kinematic Rigidbody is present, kill momentum while frozen.
        if (FreezeManager.IsFrozen && rb && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
