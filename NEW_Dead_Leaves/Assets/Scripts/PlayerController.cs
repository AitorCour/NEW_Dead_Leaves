using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameObject cameraDef;
    public GameObject mesh;
    private CameraManager cameraManager;
    private Animator animator;
    public float speed;
    private float timeRotate = 10;
    public float rayDistance;

    private bool canWalk;
    private bool canMoveForward;

    private Vector2 stopped;
    public Vector2 actualVector;
    private Vector2 input;
    private Vector3 rightDef;
    private Vector3 forwardDef;

    public LayerMask ground;
    // Start is called before the first frame update
    void Start()
    {
        cameraManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<CameraManager>();
        animator = GetComponentInChildren<Animator>();
        stopped = new Vector2(0, 0);
        canWalk = true;
        canMoveForward = true;
    }
    private void OnDrawGizmos()
    {
        Vector3 direction = mesh.transform.TransformDirection(Vector3.forward) * rayDistance;
        Gizmos.DrawRay(mesh.transform.position, direction); //forward
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 direction = mesh.transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(mesh.transform.position, direction, rayDistance, ground) && canMoveForward)
        {
            canMoveForward = false;
        }
        if (!Physics.Raycast(mesh.transform.position, direction, rayDistance, ground) && !canMoveForward)
        {
            canMoveForward = true;
        }
    }
    public void Move(Vector2 inputAxis)
    {
        input = inputAxis;
        if (cameraDef == null) return;
        else if(inputAxis == stopped)//stooped es un vector 00
        {
            Vector3 right = cameraDef.transform.right;
            Vector3 forward = Vector3.Cross(right, Vector3.up);
            rightDef = right;
            forwardDef = forward;
            animator.SetBool("Walking", false);
        }
        Vector3 movement = Vector3.zero;
        if(inputAxis != stopped) //cuando el personaje esta en movimiento
        {
            inputAxis = Vector3.ClampMagnitude(inputAxis, 1f);//hace la media para diagonal de speed

            movement += rightDef * (inputAxis.x * Time.deltaTime);
            movement += forwardDef * (inputAxis.y * Time.deltaTime);
            actualVector = inputAxis;
            animator.SetBool("Walking", true);
        }
        if(canMoveForward)
        {
            transform.Translate(movement * speed);
        }
        if (movement != Vector3.zero) //Rota el personaje. se rota su mesh, no el objeto en si
        {
            Quaternion lookRotation = Quaternion.LookRotation(movement);
            mesh.transform.rotation = Quaternion.RotateTowards(mesh.transform.rotation, lookRotation, timeRotate);
        }
    }
    public void ChangeCamera(GameObject newCamera)
    {
        cameraDef = newCamera;
        Debug.Log("CameraChanged");
    }
    public void ShootState()
    {
        animator.SetBool("Pointing", true);
        speed = speed / 2;
    }
    public void NormalState()
    {
        animator.SetBool("Pointing", false);
        speed = speed * 2;
    }
}
