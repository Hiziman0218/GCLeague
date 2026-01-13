using UnityEngine;
using System.Collections.Generic;
using Game.Enum;

public class UIManager : MonoBehaviour
{
    //UIManagerのインスタンス(シングルトン)
    public static UIManager Instance { get; private set; }

    private GameSetting m_gameSetting; //ゲームの設定保持用

    private List<UIBase> m_UIList = new List<UIBase>();

    //表示/非表示が完了した時に呼ばれる通知用イベント
    public event System.Action<UIType> OnUIShowComplete;
    public event System.Action<UIType> OnUIHideComplete;

    [Header("UI")]
    [SerializeField] private HUD m_hud;     //画面上に常に表示するUI(ヘッドアップディスプレイ)
    [SerializeField] private Timer m_timer; //回答中に表示される残り時間
    [SerializeField] private StartUI m_startUI; //ゲーム開始時に表示されるゲームの設定
    [SerializeField] private QuizUI m_quizUI;   //クイズ内容を表示するUI
    [SerializeField] private CorrectUI m_correctUI;     //正解時に表示するUI
    [SerializeField] private IncorrectUI m_incorrectUI; //不正解時に表示するUI
    [SerializeField] private FadeUI m_fadeUI;           //フェードを管理するUI

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

        //イベントを初期化
        InitializeEvent();
    }

    /// <summary>
    /// 状態を監視し、それに合わせたUIを表示/非表示
    /// </summary>
    /// <param name="state"></param>
    public void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Lobby:
                break;
            case GameState.GameStart:
                GameManager gameManager = FindObjectOfType<GameManager>();
                m_gameSetting = gameManager.GetGameSetting();
                m_hud.SetGameSetting(m_gameSetting);
                m_startUI.SetGameSetting(m_gameSetting);
                break;
            case GameState.Question:
                break;
            case GameState.Thinking:
                break;
            case GameState.Standby:
                break;
            case GameState.CorrectAnswer:
                break;
            case GameState.IncorrectAnswer:
                break;
            case GameState.GameClear:
                ShowFade(); //ゲームクリア演出が完成したら、GameManagerのゲームクリア演出終了時に移行
                break;
            case GameState.GameOver:
                ShowFade(); //ゲームオーバー演出が完成したら、GameManagerのゲームオーバー演出終了時に移行
                break;
            case GameState.WaitFade:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 受け取った正誤によってUIを切り替えて表示
    /// </summary>
    /// <param name="isCorrect"></param>
    public void ShowResult(bool isCorrect)
    {
        if (isCorrect)
            m_correctUI.Show();
        else
            m_incorrectUI.Show();
    }

    /// <summary>
    /// 受け取った数値を元にフェード演出開始
    /// </summary>
    /// <param name="fadeTime"></param>
    /// <param name="waitBetweenFade"></param>
    public void ShowFade(float fadeTime = 1.0f, float waitBetweenFade = 1.0f)
    {
        m_fadeUI.StartFade(fadeTime, waitBetweenFade);
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
    /// Typeに応じたUIを表示
    /// </summary>
    /// <param name="Type">表示したいUI</param>
    public void ShowUI(UIType Type)
    {
        switch (Type)
        {
            case UIType.HUD:
                m_hud.Show();
                break;
            case UIType.Timer:
                m_timer.Show();
                break;
            case UIType.StartUI:
                m_startUI.Show();
                break;
            case UIType.QuizUI:
                m_quizUI.Show();
                break;
            case UIType.CorrectUI:
                m_correctUI.Show();
                break;
            case UIType.IncorrectUI:
                m_incorrectUI.Show();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Typeに応じたUIを非表示
    /// </summary>
    /// <param name="Type">非表示にしたいUI</param>
    public void HideUI(UIType Type)
    {
        switch (Type)
        {
            case UIType.HUD:
                m_hud.Hide();
                break;
            case UIType.Timer:
                m_timer.Hide();
                break;
            case UIType.StartUI:
                m_startUI.Hide();
                break;
            case UIType.QuizUI:
                m_quizUI.Hide();
                break;
            case UIType.CorrectUI:
                m_correctUI.Hide();
                break;
            case UIType.IncorrectUI:
                m_incorrectUI.Hide();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// UIのリストに追加
    /// </summary>
    /// <param name="UI"></param>
    public void AddList(UIBase UI)
    {
        m_UIList.Add(UI);
    }

    /// <summary>
    /// UIの一括初期化
    /// </summary>
    public void Initialize()
    {
        foreach(UIBase UI in m_UIList)
        {
            UI.Initialize();
        }
    }

    /// <summary>
    /// 全てのUIのイベントを初期化
    /// </summary>
    public void InitializeEvent()
    {
        m_hud.RegistrationEvent();
        m_timer.RegistrationEvent();
        m_startUI.RegistrationEvent();
        m_quizUI.RegistrationEvent();
        m_correctUI.RegistrationEvent();
        m_incorrectUI.RegistrationEvent();
    }

    /// <summary>
    /// 表示が完了したUIの表示完了通知
    /// </summary>
    /// <param name="UI"></param>
    public void NotifyShowComplete(UIBase UI)
    {
        OnUIShowComplete?.Invoke(UI.GetUIType());
    }

    /// <summary>
    /// 非表示が完了したUIの非表示完了通知
    /// </summary>
    /// <param name="UI"></param>
    public void NotifyHideComplete(UIBase UI)
    {
        OnUIHideComplete?.Invoke(UI.GetUIType());
    }

    /// <summary>
    /// HUDの現在の難易度を設定
    /// </summary>
    /// <param name="difficulty"></param>
    public void UpdateDifficulty(float difficulty)
    {
        m_hud.SetCurrentDifficulty(difficulty);
    }

    /// <summary>
    /// HUDの現在の問題数を設定
    /// </summary>
    /// <param name="QuizNumber"></param>
    public void UpdateQuizNumber(int QuizNumber)
    {
        m_hud.SetCurrentQuizNumber(QuizNumber);
    }

    /// <summary>
    /// HUDの現在の残機を設定
    /// </summary>
    /// <param name="life"></param>
    public void UpdateLife(int life)
    {
        m_hud.SetCurrentLife(life);
    }

    /// <summary>
    /// Timerの現在の残り時間を設定
    /// </summary>
    /// <param name="Time"></param>
    public void UpdateTime(float Time)
    {
        m_timer.SetTime(Time);
    }

    /// <summary>
    /// QuizUIの内容を設定
    /// </summary>
    /// <param name="quiz"></param>
    public void OnQuizChanged(Quiz quiz)
    {
        m_quizUI.SetQuiz(quiz);
    }
}
