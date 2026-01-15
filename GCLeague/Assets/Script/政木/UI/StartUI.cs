using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Game.Enum;

public class StartUI : UIBase
{
    [Header("UI")]
    [Tooltip("最初の難易度")]
    [SerializeField] Text m_difficulty;
    [Tooltip("クイズの総問題数")]
    [SerializeField] Text m_quizNumber;
    [Tooltip("プレイヤーの人数")]
    [SerializeField] Text m_playerNumber;
    [Tooltip("残機")]
    [SerializeField] Text m_life;
    [Tooltip("回答にかけられる時間")]
    [SerializeField] Text m_timer;

    [Header("アニメーション設定")]
    [SerializeField] private float m_expansionDuration = 0.3f; //拡大アニメーション時間
    [SerializeField] private float m_autoHideDelay = 5f; //表示している期間

    private int m_currentPlayerCount = 0;

    private Coroutine m_animCoroutine; //コルーチン管理用

    private void Awake()
    {
        Type = UIType.StartUI;
    }

    private void Update()
    {
        UpdateStartUI();
    }

    public override void RegistrationEvent()
    {
        //表示/非表示イベントを追加
        ShowEvent += ExpansionIn;
        HideEvent += ReductionOut;
    }

    /// <summary>
    /// StartUI更新
    /// </summary>

    public void UpdateStartUI()
    {
        m_difficulty.text = $"{GameManager.Instance.GetSettingDifficulty()}";
        m_quizNumber.text = $"全{GameManager.Instance.GetSettingQuizNumber()}問";
        m_playerNumber.text = $"{m_currentPlayerCount}人";
        m_life.text = $"{GameManager.Instance.GetSettingLife()}";
        m_timer.text = $"{GameManager.Instance.GetSettingTimer()}秒";
    }

    /// <summary>
    /// 自身を拡大して表示
    /// </summary>
    public void ExpansionIn()
    {
        //親オブジェクトが設定されていなければ、以降の処理を行わない
        if (root == null) return;

        //すでにアニメーションが動いていたら止める
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
            root.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        root.transform.localScale = Vector3.one;
        ShowClear();

        m_animCoroutine = StartCoroutine(AutoHideCoroutine());
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

    /// <summary>
    /// 数秒待ってから自動的に非表示
    /// </summary>
    /// <returns></returns>
    private IEnumerator AutoHideCoroutine()
    {
        //数秒待つ
        yield return new WaitForSeconds(m_autoHideDelay);

        //非表示開始
        Hide();
    }

    /// <summary>
    /// 現在のプレイヤーの人数を設定
    /// </summary>
    /// <param name="PlayerCount"></param>
    public void SetPlayerCount(int PlayerCount)
    {
        m_currentPlayerCount = PlayerCount;
    }
}
