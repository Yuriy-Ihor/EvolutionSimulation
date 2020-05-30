using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraMovementScript : MonoBehaviour
{
    Camera camera;
    bool blockMovement = false;
    public KeyCode speedKey = KeyCode.LeftShift;

    [Header("Movement speed")]
    public float normalMovementSpeed = 2;
    public float maxMovementSpeed = 10;
    float _currentMovementSpeed
    {
        get
        {
            return Input.GetKey(speedKey) ? maxMovementSpeed : normalMovementSpeed;
        }
    }

    [Header("Rotation speeds")]
    public float normalRotationSpeed = 10;
    public float maxRotationSpeed = 40;
    float _currentRotationSpeed
    {
        get
        {
            return Input.GetKey(speedKey) ? maxRotationSpeed : normalRotationSpeed;
        }
    }

    
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        ControlMovement();
        ControlRotation();
    }

    void ControlMovement()
    {
        if(blockMovement)
        {
            return;
        }

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) 
        {
            Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            Vector3 rotation = transform.rotation.eulerAngles;
            direction = Quaternion.Euler(rotation.x, rotation.y, rotation.z) * direction;
            transform.position += _currentMovementSpeed * direction;
        }
    }

    void ControlRotation()
    {
        if(Input.GetMouseButton(0))
        {
            blockMovement = true;
            Vector3 rotation = transform.eulerAngles;
    
            rotation.x += Input.GetAxis("Vertical") * _currentRotationSpeed * Time.deltaTime;
            rotation.y += Input.GetAxis("Horizontal") * _currentRotationSpeed * Time.deltaTime; // Standart Left-/Right Arrows and A & D Keys
 
            transform.eulerAngles = rotation;
        }
        else
        {
            blockMovement = false;
        }
    }
}
