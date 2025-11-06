using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class AngelWinOnContact : MonoBehaviour
{
    [Tooltip("Tag used by guard objects")]
    public string guardTag = "Guard";

    Rigidbody rb;
    Collider col;

    void Reset()
    {
        guardTag = "Guard";
        var c = GetComponent<Collider>();
        c.isTrigger = true;

        var r = GetComponent<Rigidbody>();
        r.isKinematic = true;
        r.useGravity = false;
    }

    void Awake()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        // enforce the correct physics config for NavMeshAgent-driven characters
        col.isTrigger = true;
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(guardTag))
            GameWinManager.Instance?.Win();
    }

    // safety net in case enter fires between frames
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(guardTag))
            GameWinManager.Instance?.Win();
    }
}
