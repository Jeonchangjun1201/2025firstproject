using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public GameObject soundSettingsPanel; // 소리 세팅 패널

    // 게임 시작 버튼에 연결
    public void OnStartGame()
    {
        SceneManager.LoadScene("GameeScene"); // 실제 게임 씬 이름으로 변경
    }

    // 튜토리얼 버튼에 연결
    public void OnTutorial()
    {
        SceneManager.LoadScene("Tutorial"); // 튜토리얼 씬 이름으로 변경
    }

    // 소리 세팅 버튼에 연결
    public void OnSoundSettings()
    {
        if (soundSettingsPanel != null)
            soundSettingsPanel.SetActive(true); // 소리 세팅 패널 열기
    }
}