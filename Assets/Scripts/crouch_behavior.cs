using UnityEngine;

public class crouch_behavior : MonoBehaviour
{
    public bool crouching = false;

    private CapsuleCollider player_collider;
    private Rigidbody Player_RB;
    public float camera_crouch_offset_y = 0.25f;
    public float down_force = 0.5f;
    [SerializeField] private Transform player_cam;
    [SerializeField] private PlayerMove move_component;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_collider = transform.GetComponent<CapsuleCollider>();
        move_component = transform.GetComponent<PlayerMove>();
        Player_RB = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            Crouch();
        }
    }

    void FixedUpdate()
    {
        if (crouch_dirty_flag)
        {
            Player_RB.AddForce(Vector3.down * down_force, ForceMode.Impulse);
            crouch_dirty_flag = false;
        }
    }

    private bool crouch_dirty_flag = false;
    void Crouch()
    {
        if (crouching == false)
        {
            crouching = true;
            player_collider.height /= 2;
            player_cam.Translate(0f, -camera_crouch_offset_y, 0f);
            move_component.moveSpeed /= 2;
            crouch_dirty_flag = true;
        }
        else
        {
            crouching = false;
            player_collider.height *= 2;
            player_cam.Translate(0f, camera_crouch_offset_y, 0f);
            move_component.moveSpeed *= 2;
            
        }
    }
}
