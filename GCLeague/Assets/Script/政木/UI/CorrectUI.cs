using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CorrectUI : UIBase
{
    [Header("UI")]
    [SerializeField] private Image m_circle;
    [SerializeField] private Image m_text;

    [Header("アニメーション設定")]
    [SerializeField] private float m_animDuration = 0.3f;

    private CanvasGroup m_circleCG;
    private CanvasGroup m_textCG;

    private Coroutine m_animCoroutine;

    private Vector3 m_scale;

    private Vector2 m_textDefaultPos;

    private void Awake()
    {
        m_circleCG = m_circle.GetComponent<CanvasGroup>();
        if (m_circleCG == null) m_circleCG = m_circle.gameObject.AddComponent<CanvasGroup>();

        m_textCG = m_text.GetComponent<CanvasGroup>();
        if (m_textCG == null) m_textCG = m_text.gameObject.AddComponent<CanvasGroup>();

        m_textDefaultPos = m_text.rectTransform.anchoredPosition;

        m_scale = transform.localScale;

        ShowEvent += PlayCorrectIn;
        HideEvent += PlayCorrectOut;
    }

    /// <summary>
    /// 自身をスライド/フェードで表示
    /// </summary>
    public void PlayCorrectIn()
    {
        //親オブジェクトが設定されていなければ、以降の処理を行わない
        if (root == null) return;

        //すでにアニメーションが動いていたら止める
        if (m_animCoroutine != null) StopCoroutine(m_animCoroutine);

        //演出前の設定
        root.SetActive(true);

        //初期状態
        m_circle.rectTransform.localScale = new Vector3(0.7f, 0.7f, 1);
        m_circleCG.alpha = 0;

        m_text.rectTransform.anchoredPosition = m_textDefaultPos + new Vector2(0, 20f);
        m_textCG.alpha = 0;

        root.transform.localScale = m_scale;

        //演出開始
        m_animCoroutine = StartCoroutine(CorrectInCoroutine());
    }

    /// <summary>
    /// スライド/フェードインのコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator CorrectInCoroutine()
    {
        float timer = 0f;

        while (timer < m_animDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / m_animDuration);

            // 円：ふわっと
            m_circle.rectTransform.localScale = Vector3.Lerp(
                new Vector3(0.7f, 0.7f, 1),
                Vector3.one,
                t
            );
            m_circleCG.alpha = t;

            // 文字：上から落ちてくる
            m_text.rectTransform.anchoredPosition = Vector2.Lerp(
                m_textDefaultPos + new Vector2(0, 20f),
                m_textDefaultPos,
                t
            );
            m_textCG.alpha = t;

            yield return null;
        }

        ShowClear();
    }

    /// <summary>
    /// 自身をフェードアウトして非表示
    /// </summary>
    public void PlayCorrectOut()
    {
        //親オブジェクトが設定されていなければ、以降の処理を行わない
        if (root == null) return;

        // すでにアニメーションが動いていたら止める
        if (m_animCoroutine != null) StopCoroutine(m_animCoroutine);

        //演出開始
        m_animCoroutine = StartCoroutine(CorrectOutCoroutine());
    }

    /// <summary>
    /// フェードアウトのコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator CorrectOutCoroutine()
    {
        float timer = 0f;

        // 現在のアルファを記憶（基本は 1 のはず）
        float startAlphaCircle = m_circleCG.alpha;
        float startAlphaText = m_textCG.alpha;

        while (timer < m_animDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / m_animDuration);

            // α を徐々に 0 に
            m_circleCG.alpha = Mathf.Lerp(startAlphaCircle, 0f, t);
            m_textCG.alpha = Mathf.Lerp(startAlphaText, 0f, t);

            yield return null;
        }

        // 最終状態
        m_circleCG.alpha = 0f;
        m_textCG.alpha = 0f;

        // 親オブジェクトを非表示
        root.SetActive(false);

        // フラグ完了
        HideClear();
    }
}
