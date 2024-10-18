using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public List<GameObject> trackPrefabs;   
    public GameObject safeZonePrefab;       
    public int trackCount = 3;              
    public float trackLength = 4;           
    public float resetDistance = 500f;      
    public float safeZoneLength = 10f;      
    public float safeZoneInterval = 50f;    

    private List<GameObject> tracks;        
    private Transform playerTransform;      
    private float totalDistanceMoved = 0f;  
    [SerializeField] private float deleteDistance = 10f;
    private float distanceSinceLastSafeZone = 0f; 

    private void Start()
    {
        tracks = new List<GameObject>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        GenerateInitialTracks();
    }


    void GenerateInitialTracks()
    {

        // first track is always a Safe Zone
        GameObject safeZone = InstantiateSafeZone(0);
        tracks.Add(safeZone);


        for (int i = 1; i < trackCount; i++)
        {
            GameObject track = InstantiateRandomTrack(i * trackLength);
            tracks.Add(track);
        }

        distanceSinceLastSafeZone = 0f;
    }

    private void Update()
    {
        
        if (playerTransform.position.z - deleteDistance > tracks[0].transform.position.z + trackLength)
        {
            RecycleTrack();
        }
        
        if (totalDistanceMoved >= resetDistance)
        {
            ResetScene();
        }
    }

    
    void RecycleTrack()
    {
        GameObject oldTrack = tracks[0];
        tracks.RemoveAt(0);
        
        float newZ = tracks[tracks.Count - 1].transform.position.z + trackLength;
        if (distanceSinceLastSafeZone >= safeZoneInterval)
        {
            
            GameObject safeZone = InstantiateSafeZone(newZ );
            tracks.Add(safeZone);
            distanceSinceLastSafeZone -=safeZoneInterval;
            
        }
        else
        {
            GameObject newTrack = InstantiateRandomTrack(newZ);
            tracks.Add(newTrack);
            
        }

        Destroy(oldTrack);
        distanceSinceLastSafeZone += trackLength;
        totalDistanceMoved += trackLength;
        
    }

    private int lastTrackIndex = -1; 


    GameObject InstantiateRandomTrack(float zPosition)
    {
        int randomIndex;

        // select a random track prefab, but make sure it's not the same as the last one
        do
        {
            randomIndex = Random.Range(0, trackPrefabs.Count);
        } while (randomIndex == lastTrackIndex);

        
        lastTrackIndex = randomIndex;

        GameObject randomTrackPrefab = trackPrefabs[randomIndex];

        
        return Instantiate(randomTrackPrefab, new Vector3(0, 0, zPosition), Quaternion.identity);
    }

    
    GameObject InstantiateSafeZone(float zPosition)
    {
        return Instantiate(safeZonePrefab, new Vector3(0, 0, zPosition), Quaternion.identity);
    }

    
    void ResetScene()
    {
        
        float offsetZ = playerTransform.position.z;
        
        playerTransform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, 0);

        
        for (int i = 0; i < tracks.Count; i++)
        {
            Vector3 trackPosition = tracks[i].transform.position;
            tracks[i].transform.position = new Vector3(trackPosition.x, trackPosition.y, trackPosition.z - offsetZ);
        }
        
        totalDistanceMoved -= offsetZ;
        
    }
}
