using System;
using UnityEngine;

public class NoteLogic : MonoBehaviour
{
    [Header("Adjustables")]
    public bool isFake = false;

    // References to UI objects for start and end points
    [HideInInspector] public GameObject startPointObject;
    [HideInInspector] public GameObject endPointObject;

    // Duration to reach the target position
    public float duration = 3f;

    // Flag to track movement
    private bool isMoving = true;

    // Reference to the RectTransform of this object
    private RectTransform rectTransform;

    // Internal variables for movement
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float elapsedTime = 0f;

    //REF
    [HideInInspector]
    public SpawnManager spawnManager;
    
    public enum BeatState
    {
        Waiting, // Waiting for input
        Early,
        Great,
        Perfect,
        Late,
        Missed,
        Wrong //Decoy note
    }

    public BeatState currentState = BeatState.Waiting; // Track the current state

    void Awake()
    {
        // Get the RectTransform component
        rectTransform = GetComponent<RectTransform>();

        //fake note
        if(isFake)
        {
            currentState = BeatState.Wrong;
        }
    }

    void Start()
    {
        if (startPointObject != null)
        {
            // Get the start position from the start point object
            startPosition = startPointObject.GetComponent<RectTransform>().anchoredPosition;
            rectTransform.anchoredPosition = startPosition;
        }

        if (endPointObject != null)
        {
            // Get the target position from the end point object
            targetPosition = endPointObject.GetComponent<RectTransform>().anchoredPosition;
        }

        //dynamic duration of time taken for notes
        duration = (float)MusicSync.Instance.noteTravelTime;
    }

    void Update()
    {        
        // Increment elapsed time
        elapsedTime += Time.deltaTime;

        //movement
        if (isMoving)
        {         
            // Calculate the progress percentage
            float progress = elapsedTime / duration;

            // Move the UI element linearly
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, targetPosition, Vector2.Distance(startPosition, targetPosition) / duration * Time.deltaTime);

            // Stop moving when the duration is reached
            if (progress >= 1f || rectTransform.anchoredPosition == targetPosition)
            {
                isMoving = false;
            }
        }

        // Fake vs Real Notes
        if (!isFake)
        {
            // Determine the note state based on elapsed time
            if (elapsedTime <= duration * 1.09f)
            {
                if (elapsedTime >= duration * 0.90f)
                    currentState = BeatState.Perfect;
                else if (elapsedTime >= duration * 0.80f)
                    currentState = BeatState.Great;
                else
                    currentState = BeatState.Early;
            }
            else
            {
                currentState = BeatState.Late;

                if (elapsedTime >= duration * 1.1f)
                {
                    spawnManager.RemoveNoteFromList(this);
                    GameManager.Instance.SpawnScoreAndBeat(4); // Play VFX
                }
            }
        }
        else if (elapsedTime >= duration * 1.1f)
        {
            spawnManager.RemoveNoteFromList(this);
            //GameManager.Instance.SpawnScoreAndBeat(4); // Uncomment if needed
        }
    }


}