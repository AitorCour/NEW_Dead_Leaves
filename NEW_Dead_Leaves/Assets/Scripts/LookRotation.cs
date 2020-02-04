using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookRotation : MonoBehaviour
{
    private Vector2 axisRotation;
    private Transform cameraTransform;
    private Quaternion cameraRot;
    private Transform playerTransform;
    public Transform playerHead;
    public Transform lArm;
    public Transform rArm;
    private Quaternion playerRot;
    private Quaternion lArmRot;
    private Quaternion rArmRot;

    public bool smooth;
    public float smoothTime;

    public bool limitCamerRot;
    public float minAngle = -60;
    public float maxAngle = 60;

    // Use this for initialization
    void Start ()
    {
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        //cameraTransform = Camera.main.transform;
        cameraRot = cameraTransform.localRotation;

        //playerTransform = transform;
        playerTransform = playerHead;
        playerRot = playerTransform.localRotation;
        lArmRot = lArm.localRotation;
        rArmRot = rArm.localRotation;
    }
	// Update is called once per frame
	void Update ()
    {
        //cameraRot *= Quaternion.Euler(-axisRotation.y, 0, 0); //Quaternion: 4 variables, rotación y transformación. Se pone Euler para pasar a grados normales
        //cameraTransform.localRotation = cameraRot;   //la rotación de la camara dependerá del axis rotation.y

        playerRot *= Quaternion.Euler(-axisRotation.y, axisRotation.x, 0);
        lArmRot *= Quaternion.Euler(-axisRotation.y, 0, axisRotation.x);
        rArmRot *= Quaternion.Euler(-axisRotation.y, 0, axisRotation.x);
        //playerTransform.localRotation = playerRot;

        if (limitCamerRot)
        {
            playerRot = ClampRotationAroundXAxis(playerRot);
            playerRot = ClampRotationAroundYAxis(playerRot);
            playerRot = ClampRotationAroundZAxis(playerRot);
            lArmRot = ClampRotationAroundXAxis(lArmRot);
            lArmRot = ClampRotationAroundYAxisArms(lArmRot);
            lArmRot = ClampRotationAroundZAxisArms(lArmRot);
            rArmRot = ClampRotationAroundXAxis(rArmRot);
            rArmRot = ClampRotationAroundYAxisArms(rArmRot);
            rArmRot = ClampRotationAroundZAxisArms(rArmRot);
        }

        if(smooth)
        {
            cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, cameraRot, Time.deltaTime * smoothTime);
            playerTransform.localRotation = Quaternion.Slerp(playerTransform.localRotation, playerRot, Time.deltaTime * smoothTime);
        }
        else
        {
            //cameraTransform.localRotation = cameraRot;
            playerTransform.localRotation = playerRot;
            lArm.localRotation = lArmRot;
            rArm.localRotation = rArmRot;
        }
    }

    public void SetRotation(Vector2 mouseAxis)
    {
        axisRotation = mouseAxis;
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, minAngle, maxAngle);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
    Quaternion ClampRotationAroundYAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);

        angleY = Mathf.Clamp(angleY, minAngle, maxAngle);

        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        return q;
    }
    Quaternion ClampRotationAroundZAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);

        angleZ = Mathf.Clamp(angleZ, 0, 0);

        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return q;
    }
    Quaternion ClampRotationAroundZAxisArms(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);

        angleZ = Mathf.Clamp(angleZ, minAngle, maxAngle);

        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return q;
    }
    Quaternion ClampRotationAroundYAxisArms(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);

        angleY = Mathf.Clamp(angleY, 0, 0);

        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        return q;
    }
    public void SetNormalRotation()
    {
        playerRot = Quaternion.Euler(0, 0, 0);
        lArmRot = Quaternion.Euler(0, 0, 0);
        rArmRot = Quaternion.Euler(0, 0, 0);
        playerTransform.localRotation = playerRot;
        lArm.localRotation = lArmRot;
        rArm.localRotation = rArmRot;
    }
}
