using UnityEngine;
using Game.Enum;
using UnityEngine.UI;
using System.Collections;

public class FadeUI : UIBase
{
    [Header("フェード用UI")]
    [SerializeField] private Image m_fadeImage; //黒フェード用画像

    private void Awake()
    {
        Type = UIType.FadeUI;

        //フェード画像初期化
        if (m_fadeImage != null)
        {
            var c = m_fadeImage.color;
            c.a = 0f;
            m_fadeImage.color = c;
        }
    }

    /// <summary>
    /// フェード処理開始
    /// </summary>
    /// <param name="fadeTime">フェードにかける時間</param>
    /// <param name="waitBetweenFade">フェードアウト完了後に待つ時間</param>
    public void StartFade(float fadeTime = 1.0f, float waitBetweenFade = 1.0f)
    {
        StartCoroutine(ChangeSceneRoutine(fadeTime, waitBetweenFade));
    }

    /// <summary>
    /// フェードアウト→指定秒数待機→フェードイン→1フレーム後初期化のまとまった処理
    /// </summary>
    /// <param name="fadeTime"></param>
    /// <param name="waitBetweenFade"></param>
    /// <returns></returns>
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
        float t = 0f;
        Color c = m_fadeImage.color;

        while (t < time)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / time);
            m_fadeImage.color = c;
            yield return null;
        }

        HideClear();
    }

    /// <summary>
    /// フェードアウト（透明 → 黒）
    /// </summary>
    public IEnumerator FadeOut(float time)
    {
        float t = 0f;
        Color c = m_fadeImage.color;

        while (t < time)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / time);
            m_fadeImage.color = c;
            yield return null;
        }

        ShowClear();
    }
}
