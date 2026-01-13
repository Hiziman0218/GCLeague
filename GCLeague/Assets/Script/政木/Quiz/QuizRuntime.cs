public class QuizRuntime
{
    //クイズのデータ
    public Quiz Data { get; private set; }
    //このクイズのID
    public int QuizID { get; private set; }
    //既に出題されたか
    public bool IsUsed { get; set; }

    /// <summary>
    /// コンストラクタ クイズのデータを登録
    /// </summary>
    /// <param name="data">クイズのデータ</param>
    public QuizRuntime(Quiz data, int quizID)
    {
        Data = data;
        QuizID = quizID;
        IsUsed = false;
    }

    /// <summary>
    /// 回答を判定
    /// </summary>
    /// <param name="answerIndex">選択された回答</param>
    /// <returns>正解か</returns>
    public bool IsCorrect(int answerIndex)
    {
        // Quizの正解インデックスを参照し、プレイヤーの回答を判定
        return answerIndex == Data.CorrectIndex;
    }
}
