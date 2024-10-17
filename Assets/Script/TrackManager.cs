using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public List<GameObject> trackPrefabs;   // 多個跑道Prefab
    public int trackCount = 3;              // 同時生成的跑道數量
    public float trackLength = 4;           // 每個跑道的長度
    public float resetDistance = 500f;      // 設置一個總移動距離上限，超過後重置

    private List<GameObject> tracks;        // 跑道的實例列表
    private Transform playerTransform;      // 玩家Transform
    private float totalDistanceMoved = 0f;  // 追蹤總移動距離
    
    private void Start()
    {
        tracks = new List<GameObject>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        GenerateInitialTracks();
    }

    // 生成初始跑道
    void GenerateInitialTracks()
    {
        for (int i = 0; i < trackCount; i++)
        {
            GameObject track = InstantiateRandomTrack(i * trackLength);
            tracks.Add(track);
        }
    }

    // 每幀檢查是否需要重置跑道或重置場景
    private void Update()
    {
        // 檢查是否需要回收跑道
        if (playerTransform.position.z > tracks[0].transform.position.z + trackLength)
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

        // 隨機生成一個新跑道並重置舊跑道的位置
        GameObject newTrack = InstantiateRandomTrack(newZ);
        tracks.Add(newTrack);

        Destroy(oldTrack); // 銷毀舊的跑道（如果需要保留，可以不銷毀）

        // 每次回收跑道時，更新總移動距離
        totalDistanceMoved += trackLength;
    }

    // 隨機生成一個跑道Prefab
    GameObject InstantiateRandomTrack(float zPosition)
    {
        // 隨機選擇一個跑道Prefab
        int randomIndex = Random.Range(0, trackPrefabs.Count);
        GameObject randomTrackPrefab = trackPrefabs[randomIndex];

        // 在指定位置生成跑道
        return Instantiate(randomTrackPrefab, new Vector3(0, 0, zPosition), Quaternion.identity);
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
    }
}
