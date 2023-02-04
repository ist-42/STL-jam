using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    public float horGroundMaxSpeed = 6.0f;
    public float horGroundAcc = 60.0f;
    public float horGroundSlow = 20.0f;

    //if horizontal spped exceeds this limit on air, the player won't be able to accelerate further
    public float horAirMaxSpeed = 4.0f;
    public float horAirAcc = 6.0f;
    public float horAirSlow = 0.5f;
    

    public float jumpForce = 10.0f;

    public float groundDetectDistance = 0.1f;

    private Rigidbody2D _rb;
    private bool _onGround = false;
    public int _jumpPoint = 1;
    public Animator _animator;
    public SpriteRenderer _sp;
    public CapsuleCollider2D _capsule;
    
    

    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponent<Animator>();
        _sp = gameObject.GetComponent<SpriteRenderer>();
        _capsule = gameObject.GetComponent<CapsuleCollider2D>();
    }
    

    // Update is called once per frame
    void Update()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        

        if (_onGround)
        {
            if (Mathf.Abs(hor) > 0.001f)
            {
                _rb.AddForce(new Vector2(hor, 0.0f) * horGroundAcc * Time.deltaTime, ForceMode2D.Impulse);
            } else if (Mathf.Abs(_rb.velocity.x) > 0.01f)
            {
                _rb.AddForce(new Vector2(1.0f * -Mathf.Sign(_rb.velocity.x), 0.0f) * horGroundSlow * Time.deltaTime, ForceMode2D.Impulse);
            }
        } else
        {
            if (Mathf.Abs(hor) > 0.001f && (Mathf.Abs(_rb.velocity.x) < horAirMaxSpeed || (_rb.velocity.x * hor < 0.001f)))
            {
                _rb.AddForce(new Vector2(hor, 0.0f) * horAirAcc * Time.deltaTime, ForceMode2D.Impulse);
            }
            else if (Mathf.Abs(_rb.velocity.x) > 0.01f)
            {
                _rb.AddForce(new Vector2(1.0f * -Mathf.Sign(_rb.velocity.x), 0.0f) * horAirSlow * Time.deltaTime, ForceMode2D.Impulse);
            }
        }


        //max speed limit
        if (Mathf.Abs(_rb.velocity.x) >= horGroundMaxSpeed)
        {
            _rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * horGroundMaxSpeed, _rb.velocity.y);
        }

        if (!_onGround && Mathf.Abs(_rb.velocity.x) >= horAirMaxSpeed)
        {
            _rb.AddForce(new Vector2(1.0f * -Mathf.Sign(_rb.velocity.x), 0.0f) * horAirSlow * Time.deltaTime, ForceMode2D.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(_jumpPoint <= 0)
            {
                _jumpPoint = 0;
                return;
            }
            _jumpPoint--;
            _rb.AddForce(new Vector2(0.0f, jumpForce), ForceMode2D.Impulse);
        }

        updateOnGround();
        updateFacing();
        updateSeed();
    }

    void updateSeed()
    {
        if (Input.GetKeyDown(KeyCode.K))
        { 
            if(GameController.Instance.currentWorldNum == 0)
                GameController.Instance.createSeed(this);
        }
    }
    void updateOnGround()
    {
        // float distance = 0.1f + _capsule.size.y / 2;
        float distance = 0.1f;
        float width = _capsule.size.x;
        // 10 is the mask number of ground layer
        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector2.down, distance, GameController.Instance.standable);
        //debug
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + new Vector3(0, -distance, 0), Color.red);
        
        // 10 is the mask number of ground layer
        RaycastHit2D hit1 = Physics2D.Raycast(gameObject.transform.position + Vector3.left * (width/2-groundDetectDistance), Vector2.down, distance, GameController.Instance.standable);
        //debug
        Debug.DrawLine(gameObject.transform.position + Vector3.left * (width/2-groundDetectDistance), gameObject.transform.position + Vector3.left * (width/2-groundDetectDistance) + new Vector3(0, -distance, 0), Color.red);
        
        RaycastHit2D hit2 = Physics2D.Raycast(gameObject.transform.position - Vector3.left * (width/2-groundDetectDistance), Vector2.down, distance, GameController.Instance.standable);
        Debug.DrawLine(gameObject.transform.position - Vector3.left * (width/2-groundDetectDistance), gameObject.transform.position - Vector3.left * (width/2-groundDetectDistance) + new Vector3(0, -distance, 0), Color.red);
        
        if (hit.rigidbody || hit1.rigidbody || hit2.rigidbody)
        {
            _onGround = true;
            _jumpPoint = 1;
            
        } else
        {
            _onGround = false;
            _jumpPoint = 0;
        }
        
        
    }

    void updateFacing()
    {
        if (Input.GetAxisRaw("Horizontal") < -0.05f)
        {
            _sp.flipX = true;
        } else if (Input.GetAxisRaw("Horizontal") > 0.05f)
        {
            _sp.flipX = false;
        }
    }


    void FixedUpdate()
    {
        //dead zone
        if (Mathf.Abs(_rb.velocity.x) < .2)
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
        
        _animator.SetBool("on_ground", _onGround);
        _animator.SetFloat("hor_speed_abs", Mathf.Abs(_rb.velocity.x));
        _animator.SetFloat("ver_speed", _rb.velocity.y);
    }
}
