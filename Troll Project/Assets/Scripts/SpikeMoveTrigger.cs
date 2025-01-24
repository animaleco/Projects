using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpikeMoveTrigger : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float cooldown = 1;
    [SerializeField] private Transform[] wayPoint;

    public int wayPointIndex = 1;
    public int moveDirection = 1;
    private bool isMoving = false;
    private bool hasTriggered = false;

    private Vector3[] wayPointPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        UpdateWaypointsInfo();
    }

    private void Update()
    {
        if (isMoving)
        {
            UpdateMove();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null && !hasTriggered)
        {
            hasTriggered = true;
            isMoving = true;

        }
    }

    private void UpdateWaypointsInfo()
    {

        /*Esto crea una lista que captura los waypoint del hijo.
         * Y le damos la condición de que si la cantidad de waypoints no es igual al largo de la lista crea un nuevo waypoint.
         *  Con el for recorremos la misma cantida de waypoints haciendo que sea igual a los waypoint de la lista.
         *  Esto basicamente actualiza los waypoints automaticamente en el inspector */
        List<SpikewayPoint> wayPointList = new List<SpikewayPoint>(GetComponentsInChildren<SpikewayPoint>());

        if (wayPointList.Count != wayPoint.Length)
        {
            wayPoint = new Transform[wayPointList.Count];

            for (int i = 0; i < wayPointList.Count; i++)
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
    private void UpdateMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, wayPointPosition[wayPointIndex], moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, wayPointPosition[wayPointIndex]) < 0.1f)
        {
            if (wayPointIndex == wayPointPosition.Length - 1)
            {
                StartCoroutine(ReturnToStart());
                
            }
            else
            {
                wayPointIndex++;
            }

            
        }
    }


    private IEnumerator ReturnToStart()
    {
        isMoving = false;
        yield return new WaitForSeconds(cooldown);

        while (wayPointIndex > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, wayPointPosition[wayPointIndex - 1], moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, wayPointPosition[wayPointIndex - 1]) < 0.1f)
            {
                wayPointIndex--;
            }
            yield return null;
        }

        
        isMoving = false;

    }  
}