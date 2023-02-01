using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateralRoot : MonoBehaviour
{
    public Vector2 dir;
    public float speed = 2.0f;
    public bool alive = true;

    // Start is called before the first frame update
    void Start()
    {
        dir = dir.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if(!alive)
        {
            return;
        }
        transform.Translate(dir * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 7)
        {
            //collide with obstacle, 8 is hardcoded layer number
            alive = false;
        }
    }
}
