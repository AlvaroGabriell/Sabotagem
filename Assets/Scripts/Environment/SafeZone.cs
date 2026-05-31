using UnityEngine;

public class SafeZone : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            player.TryUpdateSafePos();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        OnTriggerStay(other);
    }
}
