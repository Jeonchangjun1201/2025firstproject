using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Story_Text : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public AudioClip typingAudio;
    public AudioClip clickAudio; // 버튼 클릭 사운드
    public float charInterval = 0.05f;
    public float lineDelay = 1.5f;
    public Button skipButton; // 스킵 버튼 연결

    private AudioSource audioSource;
    private bool isSkipping = false;
    private static GameObject audioKeeper; // 오디오 소스 보존용

    private string[] lines = {
        "기억 회상",
        "주인공 : 여긴 어디지?",
        ".",
        "주인공 : 맞아, 오늘은 저번에 찾아둔 유적을 탐사하러 가는 날 이였지",
        "주인공 : 유적 입구에 도착해서 들어가다가..",
        "주인공 : 그래,바닥의 함정을 밟고 그대로 쭉 추락했지",
        "주인공 : 그럼 여기는 유적의 내부 인건가?",
        ".",
        ".",
        "??? : 들리는가.",
        "(어디서 들리는 소리지?)",
        "주인공 : 당신은 누구십니까?",
        "??? : 나는 이 유적을 지키는 신이다.",
        "??? : 네 자신이 무슨 잘못을 하였는지 알고있는가?",
        "주인공 : 제가 무엇을 잘못하였습니까?",
        "??? : 네놈이 잘못한 것은 감히 신의 영역을 침범한 것이다.",
        "??? : 네 그 어리석은 행동으로 이 유적에 악의 기운이 퍼져버렸다.",
        "??? : 그 결과 유적을 유지하기 위한 핵심 코어인,",
        "??? : 신의 보석을 악의 기운들에게 빼았겼다.",
        "??? : 벌로 너에게 이 던전을 벗어날 수 없는 저주를 내리노라.",
        "??? : 이 저주를 풀고 이곳을 벗어나고 싶다면,",
        "??? : 이 던전이 힘을 완전히 잃어 붕괴되기 전에",
        "??? : 악의 기운이 던전에 숨겨놓은 신의 보석을 찾아 제단에 돌려놓아라.",
        "??? : 신의보석이 제 자리를 찾으면, 저주를 풀어주겠다.",
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
        GoToTitle();
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

    void GoToTitle()
    {
        SceneManager.LoadScene("00.Scenes/TitleScene");
    }
}
