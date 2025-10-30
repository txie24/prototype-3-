using UnityEngine;

public class AngelWinOnContact : MonoBehaviour
{
    [Tooltip("Tag used by guard objects")]
    public string guardTag = "Guard";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(guardTag))
            GameWinManager.Instance?.Win();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(guardTag))
            GameWinManager.Instance?.Win();
    }
}
