using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed;
    public Vector2 size;
    public float tapSpeed;
    public float maxSpeed;
    public float jumpForce;
    public float dashSpeed;
    public float dashDelay;
    public Transform groundCheck;
    public GameObject dashPrefab;
    public LayerMask whatIsGround;
    
    Rigidbody2D rb;
    private bool tapA;
    private bool tapD;
    private float dashTime;
    private bool isGrounded;
    private float lastTapTime;
    private float dashDelaySeconds;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.position, size, 0f, whatIsGround);
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), 0);
        rb.AddForce(movement * speed, ForceMode2D.Force);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

        if(dashTime <= 0)
        {
            Vector3 clampVel = rb.velocity;
            clampVel.x = Mathf.Clamp(clampVel.x, -5f, 5);
            rb.velocity = clampVel;
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            if((Time.time - lastTapTime) < tapSpeed && tapA)
            {
                dashTime = 0.7f;
                rb.velocity = Vector2.left * dashSpeed;
            }
            tapA = true;
            tapD = false;
            lastTapTime = Time.time;
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            if((Time.time - lastTapTime) < tapSpeed && tapD)
            {
                dashTime = 0.7f;
                rb.velocity = Vector2.right * dashSpeed;
            }
            tapD = true;
            tapA = false;
            lastTapTime = Time.time;
        }

        if(dashTime > 0)
        {
            dashTime -= Time.deltaTime;
            if(dashDelaySeconds > 0)
            {
                dashDelaySeconds -= Time.deltaTime;
            }else
            {
                GameObject currentDash = Instantiate(dashPrefab, transform.position, transform.rotation);
                dashDelaySeconds = dashDelay;
                Destroy(currentDash, 0.06f);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (isGrounded)
        {
            Gizmos.color = Color.green;
        }else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireCube(groundCheck.position, size);
    }
}
