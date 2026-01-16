using UnityEngine;
//Mirrorを使用可能とする
using Mirror;
using UnityEngine.UIElements;
using System.Collections;

/// <summary>
/// 簡単なMirror移動処理方法
/// 基本的にプレイヤー側の入力をサーバー側に渡して処理を行います。
/// 1.クライアント(移動量)をサーバーへ転送
/// 2.サーバーが、移動量を受け取り、キャラクターを移動
/// 3.サーバー側から、NetworkTransformReliableを経由して、クライアントのキャラクターを移動
/// 
/// これには理由があり、クライアント側から実装の移動処理を行う場合、クライアントのデータで
/// 動かすと、チートがし放題である為。
/// サーバー側に移動量を渡した際、サーバーが「移動力おかしくね?」と判断できるため、
/// サーバーからクライアントへ移動制御が入っている。
/// </summary>

///NetworkIdentityを追加(通信に必要)
[RequireComponent(typeof(NetworkIdentity))]

///NetworkTransformReliableを追加(座標・向き情報をサーバーからクライアントへ流す)
///こっちの設定を[ServerToClient]にする事
//[RequireComponent(typeof(NetworkTransformReliable))]

///NetworkAnimatorを追加(アニメーション情報をサーバーからクライアントへ流す)
///こっちの設定を[ServerToClient]にする事
//[RequireComponent(typeof(NetworkAnimator))]

public class MirrorPlayerMoves : NetworkBehaviour
{
    [Header("アニメーターリンク")]
    //public Animator m_Animator;
    [Header("キャラクターの移動速度")]
    public float m_MoveSpeed = 10f;
    [Header("キャラクターの最大速度")]
    public float m_MaxSpeed = 5f;
    [Header("移動力減衰値[0.5秒で速度が0になるようにする]")]
    public float m_DecelerationTime = 0.5f;
    [Header("物理"), SerializeField]
    private Rigidbody m_Rigidbody;
    [Header("サーバーへ渡す移動力"), SerializeField]
    private Vector3 m_InputVector;
    [Header("移動フラグ"), SerializeField]
    private bool m_IsMoving = false;
    [Header("移動アニメーションスピード"), SerializeField]
    private float m_AnimeMoveSpeed = 0;

    [Header("外部カメラリンク"), SerializeField]
    private GameObject m_CameraLink;

    [Header("キャラクターの旋回力"), SerializeField]
    private float m_RotationSpeed = 10.0f;

    [Header("[Shadow]新しい位置情報"), SerializeField, SyncVar]
    private Vector3 m_NewPosition;

    [Header("[Shadow]新しい向き情報"), SerializeField, SyncVar]
    private Quaternion m_NewRotation;

    [Header("[Shadow]入力情報"), SerializeField, SyncVar]
    private Vector3 m_NewDirection;

    [Header("外部制御")]
    [SyncVar] public bool AllowMove = true;

    public Transform m_lobbyPos; //ロビーの初期位置
    public Transform m_gamePos;  //ゲームの初期位置

    void Start()
    {
        //物理を取得
        m_Rigidbody = GetComponent<Rigidbody>();
        //NetworkTransformReliableに自身のtransformをリンクさせる
        GetComponent<NetworkTransformReliable>().target = this.transform;

        if (!isLocalPlayer)
        {
            m_Rigidbody.useGravity = false;
        }

        m_lobbyPos = GameObject.Find("LobbyPos").transform;
        m_gamePos = GameObject.Find("GamePos").transform;
    }

    void Update()
    {
        //クライアント/プレイヤーである場合、プレイヤー入力を許可
        if (isLocalPlayer)
        {
            if (!AllowMove) return;
            PlayerMove();
        }
    }

    private void LateUpdate()
    {
        if (!AllowMove) return;

        //クライアント/プレイヤーである場合、プレイヤー入力を許可
        if (isLocalPlayer)
        {
            //カメラリンクがない場合、カメラリンクを探して繋げ、ある場合は、プレイヤーに追従させる。
            if (!m_CameraLink)
                m_CameraLink = GameObject.Find("カメラリンク");
            else
            {
                m_CameraLink.transform.position = this.transform.position;
                m_CameraLink.transform.GetChild(0).Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
            }
        }
        else
        {
            // 他のクライアントのデータを受け取ってキャラクターに適用
            transform.position = Vector3.Lerp(transform.position, m_NewPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, m_NewRotation, Time.deltaTime * 10);
            //移動アニメーション処理
            MoveAnimator(m_NewDirection);
        }
    }
    /// <summary>
    /// クライアント側/プレイヤー側である場合、移動入力を許可し、サーバーへ移動量を渡す
    /// </summary>
    public void PlayerMove()
    {
        if (!m_CameraLink)
            return;

        // 入力を取得
        m_InputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //入力値が0以外であれば、m_IsMoveingをtrueとする。
        m_IsMoving = m_InputVector != Vector3.zero;

        //プレイヤーの向きを再設定する
        PlayerRotation(
            m_InputVector,
            m_CameraLink.transform.GetChild(0).forward,
            m_CameraLink.transform.GetChild(0).right,
            ref m_NewPosition,
            ref m_NewRotation);

        //サーバーへ、移動位置と向き情報を転送
        ServerMove(this.transform.position, m_NewRotation, m_InputVector);
    }
    
