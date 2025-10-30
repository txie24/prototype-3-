using System.Collections;
using UnityEngine;

public class Rotation_Behaviour : MonoBehaviour
{
    [Header("turning")]
    public bool can_rotate = true;
    public float rotate_time = 0.75f;  

    [Header("rng delays (seconds)")]
    public float cooldown_min = 3f;     
    public float cooldown_max = 5f;
    public float away_min = 3f;         
    public float away_max = 5f;

    [Header("target")]
    public Transform player;            

    Coroutine routine;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
        if (routine == null) routine = StartCoroutine(RotateRoutine());
    }

    IEnumerator RotateRoutine()
    {
        while (true)
        {
            yield return WaitWhileCanRotate(Random.Range(cooldown_min, cooldown_max));


            yield return RotateYawToward(() =>
            {
                if (!player) return transform.forward; 
                Vector3 dir = player.position - transform.position;
                dir.y = 0f;
                return dir.sqrMagnitude > 0.0001f ? dir.normalized : transform.forward;
            });

            yield return WaitWhileCanRotate(Random.Range(away_min, away_max));

            yield return RotateYawToward(() =>
            {
                if (!player) return -transform.forward; 
                Vector3 dir = transform.position - player.position; 
                dir.y = 0f;
                return dir.sqrMagnitude > 0.0001f ? dir.normalized : -transform.forward;
            });
        }
    }

    IEnumerator RotateYawToward(System.Func<Vector3> getFlatDir)
    {
        if (!can_rotate) yield break;

        Quaternion start = transform.rotation;

        Vector3 d0 = getFlatDir();
        Quaternion target = Quaternion.LookRotation(d0, Vector3.up);

        float t = 0f;
        while (t < rotate_time)
        {
            if (!can_rotate) yield break;

            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / rotate_time);

            Vector3 liveDir = getFlatDir();
            if (liveDir.sqrMagnitude > 0.0001f)
                target = Quaternion.LookRotation(liveDir, Vector3.up);

            Vector3 eStart = start.eulerAngles;
            Vector3 eTarget = target.eulerAngles;
            Quaternion startYaw = Quaternion.Euler(0f, eStart.y, 0f);
            Quaternion targetYaw = Quaternion.Euler(0f, eTarget.y, 0f);

            transform.rotation = Quaternion.Slerp(startYaw, targetYaw, u);
            yield return null;
        }

        Vector3 e = target.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, e.y, 0f);
    }

    IEnumerator WaitWhileCanRotate(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            if (can_rotate) t += Time.deltaTime;
            yield return null;
        }
    }

    public void SetCanRotate(bool value) => can_rotate = value;
}
