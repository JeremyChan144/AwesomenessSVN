using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Spawn")]
    public List<GameObject> enemyPrefabList;
    private List<GameObject> enemySpawnPoints = new List<GameObject>();

    [Header("Light Spawn")]
    public List<GameObject> lightPrefabList;
    private List<GameObject> lightSpawnPoints = new List<GameObject>();

    [Header("Beat Spawn")]
    public GameObject beatsPrefab; // Single beat prefab
    private Transform beatSpawnPoint; // Single spawn point for beats 
    private RectTransform canvasTransform; // Reference to the canvas

    [Header("Note Spawn")]
    public List<GameObject> notePrefabList;
    private Transform noteSpawnPoint;
    private Transform noteEndPoint;

    [Header("Beat Spawn")]
    public List<GameObject> beatPrefab;
    private Transform BeatSpawnPoint;

    [Header("World Spawn")]
    public List<GameObject> platformPrefabs;
    private GameObject platformSpawnRef;

    private float spawnOffset = 20f; // Offset to spawn platforms relative to each other.
    private GameObject worldParent; 

    //timers
    private float enemySpawnTimer;
    private float lightSpawnTimer;
    private float notesSpawnTimer;

    //Ref
    private MusicSync musicManager;

    [HideInInspector] // Hide it from the inspector to avoid confusion
    public List<NoteLogic> notesList = new List<NoteLogic>();  // List to track notes
    //[HideInInspector] // Hide it from the inspector to avoid confusion
    public List<GameObject> platformList = new List<GameObject>();  // List to track platforms

    void Start()
    {
        // Ref
        musicManager = GetComponent<MusicSync>();

        // Find all GameObjects with the "SpawnPoint" tag
        enemySpawnPoints.AddRange(GameObject.FindGameObjectsWithTag("SpawnPoint"));

        // Find all GameObjects with the "Lights" tag
        lightSpawnPoints.AddRange(GameObject.FindGameObjectsWithTag("LightSpawnPoint"));

        beatSpawnPoint = GameObject.Find("Move_MusicalBaseRing").transform;
        canvasTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
        noteSpawnPoint = GameObject.Find("NoteSpawnPoint").transform;
        noteEndPoint = GameObject.Find("Move_MusicalBaseRing").transform;
        BeatSpawnPoint = GameObject.Find("WorldBeatSpawnPoint").transform;

        platformSpawnRef = GameObject.Find("Platform_SpawnPointRef");
        worldParent = GameObject.Find("World(0,0,0)");

        //platform related
        GameObject firstPlatform = GameObject.FindWithTag("Platform"); // Assuming the platform has the tag "Platform"
        if(firstPlatform != null)
        {
            platformList.Add(firstPlatform); // Add the first platform to the list.
        }

        // Spawn platform 2 and platform 3
        SpawnInitialPlatforms();
    }

    void Update()
    {
        // SPAWN ENEMIES ---------------------------------------------------------------------
        enemySpawnTimer -= Time.deltaTime;

        if (enemySpawnTimer <= 0)
        {
            SpawnEnemy();
            enemySpawnTimer = Random.Range(1f, 3f); // Randomize the next spawn time
        }

        // SPAWN LIGHTS ---------------------------------------------------------------------
        lightSpawnTimer -= Time.deltaTime;

        if (lightSpawnTimer <= 0)
        {
            SpawnLight();
            lightSpawnTimer = 3f; // Fixed spawn time for lights
        }

        // SPAWN NOTES ---------------------------------------------------------------------
        notesSpawnTimer -= Time.deltaTime;

        if (notesSpawnTimer <= 0)
        {
            //SpawnNote();
            notesSpawnTimer = Random.Range(2f, 4f); // Fixed spawn time for beats
        }
    }

    void SpawnEnemy()
    {
        if (enemySpawnPoints.Count > 0 && enemyPrefabList.Count > 0)
        {
            GameObject spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
            GameObject enemyPrefab = enemyPrefabList[Random.Range(0, enemyPrefabList.Count)];

            Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }

    void SpawnLight()
    {
        if (lightSpawnPoints.Count > 0 && lightPrefabList.Count > 0)
        {
            GameObject spawnPoint = lightSpawnPoints[Random.Range(0, lightSpawnPoints.Count)];
            GameObject lightPrefab = lightPrefabList[Random.Range(0, lightPrefabList.Count)];

            Instantiate(lightPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }

    public void SpawnNote(int type)
    {
        if (canvasTransform == null || notePrefabList == null || noteSpawnPoint == null)
            return; // Exit if references are missing

        GameObject notePrefab = null;

        switch (type)
        {
            case 0: // Basic Note
                notePrefab = notePrefabList[0];
                break;

            case 1: // Decoy Note
                notePrefab = notePrefabList[1];
                break;

            case 2: // Long Press Note
                notePrefab = notePrefabList[2];
                break;

            default:
                Debug.LogWarning($"Invalid note type: {type}");
                return;
        }

        // Instantiate and configure the note
        GameObject note = Instantiate(notePrefab, noteSpawnPoint.position, Quaternion.identity);
        note.transform.SetParent(canvasTransform, false);
        note.transform.localPosition = noteSpawnPoint.localPosition;

        // Assign note logic references
        NoteLogic noteLogic = note.GetComponent<NoteLogic>();
        noteLogic.spawnManager = this;
        noteLogic.startPointObject = noteSpawnPoint.gameObject;
        noteLogic.endPointObject = noteEndPoint.gameObject;

        // Add to list
        notesList.Add(noteLogic);
    }


    // Removes a note from the list and destroys the GameObject
    public void RemoveNoteFromList(NoteLogic noteToRemove)
    {
        notesList.Remove(noteToRemove);  // List auto-shifts after removal
        Destroy(noteToRemove.gameObject); // Destroy the note
    }

    public void DebugNoteList()
    {
        Debug.Log(notesList.Count);
    }


    void SpawnInitialPlatforms()
    {
        if (platformPrefabs.Count > 0)
        {
            // Spawn platform 2 at refZ + 10
            GameObject platform2 = Instantiate(platformPrefabs[Random.Range(0, platformPrefabs.Count)], worldParent.transform);
            platformList.Add(platform2);
            platform2.transform.position = new Vector3(platformSpawnRef.transform.position.x, platformSpawnRef.transform.position.y, platformSpawnRef.transform.position.z + spawnOffset);

            // Spawn platform 3 at refZ + 20
            GameObject platform3 = Instantiate(platformPrefabs[Random.Range(0, platformPrefabs.Count)], worldParent.transform);
            platformList.Add(platform3);
            platform3.transform.position = new Vector3(platform2.transform.position.x, platform2.transform.position.y, platform2.transform.position.z + spawnOffset);

            // Spawn platform 4 at refZ + 30
            GameObject platform4 = Instantiate(platformPrefabs[Random.Range(0, platformPrefabs.Count)], worldParent.transform);
            platformList.Add(platform4);
            platform4.transform.position = new Vector3(platform3.transform.position.x, platform3.transform.position.y, platform3.transform.position.z + spawnOffset);

            // Set the spawn reference to the last spawned platform (platform4)
            platformSpawnRef = platform4;
        }
        else
        {
            Debug.LogWarning("No platform prefabs assigned to the list.");
        }
    }

    void SpawnNextPlatform()
    {
        // Makes sure there are always 4 platforms
        if (platformList.Count < 4)
        {
            // Spawn next platform at the end of the current list
            GameObject nextPlatform = Instantiate(platformPrefabs[Random.Range(0, platformPrefabs.Count)], worldParent.transform);
            platformList.Add(nextPlatform);

            // Position the new platform at the position of the last platform + spawnOffset
            nextPlatform.transform.position = new Vector3(platformSpawnRef.transform.position.x, platformSpawnRef.transform.position.y, platformSpawnRef.transform.position.z + spawnOffset);

            // Update platformSpawnRef to the last spawned platform
            platformSpawnRef = nextPlatform;
        }
    }

    public void SpawnNextWorldBeat(int type)
    {
        switch (type)
        {
            case 0:
                {
                    // Spawn the faster beat
                    Instantiate(beatPrefab[1], BeatSpawnPoint.transform.position, Quaternion.identity);
                    break;
                }
            case 1:
                {
                    // Spawn the normal beat
                    Instantiate(beatPrefab[0], BeatSpawnPoint.transform.position, Quaternion.identity);
                    break;
                }
        }
    }

    public void DestroyPlatform()
    {
        if (platformList.Count > 0)
        {
            // Remove the first platform from the list
            GameObject platformToDelete = platformList[0];
            platformList.RemoveAt(0);

            // Give it delayed destruction
            DestroyAfterSeconds destroyRef = platformToDelete.AddComponent<DestroyAfterSeconds>();
            destroyRef.delayBeforeDeath = 2.0f;

            // Spawn next platform to keep the list at 4
            SpawnNextPlatform();
        }
        else
        {
            Debug.LogWarning("No platforms to destroy.");
        }
    }
}

