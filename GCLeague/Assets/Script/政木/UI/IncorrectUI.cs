using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IncorrectUI : UIBase
{
    [Header("UI")]
    [SerializeField] private Image m_cross;
    [SerializeField] private Image m_text;

    [Header("アニメーション設定")]
    [SerializeField] private float m_animDuration = 0.3f;

    private Coroutine m_animCoroutine;

    private Vector3 m_scale;

    private CanvasGroup m_crossCG;
    private CanvasGroup m_textCG;

    private void Awake()
    {
        m_scale = transform.localScale;

        m_crossCG = m_cross.GetComponent<CanvasGroup>();
        if (m_crossCG == null) m_crossCG = m_cross.gameObject.AddComponent<CanvasGroup>();

        m_textCG = m_text.GetComponent<CanvasGroup>();
        if (m_textCG == null) m_textCG = m_text.gameObject.AddComponent<CanvasGroup>();

        ShowEvent += PlayIncorrectIn;
        HideEvent += PlayIncorrectOut;
    }

    /// <summary>
    /// 自身をガタガタさせる
    /// </summary>
    public void PlayIncorrectIn()
    {
        if (root == null) return;

        if (m_animCoroutine != null) StopCoroutine(m_animCoroutine);

        root.SetActive(true);

        // アニメ初期化
        m_cross.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        m_text.rectTransform.localScale = Vector3.one * 0.9f;

        root.transform.localScale = m_scale;

        //フェードイン初期値
        m_crossCG.alpha = 1f;
        m_textCG.alpha = 1f;

        m_animCoroutine = StartCoroutine(IncorrectInCoroutine());
    }

    /// <summary>
    /// ガタガタのコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator IncorrectInCoroutine()
    {
        float timer = 0f;

        float[] angles = { 10f, -10f, 5f, 0f };
        int index = 0;

        while (timer < m_animDuration)
        {
            timer += Time.deltaTime;
            float t = timer / m_animDuration * angles.Length;

            index = Mathf.Min((int)t, angles.Length - 2);
            float segmentT = t - index;

            float rotZ = Mathf.Lerp(angles[index], angles[index + 1], segmentT);
            m_cross.rectTransform.rotation = Quaternion.Euler(0, 0, rotZ);

            m_text.rectTransform.localScale = Vector3.Lerp(
                Vector3.one * 0.9f,
                Vector3.one,
                Mathf.Clamp01(timer / m_animDuration)
            );

            yield return null;
        }

        m_cross.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        m_text.rectTransform.localScale = Vector3.one;

        ShowClear();
    }

    /// <summary>
    /// 自身をフェードアウトして非表示
    /// </summary>
    public void PlayIncorrectOut()
    {
        if (root == null) return;

        if (m_animCoroutine != null) StopCoroutine(m_animCoroutine);

        m_animCoroutine = StartCoroutine(IncorrectOutCoroutine());
    }

    /// <summary>
    /// フェードアウトのコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator IncorrectOutCoroutine()
    {
        float timer = 0f;

        float startA_cross = m_crossCG.alpha;
        float startA_text = m_textCG.alpha;

        while (timer < m_animDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / m_animDuration);

            m_crossCG.alpha = Mathf.Lerp(startA_cross, 0f, t);
            m_textCG.alpha = Mathf.Lerp(startA_text, 0f, t);

            yield return null;
        }

        // 最終結果
        m_crossCG.alpha = 0f;
        m_textCG.alpha = 0f;

        root.SetActive(false);

        HideClear();
    }
}