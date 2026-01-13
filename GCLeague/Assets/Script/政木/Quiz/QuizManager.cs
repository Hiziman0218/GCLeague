using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    [Header("使用するクイズデータベース")]
    [SerializeField] private QuizDatabase m_quizDatabase; //通常クイズと穴吹クイズを切り替えるならここを変える

    //QuizDatabaseの保持するリストと同じ順序で保持されているQuizRuntimeのリスト
    private List<QuizRuntime> m_allQuizRuntimes = new();

    //難易度ごとのクイズリスト
    private Dictionary<int, List<QuizRuntime>> m_quizByDifficulty = new();

    private void Awake()
    {
        if (m_quizDatabase == null)
        {
            Debug.LogError("QuizDatabase が設定されていません。");
            return;
        }

        m_allQuizRuntimes.Clear();
        m_quizByDifficulty.Clear();

        //データベースからランタイム用リストを生成
        for (int i = 0; i < m_quizDatabase.Quizzes.Count; i++)
        {
            Quiz quiz = m_quizDatabase.Quizzes[i];

            if (!m_quizByDifficulty.ContainsKey(quiz.Difficulty))
            {
                m_quizByDifficulty[quiz.Difficulty] = new List<QuizRuntime>();
            }

            //動的管理クイズを生成、各リストに追加
            QuizRuntime quizRuntime = new QuizRuntime(quiz, i);
            m_allQuizRuntimes.Add(quizRuntime);
            m_quizByDifficulty[quiz.Difficulty].Add(quizRuntime);
        }
    }

    /// <summary>
    /// 指定したIDのQuizRuntimeを取得
    /// </summary>
    /// <param name="quizId"></param>
    /// <returns></returns>
    public QuizRuntime GetQuizRuntime(int quizId)
    {
        if (quizId < 0 || quizId >= m_allQuizRuntimes.Count)
            return null;

        return m_allQuizRuntimes[quizId];
    }

    /// <summary>
    /// 指定した難易度からランダムに未出題のクイズを取得
    /// </summary>
    /// <param name="difficulty">難易度</param>
    /// <returns>指定した難易度のクイズ</returns>
    public int GetRandomQuiz(int difficulty)
    {
        if (!m_quizByDifficulty.ContainsKey(difficulty))
        {
            Debug.LogWarning($"難易度 {difficulty} のクイズが見つかりません。");
            return -1;
        }

        var list = m_quizByDifficulty[difficulty];
        var unused = list.FindAll(q => !q.IsUsed);

        if (unused.Count == 0)
        {
            Debug.Log($"難易度 {difficulty} のクイズは全て出題済みです。");
            return -1;
        }

        var selected = unused[Random.Range(0, unused.Count)];
        selected.IsUsed = true;

        return selected.QuizID;
    }

    /// <summary>
    /// すべての出題済みフラグをリセット
    /// </summary>
    public void ResetUsedFlags()
    {
        foreach (var list in m_quizByDifficulty.Values)
        {
            foreach (var quiz in list)
            {
                quiz.IsUsed = false;
            }
        }
    }
}
