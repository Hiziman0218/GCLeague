using UnityEngine;
using Game.Enum;

public class GameManager : MonoBehaviour
{
    private GameState m_state = GameState.GameStart; //ゲームの状態
    private GameSetting m_gameSetting = null; //ゲーム開始前に設定される内容
    private QuizRuntime m_currentQuiz = null; //出題されているクイズ格納用
    private int m_currentDifficulty = 1; //現在出題されるクイズの難易度
    private int m_difficultyChangeCount = 0; //難易度を上昇させるためのカウント
    private int m_quizNumber = 10;       //今回のゲームにおける問題数
    private int m_clearCount = 0;        //クリアした問題数
    private int m_life = 3;              //残り残機
    private float m_elapsedTime = 0f;    //経過時間計測用
    private float m_startTime = 5f;      //GameStart時に待機する時間
    private float m_thinkingTime = 60f;  //一回の回答にかけられる時間
    private bool m_isClearQuiz = false;  //問題を正解したか

    [SerializeField] private QuizManager m_quizManager; //クイズマネージャー(シーン上から設定)
    [SerializeField] private UIManager m_UIManager;     //UIマネージャー(シーン上から設定)

    private void Awake()
    {
        //ゲームの状態を開始時に初期化
        m_state = GameState.GameStart;

        //デバッグ用にマジックナンバーを使用、最終的に削除
        m_gameSetting = new GameSetting(QuizType.Normal, 1, 10, 5, 3, 60f);
    }

    private void Start()
    {
        //出題するクイズの難易度を設定
        m_currentDifficulty = m_gameSetting.GetDifficulty();
        //今回のゲームにおける問題数を設定
        m_quizNumber = m_gameSetting.GetQuizNumber();
        //残機を設定
        m_life = m_gameSetting.GetLife();
        //一回の回答にかけられる時間を設定
        m_thinkingTime = m_gameSetting.GetTimer();
    }

