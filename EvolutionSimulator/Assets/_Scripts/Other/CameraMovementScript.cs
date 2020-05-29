using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraMovementScript : MonoBehaviour
{
    private Camera camera;

    public bool blockMovement = false;

    [Header("Movement speed")]
    public float normalMovementSpeed = 2;
    public float maxMovementSpeed = 10;
    private float currentMovementSpeed = 2;

    [Header("Rotation speeds")]
    public float normalRotationSpeed = 10;
    public float maxRotationSpeed = 40;
    private float currentRotationSpeed = 10;
    
    public KeyCode speedKey = KeyCode.LeftShift;

    void Start()
    {
        camera = GetComponent<Camera>();
    }


    void Update()
    {
        CheckSpeedKey();
        ControlMovement();
        ControlRotation();
    }

    void CheckSpeedKey()
    {
        if(Input.GetKey(speedKey))
        {
            currentMovementSpeed = maxMovementSpeed;
            currentRotationSpeed = maxRotationSpeed;
        }
        else
        {
            currentMovementSpeed = normalMovementSpeed;
            currentRotationSpeed = normalRotationSpeed;
        }
    }

    void ControlMovement()
    {
        if(blockMovement)
        {
            return;
        }

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) 
        {
            transform.position += currentMovementSpeed * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }
    }

    void ControlRotation()
    {
        if(Input.GetMouseButton(0))
        {
            blockMovement = true;
            Vector3 rotation = transform.eulerAngles;
    
            rotation.x += Input.GetAxis("Vertical") * currentRotationSpeed * Time.deltaTime;
            rotation.y += Input.GetAxis("Horizontal") * currentRotationSpeed * Time.deltaTime; // Standart Left-/Right Arrows and A & D Keys
 
            transform.eulerAngles = rotation;
        }
        else
        {
            blockMovement = false;
        }
    }
}
