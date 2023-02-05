using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    public bool planted;

    public float radius;

    public Rigidbody2D _rb;

    public GameObject growedTree;

    public bool canRetrive;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        radius = gameObject.GetComponent<CircleCollider2D>().radius;
        canRetrive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!planted)
        {
            float distance = radius / 2 + 0.02f;
            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector2.down, distance);
            if (hit && _rb.velocity.magnitude < 0.05f)
            {
                frozeAndGrow();
            }
        }

        if (canRetrive && planted)
        {
            if (Input.GetKey(KeyCode.E))
            {
                GameController.Instance.retriveSeed(this);
            }
        }
    }

    void frozeAndGrow()
    {
        if (planted)
            return;
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        GameController.Instance.seedGrow(this);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //6: layer number of player 
        if (col.gameObject.layer == 6)
        {
            canRetrive = true;
        }
    }
    
    private void OnCollisionExit2D(Collision2D col)
    {
        //6: layer number of player 
        if (col.gameObject.layer == 6)
        {
            canRetrive = false;
        }
    }
}
