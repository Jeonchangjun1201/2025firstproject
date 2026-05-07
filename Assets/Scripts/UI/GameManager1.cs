using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance;
    public float maxTime = 900f;
    public float timeLeft;
    private bool isRunning = true;
    private bool playerEscaped = false;
    private TextMeshProUGUI spriteText;
    [Header("타이머 설정")]
    public float totalTime = 900f; // 총 게임 시간(초)
    public TextMeshProUGUI timerText;

    [Header("UI 패널")]
    public GameObject settingsPanel;
    public GameObject gameOverPanel;

    [Header("게임오버 연출")]
    public GameObject blackoutPanel;

    [Header("카메라")]
    public Camera mainCamera;

    void Start()
    {
        timeLeft = totalTime;
        settingsPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        spriteText = timerText.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // ESC 키로 설정 UI 토글 및 일시정지 (미니게임 중엔 동작 안함)
        if (Input.GetKeyDown(KeyCode.Escape) && !MinigameState.IsMinigameActive)
        {
            bool isActive = !settingsPanel.activeSelf;
            settingsPanel.SetActive(isActive);
            Time.timeScale = isActive ? 0 : 1;
        }
        // 미니게임 중 ESC 입력시 안내 메시지 (선택사항)
        else if (Input.GetKeyDown(KeyCode.Escape) && MinigameState.IsMinigameActive)
        {
            Debug.Log("미니게임 중에는 설정창을 열 수 없습니다.");
            // 필요하다면 UI로 안내 메시지 표시
        }

        if (!isRunning) return;

        // 타이머 감소
        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Clamp(timeLeft, 0, maxTime);
        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);
        timerText.text = $"남은 시간: {minutes}분 {seconds}초";

        if (timeLeft <= 0)
        {
            isRunning = false;
            OnTimerEnd();
        }
    }

    public void TimePlus(int time)
    {
        StartCoroutine(GreenColor(time));
    }

    private IEnumerator GreenColor(int time)
    {
        spriteText.color = Color.green;
        timeLeft += time;
        yield return new WaitForSeconds(0.3f);
        spriteText.color = Color.white;
    }

    // 플레이어가 탈출 성공 시 호출
    public void PlayerEscape()
    {
        playerEscaped = true;
        isRunning = false;
    }

    //타이머 종료 시 처리
    void OnTimerEnd()
    {
        if (!playerEscaped)
        {
            StartCoroutine(GameOverSequence());
        }
    }

    // 플레이어 사망 시 외부에서 호출
    public void OnPlayerDeath()
    {
        if (!playerEscaped)
        {
            StartCoroutine(GameOverSequence());
        }
    }

    // 게임오버 연출 코루틴
    IEnumerator GameOverSequence()
    {
        blackoutPanel.SetActive(true);
        yield return StartCoroutine(ScreenShakeEffect(blackoutPanel.transform));
        yield return new WaitForSecondsRealtime(0.2f);
        gameOverPanel.SetActive(true);
    }

    // 화면 흔들림 연출
    IEnumerator ScreenShakeEffect(Transform target)
    {
        float shakeDuration = 0.7f;
        float shakeMagnitude = 20f; // UI는 픽셀 단위
        float elapsed = 0f;
        Vector3 originalPos = target.localPosition;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
            target.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        target.localPosition = originalPos;
    }

    public void OnRoadTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");
    }

    // 재시작 버튼 이벤트
    public void OnRestart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    // 종료 버튼 이벤트
    public void OnExit()
    {
        Application.Quit();
    }
}
