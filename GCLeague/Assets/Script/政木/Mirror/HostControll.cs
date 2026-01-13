using Mirror;
using UnityEngine;
using System.Collections;

public class HostControll : NetworkBehaviour
{
    [SerializeField] private GameObject m_lobbyUI;
    [SerializeField] private GameSettingUI m_gameSettingPannel;
    [SerializeField] private GameObject m_FinalAnswerButton;

    [SyncVar]
    public bool isHostPlayer;

    public override void OnStartLocalPlayer()
    {
        /*
        //自身がホストなら、ホスト専用UIを表示し、ゲーム設定用UIを連携
        if (isServer)
        {
            m_lobbyUI.SetActive(true);
            StartCoroutine(BindGameSystemManager());
        }*/

        if (isHostPlayer)
        {
            // 最初に入ったプレイヤーだけ
            StartCoroutine(BindGameSystemManager());
        }
    }

    public override void OnStartServer()
    {
        // 接続順で一番最初ならホスト
        if (NetworkServer.connections.Count == 1)
        {
            isHostPlayer = true;
        }
    }

    /// <summary>
    /// スタートボタンを押したときの処理
    /// </summary>
    public void OnClickStartButton()
    {
        if (!isLocalPlayer) return;
        CmdRequestStartGame();
        m_lobbyUI.SetActive(false);
    }

    /// <summary>
    /// 回答確定ボタンを押したときの処理
    /// </summary>
    public void OnClickFinalAnswerButton()
    {
        if (!isLocalPlayer) return;
        CmdRequestChangeStateJudging();
    }

    /// <summary>
    /// 回答中の回答確定UI表示
    /// </summary>
    public void OnThinkingStart()
    {
        m_FinalAnswerButton.SetActive(true);
    }

    /// <summary>
    /// 回答終了時の回答確定UI表示
    /// </summary>
    public void OnThinkingEnd()
    {
        m_FinalAnswerButton.SetActive(false);
    }

    /// <summary>
    /// ゲーム終了時のロビーUI表示
    /// </summary>
    public void OnGameEnd()
    {
        m_lobbyUI.SetActive(true);
        m_gameSettingPannel.RefreshUI();
    }

    /// <summary>
    /// コマンドでのゲームスタート呼び出し
    /// </summary>
    [Command]
    private void CmdRequestStartGame()
    {
        //サーバー側で実行される
        GameManager.Instance.PushStartButton();
    }

    /// <summary>
    /// コマンドでのプレイヤー回答終了
    /// </summary>
    [Command]
    private void CmdRequestChangeStateJudging()
    {
        GameManager.Instance.AnswerCompleted();
    }

    /// <summary>
    /// GameSystemManagerに必ずUIを登録するためのコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator BindGameSystemManager()
    {
        while (GameSystemManager.Instance == null)
        {
            yield return null;
        }

        GameSystemManager.Instance.SetGameSetting(m_gameSettingPannel);
        OnGameEnd();
    }
}