    private void Update()
    {
        switch (m_state)
        {
            case GameState.GameStart:
                UpdateGameStart();
                break;
            case GameState.Question:
                UpdateQuestion();
                break;
            case GameState.Thinking:
                UpdateThinking();
                break;
            case GameState.Judging:
                UpdateJudging();
                break;
            case GameState.Standby:
                UpdateStandby();
                break;
            case GameState.CorrectAnswer:
                UpdateCorrectAnswer();
                break;
            case GameState.IncorrectAnswer:
                UpdateIncorrectAnswer();
                break;
            case GameState.GameClear:
                UpdateGameClear();
                break;
            case GameState.GameOver:
                UpdateGameOver();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ゲームの状態を変更
    /// </summary>
    /// <param name="State">変更したい状態</param>
    public void ChangeState(GameState State)
    {
        //経過時間計測用変数を初期化
        m_elapsedTime = 0f;
        m_state = State;
    }

    /// <summary>
    /// 現在のゲームの状態を取得
    /// </summary>
    /// <returns></returns>
    public GameState GetState()
    {
        return m_state;
    }

    //各状態の更新処理
    private void UpdateGameStart()
    {
        //UIクラスにGameSettingを渡し、開始前の内容を表示してもらう
        m_UIManager.SetManagers(this, m_gameSetting);
        m_UIManager.ShowStartUI();

        //経過時間を加算
        m_elapsedTime += Time.deltaTime;

        //経過時間が、GameStart時に待機する時間を超えていたら、クイズの出題へ移行
        if (m_elapsedTime >= m_startTime)
        {
            ChangeState(GameState.Question);
            m_UIManager.HideStartUI();
        }
    }

    private void UpdateQuestion()
    {
        //クイズを取得していなければ、クイズマネージャーから指定された難易度のランダムなクイズを取得
        if (m_currentQuiz == null)
        {
            m_currentQuiz = m_quizManager.GetRandomQuiz(m_currentDifficulty);
        }

        //クイズを取得できていたら、UIに取得できたクイズを設定し、表示完了を待つ
        if (m_currentQuiz != null)
        {
            //UIクラスに取得したクイズを設定
            //ここに処理を追加
            //クイズの表示が終わったら、回答中へ移行
            if (true /*UIクラスから「表示が終わったか」を取得する処理を追加*/)
            {
                ChangeState(GameState.Thinking);
            }
        }
    }

    private void UpdateThinking()
    {
        //UIに残り時間を表示させる
        m_UIManager.ShowTimer();

        //経過時間を加算
        m_elapsedTime += Time.deltaTime;

        //経過時間が、一回の回答にかけられる時間を超えていたら、回答判定へ移行
        if (m_elapsedTime > m_thinkingTime)
        {
            ChangeState(GameState.Judging);
        }
    }

    private void UpdateJudging()
    {
        //タイマーを非表示に設定
        m_UIManager.HideTimer();

        //プレイヤーの回答を取得し、正誤によって分岐
        //IsCorrectの引数はプレイヤーまたはトロッコから取得(現在はデバッグ用に1)
        if (m_currentQuiz.IsCorrect(1/*ここに処理を追加*/))
        {
            m_isClearQuiz = true;
            ChangeState(GameState.Standby);
        }
        else
        {
            m_isClearQuiz = false;
            ChangeState(GameState.Standby);
        }
    }

    private void UpdateStandby()
    {
        //ここで正誤によってUIを切り替えて表示
        //ここに処理を追加

        //UIとフィールドの演出が終了したら、m_isClearQuizの値に応じて状態を変更
        //一旦trueとする
        if (true/*ここに処理を追加*/)
        {
            if (m_isClearQuiz)
            {
                ChangeState(GameState.CorrectAnswer);
            }
            else
            {
                ChangeState(GameState.IncorrectAnswer);
            }
        }
    }

    private void UpdateCorrectAnswer()
    {
        //クイズ格納用変数を初期化
        m_currentQuiz = null;

        //クリアした問題数を加算
        m_clearCount++;
        //難易度変更用のカウントを加算
        m_difficultyChangeCount++;

        //クリアした問題数がこのゲームの問題数と同じなら、ゲームクリアへ移行
        if (m_clearCount == m_quizNumber)
        {
            ChangeState(GameState.GameClear);
        }
        //まだゲームが終わっていないなら、難易度を上昇させるかを確認した後、クイズの出題へ移行
        else
        {
            //もし現在の難易度が5より小さいなら(5が最も難しい難易度のため)
            if(m_currentDifficulty < 5)
            {
                //難易度変更用のカウントが、今回のゲームにおける問題数を3で割った数と同じなら
                //カウントを初期化して難易度を上昇
                if(m_difficultyChangeCount == m_quizNumber / 3)
                {
                    m_difficultyChangeCount = 0;
                    m_currentDifficulty++;
                }
            }
            ChangeState(GameState.Question);
        }
    }

    private void UpdateIncorrectAnswer()
    {
        //クイズ格納用変数を初期化
        m_currentQuiz = null;
        
        //ライフが既に0なら、ゲームオーバーへ移行
        if(m_life == 0)
        {
            ChangeState(GameState.GameOver);
        }
        //まだ残機が残っているなら、残機を減少させ、クイズの出題へ移行
        else
        {
            m_life--;
            ChangeState(GameState.Question);
        }
    }

    private void UpdateGameClear()
    {
        //UIにゲームクリアの演出を再生させて、演出が終了したら一度だけロビーのシーンへ移行
        //ここに処理を追加
    }

    private void UpdateGameOver()
    {
        //UIにゲームオーバーの演出を再生させて、演出が終了したら一度だけロビーのシーンへ移行
        //ここに処理を追加
    }

    /// <summary>
    /// 現在の難易度を取得
    /// </summary>
    /// <returns></returns>
    public int GetCurrentDifficulty()
    {
        return m_currentDifficulty;
    }
    
    /// <summary>
    /// 現在の問題数を取得(クリアした問題数+1で現在の問題数)
    /// </summary>
    /// <returns></returns>
    public int GetCurrentQuizNumber()
    {
        return m_clearCount + 1;
    }

    /// <summary>
    /// 残機を取得
    /// </summary>
    /// <returns></returns>
    public int GetLife()
    {
        return m_life;
    }

    /// <summary>
    /// 回答のタイムリミットを取得(回答中状態以外なら0を返す)
    /// </summary>
    /// <returns></returns>
    public float GetLimit()
    {
        if(m_state == GameState.Thinking) return m_thinkingTime - m_elapsedTime;
        else return 0;
    }

    /// <summary>
    /// ゲームの設定を取得
    /// </summary>
    /// <returns></returns>
    public GameSetting GetSetting()
    {
        return m_gameSetting;
    }

    /// <summary>
    /// ゲーム内容の設定
    /// </summary>
    /// <param name="gameSetting">ロビーで設定した内容</param>
    public void SetGameSetting(GameSetting gameSetting)
    {
        m_gameSetting = gameSetting;
    }
}
