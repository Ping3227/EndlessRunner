using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public List<GameObject> trackPrefabs;   // 多個跑道Prefab
    public GameObject safeZonePrefab;       // Safe Zone Prefab
    public int trackCount = 3;              // 同時生成的跑道數量
    public float trackLength = 4;           // 每個跑道的長度
    public float resetDistance = 500f;      // 設置一個總移動距離上限，超過後重置
    public float safeZoneLength = 10f;      // Safe Zone 的長度
    public float safeZoneInterval = 50f;    // 每隔多少距離生成一次 Safe Zone

    private List<GameObject> tracks;        // 跑道的實例列表
    private Transform playerTransform;      // 玩家Transform
    private float totalDistanceMoved = 0f;  // 追蹤總移動距離
    [SerializeField] private float deleteDistance = 10f;
    private float distanceSinceLastSafeZone = 0f; // 距離上次 Safe Zone 的距離

    private void Start()
    {
        tracks = new List<GameObject>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        GenerateInitialTracks();
    }

    // 生成初始跑道
    void GenerateInitialTracks()
    {
        // 強制生成第一個跑道為 Safe Zone
        GameObject safeZone = InstantiateSafeZone(0);
        tracks.Add(safeZone);

        // 然後生成剩下的普通跑道
        for (int i = 1; i < trackCount; i++)
        {
            GameObject track = InstantiateRandomTrack(i * trackLength);
            tracks.Add(track);
        }

        // 確保從 Safe Zone 開始後，距離計算從 Safe Zone 之後開始
        distanceSinceLastSafeZone = 0f;  // 剛生成了一個 Safe Zone，所以距離設置為0
    }

    // 每幀檢查是否需要重置跑道或重置場景
    private void Update()
    {
        // 檢查是否需要回收跑道
        if (playerTransform.position.z - deleteDistance > tracks[0].transform.position.z + trackLength)
        {
            RecycleTrack();
        }

        // 如果移動距離超過了設定的距離上限，重置場景
        if (totalDistanceMoved >= resetDistance)
        {
            ResetScene();
        }
    }

    // 重置跑道位置到前方
    void RecycleTrack()
    {
        GameObject oldTrack = tracks[0];
        tracks.RemoveAt(0);

        float newZ = tracks[tracks.Count - 1].transform.position.z + trackLength;

        // 檢查是否需要生成 Safe Zone
        if (distanceSinceLastSafeZone >= safeZoneInterval)
        {
            // 生成 Safe Zone 並重置距離計算
            GameObject safeZone = InstantiateSafeZone(newZ);
            tracks.Add(safeZone);
            distanceSinceLastSafeZone = 0f;

            // 更新新Z位置，因為 Safe Zone 有固定長度
            newZ += safeZoneLength;
        }
        else
        {
            // 隨機生成一個新跑道並重置舊跑道的位置
            GameObject newTrack = InstantiateRandomTrack(newZ);
            tracks.Add(newTrack);
        }

        Destroy(oldTrack); // 銷毀舊的跑道（如果需要保留，可以不銷毀）

        // 每次回收跑道時，更新總移動距離和距離追蹤
        totalDistanceMoved += trackLength;
        distanceSinceLastSafeZone += trackLength;
    }

    private int lastTrackIndex = -1; // 保存上次生成的跑道Prefab索引

// 隨機生成一個跑道Prefab，確保不與上次的Prefab相同
    GameObject InstantiateRandomTrack(float zPosition)
    {
        int randomIndex;

        // 確保新選擇的索引與上次不同
        do
        {
            randomIndex = Random.Range(0, trackPrefabs.Count);
        } while (randomIndex == lastTrackIndex);

        // 更新上次的索引
        lastTrackIndex = randomIndex;

        GameObject randomTrackPrefab = trackPrefabs[randomIndex];

        // 在指定位置生成跑道
        return Instantiate(randomTrackPrefab, new Vector3(0, 0, zPosition), Quaternion.identity);
    }

    // 生成Safe Zone
    GameObject InstantiateSafeZone(float zPosition)
    {
        // 在指定位置生成 Safe Zone
        return Instantiate(safeZonePrefab, new Vector3(0, 0, zPosition), Quaternion.identity);
    }

    // 重置場景，將玩家和跑道位置重置到初始狀態
    void ResetScene()
    {
        // 計算玩家的偏移量
        float offsetZ = playerTransform.position.z;

        // 重置玩家位置
        playerTransform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, 0);

        // 重置所有跑道的位置，保持相對間距不變
        for (int i = 0; i < tracks.Count; i++)
        {
            Vector3 trackPosition = tracks[i].transform.position;
            tracks[i].transform.position = new Vector3(trackPosition.x, trackPosition.y, trackPosition.z - offsetZ);
        }

        // 重置移動距離
        totalDistanceMoved = 0f;
        distanceSinceLastSafeZone = 0f;
    }
}
