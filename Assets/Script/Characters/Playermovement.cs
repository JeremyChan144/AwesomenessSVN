using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Playermovement : MonoBehaviour
{
    [Header("Adjustable - Horizontal")]
    public float moveSpeed = 5f;
    public float laneWidth = 3f; // Distance between lanes
    public int numberOfLanes = 5; // MUST BE ODD NUMBER
    private float moveCoolDown = 0.15f;
    private float currentMoveCoolDown = 0.15f;

//ref
    private MusicSync musicSyncRef; // Store reference to MusicSync

    [Header("Adjustable - Vertical")]
    public float jumpForce = 10;
    public float extraGravity = 3;

    [Header("Buttons")]
    public List<Animator> buttons;
    private int buttonState = 0;

    private float rayDistance = 1.0f;

    public bool isGrounded = false; // Declare the variable at the top
    private bool canJump = true; // Cooldown flag
    public float jumpCooldown = 0.2f; // Cooldown duration in seconds
    private float cooldownTimer = 0f; // Timer to track cooldown time

    private Vector3 startPos;
    private float targetX;
    private float currentX;
    private float minX, maxX;

    private Rigidbody rb;

    [Header("Shooter - Assign")]
    public GameObject bulletPrefab;
    private Transform bulletSpawnPoint;

    void Start()
    {
        //load
        startPos = transform.position;
        targetX = startPos.x;
        bulletSpawnPoint = GameObject.Find("BulletSpawnPoint").transform;

        //load refs
        musicSyncRef = GameObject.FindWithTag("MainCamera").GetComponent<MusicSync>();

        // Calculate the minimum and maximum allowed X positions based on the number of lanes
        minX = startPos.x - ((numberOfLanes - 1) / 2f) * laneWidth;
        maxX = startPos.x + ((numberOfLanes - 1) / 2f) * laneWidth;

        //jumping
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Extra gravity
        rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);

        // Ground detection logic (raycast)
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.6f, LayerMask.GetMask("Ground"));

        // Auto Jump when not grounded and cooldown allows
        if (!isGrounded && canJump)
        {
            Jump();
            canJump = false;  // Start cooldown
            cooldownTimer = jumpCooldown;
        }

        if (isGrounded && !canJump)
        {
            canJump = true;
            currentMoveCoolDown = 0;
        }
    }

    private void Update()
    {
        //move
        Movement();
    }

    void Movement()
    {
        // Only allow movement if the cooldown is finished
        if (currentMoveCoolDown <= 0)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                targetX -= laneWidth;
                buttonState = 0;
                StartCoroutine(ChangeButtonState(buttonState));

                // Reset the movement cooldown
                if(isGrounded)
                {
                    currentMoveCoolDown = moveCoolDown;
                }

                else
                {
                    currentMoveCoolDown = moveCoolDown * 10f;
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                targetX += laneWidth;
                buttonState = 2;
                StartCoroutine(ChangeButtonState(buttonState));

                // Reset the movement cooldown
                if (isGrounded)
                {
                    currentMoveCoolDown = moveCoolDown;
                }

                else 
                {
                    currentMoveCoolDown = moveCoolDown * 10f;
                }
            }
        }
        else
        {
            // Decrease the cooldown timer
            currentMoveCoolDown -= Time.deltaTime;
        }

        targetX = Mathf.Clamp(targetX, minX, maxX);
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), Time.deltaTime * moveSpeed);
    }

    public void Shoot(int speed)
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform.position,Quaternion.identity);

            // Get the Rigidbody component of the bullet
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

            // Apply forward force to the bullet
            if (bulletRigidbody != null)
            {
                bulletRigidbody.AddForce(bulletSpawnPoint.transform.forward * speed, ForceMode.Impulse);
            }

            //adjust button accordingly
            StartCoroutine(ChangeButtonState(1));
        }
    }

    public void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset Y velocity to avoid double jumping
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply jump force
    }

    IEnumerator ChangeButtonState(int number)
    {
        buttons[number].SetInteger("State", 1);
        yield return new WaitForSeconds(0.1f);
        buttons[number].SetInteger("State", 0);
    }
}
