using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")] 
    private Rigidbody rb;
    public Transform meshModel;
    public Animator anim;

    [Header("Movement")] 
    public float playerAcceleration = 5;
    public float currentSpeed = 3;
    public float walkSpeed = 3; 
    public float sprintSpeed = 6;

    public float jumpForce = 4;
    public bool grounded = true;

    private Vector3 movementDirection;
    private Vector3 meshFacingDirection;

    private float zMotion;
    private float xMotion;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        meshFacingDirection = transform.forward;
    }
    
    void Update()
    {
        GroundCheck();
        MovementCalculation();
        Jump();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = (movementDirection * currentSpeed) 
                            + (Vector3.up * rb.linearVelocity.y);
    }

    private void MovementCalculation()
    {
        float xForward = Camera.main.transform.forward.x;
        float zForward = Camera.main.transform.forward.z;

        float xSide = Camera.main.transform.right.x;
        float zSide = Camera.main.transform.right.z;

        float forwardInput = Input.GetAxis("Vertical");
        float sideInput = Input.GetAxis("Horizontal");
        
        bool rightClickPressed = Input.GetMouseButton(1);
        
        //Modifed vector using the cameras direction with no y-axis
        Vector3 flatForward = new Vector3(xForward, 0, zForward).normalized;
        Vector3 flatSide = new Vector3(xSide, 0, zSide).normalized;
        
        
        
        // move in the direction based on where your looking
        movementDirection = (flatForward * forwardInput) + (flatSide * sideInput);
        
        // Alternate to Normilization
        movementDirection = Vector3.ClampMagnitude(movementDirection, 1);
        
        
        // Calculate Mesh Direction
        if (rightClickPressed)
        {
            // Smoothly face direction looking
            meshFacingDirection = Vector3.Slerp(meshFacingDirection, flatForward, 5 * Time.deltaTime);
        }
        else
        {
            // Smoothly face direct of inputs
            meshFacingDirection = Vector3.Slerp(meshFacingDirection, movementDirection.magnitude > 0 ? 
                movementDirection : meshFacingDirection, 5 * Time.deltaTime);
        }
        meshModel.rotation = Quaternion.LookRotation(meshFacingDirection);
        
        Sprinting();
        AnimationTriggers();
    }

    private void GroundCheck()
    {
        // Box Cast for more accuracy
        grounded = Physics.BoxCast(transform.position + Vector3.up, Vector3.one * 0.5f, Vector3.down, meshModel.rotation, 0.7f);
    }

    private void Jump()
    {
        bool pressedJump = Input.GetKey(KeyCode.Space);
        
        if (pressedJump && grounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.y);
        }
    }

    private void Sprinting()
    {
        bool leftShiftPressed = Input.GetKey(KeyCode.LeftShift);
        bool moving = movementDirection.magnitude > 0;
        
        
        if (moving)
        {
            // Change Between Sprint Speed & Walk Speed depending if left shift is pressed, then apply acceleration over time
            currentSpeed = Mathf.MoveTowards(currentSpeed,leftShiftPressed ? sprintSpeed : walkSpeed, playerAcceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = 0;
        }
    }

    public void AnimationTriggers()
    {
        bool playerIsMoving = movementDirection.magnitude > 0;
        bool playerIsSprinting = Input.GetKey(KeyCode.LeftShift);
        bool viewLocked = Input.GetMouseButton(1);
        float verticalInputs = Input.GetAxis("Vertical");
        float horizontalInputs = Input.GetAxis("Horizontal");

        zMotion = Mathf.MoveTowards(zMotion, verticalInputs, 5 * Time.deltaTime);
        xMotion = Mathf.MoveTowards(xMotion, horizontalInputs, 5 * Time.deltaTime);
        
        anim.SetBool("isWalking?", playerIsMoving);
        anim.SetBool("isSprinting?", playerIsSprinting);
        anim.SetBool("Locked?", viewLocked);
        anim.SetFloat("z", zMotion);
        anim.SetFloat("x", xMotion);
    }
}
