using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    [Header("使用するクイズデータベース")]
    [SerializeField] private QuizDatabase m_quizDatabase; //通常クイズと穴吹クイズを切り替えるならここを変える

    //難易度ごとのクイズリスト
    private Dictionary<int, List<QuizRuntime>> m_quizByDifficulty = new();

    private void Awake()
    {
        if (m_quizDatabase == null)
        {
            Debug.LogError("QuizDatabase が設定されていません。");
            return;
        }

        //データベースからランタイム用リストを生成
        foreach (var quiz in m_quizDatabase.Quizzes)
        {
            //クイズの難易度に合ったリストが無ければ、リストを作成
            if (!m_quizByDifficulty.ContainsKey(quiz.Difficulty))
                m_quizByDifficulty[quiz.Difficulty] = new List<QuizRuntime>();

            //難易度ごとに分けられたリストに、ランタイム用クイズを追加
            m_quizByDifficulty[quiz.Difficulty].Add(new QuizRuntime(quiz));
        }
    }

    /// <summary>
    /// 指定した難易度からランダムに未出題のクイズを取得
    /// </summary>
    /// <param name="difficulty">難易度</param>
    /// <returns>指定した難易度のクイズ</returns>
    public QuizRuntime GetRandomQuiz(int difficulty)
    {
        //指定の難易度の問題が無かった場合、nullを返却
        if (!m_quizByDifficulty.ContainsKey(difficulty))
        {
            Debug.LogWarning($"難易度 {difficulty} のクイズが見つかりません。");
            return null;
        }

        //指定の難易度のリストから、未出題のクイズを検索
        var list = m_quizByDifficulty[difficulty];
        var unused = list.FindAll(q => !q.IsUsed);

        //出題できるクイズが無かった場合、nullを返却
        if (unused.Count == 0)
        {
            Debug.Log($"難易度 {difficulty} のクイズは全て出題済みです。");
            return null;
        }

        //検索したクイズの中からランダムに問題を抽出し、出題済みに設定して返却
        var selected = unused[Random.Range(0, unused.Count)];
        selected.IsUsed = true;
        return selected;
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
