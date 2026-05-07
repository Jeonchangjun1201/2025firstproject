using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RichEnding : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public AudioClip typingAudio;
    public AudioClip clickAudio; // 버튼 클릭 사운드
    public float charInterval = 0.05f;
    public float lineDelay = 0.5f;
    public Button skipButton; // 스킵 버튼 연결

    private AudioSource audioSource;
    private bool isSkipping = false;
    private static GameObject audioKeeper; // 오디오 소스 보존용

    private string[] lines = {
        "Ending",
        "눈을 뜨자,",
        "자신이 푸르고 적막한 숲 한가운데에 누워 있다는 것을 깨달았다.",
        "주위를 둘러보았지만,",
        "불과 얼마 전까지 있었던 거대한 유적도,",
        "신비로운 제단도,",
        "모든 것은 흔적도 없이 사라지고 없었다.",
        "",
        "주인공 : 분명히… 나는 유적 안에서 모든 보석을 모았는데…",
        "주인공은 어리둥절한 마음으로",
        "자신이 메고 있던 보따리를 풀어보았다.",
        "",
        "그 안에는",
        "유적 깊은 곳에서 어렵게 모았던",
        "희귀한 보석들이",
        "그대로 담겨 있었다.",
        "",
        "숲은 고요했고,",
        "아무도 주인공을 방해하지 않았다.",
        "유적에 들어가기 전의 기억과",
        "방금 전까지의 모험이",
        "현실과 꿈의 경계처럼 뒤섞여 있었다.",
        "",
        "'이 보석들은… 어떻게 된 거지?'",
        "주인공은 보따리를 꼭 쥔 채",
        "한동안 그 자리에 앉아 있었다.",
        "",
        "바람이 나뭇잎을 흔들고,",
        "머나먼 어딘가에서 새가 우는 소리가 들려왔다.",
        "주인공은 천천히 일어나",
        "숲속 어딘가로 발걸음을 옮겼다.",
        "",
        "이후,",
        "주인공이 어떤 삶을 살았는지",
        "알지 못했다.",
        "",
        "다만,",
        "숲 어귀를 지나는 이들 중",
        "가끔씩 반짝이는 빛을 목격했다는",
        "이야기만이 전해질 뿐이었다.",
        "End<HappyEnd>",
        "End<Happy?End>",
        "",
        "",
        "Loading<RealEnd>"
    };


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // 오디오 소스가 붙은 오브젝트를 씬 전환 시에도 유지
        if (audioKeeper == null)
        {
            audioKeeper = audioSource.gameObject;
            DontDestroyOnLoad(audioKeeper);
        }

        // 스킵 버튼 클릭 이벤트 등록
        if (skipButton != null)
            skipButton.onClick.AddListener(SkipStory);
    }

    void Start()
    {
        textUI.alignment = TextAlignmentOptions.Center;
        StartCoroutine(ShowLines());
    }

    IEnumerator ShowLines()
    {
        foreach (string line in lines)
        {
            if (isSkipping)
            {
                yield break;
            }
            yield return StartCoroutine(TypeLine(line));
            yield return new WaitForSeconds(lineDelay);
        }
        yield return new WaitForSeconds(0.5f);
        GoToRealEnding();
    }

    IEnumerator TypeLine(string line)
    {
        textUI.text = "";

        if (typingAudio != null)
        {
            audioSource.clip = typingAudio;
            audioSource.Play();
        }

        foreach (char c in line)
        {
            if (isSkipping)
            {
                audioSource.Stop();
                yield break;
            }
            textUI.text += c;
            yield return new WaitForSeconds(charInterval);
        }

        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    public void SkipStory()
    {
        if (!isSkipping)
        {
            isSkipping = true;

            // 클릭 사운드 즉시 재생
            if (clickAudio != null)
            {
                audioSource.Stop(); // 혹시 재생 중인 타이핑 사운드 중지
                audioSource.PlayOneShot(clickAudio);
            }

            GoToTitle(); // 즉시 씬 전환
        }
    }
    void GoToRealEnding()
    {
        SceneManager.LoadScene("RealEnding");
    }
    void GoToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
