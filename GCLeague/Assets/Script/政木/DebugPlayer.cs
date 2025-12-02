using UnityEngine;

public class DebugPlayer : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 5f;

    private void Update()
    {
        float input = 0f;

        // 左右移動（A / D）
        if (Input.GetKey(KeyCode.A)) input = -1f;
        if (Input.GetKey(KeyCode.D)) input = 1f;

        transform.position += Vector3.right * input * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// プレイヤーのX座標が負なら0、正なら1を返す
    /// </summary>
    public int GetSideValue()
    {
        return transform.position.x >= 0f ? 1 : 0;
    }
}