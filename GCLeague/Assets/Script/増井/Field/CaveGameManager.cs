using UnityEngine;
using System.Collections;

public class CaveGameManager : MonoBehaviour
{
    [Header("洞窟ループ設定")]
    public Transform[] caveSegments;
    public float segmentLength = 50f;
    public float moveSpeed = 5f;

    [Header("レーン設定")]
    public Transform rail1Point;
    public Transform rail2Point;
    public float laneSwitchSpeed = 5f;

    [Header("L/R洞窟プレハブ")]
    public GameObject caveSegmentL;
    public GameObject caveSegmentR;

    [Header("デバッグ設定")]
    public bool isRightAnswer = true;

    private Vector3 targetLaneOffset;
    private bool canInput = true;
    private bool specialCaveSpawned = false;
    private GameObject activeSpecialCave;

    // ★ 追加：初期配置保存用
    private Vector3[] initial_offsets_;

    void Start()
    {
        targetLaneOffset = Vector3.zero;

        // ★ Scene 上の配置をそのまま基準にする
        initial_offsets_ = new Vector3[caveSegments.Length];

        for (int i = 0; i < caveSegments.Length; i++)
        {
            initial_offsets_[i] = caveSegments[i].position;
        }
    }

    void Update()
    {
        MoveCaveSegments();

        if (canInput && !specialCaveSpawned)
            HandleLaneInput();
    }

    // ============================
    // 🚃 洞窟ループ移動処理
    // ============================
    void MoveCaveSegments()
    {
        foreach (Transform segment in caveSegments)
        {
            // 前進（奥から手前へ）
            segment.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

            // レーン横移動
            Vector3 pos = segment.position;
            pos.x = Mathf.Lerp(pos.x, targetLaneOffset.x, Time.deltaTime * laneSwitchSpeed);
            segment.position = pos;

            // ループ判定
            if (segment.position.z < -segmentLength)
            {
                Transform targetHead = GetFarthestHeadPoint();

                segment.SetParent(targetHead);
                segment.localPosition = Vector3.zero;
                segment.localRotation = Quaternion.identity;

                StartCoroutine(DetachNextFrame(segment));
            }
        }

        // 特殊洞窟
        if (activeSpecialCave != null)
        {
            activeSpecialCave.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

            if (activeSpecialCave.transform.position.z < -segmentLength)
            {
                Destroy(activeSpecialCave);
                activeSpecialCave = null;
                canInput = true;
                specialCaveSpawned = false;
            }
        }
    }

    // ============================
    // 🎮 入力処理
    // ============================
    void HandleLaneInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            canInput = false;
            targetLaneOffset = new Vector3(rail1Point.position.x, 0, 0);

            bool isCorrect = !isRightAnswer;
            SpawnSpecialCave(isCorrect ? caveSegmentL : caveSegmentR, "A");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            canInput = false;
            targetLaneOffset = new Vector3(rail2Point.position.x, 0, 0);

            bool isCorrect = isRightAnswer;
            SpawnSpecialCave(isCorrect ? caveSegmentR : caveSegmentL, "S");
        }
    }

    // ============================
    // 🧩 特殊洞窟生成
    // ============================
    void SpawnSpecialCave(GameObject prefab, string inputSource)
    {
        if (specialCaveSpawned)
            return;

        Transform targetHead = GetFarthestHeadPoint();

        activeSpecialCave = Instantiate(prefab, targetHead);
        activeSpecialCave.transform.localPosition = Vector3.zero;
        activeSpecialCave.transform.localRotation = Quaternion.identity;

        StartCoroutine(DetachSpecialNextFrame(activeSpecialCave));

        Vector3 pos = activeSpecialCave.transform.position;
        pos.x = targetLaneOffset.x;
        activeSpecialCave.transform.position = pos;

        specialCaveSpawned = true;
    }

    // ============================
    // 🔍 最奥 headPoint 取得
    // ============================
    Transform GetFarthestHeadPoint()
    {
        Transform farthest = null;
        float maxZ = float.MinValue;

        foreach (Transform seg in caveSegments)
        {
            Transform head = seg.Find("headPoint");
            if (head != null && head.position.z > maxZ)
            {
                maxZ = head.position.z;
                farthest = head;
            }
        }
        return farthest;
    }

    IEnumerator DetachNextFrame(Transform segment)
    {
        yield return null;
        segment.SetParent(null);
    }

    IEnumerator DetachSpecialNextFrame(GameObject obj)
    {
        yield return null;
        if (obj != null)
            obj.transform.SetParent(null);
    }
}
