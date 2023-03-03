using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Odwolania")]
    Rigidbody rb;
    public GameObject playerObj;

    [Header("Zmienne Ruchu")]
    public float walkSpeed;
    public float runSpeed;
    public float playerHeight;

    public float crouchSpeed;
    public float crouchHeight;
    public float startHeight;
    public Transform crouchCamPlace;
    public Transform camPlace;
    public bool crouching;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDir;

    public Transform orientation;
    public Transform camHolder;

    [Header("Ground Check")]
    public bool grounded;
    public float groundDrag;
    public LayerMask whatIsGround;

    [Header("Keybind")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        startHeight = 1;
    }

    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();

        if(grounded == true)
        {
            rb.drag = groundDrag;
        }

        else
        {
            rb.drag = 0;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(Input.GetKeyDown(crouchKey))
        {
            crouching = true;
            playerHeight = 1;
            camHolder.position = crouchCamPlace.position;
            playerObj.transform.localScale = new Vector3(playerObj.transform.localScale.x, crouchHeight, playerObj.transform.localScale.z);
        }

        else if (Input.GetKeyUp(crouchKey))
        {
            playerHeight = 2;
            crouching = false;
            camHolder.position = camPlace.position;
            playerObj.transform.localScale = new Vector3(playerObj.transform.localScale.x, startHeight, playerObj.transform.localScale.z);
        }
    }

    void MovePlayer()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(grounded && crouching == false)
        {
            rb.AddForce(moveDir.normalized * walkSpeed * 50f, ForceMode.Force);
        }

        else if (grounded && crouching)
        {
            rb.AddForce(moveDir.normalized * crouchSpeed * 50f, ForceMode.Force);
        }

        else
        {
            rb.AddForce(moveDir.normalized * walkSpeed * 50f * airMultiplier, ForceMode.Force);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce * 100, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
    }
}
