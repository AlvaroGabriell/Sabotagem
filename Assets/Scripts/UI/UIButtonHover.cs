using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler
{
    //TODO: Arrumar classe
    public EventReference hoverSfx, clickSfx;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayOneShot(hoverSfx, Vector3.zero);
    }

    public void OnClick()
    {
        AudioManager.Instance.PlayOneShot(clickSfx, Vector3.zero);
    }
}
