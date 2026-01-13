using UnityEngine;
using Game.Enum;
using System.Collections;
using Mirror;

public class GameManager : NetworkBehaviour
{
    [Header("参照設定")]
    [SerializeField] private GameSystemManager m_systemManager; //システムマネージャー
    [SerializeField] private QuizManager m_quizManager; //クイズマネージャー
    [SerializeField] private QuizChecker m_quizChecker; //回答受け取り用

    [Header("詳細設定")]
    [SerializeField] private GameObject m_lobbyObjects; //ロビー時のみ存在するオブジェクト
    [SerializeField] private GameObject m_gameObjects;  //ゲーム時のみ存在するオブジェクト
    [SerializeField] private Transform m_trolleyPosition; //トロッコの位置
    [SerializeField] private Collider m_leftArea;  //左側の回答エリア
    [SerializeField] private Collider m_rightArea; //右側の回答エリア

    //GameManagerのインスタンス(シングルトン)
    public static GameManager Instance { get; private set; }

    private GameSetting m_gameSetting = null; //ゲーム開始前に設定される内容
    private int m_difficultyChangeCount = 0;  //難易度を上昇させるためのカウント
    private int m_quizNumber = 10;      //今回のゲームにおける問題数
    private float m_elapsedTime = 0f;   //経過時間計測用
    private float m_thinkingTime = 60f; //一回の回答にかけられる時間
    private float m_fadeTime = 1f;      //フェードにかける時間
    private float m_waitTime = 1f;      //フェードアウトとインの間で待機する時間
    private bool m_lobbyFlag = false;   //ロビーでの遷移管理用フラグ
    private bool m_isClearQuiz = false; //問題を正解したか
    private bool m_isGameEnd = false;   //ゲームをクリアしているか

    [SyncVar(hook = nameof(OnStateChanged))]
    private GameState m_state = GameState.Lobby; //ゲームの状態
    [SyncVar(hook = nameof(OnDifficultyChanged))]
    private int m_currentDifficulty = -1; //現在出題されるクイズの難易度
    [SyncVar(hook = nameof(OnQuizIdChanged))]
    private int m_currentQuizID = -1;     //クイズのID
    [SyncVar(hook = nameof(OnQuizNumberChanged))]
    private int m_clearCount = 0;       　//クリアした問題数
    [SyncVar(hook = nameof(OnLifeChanged))]
    private int m_life = -1;              //残り残機
    [SyncVar(hook = nameof(OnTimerChanged))]
    private float m_currentTime = 0f;   　//残り回答時間

    private void Awake()
    {
        //シングルトン
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //システムマネージャーを取得できていたら、設定を取得
        if (m_systemManager != null)
        {
            m_gameSetting = m_systemManager.GetGameSetting();
        }
    }

    private void Start()
    {
        Debug.Log($"isServer:{isServer}, isClient:{isClient}, isLocalPlayer:{isLocalPlayer}");
    }

    public override void OnStartServer()
    {
        Debug.Log("OnStartServer関数呼び出し");
        //ゲームの状態を開始時に初期化
        m_state = GameState.Lobby;
        GameEnd();
    }

    public override void OnStartClient()
    {
        Debug.Log($"My netId: {netId}");
    }

    private void OnEnable()
    {
        //通知イベントに設定
        StartCoroutine(BindUIManager());
    }

    private void OnDisable()
    {
        //通知イベントを解除
        if (UIManager.Instance == null) return;
        UIManager.Instance.OnUIShowComplete -= HandleUIShowComplete;
        UIManager.Instance.OnUIHideComplete -= HandleUIHideComplete;
    }

    private void Update()
    {
        if (isServer)
        {
            ServerUpdate();
            Debug.Log($"[GameManager] CurrentState = {m_state}");
        }
        else
        {
            if (isClient)
            {
                //ここでIDを確認して0番ならボタン表示

            }
        }
    }

