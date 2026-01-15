using UnityEngine;
using Game.Enum;
using Mirror;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    [Header("参照設定")]
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

    private int m_difficultyChangeCount = 0;  //難易度を上昇させるためのカウント
    private int m_quizNumber = 10;      //今回のゲームにおける問題数//SyncVarにする？
    private float m_elapsedTime = 0f;   //経過時間計測用
    private float m_thinkingTime = 60f; //一回の回答にかけられる時間//SyncVarにする？
    private float m_fadeTime = 1f;      //フェードにかける時間
    private float m_fadeWaitTime = 1f;  //フェードアウトとインの間で待機する時間
    private float m_waitTime = 3f;      //演出を待つ時間
    private float m_startUITime = 6f;   //StartUIの表示が終了するまでを待つ時間
    private float m_quizUIShowTime = 0.5f; //QuizUIの表示を待つ時間
    private bool m_lobbyFlag = false;   //ロビーでの遷移管理用フラグ
    private bool m_startFlag = false;   //ゲームをスタートしたか
    private bool m_isClearQuiz = false; //問題を正解したか
    private bool m_isCorrect = false;   //正解時の処理制御用フラグ
    private bool m_isSubtractionLife = false; //残機を既に減らしたか
    private bool m_isGameStart = false; //ゲームが始まっているか
    private bool m_isGameEnd = false;   //ゲームをクリアしているか

    private GameSettingUI m_settingUI;  //ゲーム内容設定UI

    [SyncVar(hook = nameof(OnStateChanged))]
    private GameState m_state = GameState.Lobby; //ゲームの状態
    [SyncVar(hook = nameof(OnDifficultyChanged))]
    private int m_currentDifficulty = -1; //現在出題されるクイズの難易度
    [SyncVar(hook = nameof(OnQuizIdChanged))]
    private int m_currentQuizID = -1;     //クイズのID
    [SyncVar(hook = nameof(OnQuizNumberChanged))]
    private int m_clearCount = 0;       　//クリアした問題数
    [SyncVar(hook = nameof(OnPlayerCountChanged))]
    private int m_playerCount;            //参加しているプレイヤーの人数
    [SyncVar(hook = nameof(OnLifeChanged))]
    private int m_life = -1;              //残り残機
    [SyncVar(hook = nameof(OnTimerChanged))]
    private float m_currentTime = 0f;   　//残り回答時間

    [SyncVar] private int m_settingDifficulty = 1;    //ゲーム設定における問題の難易度
    [SyncVar] private int m_settingQuizNumber = 10;   //ゲーム設定における総問題数
    [SyncVar] private int m_settingPlayerNumber = 1;  //ゲーム設定におけるプレイヤーの人数
    [SyncVar] private int m_settingLife = 3;          //ゲーム設定における残機
    [SyncVar] private float m_settingTimer = 60f;     //ゲーム設定における一回の回答における制限時間

    private void Awake()
    {
        //シングルトン
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Debug.Log($"isServer:{isServer}, isClient:{isClient}, isLocalPlayer:{isLocalPlayer}");
    }

    public override void OnStartServer()
    {
        //サーバー起動時に初期化
        InitializeServerState();
    }

    public override void OnStopServer()
    {
        //サーバー終了時にリセット
        ResetServerState();
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (isServer)
        {
            ServerUpdate();
            m_playerCount = NetworkServer.connections.Count;
        }

        if (isServer && NetworkServer.connections.Count == 0)
        {
            ResetServerState();
            m_state = GameState.Lobby;
        }
    }

    /*サーバーによるアップデート*/

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
        //既に正解処理をしたかフラグを初期化
        m_isCorrect = false;
        //既に残機を減らしたかフラグを初期化
        m_isSubtractionLife = false;
        //変更先がThinkingなら、制限時間を初期化し、ホストの回答ボタンを表示
        if (State == GameState.Thinking)
        {
            m_currentTime = m_settingTimer;
            RpcShowFinalAnswerButton();
        }
        else if(State == GameState.Judging)
        {
            RpcHideFinalAnswerButton();
        }

        //状態更新
        m_state = State;
        //変更された状態に合わせてクライアントでUI管理
        RpcOnStateChanged(State);
        //更新された状態確認用ログ
        Debug.Log($"[GameManager] CurrentState = {m_state}");
    }

    /*各状態の更新処理*/

    [Server]
    private void UpdateLobby()
    {
        //フェードによる処理が終了しゲームが開始されるまでの秒数
        float StartTime = m_fadeTime * 2 + m_fadeWaitTime;

        if (m_lobbyFlag)
        {
            //一定時間後進行
            m_elapsedTime += Time.deltaTime;

            if(m_elapsedTime >= m_fadeTime && !m_startFlag)
            {
                m_startFlag = true;
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
        //一定時間経過後次の問題へ
        m_elapsedTime += Time.deltaTime;

        if (m_elapsedTime >= m_startUITime)
        {
            ChangeState(GameState.Question);
        }
    }

    [Server]
    private void UpdateQuestion()
    {
        //クイズを取得していなければ、クイズマネージャーから指定された難易度のランダムなクイズを取得
        if (m_currentQuizID == -1)
        {
            m_currentQuizID = m_quizManager.GetRandomQuiz(m_currentDifficulty);
        }

        //一定時間経過後次の問題へ
        m_elapsedTime += Time.deltaTime;

        if (m_elapsedTime >= m_quizUIShowTime)
        {
            ChangeState(GameState.Thinking);
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
        JudgeByPosition();
    }

    [Server]
    private void UpdateStandby()
    {
        //クイズを正解したか不正解したかで遷移先を変更
        if (m_isClearQuiz)
        {
            ChangeState(GameState.CorrectAnswer);
        }
        else
        {
            ChangeState(GameState.IncorrectAnswer);
        }    
    }

    [Server]
    private void UpdateCorrectAnswer()
    {
        //クイズ格納用変数を初期化
        m_currentQuizID = -1;

        //まだ正解時の処理をしていなければ
        if (!m_isCorrect)
        {
            m_isCorrect = true;
            //クリアした問題数を加算
            m_clearCount++;
            //難易度変更用のカウントを加算
            m_difficultyChangeCount++;
        }

        //クリアした問題数がこのゲームの問題数と同じなら、ゲームクリアへ移行
        if (m_clearCount == m_quizNumber)
        {
            m_isGameEnd = true;
            ChangeState(GameState.GameClear);
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

            //一定時間経過後次の問題へ
            m_elapsedTime += Time.deltaTime;

            if(m_elapsedTime >= m_waitTime)
            {
                ChangeState(GameState.Question);
            }
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
            m_isGameEnd = true;
            ChangeState(GameState.GameOver);
        }
        //まだ残機が残っているなら、残機を減少させ、クイズの出題へ移行
        else
        {
            //残機がまだ減っていない場合
            if (!m_isSubtractionLife)
            {
                m_life--;
                m_isSubtractionLife = true;
            }

            //一定時間経過後次の問題へ
            m_elapsedTime += Time.deltaTime;

            if (m_elapsedTime >= m_waitTime)
            {
                ChangeState(GameState.Question);
            }
        }
    }

    [Server]
    private void UpdateGameClear()
    {
        //一定時間後ゲーム終了
        m_elapsedTime += Time.deltaTime;

        if (m_elapsedTime >= m_fadeTime)
        {
            GameEnd();
        }

        Debug.Log("Game Clear");
    }

    [Server]
    private void UpdateGameOver()
    {
        //一定時間後ゲーム終了
        m_elapsedTime += Time.deltaTime;

        if (m_elapsedTime >= m_fadeTime)
        {
            GameEnd();
        }

        Debug.Log("Game Over");
    }

    /*ゲーム開始/終了の初期化*/

    /// <summary>
    /// ゲーム開始
    /// </summary>
    [Server]
    private void GameStart()
    {
        //ゲーム用のアクティブ設定
        RpcSetGameObjects(true);
        //全てのプレイヤーを初期位置に移動
        TeleportAllPlayers(new Vector3(0f, 0.5f, 0f));

        //出題するクイズの難易度を設定
        m_currentDifficulty = m_settingDifficulty;
        //今回のゲームにおける問題数を設定
        m_quizNumber = m_settingQuizNumber;
        //残機を設定
        m_life = m_settingLife;
        //一回の回答にかけられる時間を設定
        m_thinkingTime = m_settingTimer;
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    [Server]
    private void GameEnd()
    {
        //ロビー用のアクティブ設定
        RpcSetGameObjects(false);

        //全てのプレイヤーを初期位置に移動
        TeleportAllPlayers(new Vector3(0f, 0.6f, 1.2f));

        RpcShowStartButton();

        ResetServerState();

        ChangeState(GameState.Lobby);
    }

    /// <summary>
    /// サーバー初期化共通処理
    /// </summary>
    [Server]
    private void InitializeServerState()
    {
        m_state = GameState.Lobby;
        ResetServerState();
    }

    /// <summary>
    /// サーバーリセット処理
    /// </summary>
    [Server]
    private void ResetServerState()
    {
        //ゲームに使用する各値を初期化
        m_currentDifficulty = -1;
        m_currentQuizID = -1;
        m_clearCount = 0;
        m_life = -1;
        m_currentTime = 0f;
        m_isGameStart = false;
        m_isGameEnd = false;
        m_lobbyFlag = false;
        m_startFlag = false;

        //クイズの出題フラグを初期化
        if (m_quizManager != null)
        {
            m_quizManager.ResetUsedFlags();
        }
    }

    /*一括移動処理*/

    [Server]
    public void TeleportAllPlayers(Vector3 pos)
    {
        foreach (var playerObj in ServerPlayerCollector.GetAllPlayers())
        {
            var move = playerObj.GetComponent<MirrorPlayerMoves>();
            if (move != null)
            {
                move.ServerTeleport(pos);
            }
        }
    }

    /*判定処理*/

    /// <summary>
    /// プレイヤーのいるエリアによる判定
    /// </summary>
    [Server]
    private void JudgeByPosition()
    {
        int left = 0;
        int right = 0;

        foreach (var player in ServerPlayerCollector.GetAllPlayers())
        {
            var area = player.GetComponent<PlayerAnswerArea>();
            if (area == null) continue;

            if (area.currentArea == AnswerArea.Left) left++;
            else if (area.currentArea == AnswerArea.Right) right++;
        }

        int selectedAnswer = left >= right ? 0 : 1;

        var quiz = m_quizManager.GetQuizRuntime(m_currentQuizID);
        m_isClearQuiz = quiz.IsCorrect(selectedAnswer);

        RpcShowResult(m_isClearQuiz);
        ChangeState(GameState.Standby);
    }

    /*Rpcによる演出*/

    /// <summary>
    /// トロッコ演出
    /// </summary>
    /// <param name="isCorrect"></param>
    [ClientRpc]
    private void RpcMoveTrolley(bool isCorrect)
    {
        //未完成なので形だけ
    }

    /// <summary>
    /// 変更された状態に合わせた処理
    /// </summary>
    /// <param name="state"></param>
    [ClientRpc]
    private void RpcOnStateChanged(GameState state)
    {
        UIManager.Instance.OnGameStateChanged(state);
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
    /// クライアントで正解/不正解のUI表示
    /// </summary>
    /// <param name="isCorrect"></param>
    [ClientRpc]
    private void RpcShowResult(bool isCorrect)
    {
        UIManager.Instance?.ShowResult(isCorrect);
    }
    
    /// <summary>
    /// クライアントで暗転のUI表示
    /// </summary>
    [ClientRpc]
    private void RpcShowFadeUI()
    {
        UIManager.Instance?.ShowFade(m_fadeTime, m_fadeWaitTime);
    }

    /// <summary>
    /// クライアントでホストのみスタートボタン表示
    /// </summary>
    [ClientRpc]
    private void RpcShowStartButton()
    {
        var host = FindObjectsOfType<HostControll>()
        .FirstOrDefault(h => h.isLocalPlayer && h.isHostPlayer);
        host?.OnGameEnd();
    }

    /// <summary>
    /// クライアントでホストのみ回答完了ボタン表示
    /// </summary>
    [ClientRpc]
    private void RpcShowFinalAnswerButton()
    {
        var host = FindObjectsOfType<HostControll>()
        .FirstOrDefault(h => h.isLocalPlayer && h.isHostPlayer);
        host?.OnThinkingStart();
    }

    /// <summary>
    /// クライアントでホストのみ回答完了ボタン非表示
    /// </summary>
    [ClientRpc]
    private void RpcHideFinalAnswerButton()
    {
        var host = FindObjectsOfType<HostControll>()
        .FirstOrDefault(h => h.isLocalPlayer && h.isHostPlayer);
        host?.OnThinkingEnd();
    }

    /*hook*/

    /// <summary>
    /// 状態変更時のUI切り替え
    /// </summary>
    /// <param name="oldState"></param>
    /// <param name="newState"></param>
    private void OnStateChanged(GameState oldState, GameState newState)
    {
        
    }

    /// <summary>
    /// 現在の難易度の値変更
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnDifficultyChanged(int oldValue, int newValue)
    {
        UIManager.Instance?.UpdateDifficulty(newValue);
    }

    /// <summary>
    /// クイズIDの値変更
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnQuizIdChanged(int oldValue, int newValue)
    {
        if (newValue < 0) return;
        UIManager.Instance?.OnQuizChanged(QuizDatabase.Instance.Quizzes[newValue]);
    }

    /// <summary>
    /// 現在の問題数の値変更
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnQuizNumberChanged(int oldValue, int newValue)
    {
        UIManager.Instance?.UpdateQuizNumber(newValue);
    }

    /// <summary>
    /// プレイヤーの人数の値変更
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnPlayerCountChanged(int oldValue, int newValue)
    {
        UIManager.Instance?.UpdatePlayerCount(newValue);
    }

    /// <summary>
    /// 現在の残機の値変更
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnLifeChanged(int oldValue, int newValue)
    {
        UIManager.Instance?.UpdateLife(newValue);
    }

    /// <summary>
    /// 残り回答時間の値変更
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnTimerChanged(float oldValue, float newValue)
    {
        UIManager.Instance?.UpdateTime(newValue);
    }

    /*UIからサーバーへのリクエスト*/

    [Server]
    public void ServerStartFromHost()
    {
        //クライアントへ Fade 指示
        RpcShowFadeUI();
        m_lobbyFlag = true;
        m_isGameStart = true;
    }

    [Server]
    public void ServerRequestJudge()
    {
        ChangeState(GameState.Judging);
    }

    /*その他 ゲッターセッターなど*/

    /// <summary>
    /// 受け取ったゲーム設定パネルにゲーム設定を設定
    /// </summary>
    /// <param name="gameSettingUI"></param>
    public void SetGameSettingUI(GameSettingUI gameSettingUI)
    {
        m_settingUI = gameSettingUI;
        if (m_settingUI != null) m_settingUI.SetGameSetting(m_settingDifficulty, 
            m_settingQuizNumber, m_settingLife, m_settingTimer);
    }

    public int GetSettingDifficulty()
    {
        return m_settingDifficulty;
    }

    public int GetSettingQuizNumber()
    {
        return m_settingQuizNumber;
    }

    public int GetSettingLife()
    {
        return m_settingLife;
    }

    public float GetSettingTimer()
    {
        return m_settingTimer;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetSettingDifficulty(int difficulty)
    {
        m_settingDifficulty = difficulty;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetSettingQuizNumber(int quizNumber)
    {
        m_settingQuizNumber = quizNumber;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetSettingLife(int life)
    {
        m_settingLife = life;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetSettingTimer(float timer)
    {
        m_settingTimer = timer;
    }
}