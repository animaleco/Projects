using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class TriggerTroll : MonoBehaviour
{

    private void RestartScene()
    {
        SceneManager.LoadScene(0);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();

        if (player != null)
        {       
            player.Die();
            RestartScene();
        }
    }


}
