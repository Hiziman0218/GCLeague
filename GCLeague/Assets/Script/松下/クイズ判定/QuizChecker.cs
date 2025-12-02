using UnityEngine;

public class QuizChecker : MonoBehaviour
{
    [SerializeField]
    private QuizArea areaA;
    [SerializeField]
    private QuizArea areaB;

    void Update()
    {
        //とりあえずスペースキーで判定を開始
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (areaA.playerCount > areaB.playerCount)
            {
                //Aが多かった時の処理
                Debug.Log("Aの方が多い！");
            }
            else if (areaB.playerCount > areaA.playerCount)
            {
                //Bが多かった時の処理
                Debug.Log("Bの方が多い！");
            }
            else
            {
                //同じ人数だった時の処理
                // 同じ人数 → ホストがいるエリアを優先
                bool hostInA = areaA.players.Exists(p => p.isHostPlayer);
                bool hostInB = areaB.players.Exists(p => p.isHostPlayer);

                if (hostInA && !hostInB)
                {
                    Debug.Log("同じ人数！ホストがAにいるのでA優先！");
                }
                else if (hostInB && !hostInA)
                {
                    Debug.Log("同じ人数！ホストがBにいるのでB優先！");
                }
                else
                {
                    Debug.Log("同じ人数！ホストもいないか両方にいる");
                }
            }
        }
    }
}
