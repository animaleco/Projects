using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertDynamic : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;

    private bool hasTriggered = false;
    private bool isDynamic = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void ConvertBody2D()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

       if(player != null && !hasTriggered)
       {
            hasTriggered = true;
            isDynamic = true;
            ConvertBody2D();
       }
    }

    

}   
