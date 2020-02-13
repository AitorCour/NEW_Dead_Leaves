using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerController playerController;
    private Gun gun;
    private LookRotation lookRotation;
    private CameraManager cameraManager;

    private bool pointing;
    private bool firstPerson;
    private bool canWalk;
    private bool canShot;

    private float sensitivity = 2.5f;
    public int freeCounter = 50;//si clicka, +=, si pasa el timepo -=
    private float timeCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        gun = playerController.GetComponent<Gun>();
        cameraManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<CameraManager>();
        lookRotation = playerController.GetComponent<LookRotation>();
        pointing = false;
        firstPerson = false;
        canWalk = true;
        canShot = false;
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
        if (Input.GetButtonDown("Jump") && canShot)
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
                canShot = true;
            }
        }
        if(Input.GetButton("Fire2"))
        {
            //Poner primera persona
            if(pointing && !firstPerson)
            {
                Debug.Log("FP");
                cameraManager.FPCamera();
                firstPerson = true;
                canWalk = false;
            }
            /*else if(firstPerson && pointing)
            {
                QuitFP();
            }*/
        }
        else
        {
            if(firstPerson && pointing)
            {
                QuitFP();
            } 
        }
        if(Input.GetButtonDown("ShootButton") && pointing && playerController.canShoot || Input.GetButtonDown("Fire1") && pointing && playerController.canShoot)
        {
            //Dispara
            if (firstPerson)
            {
                gun.fps = true;
                gun.Shot();
                playerController.BaangState();
                Debug.Log("Primera persona");
            }
            else
            {
                gun.fps = false;
                gun.Shot();
                playerController.BaangState();
                Debug.Log("Tercera persona");
            }
        }
        if(playerController.caught)
        {
            if(timeCounter >= 1)
            {
                freeCounter -= 6;
                timeCounter = 0;
            }
            if (Input.GetButtonDown("Fire1"))
            {
                freeCounter += 3;
            }
            if (freeCounter >= 100)
            {
                //free
                freeCounter = 50;
                playerController.SetFree();
                Debug.Log("Freeeeee");
            }
            else if (freeCounter <= 0)
            {
                //dead
                Debug.Log("deadddddd");
            }
            timeCounter += Time.deltaTime;
        }
    }
    void QuitFP()
    {
        Debug.Log("NO FP");
        firstPerson = false;
        cameraManager.FixedActive();
        lookRotation.SetNormalRotation();
        canWalk = true;
        canShot = false;
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
    public void TimerGrab()
    {

    }
}

