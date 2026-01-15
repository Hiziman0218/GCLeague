using UnityEngine;
using UnityEngine.UI;
using Game.Enum;

public class HUD : UIBase
{
    [Header("UI")]
    [Tooltip("現在の難易度")]
    [SerializeField] Text m_currentDifficulty;
    [Tooltip("総問題数")]
    [SerializeField] Text m_quizNumber;
    [Tooltip("何問目か")]
    [SerializeField] Text m_currentQuizNumber;
    [Tooltip("プレイヤーの人数")]
    [SerializeField] Text m_playerNumber;
    [Tooltip("残機")]
    [SerializeField] Text m_life;

    //各動的に変更される値の保持用
    private int m_currentDifficultyValue = 0;
    private int m_currentQuizNumberValue = 0;
    private int m_currentLifeValue = 0;
    private int m_currentPlayerCount = 0;

    private void Awake()
    {
        Type = UIType.HUD;
    }

    private void Update()
    {
        UpdateHUD();
    }

    /// <summary>
    /// HUD更新
    /// </summary>
    private void UpdateHUD()
    {
        m_currentDifficulty.text = $"{m_currentDifficultyValue}";
        m_quizNumber.text = $"{GameManager.Instance.GetSettingQuizNumber()}";
        if (m_currentQuizNumberValue + 1 >= GameManager.Instance.GetSettingQuizNumber()) m_currentQuizNumberValue = GameManager.Instance.GetSettingQuizNumber() - 1;
        m_currentQuizNumber.text = $"{m_currentQuizNumberValue + 1}"; //値が最大問題数を超えないよう調整する
        m_life.text = $"{m_currentLifeValue}";
        m_playerNumber.text = $"{m_currentPlayerCount}人";
    }

    /// <summary>
    /// 現在の難易度を設定
    /// </summary>
    /// <param name="CurrentDifficulty">現在の難易度</param>
    public void SetCurrentDifficulty(int CurrentDifficulty)
    {
        m_currentDifficultyValue = CurrentDifficulty;
    }

    /// <summary>
    /// 現在の問題数を設定
    /// </summary>
    /// <param name="CurrentQuizNumber">現在の問題数</param>
    public void SetCurrentQuizNumber(int CurrentQuizNumber)
    {
        m_currentQuizNumberValue = CurrentQuizNumber;
    }

    /// <summary>
    /// 現在の残機を設定
    /// </summary>
    /// <param name="CurrentLife">現在の残機</param>
    public void SetCurrentLife(int CurrentLife)
    {
        m_currentLifeValue = CurrentLife;
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
