using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RealEnding : MonoBehaviour
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
        "<RealEnding>",
        "주인공은 숲속에서 깨어나",
        "손에 쥔 보석들을 한참 바라본다.",
        "모험의 여운이 아직도 몸에 남아 있지만,",
        "숲속의 바람과 햇살이 점점 현실감을 되찾게 해준다.",
        "",
        "주인공은 가벼운 마음으로 숲길을 따라 마을로 돌아간다.",
        "길가의 풀잎과 작은 동물들,",
        "그리고 익숙한 풍경이 하나둘 눈에 들어온다.",
        "",
        "마을에 도착하자",
        "친구들과 가족들이 놀란 얼굴로 주인공을 맞이한다.",
        "주인공은 미소를 지으며",
        "보석이 가득 담긴 보따리를 들어 보인다.",
        "",
        "사람들은 모두 환호하고,",
        "아이들은 주인공의 모험담을 듣기 위해 모여든다.",
        "주인공은 그동안의 모험을 이야기하며",
        "웃음과 감탄을 나눈다.",
        "",
        "오랜만에 따뜻한 집에 돌아온 주인공은",
        "창밖으로 노을이 지는 풍경을 바라본다.",
        "손에는 여전히 빛나는 보석들이 남아 있지만,",
        "무엇보다 소중한 건",
        "함께 나누는 평범한 일상과",
        "돌아올 수 있는 집이라는 사실을",
        "새삼 느낀다.",
        "",
        "그렇게 주인공의 모험은 끝났고,",
        "마을에는 오랫동안",
        "행복과 평화가 머물렀다.",
        "End<RealEnding>",
        "<HappyEnd>"
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
