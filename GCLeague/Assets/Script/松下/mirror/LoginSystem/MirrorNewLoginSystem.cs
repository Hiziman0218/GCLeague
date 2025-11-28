using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using MirrorChatSystems;

/// <summary>
/// Mirrorのオリジナルログインシステム
/// 要素:
/// ①　GUIのログインシステムに準拠するように再設計している
/// ②　ログインシステムのマネージャー
/// </summary>
public class MirrorNewLoginSystem : MonoBehaviour
{
    #region 配列一式
    [Header("サーバーのIP/デフォは127.0.0.1"), 
        Tooltip("サーバーのIPアドレス[デフォは127.0.0.1]で、\n" +
        "基本グローバルIPをセットする事で\n" +
        "インターネット経由での接続が可能。")]
    public string m_NetWorkIP = "127.0.0.1";

    [Header("ネットワークマネージャーリンク"),
        Tooltip("NetworkManagerとのリンク。\n" +
        "この場合、MirrorNetWorkManagerと\n" +
        "繋がる。Networkとの連動に使用する。")]
    public NetworkManager m_NetworkManager;

    [SerializeField, Header("チェックするサーバーポートの入ったトランスポート"),
        Tooltip("ネット補使用する際の通信トランスポート。\n" +
        "KcpはMirrorのデフォルト通信方法で、\n" +
        "その他にも様々な通信方法がある。")]
    private kcp2k.KcpTransport m_KCP;
    #region トランスポートのうんちく
    ///◎ ビルトイン(デフォルト)
    ///・KcpTransport : 
    ///     Mirrorのデフォルトのトランスポート。
    ///・TelepathyTransport: 
    ///     C#のシンプルメッセージベースのTCPトランスポート。 
    ///・SimpleWebTransport : 
    ///     WebGLビルド用のWebSocketトランスポート。
    ///・MultipleTransport : 
    ///     復数のトランスポートを組み合わせて同時に処理するブリッジトランスポート。
    ///・LatencySimulation : 
    ///     理想的でないネットワーク状態をテストするためのトランスポート。

    ///◎ プラグイン
    ///・Ignorance : 
    ///     ENetをベースにしたUDPトランスポート。
    ///・LiteNetLibTransport : 
    ///     LiteNetLibをベースにしたUDPトランスポート。
    ///・FizzySteamworks :       【Steam用】
    ///     Steamworks.NETで構築したSteamP2Pネットワークを利用したトランスポート。
    ///・FizzyFacepunch :        【Steam用】
    ///     Facepunch.Steamworksで構築したSteamP2Pネットワークを利用したトランスポート。
    ///・Epic Online Services :  【UnrealEngine等用】
    ///     Epicのフリーリレーサービスを利用したリレートランスポート。
    ///・Light Reflective Mirror : 
    ///     WebGLクライアントのリレートランスポート。
    ///・Oculus Platform :       【MetaQuest用】
    ///     Oculus Quest1&2のリレートランスポート。
    #endregion

    [Header("ログインパネル"),
        Tooltip("クライアントがログインする際のパネルのリンク\n" +
        "このゲームオブジェクトをOnOffする事で、ログイン後の\n" +
        "ログインを抑制する。")]
    public GameObject m_LogInWindow;

    [Header("ログアウトパネル"),
        Tooltip("クライアントがログアウトする際のパネルのリンク\n" +
        "このゲームオブジェクトをOnOffする事で、ログイン中以外の\n" +
        "ログアウトを抑制する。")]
    public GameObject m_LogOutWindow;

    [Header("クライアント用、サーバーが起動していない警告パネル"),
        Tooltip("クライアント用で、万が一サーバーが起動していない場合" +
        "ログイン不可なので、ログイン出来ない事を伝えるパネル")]
    public GameObject m_NotServerWindow;

    [Header("プレイヤー名入力フィールド"),
        Tooltip("ゲーム中のプレイヤー名を入力するフィールド")]
    public InputField m_UserNameField;

