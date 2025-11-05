using System.Collections;
using Unity.Mathematics;
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
            StartCoroutine(SineNoiseEvaluation(broadcaster.noise_level, broadcaster.noise_duration));
            broadcaster.make_noise.Invoke();
        }
    }

    IEnumerator SineNoiseEvaluation(float scalar, float duration)
    {
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            noise_level = Mathf.Sin(timeElapsed * (Mathf.PI / duration)) * scalar;
            timeElapsed += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        noise_level = 0f;

        yield return null;
    }
}
