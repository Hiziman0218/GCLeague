using UnityEngine;

public class QuizChecker : MonoBehaviour
{
    public QuizArea areaA;
    public QuizArea areaB;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // スペースキーで判定してみる
        {
            if (areaA.playerCount > areaB.playerCount)
            {
                //Aの処理
                Debug.Log("Aの方が多い！");
            }
            else if (areaB.playerCount > areaA.playerCount)
            {
                //Bの処理
                Debug.Log("Bの方が多い！");
            }
            else
            {
                //同じ人数だった時の処理
                Debug.Log("同じ人数！");
            }
        }
    }
}
