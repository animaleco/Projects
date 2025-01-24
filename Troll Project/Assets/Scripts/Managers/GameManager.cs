using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Managers")]
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        CreateManagerIfNeeded();
    }

    private void CreateManagerIfNeeded()
    {
        if (AudioManager.instance == null)
        {
            Instantiate(audioManager);
        }

    }
}
