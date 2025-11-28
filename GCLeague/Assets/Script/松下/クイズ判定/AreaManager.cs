using UnityEngine;

public class QuizManager : MonoBehaviour
{
    public QuizArea areaA;
    public QuizArea areaB;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // スペースキーで判定してみる
        {
            if (areaA.playerCount > areaB.playerCount)
            {
                Debug.Log("Aの方が多い！");
            }
            else if (areaB.playerCount > areaA.playerCount)
            {
                Debug.Log("Bの方が多い！");
            }
            else
            {
                Debug.Log("同じ人数！");
            }
        }
    }
}
