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
    /// 全てのUIを非表示
    /// </summary>
    public void HideAll()
    {
        m_hud.Hide();
        m_timer.Hide();
        m_startUI.Hide();
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
