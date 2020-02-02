using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerController playerController;
    private LookRotation lookRotation;
    public GameObject cameraPlayer;
    private bool pointing;
    private bool firstPerson;
    private float sensitivity = 2.5f;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        lookRotation = playerController.GetComponent<LookRotation>();
        //cameraPlayer = GameObject.FindGameObjectWithTag("MainCamera");
        cameraPlayer.SetActive(false);
        pointing = false;
        firstPerson = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputAxis = Vector2.zero;

        inputAxis.x = Input.GetAxisRaw("Horizontal");
        inputAxis.y = Input.GetAxisRaw("Vertical");
        playerController.Move(inputAxis);

        if(Input.GetButtonDown("Jump"))
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
                lookRotation.SetStart();
                Vector2 mouseAxis = Vector2.zero;
                mouseAxis.x = Input.GetAxis("Mouse X") * sensitivity;
                mouseAxis.y = Input.GetAxis("Mouse Y") * sensitivity;
                lookRotation.SetRotation(mouseAxis);
                cameraPlayer.SetActive(true);
                firstPerson = true;
            }
            else if(firstPerson && pointing)
            {
                QuitFP();
            }
        }
    }
    void QuitFP()
    {
        cameraPlayer.SetActive(false);
        Debug.Log("NO FP");
        firstPerson = false;
    }
}

