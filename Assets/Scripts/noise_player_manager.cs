using UnityEngine;

public class noise_player_manager : MonoBehaviour
{
    public float noise_level = 0f;
    void OnTriggerEnter(Collider other)
    {
        audio_source_generic broadcaster = other.GetComponent<audio_source_generic>();
        if (broadcaster != null)
        {
            broadcaster.broadcast_goal = this;
        }
    }

    void OnTriggerExit(Collider other)
    {
        audio_source_generic broadcaster = other.GetComponent<audio_source_generic>();
        if (broadcaster != null)
        {
            broadcaster.broadcast_goal = null;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        audio_source_generic broadcaster = collision.transform.GetComponent<audio_source_generic>();
        if (broadcaster != null)
        {
            noise_level += broadcaster.noise_level;
            broadcaster.make_noise.Invoke();
        }
    }
}
