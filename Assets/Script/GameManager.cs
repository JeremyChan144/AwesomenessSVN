using System.Collections.Generic;
using System.Linq;
using Unity.Notifications.iOS;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static NoteLogic;

public class GameManager : MonoBehaviour
{
    //easy ref
    public static GameManager Instance;
    private MusicSync musicManager;

    private SpawnManager spawnManager; // Reference to the SpawnManager

    //Hitmarkers
    private Transform spawnPoint;
    private RectTransform canvasTransform; // Reference to the canvas
    private ScoreUpdate scoreScript;
    private GameObject player;
    private Playermovement playerMovement;

    //hitmarkers2
    private GameObject leftArrowSpawn;
    private GameObject rightArrowSpawn;

    [Header("Please assign value")]
    public int perfectScore;
    public int greatScore;
    public int earlyScore;
    public int lateScore;
    [HideInInspector] public int currentScore;

    [Header("Please Assign")]
    public GameObject UI_Perfect;
    public GameObject UI_Great;
    public GameObject UI_Early;
    public GameObject UI_Miss;
    public GameObject UI_BeatPerfect;
    public GameObject UI_BeatGreat;
    public GameObject UI_Beat_Early;
    private void Awake()
    {
        // Ensure that there's only one instance of GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Optional: Keep this across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        spawnManager = GetComponent<SpawnManager>();
        musicManager = GetComponent<MusicSync>();

        //Ref
        canvasTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
        spawnPoint = GameObject.Find("NoteSpawnPoint").transform;
        scoreScript = GameObject.Find("Score&ComboUI").GetComponent<ScoreUpdate>();

        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<Playermovement>();

        //More Ref
        leftArrowSpawn = GameObject.Find("Move_LeftArrow");
        rightArrowSpawn  = GameObject.Find("Move_RightArrow");
    }

    void Update()
    {
        // Check for spacebar press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckFirstNote();
        }

        //Cheats
        Cheats();
    }

    void CheckFirstNote()
    {
        // Ensure the list is not empty
        if (spawnManager.notesList.Count > 0)
        {
            NoteLogic firstNote = spawnManager.notesList[0];

            // Check if the note state is Perfect
            if (firstNote.currentState == BeatState.Perfect)
            {
                spawnManager.RemoveNoteFromList(firstNote); // Remove and destroy

                //VFX
                SpawnScoreAndBeat(1);

                //shoot
                playerMovement.Shoot(2000);
            }
            else if (firstNote.currentState == BeatState.Great)
            {
                spawnManager.RemoveNoteFromList(firstNote);

                //VFX
                SpawnScoreAndBeat(2);

                //shoot
                playerMovement.Shoot(1000);
            }
            else if(firstNote.currentState == BeatState.Early)
            {
                spawnManager.RemoveNoteFromList(firstNote);

                //VFX
                SpawnScoreAndBeat(3);

                //shoot
                playerMovement.Shoot(500);
            }
            else if (firstNote.currentState == BeatState.Late)
            {
                spawnManager.RemoveNoteFromList(firstNote);

                //VFX
                SpawnScoreAndBeat(4);
            }

            else if(firstNote.currentState == BeatState.Wrong)
            {
                spawnManager.RemoveNoteFromList(firstNote);

                //VFX
                SpawnScoreAndBeat(4);
            }
        }
        else
        {
            Debug.Log("No notes available to hit!");
        }
    }


    public void SpawnScoreAndBeat(int itemNo)
    {
        switch (itemNo)
        {
            //perfect
            case 1:
                {
                    //Spawn Scoremarker
                    GameObject perfectUI = Instantiate(UI_Perfect, spawnPoint);
                    perfectUI.transform.SetParent(canvasTransform, false);

                    //spawn the beat pulse (VFX)
                    GameObject perfectPulse = Instantiate(UI_BeatPerfect, spawnPoint);
                    perfectPulse.transform.SetParent(canvasTransform, false);

                    //add points
                    scoreScript.IncreaseScore(perfectScore);
                    break;
                }

            //great
            case 2:
                {
                    //Spawn Scoremarker
                    GameObject greatUI = Instantiate(UI_Great, spawnPoint);
                    greatUI.transform.SetParent(canvasTransform, false);

                    //spawn the beat pulse (VFX)
                    GameObject greatPulse = Instantiate(UI_BeatGreat, spawnPoint);
                    greatPulse.transform.SetParent(canvasTransform, false);

                    //add points
                    scoreScript.IncreaseScore(greatScore);
                    break;
                }

            //early
            case 3:
                {
                    //Spawn Scoremarker
                    GameObject earlyUI = Instantiate(UI_Early, spawnPoint);
                    earlyUI.transform.SetParent(canvasTransform, false);

                    //add points
                    scoreScript.IncreaseScore(earlyScore);
                    break;
                }

            //miss
            case 4: 
                {
                    //Spawn Scoremarker
                    GameObject missUI = Instantiate(UI_Miss, spawnPoint);
                    missUI.transform.SetParent(canvasTransform, false);

                    //add points
                    scoreScript.IncreaseScore(lateScore);
                    break;                
                }                    
        }
    }

    public void Cheats()
    {    
        //Change note type
        if(Input.GetKeyDown(KeyCode.Z))
        {
            musicManager.currentNoteType = 0;
        }

        else if(Input.GetKeyDown(KeyCode.X))
        {
            musicManager.currentNoteType = 1;
        }

        else if (Input.GetKeyDown(KeyCode.C))
        {
            musicManager.currentNoteType = 2;
        }

        else if(Input.GetKeyDown(KeyCode.V))
        {
            foreach (NoteLogic note in spawnManager.notesList)
            {
                string allNoteNames = string.Join(" ", spawnManager.notesList.Select(note => note.name));
                Debug.Log(allNoteNames);
            }
        }
    }
}