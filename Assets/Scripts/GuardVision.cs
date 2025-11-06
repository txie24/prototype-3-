// GuardVision.cs
using System;
using UnityEngine;

public class GuardVision : MonoBehaviour
{
    [Header("vision")]
    [Tooltip("degrees, total FOV angle")]
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
    [Tooltip("layers that block vision (walls, props)")]
    public LayerMask obstructionMask;
    [Tooltip("layer the player is on (only used if requireTargetCollider = true)")]
    public LayerMask targetMask;

    [Header("behavior")]
    [Tooltip("If true, requires hitting the target's collider. If false, clear LOS to target point counts as seeing.")]
    public bool requireTargetCollider = false;

    // Exposed state for other systems (wander/flee, UI, etc.)
    public bool IsSeeing { get; private set; }
    public Action<bool> OnSeeingChanged;

    bool wasSeeing;

    void Reset()
    {
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
            IsSeeing = seeing;
            OnSeeingChanged?.Invoke(seeing);
        }
    }

    bool CanSeeTarget()
    {
        if (!target) return false;

        Vector3 eyePos = eye ? eye.position : transform.position + Vector3.up * eyeHeight;

        Vector3 toTarget = target.position - eyePos;
        float dist = toTarget.magnitude;
        if (dist > viewDistance) return false;

        // FOV check in XZ plane
        Vector3 flatForward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        Vector3 flatToTarget = new Vector3(toTarget.x, 0f, toTarget.z);
        if (flatToTarget.sqrMagnitude > 0.0001f)
        {
            float angle = Vector3.Angle(flatForward, flatToTarget.normalized);
            if (angle > viewAngle * 0.5f) return false;
        }

        if (requireTargetCollider)
        {
            int mask = obstructionMask | targetMask;
            if (Physics.Raycast(eyePos, toTarget.normalized, out RaycastHit hit, dist, mask, QueryTriggerInteraction.Ignore))
            {
                return hit.transform == target || hit.transform.IsChildOf(target);
            }
            return false;
        }
        else
        {
            // No obstruction between eye and target point = visible.
            if (Physics.Raycast(eyePos, toTarget.normalized, dist, obstructionMask, QueryTriggerInteraction.Ignore))
                return false;
            return true;
        }
    }

    void OnDisable()
    {
        if (wasSeeing)
        {
            FreezeManager.SetWatching(this, false);
            wasSeeing = false;
            IsSeeing = false;
            OnSeeingChanged?.Invoke(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = eye ? eye.position : transform.position + Vector3.up * eyeHeight;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, 0.08f);

        Quaternion leftRot = Quaternion.AngleAxis(-viewAngle * 0.5f, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(viewAngle * 0.5f, Vector3.up);
        Vector3 left = leftRot * transform.forward;
        Vector3 right = rightRot * transform.forward;

        Gizmos.DrawLine(origin, origin + left * viewDistance);
        Gizmos.DrawLine(origin, origin + right * viewDistance);

        int segs = 18;
        Vector3 prev = origin + left * viewDistance;
        for (int i = 1; i <= segs; i++)
        {
            float t = i / (float)segs;
            Quaternion r = Quaternion.AngleAxis(Mathf.Lerp(-viewAngle * 0.5f, viewAngle * 0.5f, t), Vector3.up);
            Vector3 dir = r * transform.forward;
            Vector3 next = origin + dir * viewDistance;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}
