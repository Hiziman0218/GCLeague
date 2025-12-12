using Mirror;
using UnityEngine;

public class GameNetworkController : NetworkBehaviour
{
    public static GameNetworkController Instance;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// クライアント→サーバー：ゲーム開始リクエスト
    /// </summary>
    [Command]
    public void CmdRequestGameStart(string sceneName)
    {
        // サーバーだけがシーンを変更できる
        NetworkManager.singleton.ServerChangeScene(sceneName);
    }

    public void RequestGameStart(string sceneName)
    {
        if (isServer)
        {
            // サーバーなら直接実行
            NetworkManager.singleton.ServerChangeScene(sceneName);
        }
        else
        {
            // クライアントならサーバーにリクエストを送る
            CmdRequestGameStart(sceneName);
        }
    }
}