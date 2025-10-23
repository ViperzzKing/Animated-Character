using System;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    [Header("Settings")] 
    public float sensitivity = 120;

    public float minClamp = -90, maxClamp = 90;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime, 0, Space.World);
        transform.Rotate(-Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime, 0, 0, Space.Self);

        float xRotation = transform.eulerAngles.x;
        
        if (xRotation > 180) xRotation -= 360;
        xRotation = Mathf.Clamp(xRotation, minClamp, maxClamp);
        transform.eulerAngles = new Vector3(xRotation, transform.eulerAngles.y, 0);
    }
}