    /// <summary>
    /// サーバー側が受け取る処理[移動処理]
    /// </summary>
    /// <param name="direction">移動量</param>
    //↓サーバーへデータ転送するシンボル
    [Command]
    void ServerMove(
            Vector3 N_Position, //プレイヤーの座標
            Quaternion N_Rotation,  //プレイヤーの向き
            Vector3 N_InputVector)  //キー入力
    {
        if (!AllowMove) return;

        //isLocalPlayer=プレイヤーの本体
        //!isLocalPlayer=プレイヤーではない
        //if (!isLocalPlayer)
        //{
        //座標更新
        m_NewPosition = N_Position;
        //向き更新
        m_NewRotation = N_Rotation;

        m_NewDirection = N_InputVector;
    }

    /// <summary>
    /// キャラクターの向きをカメラ向きと、入力から割り出す
    /// </summary>
    public void PlayerRotation(
        Vector3 direction,
        Vector3 CameraForward,
        Vector3 CameraRight,
        ref Vector3 N_Position,
        ref Quaternion N_Rotation)
    {
        // 入力方向のベクトルを生成
        Vector3 MoveDirection = direction.normalized;

        // 入力があれば回転処理を実行
        if (MoveDirection.magnitude > 0)
        {
            // カメラの前方方向から水平面の向きを取得（Y軸は無視）
            CameraForward.y = 0;  // キャラクターのY軸回転に影響しないようにする
            CameraForward.Normalize();

            // カメラの右方向ベクトルを取得
            CameraRight.y = 0; // 同様にY軸の影響を無視
            CameraRight.Normalize();

            // カメラ基準の移動方向を算出
            Vector3 DesiredDirection = CameraForward * direction.z + CameraRight * direction.x;

            // キャラクターの向きを回転させる
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(DesiredDirection), Time.deltaTime * 10f);

            // もし移動入力があれば力を加える
            if (DesiredDirection.magnitude > 0)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(DesiredDirection),
                    Time.deltaTime * 10f
                );

                Vector3 move = DesiredDirection.normalized * m_MoveSpeed;
                m_Rigidbody.velocity = move;

                N_Position = transform.position;
                N_Rotation = transform.rotation;
            }
        }
        else
        {
            m_Rigidbody.velocity = Vector3.zero;

            N_Position = transform.position;
            N_Rotation = transform.rotation;
        }

        //移動アニメーション処理
        MoveAnimator(direction);
    }

    /// <summary>
    /// サーバー側での移動アニメーション処理
    /// </summary>
    /// <param name="direction"></param>
    public void MoveAnimator(Vector3 direction)
    {
        //移動アニメーション実行
        //サーバーが受けたプレイヤーからの移動力が0ではない。
        if (direction != Vector3.zero)
        {
            //移動アニメーションスピード減衰(2倍秒)
            m_AnimeMoveSpeed += 2 * Time.deltaTime;
            if (m_AnimeMoveSpeed > 1) m_AnimeMoveSpeed = 1;
        }
        else
        {
            //移動アニメーションスピード増加(2倍秒)
            m_AnimeMoveSpeed -= 2 * Time.deltaTime;
            if (m_AnimeMoveSpeed <= 0) m_AnimeMoveSpeed = 0;
        }
    }

    [Server]
    public void ServerTeleportToLobby()
    {
        // 入力・補間を止める
        AllowMove = false;

        // Rigidbodyを安全にワープ
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
        m_Rigidbody.position = m_lobbyPos.position;

        // NetworkTransform用の同期変数も更新
        m_NewPosition = m_lobbyPos.position;
        m_NewRotation = transform.rotation;

        Debug.Log("Move Lobby");

        // 1フレーム後に移動再開
        StartCoroutine(EnableMoveNextFrame());
    }

    [Server]
    public void ServerTeleportToGame()
    {
        AllowMove = false;

        //物理を完全停止
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
        m_Rigidbody.isKinematic = true;

        transform.SetPositionAndRotation(
            m_gamePos.position,
            m_gamePos.rotation
        );

        m_NewPosition = m_gamePos.position;
        m_NewRotation = m_gamePos.rotation;
        m_NewDirection = Vector3.zero;

        StartCoroutine(EnableMoveNextFrame());
    }

    [Server]
    private IEnumerator EnableMoveNextFrame()
    {
        yield return null;
        yield return new WaitForFixedUpdate();

        m_Rigidbody.isKinematic = false;
        AllowMove = true;
    }
}
