using UnityEngine;
using UnityEngine.UI;
using Game.Enum;

public class HUD : UIBase
{
    [Header("UI")]
    [Tooltip("クイズ形式")]
    [SerializeField] Text m_quizType;
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

    private GameSetting m_gameSetting; //ゲームの設定保持用

    //各動的に変更される値の保持用
    private float m_currentDifficultyValue;
    private int m_currentQuizNumberValue;
    private int m_currentLifeValue;

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
        if (m_currentQuizNumberValue + 1 >= m_gameSetting.GetQuizNumber()) m_currentQuizNumberValue = m_gameSetting.GetQuizNumber() - 1;
        m_currentQuizNumber.text = $"{m_currentQuizNumberValue + 1}"; //値が最大問題数を超えないよう調整する
        m_life.text = $"{m_currentLifeValue}";
        m_playerNumber.text = $"{m_gameSetting.GetPlayerNumber()}人";
    }

    /// <summary>
    /// ゲームの設定を設定
    /// </summary>
    /// <param name="Setting"></param>
    public void SetGameSetting(GameSetting Setting)
    {
        //nullの状態のみ設定
        if(m_gameSetting == null) m_gameSetting = Setting;

        //ゲーム中に変更されない要素はここで設定
        //クイズ形式
        switch (m_gameSetting.GetQuizType())
        {
            case QuizType.Normal:
                m_quizType.text = "【通常クイズ】";
                break;
            case QuizType.Anabuki:
                m_quizType.text = "【穴吹クイズ】";
                break;
        }
        //総問題数
        m_quizNumber.text = $"{m_gameSetting.GetQuizNumber()}";
    }

    /// <summary>
    /// 現在の難易度を設定
    /// </summary>
    /// <param name="CurrentDifficulty">現在の難易度</param>
    public void SetCurrentDifficulty(float CurrentDifficulty)
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
}
