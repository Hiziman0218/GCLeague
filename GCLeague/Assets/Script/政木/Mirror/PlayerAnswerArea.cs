using Mirror;
using Game.Enum;

public class PlayerAnswerArea : NetworkBehaviour
{
    [SyncVar]
    public AnswerArea currentArea = AnswerArea.None;

    [Command]
    public void CmdSetArea(AnswerArea area)
    {
        currentArea = area;
    }
}
