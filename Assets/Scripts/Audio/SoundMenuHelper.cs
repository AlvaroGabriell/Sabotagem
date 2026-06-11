using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundMenuHelper : MonoBehaviour
{
    [SerializeField] private Slider masterSlider, musicSlider, sfxSlider;
    public TextMeshProUGUI masterPercentage, musicPercentage, sfxPercentage;
    
    void Start()
    {
        // Carrega os valores salvos
        masterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("masterVolume", 1f));
        musicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("musicVolume", 1f));
        sfxSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("sfxVolume", 1f));

        // Conecta os listeners
        masterSlider.onValueChanged.AddListener(value =>
        {
            AudioManager.Instance.SetMasterVolume(value);
            UpdatePercentage(masterPercentage, value);
        });
        musicSlider.onValueChanged.AddListener(value =>
        {
            AudioManager.Instance.SetMusicVolume(value);
            UpdatePercentage(musicPercentage, value);
        });
        sfxSlider.onValueChanged.AddListener(value =>
        {
            AudioManager.Instance.SetSFXVolume(value);
            UpdatePercentage(sfxPercentage, value);
        });

        // Inicializa os labels com os valores atuais
        UpdatePercentage(masterPercentage, masterSlider.value);
        UpdatePercentage(musicPercentage, musicSlider.value);
        UpdatePercentage(sfxPercentage, sfxSlider.value);
    }

    void OnDestroy()
    {
        masterSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
    }

    private void UpdatePercentage(TextMeshProUGUI label, float value)
    {
        label.text = Mathf.RoundToInt(value * 100) + "%";
    }
}
