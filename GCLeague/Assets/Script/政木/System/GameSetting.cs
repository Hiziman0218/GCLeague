using Game.Enum;

public class GameSetting
{
    private QuizType m_quizType; //選択されているクイズタイプ
    private int m_difficulty;    //問題の難易度
    private int m_quizNumber;    //総問題数
    private int m_playerNumber;  //プレイヤーの人数
    private int m_life;          //残機
    private float m_timer;       //一回の回答における制限時間

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="QuizType">クイズタイプ</param>
    /// <param name="Difficulty">問題の難易度</param>
    /// <param name="QuizNumber">総問題数</param>
    /// <param name="PlayerNumber">プレイヤーの人数</param>
    /// <param name="Life">残機</param>
    /// <param name="Timer">回答の制限時間</param>
    public GameSetting(QuizType QuizType, int Difficulty, int QuizNumber, int PlayerNumber, int Life, float Timer)
    {
        m_quizType = QuizType;
        m_difficulty = Difficulty;
        m_quizNumber = QuizNumber;
        m_playerNumber = PlayerNumber;
        m_life = Life;
        m_timer = Timer;
    }

    //各ゲッター

    public QuizType GetQuizType()
    { 
        return m_quizType; 
    }

    public int GetDifficulty()
    {
        return m_difficulty;
    }

    public int GetQuizNumber()
    {
        return m_quizNumber;
    }

    public int GetPlayerNumber()
    {
        return m_playerNumber;
    }

    public int GetLife()
    {
        return m_life;
    }

    public float GetTimer()
    {
        return m_timer;
    }

    //各セッター

    public void SetQuizType(QuizType QuizType)
    {
        m_quizType = QuizType;
    }

    public void SetDifficulty(int Difficulty)
    {
        m_difficulty = Difficulty;
    }

    public void SetQuizNumber(int QuizNumber)
    {
        m_quizNumber = QuizNumber;
    }

    public void SetPlayerNumber(int PlayerNumber)
    {
        m_playerNumber = PlayerNumber;
    }

    public void SetLife(int Life)
    {
        m_life = Life;
    }

    public void SetTimer(float Timer)
    {
        m_timer = Timer;
    }
}
