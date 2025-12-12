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

    [Header("表示テキスト")]
    [SerializeField] private Text m_difficultyText;
    [SerializeField] private Text m_quizNumberText;
    [SerializeField] private Text m_lifeText;
    [SerializeField] private Text m_timerText;

    private GameSetting m_gameSetting;

    private bool m_isInitializing = false;

    private void Start()
    {
        //クイズタイプ
        m_quizTypeDropdown.onValueChanged.AddListener(val =>
        {
            if (m_isInitializing) return;
            m_gameSetting.SetQuizType((QuizType)val);
        });

        //難易度
        m_difficultySlider.onValueChanged.AddListener(val =>
        {
            if (m_isInitializing) return;
            int difficulty = (int)val;
            m_gameSetting.SetDifficulty(difficulty);
            m_difficultyText.text = difficulty.ToString();
        });

        // 問題数(5刻み)
        m_quizNumberSlider.onValueChanged.AddListener(val =>
        {
            if (m_isInitializing) return;
            int snapped = Mathf.RoundToInt(val / 5f) * 5;

            //スライダー側も snap 後の値に更新
            m_quizNumberSlider.SetValueWithoutNotify(snapped);

            m_gameSetting.SetQuizNumber(snapped);
            m_quizNumberText.text = snapped.ToString();
        });

        //残機
        m_lifeSlider.onValueChanged.AddListener(val =>
        {
            if (m_isInitializing) return;
            int life = (int)val;
            m_gameSetting.SetLife(life);
            m_lifeText.text = life.ToString();
        });

        //タイマー(30刻み)
        m_timerSlider.onValueChanged.AddListener(val =>
        {
            if (m_isInitializing) return;
            float snapped = Mathf.Round(val / 30f) * 30f;

            //スライダー側も snap 後の値に更新
            m_timerSlider.SetValueWithoutNotify(snapped);

            m_gameSetting.SetTimer(snapped);
            m_timerText.text = snapped.ToString("0");
        });

        RefreshUI();
    }

    /// <summary>
    /// UIを現在値に合わせて更新
    /// </summary>
    private void RefreshUI()
    {
        m_isInitializing = true;

        m_quizTypeDropdown.value = (int)m_gameSetting.GetQuizType();

        m_difficultySlider.SetValueWithoutNotify(m_gameSetting.GetDifficulty());
        m_quizNumberSlider.SetValueWithoutNotify(m_gameSetting.GetQuizNumber());
        m_lifeSlider.SetValueWithoutNotify(m_gameSetting.GetLife());
        m_timerSlider.SetValueWithoutNotify(m_gameSetting.GetTimer());

        m_difficultyText.text = m_gameSetting.GetDifficulty().ToString();
        m_quizNumberText.text = m_gameSetting.GetQuizNumber().ToString();
        m_lifeText.text = m_gameSetting.GetLife().ToString();
        m_timerText.text = m_gameSetting.GetTimer().ToString("0");

        m_isInitializing = false;
    }

    public void SetGameSetting(GameSetting GameSetting)
    {
        m_gameSetting = GameSetting;
    }
}