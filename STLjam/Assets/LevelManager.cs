using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject player;

    public Level[] levels;

    public int currentLevel = 0;

    private static LevelManager _instance;

    public static LevelManager Instance
    {
        get
        {
            if (!_instance)
                Debug.Log("GameController Error: null instance");
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void startLevel(int n)
    {
        levels[currentLevel].gameObject.SetActive(false);
        levels[n].gameObject.SetActive(true);
        player.transform.position = levels[n].startingPoint.transform.position;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        currentLevel = n;
    }
}
