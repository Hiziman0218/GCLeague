using Mirror;
using UnityEngine;
using System.Collections;

public class HostControll : NetworkBehaviour
{
    [SerializeField] private GameObject m_lobbyUI;
    [SerializeField] private GameSettingUI m_gameSettingPannel;
    [SerializeField] private GameObject m_FinalAnswerButton;

    [SyncVar(hook = nameof(OnHostChanged))]
    public bool isHostPlayer;

    public override void OnStartServer()
    {
        //接続順で一番最初ならホスト
        if (NetworkServer.connections.Count == 1)
        {
            isHostPlayer = true;
        }
    }

    public override void OnStartLocalPlayer()
    {
        if (isHostPlayer)
        {
            //最初に入ったプレイヤーだけ
            StartCoroutine(BindGameManager());
        }
    }

    public override void OnStopServer()
    {
        //自分がホストじゃなければ何もしない
        if (!isHostPlayer) return;

        AssignNextHost();
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
        // ここはサーバー上で実行されるので、サーバー側の GameManager のサーバーメソッドを直接呼ぶ
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null on server in CmdRequestStartGame");
            return;
        }
        GameManager.Instance.ServerStartFromHost();
    }

    /// <summary>
    /// コマンドでのプレイヤー回答終了
    /// </summary>
    [Command]
    private void CmdRequestChangeStateJudging()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null on server in CmdRequestChangeStateJudging");
            return;
        }
        GameManager.Instance.ServerRequestJudge();
    }

    /// <summary>
    /// GameSystemManagerに必ずUIを登録するためのコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator BindGameManager()
    {
        while (GameManager.Instance == null)
        {
            yield return null;
        }

        GameManager.Instance.SetGameSettingUI(m_gameSettingPannel);
        OnGameEnd();
    }

    /// <summary>
    /// ホストが切り替わった時の処理
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    void OnHostChanged(bool oldValue, bool newValue)
    {
        // ローカルプレイヤーかつホストになった瞬間
        if (!isLocalPlayer) return;

        if (newValue)
        {
            Debug.Log("I am new host");
            StartCoroutine(BindGameManager());
        }
    }

    [Server]
    void AssignNextHost()
    {
        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn.identity == null) continue;

            HostControll nextHost = conn.identity.GetComponent<HostControll>();
            if (nextHost != null)
            {
                nextHost.isHostPlayer = true;
                Debug.Log($"New host assigned: netId={nextHost.netId}");
                break;
            }
        }
    }
}
