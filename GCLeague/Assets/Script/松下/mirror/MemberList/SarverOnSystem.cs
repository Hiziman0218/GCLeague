using Mirror;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[AddComponentMenu("")]
public class SarverOnSystem : NetworkManager
{
    // クライアントが接続した際に呼ばれる
    public override void OnStartServer()
    {
    }
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn); // 親クラスの処理を呼び出す（オプション）

        // クライアントが接続した際のカスタム処理
        Debug.Log($"クライアントが接続しました: {conn.connectionId}");

        // 例えば、特定のデータを送る、ログを記録するなど
        // conn.Send(new CustomMessage { data = "Welcome!" });
    }

    // クライアントにプレイヤーリストを送信
    [Server]
    private void UpdateClientPlayerList()
    {
    }
}
