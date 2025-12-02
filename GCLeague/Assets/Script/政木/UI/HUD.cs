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

    private GameManager m_gameManager; //ゲームマネージャー保持用
    private GameSetting m_gameSetting; //ゲームの設定保持用

    private void Update()
    {
        UpdateHUD();
    }

    /// <summary>
    /// HUD更新
    /// </summary>
    private void UpdateHUD()
    {
        m_currentDifficulty.text = $"{m_gameManager.GetCurrentDifficulty()}";
        int QuizNumber = m_gameManager.GetCurrentQuizNumber();
        if (QuizNumber >= m_gameSetting.GetQuizNumber()) QuizNumber = m_gameSetting.GetQuizNumber();
        m_currentQuizNumber.text = $"{QuizNumber}";
        m_life.text = $"{m_gameManager.GetLife()}";
        m_playerNumber.text = $"{m_gameSetting.GetPlayerNumber()}人";
    }

    /// <summary>
    /// ゲームの設定を設定
    /// </summary>
    /// <param name="Setting"></param>
    public void SetGameSetting(GameManager Manager, GameSetting Setting)
    {
        //nullの状態のみ設定
        if(m_gameManager == null) m_gameManager = Manager;
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
}
