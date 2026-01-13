using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Game.Enum;

public class GameSystemManager : MonoBehaviour
{
    public static GameSystemManager Instance { get; private set; }

    [Header("詳細設定")]
    [SerializeField] private GameSettingUI m_settingUI; //設定UI

    private GameSetting m_setting;

    private void Awake()
    {
        //シングルトン
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //GameSetting を生成
        m_setting = new GameSetting(QuizType.Normal, 1, 10, 1, 3, 60f);
    }

    /// <summary>
    /// 現在の設定を GameManager などが取得するための関数
    /// </summary>
    public GameSetting GetGameSetting()
    {
        return m_setting;
    }

    /// <summary>
    /// 受け取ったゲーム設定パネルにゲーム設定を設定
    /// </summary>
    /// <param name="gameSettingUI"></param>
    public void SetGameSetting(GameSettingUI gameSettingUI)
    {
        m_settingUI = gameSettingUI;
        if (m_settingUI != null) m_settingUI.SetGameSetting(m_setting);
    }
}
