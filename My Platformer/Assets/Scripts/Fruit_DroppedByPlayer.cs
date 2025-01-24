using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Fruit_DroppedByPlayer : Fruit
{
   [SerializeField] private Vector2 velocity;
   [SerializeField] private Color transparentColor;
   [SerializeField] private float[] waitTime;
    private bool canPickUp;


    protected override void Start()
    {
        base.Start(); 
        StartCoroutine(BlinkCorotuine());
    }

    private void Update()
    {
        transform.position += new Vector3(velocity.x, velocity.y) * Time.deltaTime;  
    }
    private IEnumerator BlinkCorotuine()
    {

        animator.speed = 0;

        foreach(float seconds in waitTime)
        {
            ToggleSpeedAndColor(transparentColor);

            yield return new WaitForSeconds(seconds);

            ToggleSpeedAndColor(Color.white);

            yield return new WaitForSeconds(seconds);
        }

        velocity.x = 0;
        animator.speed = 1;
        canPickUp = true    ;
        
    }

    private void ToggleSpeedAndColor(Color color)
    {
        velocity.x = velocity.x * -1;
        sr.color = color;   
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (canPickUp == false)
            return;

        base.OnTriggerEnter2D(collision);

    }

}
