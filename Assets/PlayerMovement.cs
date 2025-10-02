using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")] 
    private Rigidbody rb;
    public Transform meshModel;
    
    [Header("Movement")]
    public float moveSpeed = 3;

    private Vector3 movementDirection;
    private Vector3 meshDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        meshDirection = transform.forward;
    }
    
    void Update()
    {
        // Calculate movementDirection using WASD
        movementDirection = (transform.forward * Input.GetAxis("Vertical")) 
                         + (transform.right * Input.GetAxis("Horizontal"));
        
        //movementVector.Normalize();
        movementDirection = Vector3.ClampMagnitude(movementDirection, 1);
        
        // Calculate Mesh Direction
        meshDirection = Vector3.Slerp(meshDirection, movementDirection.magnitude > 0 ? 
                movementDirection : meshDirection, 5 * Time.deltaTime);
        
        meshModel.rotation = Quaternion.LookRotation(meshDirection);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movementDirection * moveSpeed;
    }
}
