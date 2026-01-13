using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Game.Enum;

public class QuizUI : UIBase
{
    [Header("UI")]
    [Tooltip("問題文")]
    [SerializeField] private Text m_question;
    [Tooltip("回答文1")]
    [SerializeField] private Text m_answer1;
    [Tooltip("回答文2")]
    [SerializeField] private Text m_answer2;
    [Tooltip("回答画像1")]
    [SerializeField] private Image m_choise1;
    [Tooltip("回答画像2")]
    [SerializeField] private Image m_choise2;

    [Header("アニメーション設定")]
    [SerializeField] private float m_expansionDuration = 0.3f; //拡大アニメーション時間

    private Vector3 m_scale = Vector3.one;

    private Coroutine m_animCoroutine; //コルーチン管理用

    private void Awake()
    {
        //初期拡縮を保存
        m_scale = transform.localScale;

        Type = UIType.QuizUI;
    }

    public override void RegistrationEvent()
    {
        //表示/非表示イベントを追加
        ShowEvent += ExpansionIn;
        HideEvent += ReductionOut;
    }

    /// <summary>
    /// クイズの内容を設定
    /// </summary>
    /// <param name="quiz">表示するクイズ</param>
    public void SetQuiz(Quiz quiz)
    {
        m_question.text = quiz.QuestionText;
        m_answer1.text = quiz.Choice1;
        m_answer2.text = quiz.Choice2;
        m_choise1.sprite = quiz.Choice1Image;
        m_choise2.sprite = quiz.Choice2Image;
    }

    /// <summary>
    /// 自身を拡大して表示
    /// </summary>
    public void ExpansionIn()
    {
        //親オブジェクトが設定されていなければ、以降の処理を行わない
        if (root == null) return;

        // すでにアニメーションが動いていたら止める
        if (m_animCoroutine != null) StopCoroutine(m_animCoroutine);

        //拡大前の設定
        root.SetActive(true);
        root.transform.localScale = Vector3.zero;

        //拡大開始
        m_animCoroutine = StartCoroutine(ExpansionCoroutine());
    }

    /// <summary>
    /// 拡大表示のコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExpansionCoroutine()
    {
        float timer = 0f;
        while (timer < m_expansionDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / m_expansionDuration);
            root.transform.localScale = Vector3.Lerp(Vector3.zero, m_scale, t);
            yield return null;
        }
        root.transform.localScale = m_scale;
        ShowClear();
    }

    /// <summary>
    /// 自身を縮小して非表示
    /// </summary>
    public void ReductionOut()
    {
        //親オブジェクトが設定されていなければ、以降の処理を行わない
        if (root == null) return;

        // すでにアニメーションが動いていたら止める
        if (m_animCoroutine != null) StopCoroutine(m_animCoroutine);

        //縮小開始
        m_animCoroutine = StartCoroutine(ReductionCoroutine());
    }

    /// <summary>
    /// 縮小非表示のコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReductionCoroutine()
    {
        float timer = 0f;
        Vector3 startScale = root.transform.localScale;
        Vector3 endScale = Vector3.zero;

        while (timer < m_expansionDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / m_expansionDuration);
            root.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        root.transform.localScale = Vector3.zero;
        root.SetActive(false);
        HideClear();
    }
}
