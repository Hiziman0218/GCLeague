using UnityEngine;
using UnityEngine.UI;
using Game.Enum;

public class GameSettingUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Dropdown m_quizTypeDropdown;
    [SerializeField] private Slider m_difficultySlider;
    [SerializeField] private Slider m_quizNumberSlider;
    [SerializeField] private Slider m_lifeSlider;
    [SerializeField] private Slider m_timerSlider;

    [Header("表示テキスト（TM）")]
    [SerializeField] private Text m_difficultyText;
    [SerializeField] private Text m_quizNumberText;
    [SerializeField] private Text m_lifeText;
    [SerializeField] private Text m_timerText;

    private GameSetting m_gameSetting; //ゲーム設定の参照

    private void Start()
    {
        //------------------------------
        // クイズタイプ（Dropdown）
        //------------------------------
        m_quizTypeDropdown.onValueChanged.AddListener(val =>
            m_gameSetting.SetQuizType((QuizType)val)
        );

        //------------------------------
        // 難易度（Slider）
        //------------------------------
        m_difficultySlider.onValueChanged.AddListener(val =>
        {
            int difficulty = (int)val;
            m_gameSetting.SetDifficulty(difficulty);
            m_difficultyText.text = difficulty.ToString();
        });

        //------------------------------
        // 問題数（Slider, 10刻み）
        //------------------------------
        m_quizNumberSlider.onValueChanged.AddListener(val =>
        {
            int snapped = Mathf.RoundToInt(val / 10f) * 10;
            m_gameSetting.SetQuizNumber(snapped);
            m_quizNumberText.text = snapped.ToString();
        });

        //------------------------------
        // 残機（Slider）
        //------------------------------
        m_lifeSlider.onValueChanged.AddListener(val =>
        {
            int life = (int)val;
            m_gameSetting.SetLife(life);
            m_lifeText.text = life.ToString();
        });

        //------------------------------
        // タイマー（Slider, 30刻み）
        //------------------------------
        m_timerSlider.onValueChanged.AddListener(val =>
        {
            float snapped = Mathf.Round(val / 30f) * 30f;
            m_gameSetting.SetTimer(snapped);
            m_timerText.text = snapped.ToString("0");
        });

        //------------------------------
        // 初期値を UI に反映
        //------------------------------
        RefreshUI();
    }

    /// <summary>
    /// ゲーム設定を設定し、UIも更新する
    /// </summary>
    public void SetGameSetting(GameSetting gameSetting)
    {
        m_gameSetting = gameSetting;
        RefreshUI();
    }

    /// <summary>
    /// スライダーとテキストの表示を、現在の GameSetting に合わせて初期化
    /// </summary>
    private void RefreshUI()
    {
        if (m_gameSetting == null) return;

        m_difficultySlider.value = m_gameSetting.GetDifficulty();
        m_quizNumberSlider.value = m_gameSetting.GetQuizNumber();
        m_lifeSlider.value = m_gameSetting.GetLife();
        m_timerSlider.value = m_gameSetting.GetTimer();

        m_difficultyText.text = m_gameSetting.GetDifficulty().ToString();
        m_quizNumberText.text = m_gameSetting.GetQuizNumber().ToString();
        m_lifeText.text = m_gameSetting.GetLife().ToString();
        m_timerText.text = m_gameSetting.GetTimer().ToString("0");
    }
}