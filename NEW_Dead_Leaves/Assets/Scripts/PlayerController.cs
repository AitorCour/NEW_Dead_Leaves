using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera cameraDef;
    public GameObject mesh;

    public float speed;
    private float timeRotate = 10;
    public float rayDistance;

    private bool canWalk;
    public bool canMoveForward;
    public bool canMoveBackwards;
    public bool canMoveRight;
    public bool canMoveLeft;

    private Vector2 stopped;
    public Vector2 actualVector;
    private Vector3 rightDef;
    private Vector3 forwardDef;

    public LayerMask ground;
    // Start is called before the first frame update
    void Start()
    {
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
        if (cameraDef == null) return;
        else if(inputAxis == stopped)
        {
            Vector3 right = cameraDef.transform.right;
            Vector3 forward = Vector3.Cross(right, Vector3.up);
            rightDef = right;
            forwardDef = forward;
        }
        Vector3 movement = Vector3.zero;
        if(inputAxis != stopped) //cuando el personaje esta en movimiento
        {
            inputAxis = Vector3.ClampMagnitude(inputAxis, 1f);

            movement += rightDef * (inputAxis.x * Time.deltaTime);
            movement += forwardDef * (inputAxis.y * Time.deltaTime);
            actualVector = inputAxis;
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
}
