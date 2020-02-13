using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public enum State { Idle, Patrol, Chase, Grab, Stun, Hit, Dead };
    public State state;

    protected NavMeshAgent agent;
    protected PlayerController player;
    private CapsuleCollider colliderEnemy;
    private Animator animator;

    public float iniLife;
    protected float life;
    public float maxAngle;
    public float maxRadius;
    private float idleTime = 3.0f;
    public float distancePath;

    private bool isInFov;
    private bool detected;
    public bool changePoint;
    private bool canGrab;

    public GameObject[] path;
    protected Transform[] transformPath;
    private int pathNum;
    private Quaternion tRotation;


    // Start is called before the first frame update
    void Start()
    {
        life = iniLife;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
        path = GameObject.FindGameObjectsWithTag("Path");
        
        isInFov = false;
        detected = false;
        changePoint = false;
        canGrab = true;

        transformPath = new Transform[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            transformPath[i] = path[i].GetComponent<Transform>();
        }

        SetIdle();
    }
    private void OnDrawGizmos() //Dibujar el campo de visión
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (!isInFov)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        //Gizmos.DrawRay(transform.position, (player.transform.position - transform.position).normalized * maxRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    }

    public static bool inFOV(Transform checkingObject, Transform target, float maxAngle, float maxRadius)
    {
        Collider[] overlaps = new Collider[100];
        int count = Physics.OverlapSphereNonAlloc(checkingObject.position, maxRadius, overlaps);

        for (int i = 0; i < count + 1; i++)
        {
            if (overlaps[i] != null)
            {
                if (overlaps[i].transform == target)
                {
                    Vector3 directionBetween = (target.position - checkingObject.position).normalized;
                    directionBetween.y *= 0;

                    float angle = Vector3.Angle(checkingObject.forward, directionBetween);

                    if (angle <= maxAngle)
                    {
                        Ray ray = new Ray(checkingObject.position, target.position - checkingObject.position);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, maxRadius))
                        {
                            if (hit.transform == target)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle:
                IdleUpdate();
                break;
            case State.Patrol:
                PatrolUpdate();
                break;
            case State.Chase:
                ChaseUpdate();
                break;
            case State.Grab:
                GrabUpdate();
                break;
            case State.Stun:
                StunUpdate();
                break;
            /*case State.Hit:
                //HitUpdate();
                break;*/
            default:
                break;
        }
        isInFov = inFOV(transform, player.transform, maxAngle, maxRadius);

        if(isInFov)
        {
            detected = true;
        }
    }
    void IdleUpdate()
    {
        if(detected)
        {
            //Persiguiendo
            //SetChase();
        }
        else
        {
            //StartCoroutine(IdleTime());
        }
    }
    void PatrolUpdate()
    {
        distancePath = Vector3.Distance(path[pathNum].transform.position, transform.position);
        if (detected)
        {
            //Persiguiendo
            SetChase();
        }
        if (distancePath <= 1)
        {
            SetIdle();
            //changePoint = true;
        }
    }
    void ChaseUpdate()
    {
        distancePath = Vector3.Distance(player.transform.position, transform.position);
        agent.SetDestination(player.transform.position - transform.forward);
        if(distancePath <= 1.2 && canGrab && !player.caught)
        {
            SetGrab();
        }
    }
    void GrabUpdate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        tRotation = targetRotation;
        agent.SetDestination(player.transform.position - transform.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, tRotation, 10f);
        if(!player.caught)
        {
            Debug.Log("Not Grabbed");
            SetStunned();
            
        }
    }
    void StunUpdate()
    {
        //agent.SetDestination(player.transform.position - transform.forward * 2);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, tRotation, 10f);
    }
    //a ver... primero los updates, y luego desde esos updates llamar a estos sets
    #region Sets
    void SetIdle()
    {
        //decir que haga animación
        state = State.Idle;
        animator.SetBool("Grabing", false);
        animator.SetBool("Walking", false);
        StartCoroutine(IdleTime());
    }
    void SetPatrol()
    {
        //decir que vaya a los puntos
        state = State.Patrol;
        animator.SetBool("Walking", true);

        if (changePoint)
        {
            GoNearestPath();
            //Debug.Log("newPath");
            changePoint = false;
        }
        if (!changePoint)
        {
            agent.SetDestination(path[pathNum].transform.position);
            //Debug.Log(pathNum);
        }
    }
    void SetChase()
    {
        state = State.Chase;
        animator.SetBool("Walking", true);
    }
    void SetGrab()
    {
        if (!canGrab) return;
        state = State.Grab;
        animator.SetBool("Walking", false);
        animator.SetBool("Grabing", true);
        player.Caught();
        //StartCoroutine(GrabTime());
    }
    void SetStunned()
    {
        state = State.Stun;
        agent.SetDestination(player.transform.position - transform.forward * 2);
        canGrab = false;
        StartCoroutine(StunnedTime());
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, tRotation, 10f);
        animator.SetBool("Grabing", false);
        animator.SetBool("Walking", false);
    }
    #endregion
    private IEnumerator IdleTime()
    {
        yield return new WaitForSeconds(idleTime);
        if(detected)
        {
            SetChase();
        }
        else
        {
            SetPatrol();
            changePoint = true;
        }
    }
    void GoNearestPath()
    {
        int i = Random.Range(0, path.Length);
        pathNum = i;
    }
    private IEnumerator GrabTime()
    {
        yield return new WaitForSeconds(idleTime);
        player.SetFree();
        canGrab = false;
        StartCoroutine(StunnedTime());
        //SetIdle();
        SetStunned();
    }
    
    private IEnumerator StunnedTime()
    {
        yield return new WaitForSeconds(idleTime);
        canGrab = true;
        SetIdle();
    }
}