    public enum MirrorSystemMode
    {
        未処理,
        クライアント,
        サーバー,
    }
    [Header("[初期状態]サーバーにすると、自動でサーバー機として機能する"),
        Tooltip("現在の処理モード。\n" +
        "初期状態は、未処理とする事で、クライアントとして実行可能。\n" +
        "逆にサーバーと設定した場合、ビルド後起動時にサーバーとして\n" +
        "稼働する事になる。")]
    public MirrorSystemMode m_MirrorSystemMode = MirrorSystemMode.未処理;

    [SerializeField, Header("テスト用デバックモード"),
        Tooltip("テスト用デバックモード\n" +
        "これがtrueであれば、ビルド後実行したアプリは必ずサーバーとなる\n" +
        "その際、処理モードが未処理でも、確定サーバーとなる")]
    private bool m_DebugMode = false;

    [Header("チャットメッセージプレート"),
        Tooltip("チャットメッセージの入力と表示用プレート\n" +
        "これがOnになれば、チャット可能となる。\n" +
        "これがOffの場合、チャット機能は停止、もしくは無視される")]
    public GameObject m_MessageObject;

    #endregion

    #region 自動起動部分
    private void Awake()
    {

#if UNITY_EDITOR
        //UnityEditor上で、デバッグモードがOnの場合はクライアントとして処理
        if (m_DebugMode)
            m_MirrorSystemMode = MirrorSystemMode.未処理;
#else
        //UnityEditor以外は、デバッグモードがOnの場合はサーバーとして処理
        if (m_DebugMode)
            m_MirrorSystemMode = MirrorSystemMode.サーバー;
#endif
        //ネットワークマネージャーがない場合、ネットワークマネージャーを代入
        if (!m_NetworkManager)
            m_NetworkManager = this.GetComponent<NetworkManager>();

        //KCPがない場合、KCPを代入する(他のトランスポーターの場合は書き直し)
        if (!m_KCP)
            m_KCP = GetComponent<kcp2k.KcpTransport>();

        // サーバーのアドレス設定
        //NetworkManager.singleton.networkAddress = m_NetWorkIP;

        //初期からサーバー起動指定している場合、全てのウィンドゥ(チャット以外)はOff
        if (m_MirrorSystemMode == MirrorSystemMode.サーバー)
        {
            //全て(ログイン、ログアウト、警告)は非表示
            m_LogInWindow.gameObject.SetActive(false);
            m_LogOutWindow.gameObject.SetActive(false);
            m_NotServerWindow.gameObject.SetActive(false);
            //サーバーで起動
            OnServerButton();
        }
        else
        {
            //サーバーではない場合は、ログイン画面を表示状態にする

            //ログインウィンドゥを表示、それ以外は非表示
            if (m_LogInWindow.gameObject.activeSelf == false)
                OnLogInWindows();
            if (m_LogOutWindow.gameObject.activeSelf == true)
                OnLogOutWindows();
            if (m_NotServerWindow.gameObject.activeSelf == true)
                OnNotServerWindows();

            //未処理モードにして選択出来るようにする
            m_MirrorSystemMode = MirrorSystemMode.未処理;
        }
    }
#endregion

    #region ログインパネル表示時

    #region  [サーバーボタン押下時に呼ばれる]DedicatedServerを使用する為、サーバーはAuto。以下の機能はオミットします。
    public void OnServerButton()
    {
        //サーバーが起動
        m_NetworkManager.StartServer();
        //NetworkManager.singleton.StartServer();
        //現在のモードはサーバーである
        m_MirrorSystemMode = MirrorSystemMode.サーバー;
    }
    #endregion

