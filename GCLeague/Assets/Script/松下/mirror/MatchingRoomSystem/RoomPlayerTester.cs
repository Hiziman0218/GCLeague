using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーサイドの情報
/// </summary>
public class RoomPlayerTester : NetworkRoomPlayer
{
    [SyncVar,Header("ルーム待機完了状態")]
    public bool isReady = false;
    [SyncVar,Header("ルームマスターであるか?")]
    public bool isRoomLeader = false;

    public override void OnStartClient()
    {
        base.OnStartClient();
        // ルーム主の設定（最初にルームを作った人）
        if (isRoomLeader)
        {
            Debug.Log("貴方がルームマスターです。");
        }
    }

    /// <summary>
    /// サーバー側へのアクセス
    /// ルーム全てのプレイヤーが、準備完了
    /// </summary>
    /// <param name="ready">準備完了フラグ</param>
    [Command]
    public void CmdSetReadyState(bool ready)
    {
        // プレイヤーが準備完了ボタンを押した
        isReady = ready;
        // サーバーに通知
        Debug.Log($"プレイヤー: {netId} の準備状態は: 【{ready}】です。");
    }

    /// <summary>
    /// ルームマスター側にゲーム開始ボタンを表示させる
    /// </summary>
    [TargetRpc]
    public void TargetShowStartButton()
    {
        // ルーム主にゲーム開始ボタンを表示
        Debug.Log("とりあえず、ルームマスターにスタートボタンを表示させる");
        // ボタンの表示ロジックをUIで実装する
    }
}
