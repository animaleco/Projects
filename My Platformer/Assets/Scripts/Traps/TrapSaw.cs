using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrapSaw : MonoBehaviour
{

    private SpriteRenderer sr;
    private Animator animator;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float cooldown = 1;
    [SerializeField] private Transform[] wayPoint;

    private Vector3[] wayPointPosition;

    public int wayPointIndex = 1;
    public int moveDirection = 1;
    private bool canMove = true;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();    
    }
    private void Start()
    {
        UpdateWaypointsInfo();
        transform.position = wayPointPosition[0];
    }

    private void UpdateWaypointsInfo()
    {

        /*Esto crea una lista que captura los waypoint del hijo.
         * Y le damos la condición de que si la cantidad de waypoints no es igual al largo de la lista crea un nuevo waypoint.
         *  Con el for recorremos la misma cantida de waypoints haciendo que sea igual a los waypoint de la lista.
         *  Esto basicamente actualiza los waypoints automaticamente en el inspector */
        List <Trap_sawWayPoint> wayPointList = new List<Trap_sawWayPoint>(GetComponentsInChildren<Trap_sawWayPoint>()); 

        if(wayPointList.Count != wayPoint.Length)
        {
            wayPoint = new Transform[wayPointList.Count];

            for(int i = 0; i < wayPointList.Count; i++)
            {
                wayPoint[i] = wayPointList[i].transform;
            }
        }

        wayPointPosition = new Vector3[wayPoint.Length];

        for (int i = 0; i < wayPoint.Length; i++)
        {
            wayPointPosition[i] = wayPoint[i].position;
        }
    }

    private void Update()
    {
        animator.SetBool("active", canMove);

        if (canMove == false)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, wayPointPosition[wayPointIndex], moveSpeed * Time.deltaTime);

        if(Vector2.Distance(transform.position, wayPointPosition[wayPointIndex])< .1f )
        {
            if(wayPointIndex == wayPointPosition.Length - 1 || wayPointIndex == 0)
            {
                moveDirection = moveDirection * - 1;
                StartCoroutine(StopMovement(cooldown));
            }

            wayPointIndex = wayPointIndex + moveDirection;
        }
    }

    private IEnumerator StopMovement(float delay)
    {
        canMove = false;

        yield return new WaitForSeconds(delay);

        canMove = true;
        sr.flipX = !sr.flipX;
    }
}
