using UnityEngine;

public class pot_behavior : MonoBehaviour
{
    audio_source_generic source;

    void Start()
    {
        source = transform.GetComponent<audio_source_generic>();
    }

    public void Break()
    {
        source.noise_level *= 1.3f;
        Destroy(gameObject);
    }
}
