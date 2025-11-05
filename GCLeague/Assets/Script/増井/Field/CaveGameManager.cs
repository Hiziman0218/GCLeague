using UnityEngine;

public class CaveGameManager : MonoBehaviour
{
    [Header("洞窟ループ設定")]
    public Transform[] caveSegments;
    public float segmentLength = 50f;
    public float moveSpeed = 5f;

    [Header("トロッコ設定")]
    public Transform tram;

    [Header("T字洞窟設定")]
    public GameObject tCavePrefab;
    public float spawnDistance = 50f;

    private GameObject currentTCave;
    private TCaveController currentTCaveController;
    private bool waitingForInput = true;
    private Transform hiddenSegment;
    private float loopStartZ = 0f;  // ループ再開時の基準Z

    void Start()
    {
        for (int i = 0; i < caveSegments.Length; i++)
        {
            caveSegments[i].position = new Vector3(0, 0, i * segmentLength);
        }
    }

    void Update()
    {
        // 通常洞窟ループの実行条件
        bool allowLoop = currentTCave == null ||
                         (currentTCaveController != null && !currentTCaveController.IsTurning());

        if (allowLoop)
        {
            MoveCaveSegments();
        }

        // 入力受付
        if (waitingForInput)
        {
            if (Input.GetKeyDown(KeyCode.A))
                SpawnTCave(true);
            else if (Input.GetKeyDown(KeyCode.S))
                SpawnTCave(false);
        }

        // T字洞窟回転終了判定
        if (currentTCaveController != null && !currentTCaveController.IsTurning() && !waitingForInput)
        {
            waitingForInput = true;

            // 回転終了時に基準Zを更新
            loopStartZ = GetMaxSegmentZ();

            currentTCave = null;
            currentTCaveController = null;

            if (hiddenSegment != null)
            {
                hiddenSegment.gameObject.SetActive(true);
                hiddenSegment = null;
            }

            Debug.Log("[Update] T字洞窟終了後の maxZ: " + loopStartZ);
        }
    }

    void MoveCaveSegments()
    {
        // T字洞窟回転中は完全に停止
        if (currentTCave != null && currentTCaveController != null && currentTCaveController.IsTurning())
        {
            // T字洞窟自身は回転だけさせる（必要なら回転中前進もここで処理）
            currentTCave.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.Self);
            return;
        }

        // 通常洞窟移動
        foreach (Transform segment in caveSegments)
        {
            segment.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

            if (segment.position.z < -segmentLength)
            {
                // 回転が終わった後に loopStartZ を参照して再配置
                float maxZ = Mathf.Max(GetMaxSegmentZ(), loopStartZ);
                segment.position = new Vector3(0, 0, maxZ + segmentLength);
            }
        }
    }

    void SpawnTCave(bool isRight)
    {
        if (currentTCave != null) return;

        Transform frontSegment = GetFrontSegment();
        Transform tPoint = frontSegment.Find("TPoint");
        if (tPoint == null)
        {
            Debug.LogWarning("TPoint not found in " + frontSegment.name);
            return;
        }

        currentTCave = Instantiate(tCavePrefab, tPoint.position, tPoint.rotation);
        currentTCave.transform.SetParent(tPoint);

        Rigidbody rb = currentTCave.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        currentTCaveController = currentTCave.GetComponent<TCaveController>();
        if (currentTCaveController != null)
            currentTCaveController.SetTurnDirection(isRight);

        hiddenSegment = frontSegment;
        hiddenSegment.gameObject.SetActive(false);

        waitingForInput = false;

        Debug.Log($"[SpawnTCave] {currentTCave.name} を {frontSegment.name}/TPoint に親子付けしました");
    }

    Transform GetFrontSegment()
    {
        Transform front = caveSegments[0];
        float maxZ = front.position.z;

        foreach (Transform seg in caveSegments)
        {
            if (seg.position.z > maxZ)
            {
                front = seg;
                maxZ = seg.position.z;
            }
        }
        return front;
    }

    float GetMaxSegmentZ()
    {
        float maxZ = float.MinValue;

        foreach (Transform segment in caveSegments)
        {
            if (segment.position.z > maxZ)
                maxZ = segment.position.z;
        }

        if (currentTCave != null && currentTCave.transform.position.z > maxZ)
            maxZ = currentTCave.transform.position.z;

        return maxZ;
    }
}
