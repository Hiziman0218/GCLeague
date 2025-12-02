using Mirror;
using UnityEngine;

namespace MirrorChatSystems
{

    /// NetworkManagerを拡張して、チャット用のカスタムネットワークマネージャーを実装する
    /// <summary>
    /// Mirrorのデフォルトネットワークマネージャーを継承したシステム
    /// 以後、ネットワークマネージャーはこちらを使用し、対応する
    /// </summary>
    [AddComponentMenu("")]

    public class MirrorNetWorkManage : NetworkManager
    {
        /*
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // サーバーがクライアント接続を受け入れ、プレイヤーを生成する
            string playerName = conn.authenticationData as string;

            // プレイヤーハブの生成
            GameObject player = Instantiate(playerPrefab);
            PlayerNetWorkSystem playerNetworkSystem = player.GetComponent<PlayerNetWorkSystem>();
            playerNetworkSystem.SetPlayerName(playerName); // プレイヤー名を設定

            // プレイヤーをゲームに追加
            NetworkServer.AddPlayerForConnection(conn, player);
        }
        */

        [Header("ログインシステムリンク"),
            Tooltip("ログインする為のシステム")]
        public MirrorNewLoginSystem m_MirrorNewLoginSystem;

        // クライアントから送信されるプレイヤー名（クライアントの接続時に設定される）
        [Header("クライアント側プレイヤー名"),
            Tooltip("クライアント側のプレイヤー名/サーバー側のサーバー管理者名[GM]")]
        public string m_PlayerName;

        /// ---------------------------------------------------------------

        #region ゲーム(オンライン/メタバース)専用処理一覧/クライアント And サーバー

        #region クライアント側処理一式

        /// <summary>
        /// クライアントログアウト処理
        /// </summary>
        void ClientLogout()
        {
            // ログアウト処理を実装
            // 例: ログイン画面に戻る、セッション情報をクリアする等
            Debug.Log("ログアウト処理を実行します。");

            //ウィンドゥをログイン前状態に戻す。
            if (!m_MirrorNewLoginSystem)
                m_MirrorNewLoginSystem = GetComponent<MirrorNewLoginSystem>();

            //UI【ログインパネル】がアクティブなら
            if (m_MirrorNewLoginSystem.m_LogInWindow.gameObject.activeSelf)
                m_MirrorNewLoginSystem.OnLogInWindows();

            //UI【ログアウトパネル】がアクティブなら
            if (m_MirrorNewLoginSystem.m_LogOutWindow.gameObject.activeSelf)
                m_MirrorNewLoginSystem.OnLogOutWindows();

            //UI【クライアント用、サーバーが起動していない警告パネル】がアクティブなら
            if (!m_MirrorNewLoginSystem.m_NotServerWindow.gameObject.activeSelf)
                m_MirrorNewLoginSystem.OnNotServerWindows();

            //m_MPF_NewLoginSystem.OnLogOutButton();
        }
        #endregion

        #region サーバー側処理一式
        /// <summary>
        /// サーバー側でプレイヤーが追加されたので、プレイヤープレハブを生成して
        /// 接続情報認証からプレイヤーを設定、データに加える
        /// </summary>
        /// <param name="NCTC"></param>
        public void ServerAddPlayer(NetworkConnectionToClient conn)
        {
            GameObject player = Instantiate(playerPrefab);
            PlayerNetWorkSystem playerNetSystem = player.GetComponent<PlayerNetWorkSystem>();

            if (NetworkServer.connections.Count == 1)
            {
                playerNetSystem.isHostPlayer = true;
            }

            if (playerNetSystem != null)
            {
                string name = null;

                if (conn.authenticationData != null)
                    name = (string)conn.authenticationData;
                else if (!string.IsNullOrEmpty(m_PlayerName))
                    name = m_PlayerName + conn.connectionId;


                // ここで必ずサーバー側メソッドを呼ぶ
                playerNetSystem.SetPlayerName(name);
                Debug.Log("名前設定: " + name);
            }

            NetworkServer.AddPlayerForConnection(conn, player);
        }

        #endregion

        #endregion

        /// ---------------------------------------------------------------

        #region Mirror専用 On系予約メソッド

