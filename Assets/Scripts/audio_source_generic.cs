using UnityEngine;
using UnityEngine.Events;

public class audio_source_generic : MonoBehaviour
{
    public noise_player_manager broadcast_goal;
    public float noise_level = 1f;
    public UnityEvent make_noise;
}
