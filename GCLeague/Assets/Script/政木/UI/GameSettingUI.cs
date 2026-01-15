using UnityEngine;
using UnityEngine.UI;

public class GameSettingUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider m_difficultySlider;
    [SerializeField] private Slider m_quizNumberSlider;
    [SerializeField] private Slider m_lifeSlider;
    [SerializeField] private Slider m_timerSlider;

    [Header("表示テキスト")]
    [SerializeField] private Text m_difficultyText;
    [SerializeField] private Text m_quizNumberText;
    [SerializeField] private Text m_lifeText;
    [SerializeField] private Text m_timerText;

    private int m_settingDifficulty;    //ゲーム設定における問題の難易度
    private int m_settingQuizNumber;    //ゲーム設定における総問題数
    private int m_settingLife;          //ゲーム設定における残機
    private float m_settingTimer;       //ゲーム設定における一回の回答における制限時間

    private bool m_isInitializing = false;

    private void Start()
    {
        //難易度
        m_difficultySlider.onValueChanged.AddListener(val =>
        {
            if (m_isInitializing) return;
            int difficulty = (int)val;
            GameManager.Instance.CmdSetSettingDifficulty(difficulty);
            m_difficultyText.text = difficulty.ToString();
        });

        // 問題数(5刻み)
        m_quizNumberSlider.onValueChanged.AddListener(val =>
        {
            if (m_isInitializing) return;
            int snapped = Mathf.RoundToInt(val / 5f) * 5;

            //スライダー側も snap 後の値に更新
            m_quizNumberSlider.SetValueWithoutNotify(snapped);

            GameManager.Instance.CmdSetSettingQuizNumber(snapped);
            m_quizNumberText.text = snapped.ToString();
        });

        //残機
        m_lifeSlider.onValueChanged.AddListener(val =>
        {
            if (m_isInitializing) return;
            int life = (int)val;
            GameManager.Instance.CmdSetSettingLife(life);
            m_lifeText.text = life.ToString();
        });

        //タイマー(30刻み)
        m_timerSlider.onValueChanged.AddListener(val =>
        {
            if (m_isInitializing) return;
            float snapped = Mathf.Round(val / 30f) * 30f;

            //スライダー側も snap 後の値に更新
            m_timerSlider.SetValueWithoutNotify(snapped);

            GameManager.Instance.CmdSetSettingTimer(snapped);
            m_timerText.text = snapped.ToString("0");
        });

        RefreshUI();
    }

    /// <summary>
    /// UIを現在値に合わせて更新
    /// </summary>
    public void RefreshUI()
    {
        m_isInitializing = true;

        m_difficultySlider.SetValueWithoutNotify(GameManager.Instance.GetSettingDifficulty());
        m_quizNumberSlider.SetValueWithoutNotify(GameManager.Instance.GetSettingQuizNumber());
        m_lifeSlider.SetValueWithoutNotify(GameManager.Instance.GetSettingLife());
        m_timerSlider.SetValueWithoutNotify(GameManager.Instance.GetSettingTimer());

        m_difficultyText.text = GameManager.Instance.GetSettingDifficulty().ToString();
        m_quizNumberText.text = GameManager.Instance.GetSettingQuizNumber().ToString();
        m_lifeText.text = GameManager.Instance.GetSettingLife().ToString();
        m_timerText.text = GameManager.Instance.GetSettingTimer().ToString("0");

        m_isInitializing = false;
    }

    /// <summary>
    /// ゲーム内容設定を設定
    /// </summary>
    /// <param name="DifficultySetting"></param>
    /// <param name="QuizNumberSetting"></param>
    /// <param name="LifeSetting"></param>
    /// <param name="TimerSetting"></param>
    public void SetGameSetting(int DifficultySetting, int QuizNumberSetting, int LifeSetting, float TimerSetting)
    {
        m_settingDifficulty = DifficultySetting;
        m_settingQuizNumber = QuizNumberSetting;
        m_settingLife = LifeSetting;
        m_settingTimer = TimerSetting;
    }
}