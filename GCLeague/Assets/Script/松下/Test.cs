using UnityEngine;

/// <summary>
/// シンプルなプレイヤー移動スクリプト（Mirror依存なし）
/// 入力 → Rigidbody移動 → Animator制御 の流れ
/// </summary>
public class PlayerMove : MonoBehaviour
{
    // アニメーション制御用
    public Animator m_Animator;

    // キャラクターの移動速度
    public float m_MoveSpeed = 10f;

    // 最大速度（これ以上速くならないよう制限）
    public float m_MaxSpeed = 5f;

    // 減速にかける時間（未使用だが残してある）
    public float m_DecelerationTime = 0.5f;

    // Rigidbody参照
    private Rigidbody m_Rigidbody;

    // 入力ベクトル（WASDや矢印キー）
    private Vector3 m_InputVector;

    // 移動中かどうかのフラグ
    private bool m_IsMoving = false;

    // アニメーション用の移動スピード値
    private float m_AnimeMoveSpeed = 0;

    // カメラ追従用オブジェクト（シーン内に "カメラリンク" を置く想定）
    private GameObject m_CameraLink;

    void Start()
    {
        // Animatorを取得
        m_Animator = GetComponent<Animator>();

        // Rigidbodyを取得
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        PlayerMoveLogic();
    }

    private void LateUpdate()
    {
        // カメラリンクが未設定なら探す
        if (!m_CameraLink)
            m_CameraLink = GameObject.Find("カメラリンク");
        else
        {
            // プレイヤーに追従させる
            m_CameraLink.transform.position = this.transform.position;

            // マウスのX入力でカメラを回転
            m_CameraLink.transform.GetChild(0).Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
        }
    }

    /// <summary>
    /// 入力を受け取り、移動処理を行う
    /// </summary>
    void PlayerMoveLogic()
    {
        if (!m_CameraLink) return;

        // 入力を取得（WASD / 矢印キー）
        m_InputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // 入力があるかどうか
        m_IsMoving = m_InputVector != Vector3.zero;

        // カメラ基準で移動方向を決定し、回転・移動
        PlayerRotation(
            m_InputVector,
            m_CameraLink.transform.GetChild(0).forward,
            m_CameraLink.transform.GetChild(0).right
        );
    }

    /// <summary>
    /// カメラ方向を基準にキャラクターを回転・移動させる
    /// </summary>
    void PlayerRotation(Vector3 direction, Vector3 CameraForward, Vector3 CameraRight)
    {
        // 入力方向を正規化
        Vector3 MoveDirection = direction.normalized;

        if (MoveDirection.magnitude > 0)
        {
            // カメラの前方ベクトルを水平面に投影
            CameraForward.y = 0; CameraForward.Normalize();

            // カメラの右方向ベクトルを水平面に投影
            CameraRight.y = 0; CameraRight.Normalize();

            // 入力に基づく移動方向を算出
            Vector3 DesiredDirection = CameraForward * direction.z + CameraRight * direction.x;

            // プレイヤーをその方向に回転させる
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(DesiredDirection),
                Time.deltaTime * 10f
            );

            // Rigidbodyに力を加えて移動
            if (DesiredDirection.magnitude > 0)
                m_Rigidbody.AddForce(DesiredDirection * m_MoveSpeed);

            // 平面速度を制限（XZ平面のみ）
            Vector3 flatVelocity = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);
            if (flatVelocity.magnitude > m_MaxSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * m_MaxSpeed;
                m_Rigidbody.velocity = new Vector3(limitedVelocity.x, m_Rigidbody.velocity.y, limitedVelocity.z);
            }
        }

        // アニメーション制御
        //MoveAnimator(direction);
    }

    /// <summary>
    /// アニメーションのSpeedパラメータを更新
    /// </summary>
    void MoveAnimator(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            // 移動中なら徐々にSpeedを上げる
            m_AnimeMoveSpeed += 2 * Time.deltaTime;
            if (m_AnimeMoveSpeed > 1) m_AnimeMoveSpeed = 1;
        }
        else
        {
            // 停止中なら徐々にSpeedを下げる
            m_AnimeMoveSpeed -= 2 * Time.deltaTime;
            if (m_AnimeMoveSpeed <= 0) m_AnimeMoveSpeed = 0;
        }

        // Animatorに反映
        m_Animator.SetFloat("Speed", m_AnimeMoveSpeed);
    }
}
