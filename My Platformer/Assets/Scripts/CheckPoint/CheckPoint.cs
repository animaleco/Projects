using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Animator animator => GetComponent<Animator>();
    private bool active;

    [SerializeField] private bool canBeReactivated;

    private void Start()
    {
        canBeReactivated = GameManager.instance.canReactivate;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active)
        {
            return;
        }

        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            ActiveCheckpoint();
        }
    }

    private void ActiveCheckpoint()
    {
        active = true;
        animator.SetTrigger("activate");
        PlayerManager.instance.UpdateRespawnPosition(transform);
    }
}
