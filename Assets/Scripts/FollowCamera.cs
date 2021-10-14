using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] float turnSpeed;

    Camera cam;

    float minView = 5f;
    float maxView = 20f;
    [SerializeField] float viewInterval;

    Vector3 camPos;

    void Start()
    {
        cam = Camera.main;
        cam.fieldOfView = maxView;

        camPos = target.position + offset;
    }

    private void Update()
    {
        ChangeFieldOfView();
    }

    void LateUpdate()
    {
        Quaternion rotateX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up);
        Quaternion rotateY = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeed, Vector3.left);

        camPos = rotateX * rotateY * camPos;

        transform.position = target.position + camPos;

        transform.LookAt(target);
    }

    void ChangeFieldOfView()
    {
        float wheel = Input.GetAxis("Mouse ScrollWheel");

        if(wheel > 0f)
            cam.fieldOfView -= viewInterval;
        else if(wheel < 0f)
            cam.fieldOfView += viewInterval;

        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minView, maxView);
    }
}
