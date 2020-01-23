using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera cameraDef;
    public GameObject mesh;
    public float speed;
    private float timeRotate = 5;
    private Vector2 stopped;
    private Vector3 rightDef;
    private Vector3 forwardDef;
    // Start is called before the first frame update
    void Start()
    {
        stopped = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
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
            movement += rightDef * (inputAxis.x * speed * Time.deltaTime);
            movement += forwardDef * (inputAxis.y * speed * Time.deltaTime);
        }
        transform.Translate(movement);
        if (movement != Vector3.zero) //Rota el personaje. se rota su mesh, no el objeto en si
        {
            Quaternion lookRotation = Quaternion.LookRotation(movement);
            mesh.transform.rotation = Quaternion.RotateTowards(mesh.transform.rotation, lookRotation, timeRotate);

        }
    }
}
