using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField] private LaserBehaviour[] linkedLasers;

    private int overlapCount = 0;

    void OnTriggerEnter(Collider other)
    {
        overlapCount++;
        if(overlapCount == 1){
            foreach (LaserBehaviour laser in linkedLasers)
            {
                laser.Active = !laser.Active;
                AudioManager.Instance.PlayOneShot(AudioEvents.SFX.PressurePlate.Press, gameObject.transform.position);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        overlapCount--;
        if(overlapCount == 0)
        {
            foreach (LaserBehaviour laser in linkedLasers)
            {
                laser.Active = !laser.Active;
                AudioManager.Instance.PlayOneShot(AudioEvents.SFX.PressurePlate.Release, gameObject.transform.position);
            }
        }
    }
}
