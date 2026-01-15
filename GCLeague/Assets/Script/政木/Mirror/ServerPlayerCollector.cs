using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// サーバー上に参加している全プレイヤーを取得する専用クラス
/// </summary>
public static class ServerPlayerCollector
{
    /// <summary>
    /// 現在サーバーに参加している全プレイヤーのGameObjectを取得する
    /// </summary>
    public static List<GameObject> GetAllPlayers()
    {
        List<GameObject> players = new List<GameObject>();

        // サーバーでなければ空リストを返す
        if (!NetworkServer.active)
        {
            Debug.LogWarning("GetAllPlayers was called, but server is not active.");
            return players;
        }

        foreach (var conn in NetworkServer.connections.Values)
        {
            // 接続はあるが、まだプレイヤーがSpawnされていない場合がある
            if (conn == null || conn.identity == null)
                continue;

            players.Add(conn.identity.gameObject);
        }

        return players;
    }

    /// <summary>
    /// 現在のホストプレイヤーの HostControll を取得する
    /// </summary>
    public static HostControll GetHostController()
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("GetHostController was called, but server is not active.");
            return null;
        }

        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn == null || conn.identity == null)
                continue;

            HostControll hc = conn.identity.GetComponent<HostControll>();
            if (hc != null && hc.isHostPlayer)
            {
                return hc;
            }
        }

        return null;
    }
}