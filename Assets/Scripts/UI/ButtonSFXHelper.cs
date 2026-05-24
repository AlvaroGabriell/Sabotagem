using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayOneShot("buttonHover", Vector3.zero);
    }

    public void OnClick()
    {
        AudioManager.Instance.PlayOneShot("buttonClick", Vector3.zero);
    }
}
