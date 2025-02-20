using UnityEngine;

public class MusicSync : MonoBehaviour
{
    public AudioSource musicSource; // Audio source playing the song
    public float bpm = 160f;        // Song BPM
    public GameObject noteDummy;    // Reference to the prefab or note object with NoteLogic

    [Header("Monitor This")]
    public int currentNoteType = 0; // Track the type of note being spawned (Normal/Longhold/etc.)

    [Header("Adjustable")]
    public int beatsbeforeSpawn; // How many beats before one note is spawned
    public int fasterBeatsBeforeSpawn = 2; // Faster spawn interval for the second note type (use for faster spawns)
    public float speedDivider = 1; //slow down the note speed

    public enum BeatType
    {
        normal, longhold, dummy
    }

    public BeatType currentState = BeatType.normal; // Track the current state of the beat (normal, longhold, etc.)

    [HideInInspector] public double currentTime;      // Current time of the song
    [HideInInspector] public double nextBeatTime;    // Time for the next normal beat
    [HideInInspector] public double nextBeatTimeB;  
    [HideInInspector] public double nextBeatTimeC;  

    [HideInInspector] public double interval;        // Time interval between normal beats
    [HideInInspector] public double intervalB;   
    [HideInInspector] public double intervalC;       


    private double noteTravelTime = 0;

    private SpawnManager spawnManagerRef;
    private CameraShake cameraShakeRef;
    private Playermovement playerMoveRef;
    private GameManager gameManagerRef;
    private NoteLogic noteLogic;
    private bool hasStarted = false;

    public static MusicSync Instance; 

    void Start()
    {
        // Ensure that there's only one instance of GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }

        // Reference components
        spawnManagerRef = GetComponent<SpawnManager>();
        cameraShakeRef = GetComponent<CameraShake>();
        musicSource = GetComponent<AudioSource>();
        gameManagerRef = GetComponent<GameManager>();   
        playerMoveRef = GameObject.FindWithTag("Player").GetComponent<Playermovement>();
        noteDummy = GameObject.Find("PREFAB_NOTEDUMMY");

        // Access NoteLogic component from the note prefab or object
        if (noteDummy != null)
        {
            noteLogic = noteDummy.GetComponent<NoteLogic>();
            if (noteLogic != null)
            {
                // Get the dynamic travel time (if needed)
            }
            else
            {
                Debug.LogError("NoteLogic component not found on noteDummy!");
            }
        }
        else
        {
            Debug.LogError("NoteDummy object cannot be found! Make sure it is assigned in the inspector.");
        }

        UpdateInterval(beatsbeforeSpawn);

        // Initialize the next beat times for normal and faster beats
        SyncStartTime();
    }

    void Update()
    {
        // Ensure the music is playing before syncing
        if (!hasStarted && musicSource.isPlaying)
        {
            hasStarted = true;
        }

        if (hasStarted)
        {
            currentTime = AudioSettings.dspTime;

            // Spawn normal note if the current time has passed the next normal beat time
            if (currentTime >= nextBeatTime)
            {
                // Spawn the normal beat
                spawnManagerRef.SpawnNextWorldBeat(0);

                // Update the next normal beat time
                nextBeatTime += interval;

                // Camera shake for visual effect
                StartCoroutine(cameraShakeRef.Shake(0.2f, 0.02f));
            }

           
            // Spawn faster note for intervalB (if needed)
            if (currentTime >= nextBeatTimeB)
            {
                // Spawn the faster beat (you could adjust this spawn method to do something different)
                spawnManagerRef.SpawnNextWorldBeat(1);  // This could represent a "faster" beat

                // Update the next faster beat time (intervalB)
                nextBeatTimeB += intervalB;
            }

            
           // Spawn notes based on intervalC (another faster beat or specific timing)
           if (currentTime >= nextBeatTimeC)
           {
                spawnManagerRef.SpawnNote(currentNoteType);

                // Update the next "intervalC" time
                nextBeatTimeC += intervalC;
           }          
        }
    }

    void SyncStartTime()
    {
        // Sync the first beat time based on the current DSP time and interval
        nextBeatTime = AudioSettings.dspTime + (interval * 1f);
        nextBeatTimeB = AudioSettings.dspTime + (interval * 1f);
        nextBeatTimeC = AudioSettings.dspTime + (interval * 1f);
    }

    public void UpdateInterval(int newBeatsValue)
    {
        // Calculate the interval between beats for normal and faster notes
        beatsbeforeSpawn = newBeatsValue;
        interval = (60.0 / bpm) * beatsbeforeSpawn;  // Normal interval for beats
        intervalB = (60.0 / bpm) * beatsbeforeSpawn * 2;
        intervalC = (60.0 / bpm) * beatsbeforeSpawn * 6;
    }
}