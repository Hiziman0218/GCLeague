using Game.Enum;
using Mirror;
using UnityEngine;

public class AnswerAreaTrigger : MonoBehaviour
{
    public AnswerArea areaType;

    private void OnTriggerEnter(Collider other)
    {
        var answer = other.GetComponent<PlayerAnswerArea>();
        if (answer == null) return;

        // ローカルプレイヤーのみが Cmd を送る
        if (!answer.isLocalPlayer) return;

        answer.CmdSetArea(areaType);
    }
}
