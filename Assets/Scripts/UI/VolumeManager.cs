using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 볼륨 조절 및 볼륨 UI를 관리하는 매니저.
/// </summary>
public class VolumeManager : MonoBehaviour
{
    public Slider volumeSlider;
    public TextMeshProUGUI volumePercentText;

    void Start()
    {
        // 저장된 볼륨 불러오기 (없으면 1)
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
            volumeSlider.value = savedVolume;

        UpdateVolumeText(savedVolume);

        // 슬라이더 값 변경 이벤트 등록
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChange);
    }

    public void OnVolumeChange(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
        UpdateVolumeText(value);
    }

    private void UpdateVolumeText(float value)
    {
        if (volumePercentText != null)
        {
            int percent = Mathf.RoundToInt(value * 100);
            volumePercentText.text = $"{percent}%";
        }
    }
}