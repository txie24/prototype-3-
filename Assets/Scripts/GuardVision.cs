using UnityEngine;

public class GuardVision : MonoBehaviour
{
    [Header("vision")]
    [Tooltip("degrees, total fov angle")]
    public float viewAngle = 90f;
    [Tooltip("how far the guard can see")]
    public float viewDistance = 15f;

    [Header("scene refs")]
    [Tooltip("the player/angel transform")]
    public Transform target;
    [Tooltip("optional eye origin; if null uses this transform + eyeHeight")]
    public Transform eye;
    [Tooltip("height offset if no explicit eye")]
    public float eyeHeight = 1.6f;

    [Header("layers")]
    [Tooltip("layers that block vision (walls, props). should NOT include the player")]
    public LayerMask obstructionMask;
    [Tooltip("layer of the player, only used for gizmos/clarity")]
    public LayerMask targetMask;

    bool wasSeeing;

    void Reset()
    {
        // try auto-assign target by tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) target = p.transform;
    }

    void Update()
    {
        bool seeing = CanSeeTarget();
        if (seeing != wasSeeing)
        {
            FreezeManager.SetWatching(this, seeing);
            wasSeeing = seeing;
        }
    }

    bool CanSeeTarget()
    {
        if (target == null) return false;

        Vector3 eyePos = eye ? eye.position : transform.position + Vector3.up * eyeHeight;
        Vector3 toTarget = target.position - eyePos;
        float dist = toTarget.magnitude;
        if (dist > viewDistance) return false;

        // fov check
        Vector3 toTargetDir = toTarget.normalized;
        float angle = Vector3.Angle(transform.forward, toTargetDir);
        if (angle > viewAngle * 0.5f) return false;

        // line of sight: if something on obstructionMask blocks, you don't see the target
        if (Physics.Raycast(eyePos, toTargetDir, dist, obstructionMask))
            return false;

        return true;
    }

    // optional: gizmos so you can actually debug this without crying
    void OnDrawGizmosSelected()
    {
        Vector3 origin = eye ? eye.position : transform.position + Vector3.up * eyeHeight;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, 0.1f);

        // draw fov edges
        Quaternion leftRot = Quaternion.AngleAxis(-viewAngle * 0.5f, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(viewAngle * 0.5f, Vector3.up);
        Vector3 left = leftRot * transform.forward;
        Vector3 right = rightRot * transform.forward;

        Gizmos.DrawLine(origin, origin + left * viewDistance);
        Gizmos.DrawLine(origin, origin + right * viewDistance);

        // approximate arc
        int segs = 18;
        Vector3 prev = origin + left * viewDistance;
        for (int i = 1; i <= segs; i++)
        {
            float t = (float)i / segs;
            Quaternion r = Quaternion.AngleAxis(Mathf.Lerp(-viewAngle * 0.5f, viewAngle * 0.5f, t), Vector3.up);
            Vector3 dir = r * transform.forward;
            Vector3 next = origin + dir * viewDistance;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }

    void OnDisable()
    {
        if (wasSeeing)
        {
            FreezeManager.SetWatching(this, false);
            wasSeeing = false;
        }
    }
}