    /// <summary>
    /// サーバーによる更新処理
    /// </summary>
    [Server]
    private void ServerUpdate()
    {
        switch (m_state)
        {
            case GameState.Lobby:
                UpdateLobby();
                break;
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
            case GameState.WaitFade:
                UpdateWaitFade();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ゲームの状態を変更
    /// </summary>
    /// <param name="State">変更したい状態</param>
    [Server]
    public void ChangeState(GameState State)
    {
        //経過時間計測用変数を初期化
        m_elapsedTime = 0f;
        m_state = State;
        RpcUIControl(State);
    }

    //各状態の更新処理

    [Server]
    private void UpdateLobby()
    {
        //フェードによる処理が終了しゲームが開始されるまでの秒数
        float StartTime = m_fadeTime * 2 + m_waitTime;

        if (m_lobbyFlag)
        {
            //一定時間後進行
            m_elapsedTime += Time.deltaTime;

            if(m_elapsedTime >= m_fadeTime)
            {
                GameStart();
            }

            if (m_elapsedTime >= StartTime)
            {
                ChangeState(GameState.GameStart);
            }
        }
    }

    [Server]
    private void UpdateGameStart()
    {

    }

    [Server]
    private void UpdateQuestion()
    {
        //クイズを取得していなければ、クイズマネージャーから指定された難易度のランダムなクイズを取得
        if (m_currentQuizID == -1)
        {
            m_currentQuizID = m_quizManager.GetRandomQuiz(m_currentDifficulty);
        }
    }

    [Server]
    private void UpdateThinking()
    {
        //経過時間を減算
        m_currentTime -= Time.deltaTime;

        if (m_currentTime <= 0f)
        {
            m_currentTime = 0f;
            ChangeState(GameState.Judging);
        }
    }

    [Server]
    private void UpdateJudging()
    {

    }

    [Server]
    private void UpdateStandby()
    {

    }

    [Server]
    private void UpdateCorrectAnswer()
    {
        //クイズ格納用変数を初期化
        m_currentQuizID = -1;

        //クリアした問題数を加算
        m_clearCount++;
        //難易度変更用のカウントを加算
        m_difficultyChangeCount++;

        //クリアした問題数がこのゲームの問題数と同じなら、ゲームクリアへ移行
        if (m_clearCount == m_quizNumber)
        {
            ChangeState(GameState.GameClear);
            m_isGameEnd = true;
        }
        //まだゲームが終わっていないなら、難易度を上昇させるかを確認した後、クイズの出題へ移行
        else
        {
            //もし現在の難易度が5より小さいなら(5が最も難しい難易度のため)
            if (m_currentDifficulty < 5)
            {
                //難易度変更用のカウントが、今回のゲームにおける問題数を3で割った数と同じなら
                //カウントを初期化して難易度を上昇
                if (m_difficultyChangeCount == m_quizNumber / 3)
                {
                    m_difficultyChangeCount = 0;
                    m_currentDifficulty++;
                }
            }
            ChangeState(GameState.Question);
        }
    }

    [Server]
    private void UpdateIncorrectAnswer()
    {
        //クイズ格納用変数を初期化
        m_currentQuizID = -1;

        //ライフが既に0なら、ゲームオーバーへ移行
        if (m_life == 0)
        {
            ChangeState(GameState.GameOver);
            m_isGameEnd = true;
        }
        //まだ残機が残っているなら、残機を減少させ、クイズの出題へ移行
        else
        {
            m_life--;
            ChangeState(GameState.Question);
        }
    }

    [Server]
    private void UpdateGameClear()
    {
        Debug.Log("ゲームクリア");
    }

    [Server]
    private void UpdateGameOver()
    {
        Debug.Log("ゲームオーバー");
    }

    [Server]
    private void UpdateWaitFade()
    {

    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    [Server]
    private void GameStart()
    {
        //ゲーム用のアクティブ設定
        RpcSetGameObjects(true);

        //全てのプレイヤーを初期位置に移動
        foreach (var player in ServerPlayerCollector.GetAllPlayers())
        {
            //ServerMessageTesterにあるSendPlayerWarpAll処理が使える？
            player.transform.position = Vector3.zero;
        }

        //出題するクイズの難易度を設定
        m_currentDifficulty = m_gameSetting.GetDifficulty();
        //今回のゲームにおける問題数を設定
        m_quizNumber = m_gameSetting.GetQuizNumber();
        //残機を設定
        m_life = m_gameSetting.GetLife();
        //一回の回答にかけられる時間を設定
        m_thinkingTime = m_gameSetting.GetTimer();
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    [Server]
    private void GameEnd()
    {
        //ロビー用のアクティブ設定
        RpcSetGameObjects(false);

        Debug.Log("GameEnd関数呼び出し");

        //ホストを取得し、ホストのロビー用UIを表示
        if (NetworkServer.localConnection != null)
        {
            var hostConn = NetworkServer.localConnection;
            if (hostConn.identity != null)
            {
                HostControll hostPlayer = hostConn.identity.GetComponent<HostControll>();
                hostPlayer.OnGameEnd();
            }
        }

        //全てのプレイヤーを初期位置に移動
        foreach (var player in ServerPlayerCollector.GetAllPlayers())
        {
            //ServerMessageTesterにあるSendPlayerWarpAll処理が使える？
            player.transform.position = new Vector3(0f, 0.7f, 0.25f);
        }

        //ゲームに使用する各値を初期化
        m_currentDifficulty = -1;
        m_currentQuizID = -1;
        m_clearCount = 0;
        m_life = -1;
        m_currentTime = 0f;
        m_isGameEnd = false;

        //クイズの出題フラグを初期化
        m_quizManager.ResetUsedFlags();
        //ロビーのフラグを初期化
        m_lobbyFlag = false;
    }

    /// <summary>
    /// UIManagerに必ずイベントを登録するためのコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator BindUIManager()
    {
        while (UIManager.Instance == null)
        {
            yield return null;
        }

        UIManager.Instance.OnUIShowComplete += HandleUIShowComplete;
        UIManager.Instance.OnUIHideComplete += HandleUIHideComplete;
    }

    /// <summary>
    /// フラグに合わせたロビーとゲームの切り替え
    /// </summary>
    /// <param name="isGame"></param>
    [ClientRpc]
    private void RpcSetGameObjects(bool isGame)
    {
        m_lobbyObjects.SetActive(!isGame);
        m_gameObjects.SetActive(isGame);
    }

    /// <summary>
    /// プレイヤーのいるエリアによる判定
    /// </summary>
    [Server]
    private void JudgeByPosition()
    {
        int leftCount = 0;
        int rightCount = 0;

        foreach (var player in ServerPlayerCollector.GetAllPlayers())
        {
            Vector3 pos = player.transform.position;

            if (m_leftArea.bounds.Contains(pos))
            {
                leftCount++;
            }
            else if (m_rightArea.bounds.Contains(pos))
            {
                rightCount++;
            }
        }

        //左が 0、右が 1
        int selectedAnswer = leftCount >= rightCount ? 0 : 1;

        //IDを元にクイズを取得
        QuizRuntime quiz = m_quizManager.GetQuizRuntime(m_currentQuizID);

        bool isCorrect = quiz.IsCorrect(selectedAnswer);
        RpcShowResult(isCorrect);
        m_isClearQuiz = isCorrect;

        RpcMoveTrolley(m_isClearQuiz);
        ChangeState(GameState.Standby);
    }

    /// <summary>
    /// トロッコ演出
    /// </summary>
    /// <param name="isCorrect"></param>
    [ClientRpc]
    void RpcMoveTrolley(bool isCorrect)
    {
        //できあがってから
        //trolleyAnimator.Play(isCorrect ? "Forward" : "Fall");
    }

    /// <summary>
    /// クライアントでUIを管理
    /// </summary>
    [ClientRpc]
    void RpcUIControl(GameState state)
    {
        //UIManagerがまだ生成されていない場合は待つ
        if (UIManager.Instance == null)
        {
            StartCoroutine(WaitForUIManager(state));
            return;
        }

        ApplyUI(state);
    }

    /// <summary>
    /// UIManagerを経由してUI管理
    /// </summary>
    /// <param name="state"></param>
    void ApplyUI(GameState state)
    {
        switch (state)
        {
            case GameState.Lobby:
                UIManager.Instance.HideUI(UIType.HUD);
                UIManager.Instance.Initialize();
                //ゲームクリア非表示
                //ゲームオーバー非表示
                break;
            case GameState.GameStart:
                UIManager.Instance.ShowUI(UIType.StartUI);
                break;
            case GameState.Question:
                UIManager.Instance.ShowUI(UIType.HUD);
                UIManager.Instance.ShowUI(UIType.QuizUI);
                break;
            case GameState.Thinking:
                UIManager.Instance.ShowUI(UIType.Timer);
                break;
            case GameState.Judging:
                UIManager.Instance.HideUI(UIType.Timer);
                break;
            case GameState.Standby:
                break;
            case GameState.CorrectAnswer:
                break;
            case GameState.IncorrectAnswer:
                break;
            case GameState.GameClear:
                //ゲームクリア表示
                break;
            case GameState.GameOver:
                //ゲームオーバー表示
                break;
            case GameState.WaitFade:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// UIManagerが生成されていない場合の対策コルーチン
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    IEnumerator WaitForUIManager(GameState state)
    {
        // UIManager が生成されるまで待つ
        while (UIManager.Instance == null)
        {
            yield return null;
        }

        ApplyUI(state);
    }

    /// <summary>
    /// クライアントで正解/不正解のUI表示
    /// </summary>
    /// <param name="isCorrect"></param>
    [ClientRpc]
    void RpcShowResult(bool isCorrect)
    {
        UIManager.Instance.ShowResult(isCorrect);
    }

    /// <summary>
    /// 状態変更時のUI切り替え
    /// </summary>
    /// <param name="oldState"></param>
    /// <param name="newState"></param>
    private void OnStateChanged(GameState oldState, GameState newState)
    {
        UIManager.Instance.OnGameStateChanged(newState);

        switch(newState)
        {
            case GameState.Thinking:
                //ホストを取得し、ホストのロビー用UIを表示
                if (NetworkServer.localConnection != null)
                {
                    var hostConn = NetworkServer.localConnection;
                    if (hostConn.identity != null)
                    {
                        HostControll hostPlayer = hostConn.identity.GetComponent<HostControll>();
                        hostPlayer.OnThinkingStart();
                    }
                }
                break;
            case GameState.Judging:
                //回答判定
                JudgeByPosition();
                //ホストを取得し、ホストのロビー用UIを表示
                if (NetworkServer.localConnection != null)
                {
                    var hostConn = NetworkServer.localConnection;
                    if (hostConn.identity != null)
                    {
                        HostControll hostPlayer = hostConn.identity.GetComponent<HostControll>();
                        hostPlayer.OnThinkingEnd();
                    }
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 現在の難易度の値変更
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnDifficultyChanged(int oldValue, int newValue)
    {
        UIManager.Instance.UpdateDifficulty(newValue);
    }

    /// <summary>
    /// クイズIDの値変更
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnQuizIdChanged(int oldValue, int newValue)
    {
        if (newValue < 0) return;

        Quiz quiz = QuizDatabase.Instance.Quizzes[newValue];

        UIManager.Instance.OnQuizChanged(quiz);
    }

    /// <summary>
    /// 現在の問題数の値変更
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnQuizNumberChanged(int oldValue, int newValue)
    {
        UIManager.Instance.UpdateQuizNumber(newValue);
    }

    /// <summary>
    /// 現在の残機の値変更
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnLifeChanged(int oldValue, int newValue)
    {
        UIManager.Instance.UpdateLife(newValue);
    }

    /// <summary>
    /// 残り回答時間の値変更
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnTimerChanged(float oldValue, float newValue)
    {
        UIManager.Instance.UpdateTime(newValue);

        if (newValue <= 0f)
        {
            //残り回答時間が無くなったら、クイズを非表示
            UIManager.Instance.HideUI(UIType.QuizUI);
        }
    }

    /// <summary>
    /// 表示完了時の処理
    /// </summary>
    /// <param name="type"></param>
    private void HandleUIShowComplete(UIType type)
    {
        //サーバーでなければ、以降の処理を行わない
        if (!isServer) return;

        /*受け取ったUIのタイプに合わせた処理を書く*/
        switch (type)
        {
            case UIType.HUD:
                break;
            case UIType.Timer:
                break;
            case UIType.StartUI:
                break;
            case UIType.QuizUI:
                m_currentTime = m_thinkingTime;
                ChangeState(GameState.Thinking);
                break;
            case UIType.CorrectUI:
                break;
            case UIType.IncorrectUI:
                break;
            case UIType.GameClearUI:
                break;
            case UIType.GameOverUI:
                break;
            case UIType.FadeUI:
                if (m_isGameEnd)
                {
                    GameEnd();
                    ChangeState(GameState.Lobby);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 非表示完了時の処理
    /// </summary>
    /// <param name="type"></param>
    private void HandleUIHideComplete(UIType type)
    {
        //サーバーでなければ、以降の処理を行わない
        if (!isServer) return;

        /*受け取ったUIのタイプに合わせた処理を書く*/
        switch (type)
        {
            case UIType.HUD:
                break;
            case UIType.Timer:
                break;
            case UIType.StartUI:
                ChangeState(GameState.Question);
                break;
            case UIType.QuizUI:
                break;
            case UIType.CorrectUI:
                ChangeState(GameState.CorrectAnswer);
                break;
            case UIType.IncorrectUI:
                ChangeState(GameState.IncorrectAnswer);
                break;
            case UIType.GameClearUI:
                break;
            case UIType.GameOverUI:
                break;
            case UIType.FadeUI:    
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ゲームの設定を取得
    /// </summary>
    /// <returns></returns>
    public GameSetting GetGameSetting()
    {
        return m_gameSetting;
    }

    /// <summary>
    /// スタートボタンを押したときの処理
    /// </summary>
    public void PushStartButton()
    {
        //m_systemManager.ChangeScene(m_fadeTime, m_waitTime);
        UIManager.Instance.ShowFade(m_fadeTime, m_waitTime);
        m_lobbyFlag = true;
        /*
        if (!isLocalPlayer) return;
        CmdPushStartButton();*/
    }

    [Command]
    void CmdPushStartButton()
    {
        //m_systemManager.ChangeScene(m_fadeTime, m_waitTime);
        UIManager.Instance.ShowFade(m_fadeTime, m_waitTime);
        m_lobbyFlag = true;
    }

    /// <summary>
    /// 回答終了ボタンを押したときの処理
    /// </summary>
    public void AnswerCompleted()
    {
        //強制的に回答終了
        ChangeState(GameState.Judging);
        UIManager.Instance.HideUI(UIType.QuizUI);
    }
}
