using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Select, Vector3.zero);
    }

    public void OnClick()
    {
        AudioManager.Instance.PlayOneShot(AudioEvents.SFX.Menu.Click, Vector3.zero);
    }
}
