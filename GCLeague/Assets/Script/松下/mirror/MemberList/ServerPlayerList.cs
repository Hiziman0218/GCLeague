using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class ServerPlayerList : MonoBehaviour
{
    public string m_PlayerListMessage;
    void Update()
    {
        PlayerListData();
    }
    public void PlayerListData()
    {
        // サーバーがアクティブな場合のみ接続数を確認
        if (NetworkServer.active)
        {
            //プレイヤーリスト受け皿を初期化
            m_PlayerListMessage = "";
            //現在サーバーに接続しているプレイヤー数を抽出
            int playerCount = NetworkServer.connections.Count;
            //受け皿に現在のプレイヤー数を代入
            m_PlayerListMessage = "現在のプレイヤー数: " + playerCount.ToString() + "\n";
            //現在の番号を初期化
            int No = 0;
            //Serverのスポーン状況を全て走査
            foreach (var kvp in NetworkServer.spawned)
            {
                //データが存在する場合
                if (kvp.Value)
                {
                    //対象のゲームオブジェクトを抽出(プレイヤー)
                    GameObject playerObject = kvp.Value.gameObject;
                    //対象のゲームオブジェクトの名前を割り出して登録する
                    m_PlayerListMessage += No.ToString() + ": " + playerObject.name + "\n";
                    //現在の番号を加算
                    No++;
                }
            }
            //デバック
            //Debug.Log(m_PlayerListMessage);
        }
    }
}
