using System.Collections.Generic;
using UnityEngine;

public class QuizArea : MonoBehaviour
{
    [Header("エリアに乗っているプレイヤー情報")]
    public List<MirrorChatSystems.PlayerNetWorkSystem> players = new List<MirrorChatSystems.PlayerNetWorkSystem>();
    public MirrorChatSystems.PlayerNetWorkSystem m_player;

    public int playerCount => players.Count;

    //エリア内のプレイヤーをリストに追加
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<MirrorChatSystems.PlayerNetWorkSystem>();
        if (player != null && !players.Contains(player))
        {
            players.Add(player);
        }
    }

    //エリア外にいったプレイヤーをリストから削除
    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<MirrorChatSystems.PlayerNetWorkSystem>();
        if (player != null && players.Contains(player))
        {
            players.Remove(player);
        }
    }
}
