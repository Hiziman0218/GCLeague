using UnityEngine;

public class QuizChecker : MonoBehaviour
{
    [SerializeField]
    private QuizArea m_AreaLeft;
    [SerializeField]
    private QuizArea m_AreaRight;

    public enum AnswerSide { Left = 0, Right = 1 }

    AnswerSide PlayerCheck()
    {
        if (m_AreaLeft.playerCount > m_AreaRight.playerCount)
        {
            //Debug.Log("Aの方が多い！");
            return AnswerSide.Left;
        }
        else if (m_AreaRight.playerCount > m_AreaLeft.playerCount)
        {
            //Debug.Log("Bの方が多い！");
            return AnswerSide.Right;
        }
        else
        {
            //両方同じ人数ならホストの選択優先
            bool hostInA = m_AreaLeft.players.Exists(p => p.isHostPlayer);
            bool hostInB = m_AreaRight.players.Exists(p => p.isHostPlayer);

            if (hostInA && !hostInB)
            {
                //Debug.Log("同じ人数！ホストがAにいるのでA優先！");
                return AnswerSide.Left;
            }
            else if (hostInB && !hostInA)
            {
                //Debug.Log("同じ人数！ホストがBにいるのでB優先！");
                return AnswerSide.Right;
            }
            else
            {
                //Debug.Log("同じ人数！ホストもいないか両方にいる");
                //念のためのセーフティでホストを確認できなかった場合左の選択肢を選んだことにする
                return AnswerSide.Left;
            }
        }
    }
}
