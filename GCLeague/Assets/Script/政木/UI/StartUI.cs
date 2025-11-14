using UnityEngine;
using TMPro;
using Game.Enum;

public class StartUI : UIBase
{
    [Header("UI")]
    [Tooltip("クイズ形式")]
    [SerializeField] TextMeshProUGUI m_quizType;
    [Tooltip("最初の難易度")]
    [SerializeField] TextMeshProUGUI m_difficulty;
    [Tooltip("クイズの総問題数")]
    [SerializeField] TextMeshProUGUI m_quizNumber;
    [Tooltip("プレイヤーの人数")]
    [SerializeField] TextMeshProUGUI m_playerNumber;
    [Tooltip("残機")]
    [SerializeField] TextMeshProUGUI m_life;
    [Tooltip("回答にかけられる時間")]
    [SerializeField] TextMeshProUGUI m_timer;

    private GameSetting m_gameSetting; //ゲームの設定保持用

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
                m_quizType.text = "通常モード";
                break;
            case QuizType.Anabuki:
                m_quizType.text = "穴吹モード";
                break;
        }

        m_difficulty.text = $"{m_gameSetting.GetDifficulty()}";
        m_quizNumber.text = $"{m_gameSetting.GetQuizNumber()}";
        m_playerNumber.text = $"{m_gameSetting.GetPlayerNumber()}人";
        m_life.text = $"{m_gameSetting.GetLife()}";
        m_timer.text = $"{m_gameSetting.GetTimer()}";
    }

    /// <summary>
    /// ゲームの設定を設定
    /// </summary>
    /// <param name="gameSetting"></param>
    public void SetGameSetting(GameSetting gameSetting)
    {
        m_gameSetting = gameSetting;
    }
}
