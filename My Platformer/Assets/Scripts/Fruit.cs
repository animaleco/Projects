using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FruitType {Apple, Banana, Cherries, Kiwi, Melon, Orange, pineapple, StrawBerry}

public class Fruit : MonoBehaviour
{
    [SerializeField] private FruitType fruitType;
    [SerializeField] private GameObject pickupVfx;

    private GameManager gameManager;
    protected Animator animator;
    protected SpriteRenderer sr;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();       
        sr = GetComponentInChildren<SpriteRenderer>();
    }
    protected virtual void Start()
    {
        gameManager = GameManager.instance;
        SetRandomLookIfNeeded();
    }

    private void SetRandomLookIfNeeded()
    {
        if(gameManager.FruitsHaveRandomLook() == false)
        {
            UpdateFruitVisual();
            return;
        }


        int randomIndex = Random.Range(0, 8);
        animator.SetFloat("fruitindex", randomIndex);
    }

    private void UpdateFruitVisual()
    {
        animator.SetFloat("fruitindex", (int) fruitType); // Esto llama al numero del indice de las frutas en el animator
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        
        if (player != null)
        {
            gameManager.AddFruit();
            AudioManager.instance.PlaySFX(7);
            Destroy(gameObject);

            GameObject newFx = Instantiate(pickupVfx, transform.position, Quaternion.identity);
     
        }

    }
}
