using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Game.Enum;

public class StartUI : UIBase
{
    [Header("UI")]
    [Tooltip("クイズ形式")]
    [SerializeField] Text m_quizType;
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
    [SerializeField] private float expansionDuration = 0.3f; //拡大アニメーション時間

    private GameSetting m_gameSetting; //ゲームの設定保持用
    private Coroutine animCoroutine;   //コルーチン管理用

    private void Awake()
    {
        //表示/非表示イベントを追加
        ShowEvent += ExpansionIn;
        HideEvent += ReductionOut;
    }

    private void Update()
    {
        UpdateStartUI();
    }

    /// <summary>
    /// StartUI更新
    /// </summary>

    public void UpdateStartUI()
    {
        switch (m_gameSetting.GetQuizType())
        {
            case QuizType.Normal:
                m_quizType.text = "【通常クイズ】";
                break;
            case QuizType.Anabuki:
                m_quizType.text = "【穴吹クイズ】";
                break;
        }

        m_difficulty.text = $"{m_gameSetting.GetDifficulty()}";
        m_quizNumber.text = $"全{m_gameSetting.GetQuizNumber()}問";
        m_playerNumber.text = $"{m_gameSetting.GetPlayerNumber()}人";
        m_life.text = $"{m_gameSetting.GetLife()}";
        m_timer.text = $"{m_gameSetting.GetTimer()}秒";
    }

    /// <summary>
    /// ゲームの設定を設定
    /// </summary>
    /// <param name="gameSetting"></param>
    public void SetGameSetting(GameSetting gameSetting)
    {
        m_gameSetting = gameSetting;
    }

    /// <summary>
    /// 自身を拡大して表示
    /// </summary>
    public void ExpansionIn()
    {
        //親オブジェクトが設定されていなければ、以降の処理を行わない
        if (root == null) return;

        // すでにアニメーションが動いていたら止める
        if (animCoroutine != null) StopCoroutine(animCoroutine);

        //拡大前の設定
        root.SetActive(true);
        root.transform.localScale = Vector3.zero;

        //拡大開始
        animCoroutine = StartCoroutine(ExpansionCoroutine());
    }

    /// <summary>
    /// 拡大表示のコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExpansionCoroutine()
    {
        float timer = 0f;
        while (timer < expansionDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / expansionDuration);
            root.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        root.transform.localScale = Vector3.one;
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
        if (animCoroutine != null) StopCoroutine(animCoroutine);

        //縮小開始
        animCoroutine = StartCoroutine(ReductionCoroutine());
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

        while (timer < expansionDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / expansionDuration);
            root.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        root.transform.localScale = Vector3.zero;
        root.SetActive(false);
        HideClear();
    }
}
