using UnityEngine;
using System.Collections;

public class CaveGameManager : MonoBehaviour
{
    [Header("洞窟ループ設定")]
    public Transform[] caveSegments;     // 通常洞窟セグメント
    public float segmentLength = 50f;    // 洞窟1つの長さ
    public float moveSpeed = 5f;         // 洞窟の移動速度

    [Header("レーン設定")]
    public Transform rail1Point;         // 左レーン位置
    public Transform rail2Point;         // 右レーン位置
    public float laneSwitchSpeed = 5f;   // 洞窟の横移動速度

    [Header("L/R洞窟プレハブ")]
    public GameObject caveSegmentL;      // 左分岐プレハブ
    public GameObject caveSegmentR;      // 右分岐プレハブ

    [Header("デバッグ設定")]
    public bool isRightAnswer = true;    // 右が正解ならtrue、左が正解ならfalse

    private Vector3 targetLaneOffset;    // 現在目指すレーン位置（X方向）
    private bool isRightLane = false;    // 現在右レーンにいるか
    private bool canInput = true;        // 入力可能フラグ
    private bool specialCaveSpawned = false; // L/R洞窟を出したかどうか
    private GameObject activeSpecialCave;    // 出現中のL/R洞窟参照

    void Start()
    {
        targetLaneOffset = Vector3.zero;

        // 洞窟の初期配置
        for (int i = 0; i < caveSegments.Length; i++)
        {
            caveSegments[i].position = new Vector3(0, 0, i * segmentLength);
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
            // 奥へ移動
            segment.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

            // X方向のレーン移動
            Vector3 pos = segment.position;
            pos.x = Mathf.Lerp(pos.x, targetLaneOffset.x, Time.deltaTime * laneSwitchSpeed);
            segment.position = pos;

            // Z が一定以下でループ
            if (segment.position.z < -segmentLength)
            {
                Transform targetHead = GetFarthestHeadPoint();

                // 子にする
                segment.SetParent(targetHead);

                // ローカルを 0 に
                segment.localPosition = Vector3.zero;
                segment.localRotation = Quaternion.identity;

                // 1フレーム後に unparent
                StartCoroutine(DetachNextFrame(segment));
            }

        }

        // 特殊洞窟 L/R も流す
        if (activeSpecialCave != null)
        {
            activeSpecialCave.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

            if (activeSpecialCave.transform.position.z < -segmentLength)
            {
                Destroy(activeSpecialCave);
                activeSpecialCave = null;

                canInput = true;
                specialCaveSpawned = false;

                Debug.Log("[Cave] L/R洞窟が通過 → 入力再開");
            }
        }
    }

    // ============================
    // 🎮 入力処理
    // ============================
    void HandleLaneInput()
    {
        // --- 左レーン ---
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("[Input] 左レーン選択");
            canInput = false;
            targetLaneOffset = new Vector3(rail1Point.localPosition.x, 0, 0);

            bool isCorrect = (isRightAnswer == false); // 左が正解なら true

            // 正解ならL、不正解ならR を生成
            SpawnSpecialCave(isCorrect ? caveSegmentL : caveSegmentR, "Aキー押下");
        }

        // --- 右レーン ---
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("[Input] 右レーン選択");
            canInput = false;
            targetLaneOffset = new Vector3(rail2Point.localPosition.x, 0, 0);

            bool isCorrect = (isRightAnswer == true); // 右が正解なら true

            // 正解ならR、不正解ならL を生成
            SpawnSpecialCave(isCorrect ? caveSegmentR : caveSegmentL, "Sキー押下");
        }
    }


    // ============================
    // 🧩 L/R 洞窟生成処理
    // ============================
    void SpawnSpecialCave(GameObject prefab, string inputSource)
    {
        if (specialCaveSpawned)
            return;

        Transform targetHead = GetFarthestHeadPoint();

        // --- ① まず headPoint の子として生成（ズレ防止の基本形）---
        activeSpecialCave = Instantiate(prefab, targetHead);

        // ② localPosition をゼロ化（Z/Y のズレ完全防止）
        activeSpecialCave.transform.localPosition = Vector3.zero;
        //activeSpecialCave.transform.localRotation = Quaternion.identity;

        // ③ 1フレーム後に unparent
        StartCoroutine(DetachSpecialNextFrame(activeSpecialCave));

        // ④ ここが重要：レーン位置（X）を現在の targetLaneOffset に合わせる！
        Vector3 pos = activeSpecialCave.transform.position;
        pos.x = targetLaneOffset.x;  // ← レーン位置を洞窟へ反映
        activeSpecialCave.transform.position = pos;

        specialCaveSpawned = true;

        Debug.Log($"[Cave] {inputSource} → {prefab.name} 生成（正解: {(isRightAnswer ? "右" : "左")}）");
    }



    // ============================
    // 🔍 一番奥にある洞窟の headPoint を取得
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
        // 1フレーム待つ
        yield return null;

        // 親を解除
        segment.SetParent(null);
    }

    IEnumerator DetachSpecialNextFrame(GameObject obj)
    {
        // 1フレーム待つ
        yield return null;

        // 親を外す
        if (obj != null)
            obj.transform.SetParent(null);
    }


}
