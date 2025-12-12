using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Game.Enum;

public class GameSystemManager : MonoBehaviour
{
    public static GameSystemManager Instance { get; private set; }

    [Header("フェード用UI")]
    [SerializeField] private Image m_fadeImage;            //黒フェード用画像

    [Header("詳細設定")]
    [SerializeField] private GameSettingUI m_settingUI;    //設定UI

    private GameSetting m_setting;

    private bool m_isFadeInFinished = false;
    private bool m_isFadeOutFinished = false;

    private void Awake()
    {
        //シングルトン
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //GameSetting を生成
        m_setting = new GameSetting(QuizType.Normal, 1, 10, 1, 3, 60f);

        //設定用UIに設定
        if (m_settingUI != null)
            m_settingUI.SetGameSetting(m_setting);

        //フェード画像初期化
        if (m_fadeImage != null)
        {
            var c = m_fadeImage.color;
            c.a = 0f;
            m_fadeImage.color = c;
        }
    }

    /// <summary>
    /// 現在の設定を GameManager などが取得するための関数
    /// </summary>
    public GameSetting GetGameSetting()
    {
        return m_setting;
    }

    public bool IsFadeInFinished()
    {
        return m_isFadeInFinished;
    }

    public bool IsFadeOutFinished()
    {
        return m_isFadeOutFinished;
    }

    /// <summary>
    /// フェードアウト → シーン遷移 → フェードイン
    /// </summary>
    /// <param name="fadeTime">フェードにかける時間</param>
    /// <param name="waitBetweenFade">フェードアウト完了後に待つ時間</param>
    public void ChangeScene(float fadeTime = 1.0f, float waitBetweenFade = 1.0f)
    {
        StartCoroutine(ChangeSceneRoutine(fadeTime, waitBetweenFade));
    }

    private IEnumerator ChangeSceneRoutine(float fadeTime, float waitBetweenFade)
    {
        //フェードアウト
        yield return StartCoroutine(FadeOut(fadeTime));

        //待機
        yield return new WaitForSeconds(waitBetweenFade);

        //フェードイン
        yield return StartCoroutine(FadeIn(fadeTime));

        //1フレーム後にフラグリセット
        yield return null;
        Initialize();
    }

    /// <summary>
    /// フェードイン（黒 → 透明）
    /// </summary>
    public IEnumerator FadeIn(float time)
    {
        m_isFadeInFinished = false;

        float t = 0f;
        Color c = m_fadeImage.color;

        while (t < time)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / time);
            m_fadeImage.color = c;
            yield return null;
        }

        m_isFadeInFinished = true;
    }

    /// <summary>
    /// フェードアウト（透明 → 黒）
    /// </summary>
    public IEnumerator FadeOut(float time)
    {
        m_isFadeOutFinished = false;

        float t = 0f;
        Color c = m_fadeImage.color;

        while (t < time)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / time);
            m_fadeImage.color = c;
            yield return null;
        }

        m_isFadeOutFinished = true;
    }

    /// <summary>
    /// フェードイン/アウト完了フラグを初期化
    /// </summary>
    public void Initialize()
    {
        m_isFadeInFinished = false;
        m_isFadeOutFinished = false;
    }

    /// <summary>
    /// ボタンでの呼び出し用
    /// </summary>
    public void ChangeSceneInButton()
    {
        ChangeScene();
    }
}
