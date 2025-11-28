using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarterTester : NetworkBehaviour
{
    /// <summary>
    /// ゲームスタートボタンが押された場合に起動
    /// </summary>
    public void StartGame()
    {
        //自身はサーバーではない場合は無効
        if (!isServer) return;

        // シーンを切り替えてゲームを開始
        NetworkManager.singleton.ServerChangeScene("GameScene");
    }

    /// <summary>
    /// プレイヤーが接続を断った場合
    /// </summary>
    /// <param name="NC">ルームのコネクション情報</param>
    public void OnPlayerDisconnect(NetworkConnection NC)
    {
        // .ルームマスターがルームから離脱
        if (NC.identity.GetComponent<RoomPlayerTester>().isRoomLeader)
        {
            Debug.Log("ルームマスターが退室しました。\n部屋を解散します。");
            //全てのプレイヤーをルームから削除
            NetworkServer.DisconnectAll();
        }
        else
        {
            //ルーム主以外がルームから離脱
            //Debug.Log($"プレイヤー: {NC.connectionId} が、ルームから退室しました");
            //対象をルームから削除
            NC.Disconnect();
        }
    }
}
