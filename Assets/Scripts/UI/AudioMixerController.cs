using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class AudioMixerController : MonoBehaviour
{
    public GameObject SoundSetting;
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI bgmVolumeText;
    public TextMeshProUGUI sfxVolumeText;

    void Start()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float bgm = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

        masterSlider.value = master;
        bgmSlider.value = bgm;
        sfxSlider.value = sfx;

        SetMasterVolume(master);
        SetBGMVolume(bgm);
        SetSFXVolume(sfx);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        masterVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void SetBGMVolume(float value)
    {
        audioMixer.SetFloat("BGMVolume", value > 0 ? Mathf.Log10(value) * 20 : -80);
        PlayerPrefs.SetFloat("BGMVolume", value);
        bgmVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", value > 0 ? Mathf.Log10(value) * 20 : -80);
        PlayerPrefs.SetFloat("SFXVolume", value);
        sfxVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void Close()
    {
        SoundSetting.SetActive(false);
    }
}