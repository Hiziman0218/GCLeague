using UnityEngine;
using Mirror;

/// <summary>
/// Mirror 用プレイヤー移動（完成版）
/// ・入力はローカルのみ
/// ・移動と回転は FixedUpdate + Rigidbody
/// ・同期は NetworkTransformReliable に完全委譲
/// </summary>
[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformReliable))]
[RequireComponent(typeof(Rigidbody))]
public class NewMirrorPlayerMoves : NetworkBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("外部制御")]
    [SyncVar] public bool AllowMove = true;

    [Header("カメラリンク")]
    [SerializeField] private GameObject cameraLink;

    Transform cameraYaw;

    private Rigidbody rb;

    // 入力・移動用
    private Vector3 inputVector;
    private Vector3 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Rigidbody 設定（重要）
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.None;
    }

    public override void OnStartLocalPlayer()
    {
        // NetworkTransformReliable に target を設定
        GetComponent<NetworkTransformReliable>().target = transform;

        // カメラリンク取得（必要に応じて変更）
        if (cameraLink == null)
        {
            cameraLink = GameObject.Find("カメラリンク");
            cameraYaw = cameraLink.transform.GetChild(0);
        }
        else
        {
            cameraLink.transform.position = transform.position;
            cameraLink.transform.GetChild(0).Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
        }
    }

    void Update()
    {
        if (!isLocalPlayer || !AllowMove)
        {
            inputVector = Vector3.zero;
            moveDirection = Vector3.zero;
            return;
        }

        // 入力取得
        inputVector = new Vector3(
            Input.GetAxis("Horizontal"),
            0f,
            Input.GetAxis("Vertical")
        );

        // カメラ基準で移動方向を計算
        if (cameraLink != null && inputVector.sqrMagnitude > 0.001f)
        {
            Vector3 camForward = cameraYaw.forward;
            Vector3 camRight = cameraYaw.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            moveDirection =
                camForward * inputVector.z +
                camRight * inputVector.x;

            moveDirection.Normalize();
        }
        else
        {
            // 入力が無い場合は必ず停止
            moveDirection = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer || !AllowMove) return;

        if (moveDirection == Vector3.zero) return;

        // 移動
        Vector3 move =
            moveDirection * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + move);

        // 回転
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            )
        );
    }

    void LateUpdate()
    {
        if (!isLocalPlayer) return;
        if (cameraLink == null) return;

        cameraLink.transform.position = transform.position;

        cameraYaw.Rotate(
            0f,
            Input.GetAxis("Mouse X") * 3f,
            0f
        );
    }

    [Server]
    public void ServerRequestTeleport(Vector3 pos)
    {
        TargetTeleport(connectionToClient, pos);
    }

    [TargetRpc]
    void TargetTeleport(NetworkConnection target, Vector3 pos)
    {
        CmdTeleport(pos);
    }

    [Command]
    void CmdTeleport(Vector3 pos)
    {
        transform.position = pos;
    }
}
