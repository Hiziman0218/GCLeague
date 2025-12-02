using UnityEngine;
using Game.Enum;

public class UIManager : MonoBehaviour
{
    //UIManagerのインスタンス(シングルトン)
    public static UIManager Instance { get; private set; }

    private GameManager m_gameManager; //ゲームマネージャー保持用
    private GameSetting m_gameSetting; //ゲームの設定保持用

    [Header("UI")]
    [SerializeField] private HUD m_hud;     //画面上に常に表示するUI(ヘッドアップディスプレイ)
    [SerializeField] private Timer m_timer; //回答中に表示される残り時間
    [SerializeField] private StartUI m_startUI; //ゲーム開始時に表示されるゲームの設定
    [SerializeField] private QuizUI m_quizUI;   //クイズ内容を表示するUI
    [SerializeField] private CorrectUI m_correctUI;     //正解時に表示するUI
    [SerializeField] private IncorrectUI m_incorrectUI; //不正解時に表示するUI

    private void Awake()
    {
        //シングルトン
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        //一度全てのUIを非表示に設定
        HideAll();
    }

    private void Update()
    {
        //回答中状態なら、残りの回答時間をタイマーに設定
        if (m_gameManager.GetState() == GameState.Thinking) m_timer.SetTime(m_gameManager.GetLimit());
    }

    /// <summary>
    /// HUDを表示
    /// </summary>
    public void ShowHUD()
    {
        m_hud.Show();
    }

    /// <summary>
    /// HUDを非表示
    /// </summary>
    public void HideHUD()
    {
        m_hud.Hide();
    }

    /// <summary>
    /// タイマーを表示
    /// </summary>
    public void ShowTimer()
    {
        m_timer.Show();
    }

    /// <summary>
    /// タイマーを非表示
    /// </summary>
    public void HideTimer()
    {
        m_timer.Hide();
    }

    /// <summary>
    /// スタートUIを表示
    /// </summary>
    public void ShowStartUI()
    {
        m_startUI.Show();
    }

    /// <summary>
    /// スタートUIを非表示
    /// </summary>
    public void HideStartUI()
    {
        m_startUI.Hide();
    }

    /// <summary>
    /// クイズUIを表示
    /// </summary>
    /// <param name="question">問題文</param>
    /// <param name="answer1">回答文1</param>
    /// <param name="answer2">回答文2</param>
    /// <param name="choise1">回答画像1</param>
    /// <param name="choise2">回答画像2</param>
    public void ShowQuizUI(string question, string answer1, string answer2, Sprite choise1, Sprite choise2)
    {
        m_quizUI.SetQuiz(question, answer1, answer2, choise1, choise2);
        m_quizUI.Show();
    }

    /// <summary>
    /// クイズUIを非表示
    /// </summary>
    public void HideQuizUI()
    {
        m_quizUI.Hide();
    }

    /// <summary>
    /// 正解UIを表示
    /// </summary>
    public void ShowCorrectUI()
    {
        m_correctUI.Show();
    }

    /// <summary>
    /// 正解UIを非表示
    /// </summary>
    public void HideCorrectUI()
    {
        m_correctUI.Hide();
    }

    /// <summary>
    /// 不正解UIを表示
    /// </summary>
    public void ShowIncorrectUI()
    {
        m_incorrectUI.Show();
    }

    /// <summary>
    /// 不正解UIを非表示
    /// </summary>
    public void HideIncorrectUI()
    {
        m_incorrectUI.Hide();
    }

    /// <summary>
    /// 全てのUIを非表示
    /// </summary>
    public void HideAll()
    {
        m_hud.gameObject.SetActive(false);
        m_timer.gameObject.SetActive(false);
        m_startUI.gameObject.SetActive(false);
        m_quizUI.gameObject.SetActive(false);
        m_correctUI.gameObject.SetActive(false);
        m_incorrectUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// 引数で渡されたUIの表示が終わったかを取得
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool GetIsShowClear(UIType type)
    {
        switch (type)
        {
            case UIType.HUD:
                return m_hud.IsShowClear();
            case UIType.Timer:
                return m_timer.IsShowClear();
            case UIType.StartUI:
                return m_startUI.IsShowClear();
            case UIType.QuizUI:
                return m_quizUI.IsShowClear();
            case UIType.CorrectUI:
                return m_correctUI.IsShowClear();
            case UIType.IncorrectUI:
                return m_incorrectUI.IsShowClear();
            default:
                return false;
        }
    }

    /// <summary>
    /// 引数で渡されたUIの非表示が終わったかを取得
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool GetIsHideClear(UIType type)
    {
        switch (type)
        {
            case UIType.HUD:
                return m_hud.IsHideClear();
            case UIType.Timer:
                return m_timer.IsHideClear();
            case UIType.StartUI:
                return m_startUI.IsHideClear();
            case UIType.QuizUI:
                return m_quizUI.IsHideClear();
            case UIType.CorrectUI:
                return m_correctUI.IsHideClear();
            case UIType.IncorrectUI:
                return m_incorrectUI.IsHideClear();
            default:
                return false;
        }
    }

    /// <summary>
    /// マネージャーとゲームの設定を設定
    /// </summary>
    /// <param name="Manager"></param>
    /// <param name="Setting"></param>
    public void SetManagers(GameManager Manager, GameSetting Setting)
    {
        m_gameManager = Manager;
        m_gameSetting = Setting;

        //設定が必要なUIにも設定
        m_hud.SetGameSetting(m_gameManager, m_gameSetting);
        m_startUI.SetGameSetting(m_gameSetting);
    }
}
