using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NormalEnding : MonoBehaviour
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
        "제단 위에 마지막 보석을 올려놓자",
        "신비로운 빛이 눈 앞을 가득 채웠다.",
        "주인공은 눈을 감고, 온몸을 감싸는 따스한 기운과",
        "어딘가 익숙한 향기를 느꼈다.",
        ".",
        "잠시 후,",
        "희미한 빛 속에서 천천히 눈을 뜬 주인공은",
        "자신이 낯익은 방의 침대에 누워 있다는 사실을 깨달았다.",
        "창밖으로는 아침 햇살이 스며들고,",
        "방 안은 고요했다.",
        ".",
        "주인공 : …분명히 나는 유적에서 제단에 보석을 바쳤는데?",
        "주인공은 손을 들어 자신의 손바닥을 바라보았다.",
        "유적에서의 감각,",
        "차가운 돌바닥,",
        "숨 막히는 긴장감,",
        "그리고 마지막 순간의 환희까지",
        "모두가 너무나도 생생했다.",
        "하지만,",
        "방 안에는 보석도, 먼지 묻은 지도도,",
        "모험의 흔적도 남아 있지 않았다.",
        "주인공 : 정말 꿈이었을까…?",
        "주인공은 침대에 앉아 한참을 멍하니 있다가",
        "조용히 창밖을 바라보았다.",
        ".",
        "아직도 귓가에는",
        "제단에서 울리던 신비로운 소리가",
        "희미하게 맴도는 듯했다.",
        "이 모든 것이 꿈이었는지,",
        "아니면 또 다른 세계에서의 진짜 경험이었는지",
        "주인공은 끝내 확신할 수 없었다.",
        "End<NormalEnding>",
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
        SceneManager.LoadScene("TitleScene");
    }
}
