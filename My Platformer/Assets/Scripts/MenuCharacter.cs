using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class MenuCharacter : MonoBehaviour
{
    [SerializeField] private float speed;

    private Vector3 destination;
    private Animator animator;

    private bool isMoving;
    private int facingDirection = 1;
    private bool isFacingRight = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);

            if (Vector2.Distance(transform.position, destination) < .1f)
            {
                isMoving = false;
            }
        }
    }

    public void MoveTo(Transform newDestination)
    {
        destination = newDestination.position;
        destination.y = transform.position.y;

        isMoving = true;
        HandleFlip(destination.x);
    }

    private void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && isFacingRight || xValue > transform.position.x && !isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingDirection = facingDirection * -1;
        transform.Rotate(0, 180, 0);
        isFacingRight = !isFacingRight;

    }
}
