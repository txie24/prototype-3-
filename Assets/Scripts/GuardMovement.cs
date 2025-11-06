// GuardWanderNavMesh.cs
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(GuardVision))]
public class GuardWanderNavMesh : MonoBehaviour
{
    [Header("wander")]
    public float wanderRadius = 12f;
    public Vector2 idleDelay = new Vector2(0.5f, 1.5f);
    public int sampleTries = 20;

    [Header("optional roam bounds")]
    public Transform roamCenter;
    public float roamRadius = 0f;

    [Header("flee")]
    [Tooltip("How far ahead to try fleeing when the angel is seen.")]
    public float fleeDistance = 8f;
    [Tooltip("How often to refresh the flee destination while seeing the angel.")]
    public float fleeRepathInterval = 0.25f;
    [Tooltip("Speed multiplier while fleeing.")]
    public float fleeSpeedMultiplier = 1.25f;
    [Tooltip("Random sideways jitter while choosing a flee point.")]
    public float fleeSideJitter = 1.5f;

    NavMeshAgent agent;
    GuardVision vision;
    float baseSpeed;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<GuardVision>();

        agent.updateRotation = false; // rotation is handled elsewhere
        if (agent.stoppingDistance < 0.1f) agent.stoppingDistance = 0.1f;
        if (agent.radius < 0.1f) agent.radius = 0.3f;

        baseSpeed = Mathf.Max(0.01f, agent.speed);
    }

    void OnEnable()
    {
        if (vision != null) vision.OnSeeingChanged += OnSeeingChanged;
    }

    void OnDisable()
    {
        if (vision != null) vision.OnSeeingChanged -= OnSeeingChanged;
    }

    void OnSeeingChanged(bool seeing)
    {
        // Nudge speed immediately when the state flips
        agent.speed = seeing ? baseSpeed * fleeSpeedMultiplier : baseSpeed;
        // Clear current path so we re-evaluate quickly
        if (agent.isOnNavMesh) agent.ResetPath();
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.2f));

        while (true)
        {
            if (!agent.isOnNavMesh) { yield return null; continue; }

            if (vision && vision.IsSeeing && vision.target)
            {
                // Flee state: keep choosing a point away from the angel
                if (TryPickFleePoint(vision.target.position, out Vector3 fleeTo))
                    agent.SetDestination(fleeTo);

                yield return new WaitForSeconds(fleeRepathInterval);
                continue;
            }

            // Wander state (your original logic)
            bool needNewPoint = !agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + 0.05f;
            if (needNewPoint)
            {
                float wait = Mathf.Clamp(Random.Range(idleDelay.x, idleDelay.y), 0f, 10f);
                if (wait > 0f) yield return new WaitForSeconds(wait);

                if (TryPickRandomPoint(out Vector3 p))
                    agent.SetDestination(p);
            }

            yield return null;
        }
    }

    bool TryPickFleePoint(Vector3 threatPos, out Vector3 result)
    {
        Vector3 origin = transform.position;
        Vector3 away = origin - threatPos;
        away.y = 0f;

        if (away.sqrMagnitude < 0.0001f)
            away = transform.forward; // degenerate case

        away.Normalize();

        // Try a few distances forward, with a bit of sideways jitter to avoid dead-ends
        float[] dists = { fleeDistance, fleeDistance * 0.75f, fleeDistance * 0.5f, fleeDistance * 0.33f };
        for (int i = 0; i < dists.Length; i++)
        {
            Vector2 jitter = Random.insideUnitCircle * fleeSideJitter;
            Vector3 sideways = new Vector3(jitter.x, 0f, jitter.y);
            Vector3 candidate = origin + away * dists[i] + sideways;

            // keep inside an optional roam bubble, if configured
            if (roamCenter && roamRadius > 0f)
            {
                Vector3 to = candidate - roamCenter.position; to.y = 0f;
                if (to.magnitude > roamRadius)
                    candidate = roamCenter.position + to.normalized * roamRadius;
            }

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = origin;
        return false;
    }

    bool TryPickRandomPoint(out Vector3 result)
    {
        Vector3 center = roamCenter ? roamCenter.position : transform.position;
        float radius = roamRadius > 0f ? roamRadius : wanderRadius;

        for (int i = 0; i < sampleTries; i++)
        {
            Vector2 r = Random.insideUnitCircle * radius;
            Vector3 candidate = new Vector3(center.x + r.x, center.y, center.z + r.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = transform.position;
        return false;
    }

    void OnDrawGizmosSelected()
    {
        var c = roamCenter ? roamCenter.position : transform.position;
        float r = roamRadius > 0f ? roamRadius : wanderRadius;

        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.15f);
        Gizmos.DrawSphere(c, 0.1f);
        Gizmos.DrawWireSphere(c, r);
    }
}
