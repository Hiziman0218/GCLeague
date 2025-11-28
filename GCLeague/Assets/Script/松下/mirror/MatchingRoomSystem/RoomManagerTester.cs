using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NetworkRoomManagerを継承してルームシステムを拡張する
/// </summary>
public class RoomManagerTester : NetworkRoomManager
{
    protected List<RoomPlayerTester> roomSlots = new List<RoomPlayerTester>();
    public override void OnRoomServerPlayersReady()
    {
        // 全員準備完了のとき、ルーム主にスタートボタンを表示
        if (IsRoomLeader(out var leader))
        {
            leader.TargetShowStartButton();
        }
    }

    public override void OnRoomClientExit()
    {
        base.OnRoomClientExit();

        // roomSlotsリストを利用してプレイヤーの状態を確認
        foreach (RoomPlayerTester player in roomSlots)
        {
            if (player != null && player.isRoomLeader)
            {
                // ルーム主が退出した場合、全員をサーバーに戻す
                Debug.Log("ルームリーダーが退出しました。ルームを解散します。");
                DisbandRoom();
                return;
            }
        }

        Debug.Log("プレイヤーがルームを退出しました。");
    }

    private void DisbandRoom()
    {
        // ルームの全プレイヤーを切断
        foreach (var player in roomSlots)
        {
            if (player != null && player.connectionToClient != null)
            {
                player.connectionToClient.Disconnect();
            }
        }
    }

    // ルーム主かどうかをチェックするヘルパーメソッド
    private bool IsRoomLeader(out RoomPlayerTester leader)
    {
        foreach (var player in roomSlots)
        {
            if (player.GetComponent<RoomPlayerTester>().isRoomLeader)
            {
                leader = player.GetComponent<RoomPlayerTester>();
                return true;
            }
        }
        leader = null;
        return false;
    }
}
