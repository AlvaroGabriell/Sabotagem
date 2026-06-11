using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField] private LaserBehaviour[] linkedLasers;
    BoxCollider boxCollider;

    private int overlapCount = 0;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        overlapCount++;
        if(overlapCount == 1){
            foreach (LaserBehaviour laser in linkedLasers)
            {
                laser.Active = !laser.Active;
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
            }
        }
    }
}
