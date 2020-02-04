using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerController playerController;
    private LookRotation lookRotation;
    private CameraManager cameraManager;
    private bool pointing;
    private bool firstPerson;
    private bool canWalk;
    private float sensitivity = 2.5f;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        cameraManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<CameraManager>();
        lookRotation = playerController.GetComponent<LookRotation>();
        pointing = false;
        firstPerson = false;
        canWalk = true;
        HideCursor();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputAxis = Vector2.zero;

        inputAxis.x = Input.GetAxisRaw("Horizontal");
        inputAxis.y = Input.GetAxisRaw("Vertical");
        if(canWalk)
        {
            playerController.Move(inputAxis);
        }
        

        //FP
        Vector2 mouseAxis = Vector2.zero;
        mouseAxis.x = Input.GetAxis("Mouse X") * sensitivity;
        mouseAxis.y = Input.GetAxis("Mouse Y") * sensitivity;
        if (firstPerson)
        {
            lookRotation.SetRotation(mouseAxis);
        }
        //else return;
        //

        if (Input.GetMouseButtonDown(0)) HideCursor();
        else if (Input.GetKeyDown(KeyCode.Escape)) ShowCursor();
        if (Input.GetButtonDown("Jump"))
        {
            if(pointing)
            {
                //Deja de apuntar
                pointing = false;
                playerController.NormalState();
                QuitFP();
            }
            else
            {
                pointing = true;
                playerController.ShootState();

            }
        }
        if(Input.GetButtonDown("FP"))
        {
            //Poner primera persona
            if(pointing && !firstPerson)
            {
                Debug.Log("FP");
                cameraManager.FPCamera();
                firstPerson = true;
                canWalk = false;
            }
            else if(firstPerson && pointing)
            {
                QuitFP();
            }
        }
    }
    void QuitFP()
    {
        Debug.Log("NO FP");
        firstPerson = false;
        cameraManager.FixedActive();
        lookRotation.SetNormalRotation();
        canWalk = true;
    }
    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

