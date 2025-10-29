using System.Collections;
using UnityEngine;

public class Rotation_Behaviour : MonoBehaviour
{
    public bool can_rotate = true;
    public float rotate_time = 3f;
    public float rotate_cooldown = 5f;
    public float rotate_amount_euler = 90f;

    private float current_time = 0f;
    private Vector3 rotation_start_cache;
    private Vector3 rotation_end_cache;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotation_start_cache = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (can_rotate)
        {
            StartCoroutine(Rotate(rotate_time+rotate_cooldown));
        }
        else
        {
            transform.eulerAngles = Vector3.Lerp(rotation_start_cache, rotation_end_cache, (current_time / rotate_time));
            current_time += Time.deltaTime;
        }
    }
    
    IEnumerator Rotate(float time)
    {
        can_rotate = false;
        rotation_end_cache = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + rotate_amount_euler, transform.eulerAngles.z);
        yield return new WaitForSeconds(time);
        rotation_start_cache = transform.rotation.eulerAngles;
        can_rotate = true;
        current_time = 0f;
        yield return null;
    }
}