    #region [クライアントボタンが押下時に呼ばれる]
    public void OnClientButton()
    {
        //クライアントが起動してない場合のみ実行
        if (!NetworkClient.active)
        {
            //クライアントは、サーバーが起動しているかどうか確認するまでは、全てオフ
            if (m_LogInWindow.gameObject.activeSelf)
                OnLogInWindows();
            if (!m_LogOutWindow.gameObject.activeSelf)
                OnLogOutWindows();
            if (m_NotServerWindow.gameObject.activeSelf)
                OnNotServerWindows();

            // 入力フィールドからプレイヤー名を取得
            string playerName = m_UserNameField.text;
            Debug.Log("入力されたプレイヤー名: " + playerName);

            // NetworkManager に渡す（例: カスタムNetworkManagerに保持させる）
            ((MirrorNetWorkManage)m_NetworkManager).m_PlayerName = playerName;


            //現在のモード
            m_MirrorSystemMode = MirrorSystemMode.クライアント;
            //サーバー側のIPを指定する場合はこれをつける
            //m_NetworkManager.networkAddress = m_NetWorkIP;

            // サーバーのアドレス設定
            m_NetworkManager.networkAddress = m_NetWorkIP;

            //新規クライアントログイン
            //m_NetworkManager.StartClient();
            NetworkManager.singleton.StartClient();

            //メッセージウィンドゥを表示状態に
            if(m_MessageObject)
                if(!m_MessageObject.activeSelf)
                     m_MessageObject.SetActive(true);
        }
    }
    #endregion

    #region [サーバーが存在していない場合に警告ボタン押下時に呼ばれる]
    public void OnNotServerButton()
    {
        //クライアントは、サーバーが起動しているかどうか確認するまでは、全てオフ
        if (!m_LogInWindow.gameObject.activeSelf)
            OnLogInWindows();
        if (m_LogOutWindow.gameObject.activeSelf)
            OnLogOutWindows();
        if (m_NotServerWindow.gameObject.activeSelf)
            OnNotServerWindows();

        //現在のモード
        m_MirrorSystemMode = MirrorSystemMode.未処理;
    }
    #endregion

    #endregion

    #region ログアウト処理
    public void OnLogOutButton()
    {
        //サーバー、クライアント各種モードでの処理切り替え
        switch (m_MirrorSystemMode)
        {
            //自身がServerである場合
            case MirrorSystemMode.サーバー:
                //サーバーを停止する
                m_NetworkManager.StopServer();
                break;
            //自身がClientである場合
            case MirrorSystemMode.クライアント:
                //クライアントを停止する
                m_NetworkManager.StopClient();
                break;
            //それ以外の場合は基本エラーである
            default:
                Debug.LogWarning("エラー:本来では実行されない処理が実行された!!");
                break;
        }

        //サーバーが存在するので、ログアウト画面を再表示
        if (!m_LogInWindow.gameObject.activeSelf)
            OnLogInWindows();
        if (m_LogOutWindow.gameObject.activeSelf)
            OnLogOutWindows();
        if (m_NotServerWindow.gameObject.activeSelf)
            OnNotServerWindows();
        //ログアウトしたので、未処理扱いに切り替わる
        m_MirrorSystemMode = MirrorSystemMode.未処理;
    }
    #endregion

    #region ログイン・アウトウィンドゥ処理
    /// <summary>
    /// ログインウィンドゥ処理(反転処理)
    /// </summary>
    public void OnLogInWindows()
    {
        //ログインウィンドゥに干渉
        m_LogInWindow.gameObject.SetActive(!m_LogInWindow.gameObject.activeSelf);
    }
    /// <summary>
    /// ログアウトウィンドゥ処理(反転処理)
    /// </summary>
    public void OnLogOutWindows()
    {
        //ログアウトウィンドゥに干渉
        m_LogOutWindow.gameObject.SetActive(!m_LogOutWindow.gameObject.activeSelf);
    }
    /// <summary>
    /// サーバー未起動警告ウィンドゥ処理(反転処理)
    /// </summary>
    public void OnNotServerWindows()
    {
        //警告ウィンドゥに干渉
        m_NotServerWindow.gameObject.SetActive(!m_NotServerWindow.gameObject.activeSelf);
    }
    #endregion
}
