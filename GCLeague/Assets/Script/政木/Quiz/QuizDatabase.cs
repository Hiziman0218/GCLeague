using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuizDatabase", menuName = "Game/Quiz Database")]
public class QuizDatabase : ScriptableObject
{
    [Tooltip("全てのクイズの内容")]
    [SerializeField] private List<Quiz> m_quizzes = new List<Quiz>();

    public static QuizDatabase Instance { get; private set; }

    public List<Quiz> Quizzes => m_quizzes;

    private void OnEnable()
    {
        Instance = this;
    }
}