        #region ホスト・クライアント・サーバーの開始/停止
        /// <summary>
        /// ホストの開始時に呼ばれる
        /// </summary>
        public override void OnStartHost()
        {
            base.OnStartHost();
            print("ホストスタートしました。");
        }

        /// <summary>
        /// ホストの停止時に呼ばれる
        /// </summary>
        public override void OnStopHost()
        {
            base.OnStopHost();
            print("ホストが停止しました");
        }


        /// <summary>
        /// クライアントの開始時に呼ばれる
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();
            print("クライアントスタートしました");
        }

        /// <summary>
        /// クライアント側から、サーバーが切断された場合自動で実行
        /// </summary>
        public override void OnStopClient()
        {
            //クライアントを停止させる
            base.OnStopClient();
            Debug.Log("クライアントがサーバーから切断されました。");
            // ここでログアウト処理を行う
            ClientLogout();
        }

        /// <summary>
        /// サーバーの開始時に呼ばれる
        /// </summary>
        public override void OnStartServer()
        {
            base.OnStartServer();
            print("サーバースタートしました");
        }

        // サーバーの停止時に呼ばれる
        public override void OnStopServer()
        {
            base.OnStopServer();
            print("サーバーが停止しました");
        }
        #endregion

        #region クライアント系
        // クライアントの接続時に呼ばれる
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            print("クライアントが接続しました : ");
        }

        // クライアントの切断時に呼ばれる
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            print("クライアントが切断されました : ");
        }

        /*
        // クライアントのエラー時に呼ばれる
        public override void OnClientError(Exception exception)
        {
            base.OnClientError(exception);
            print("OnClientError : " + exception);
        }
        */

        // クライアントの未準備時に呼ばれる
        public override void OnClientNotReady()
        {
            base.OnClientNotReady();
            print("クライアントは未準備状態です : ");
        }

        // クライアントのシーン読み込み完了時に呼ばれる
        public override void OnClientChangeScene(
            string sceneName,
            SceneOperation sceneOperation,
            bool customHandlin)
        {
            base.OnClientChangeScene(sceneName, sceneOperation, customHandlin);
            print("クライアントは以下のSceneの読み込みが完了しました : " + sceneName);
        }

        #endregion

        #region サーバー系
        /// <summary>
        /// サーバーの接続時に呼ばれる
        /// </summary>
        /// <param name="NCTC"></param>
        public override void OnServerConnect(NetworkConnectionToClient NCTC)
        {
            base.OnServerConnect(NCTC);
            print("サーバーの接続時 : " + NCTC.connectionId);
        }

        /// <summary>
        /// サーバーの切断時に呼ばれる
        /// </summary>
        /// <param name="NCTC"></param>
        public override void OnServerDisconnect(NetworkConnectionToClient NCTC)
        {
            base.OnServerDisconnect(NCTC);
            print("サーバー切断時 : " + NCTC.connectionId);
        }

        /// <summary>
        /// サーバーの準備完了時に呼ばれる
        /// </summary>
        /// <param name="NCTC"></param>
        public override void OnServerReady(NetworkConnectionToClient NCTC)
        {
            base.OnServerReady(NCTC);
            print("サーバー準備完了時 : " + NCTC.connectionId);
        }

        /*
        // サーバーのエラー時に呼ばれる
        public override void OnServerError(NetworkConnectionToClient NCTC, Exception exception)
        {
            base.OnServerError(NCTC, exception);
            print("サーバーエラー時 : " + NCTC.connectionId + "," + exception);
        }
        */

        /// <summary>
        /// サーバー側でプレイヤーが追加された際に呼ばれるメソッド 
        /// </summary>
        /// <param name="NCTC"></param>
        public override void OnServerAddPlayer(NetworkConnectionToClient NCTC)
        {
            base.OnServerAddPlayer(NCTC);
            //サーバー側にプレイヤーが追加されたので、プレイヤーPrefabを生成し接続
            ServerAddPlayer(NCTC);
        }

        /// <summary>
        /// サーバーのシーン読み込み完了時に呼ばれる
        /// </summary>
        /// <param name="sceneName"></param>
        public override void OnServerChangeScene(string sceneName)
        {
            base.OnServerChangeScene(sceneName);
            print("サーバーのScene読み込み完了時 : " + sceneName);
        }
        #endregion

        #endregion
    }
}