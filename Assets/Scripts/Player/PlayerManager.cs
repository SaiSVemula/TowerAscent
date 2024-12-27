using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public Player PlayerInstance;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Create PlayerInstance if not assigned
            if (PlayerInstance == null)
            {
                GameObject playerObject = new GameObject("Player");
                PlayerInstance = playerObject.AddComponent<Player>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
