using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizArea : MonoBehaviour
{
    [Header("ƒGƒŠƒA‚É‚¢‚él”")]
    public int playerCount = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount--;
        }
    }
}
