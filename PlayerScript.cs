using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [Header("Player variables")]
    [SerializeField] [Range(0, 100)] int health;
    [SerializeField] [Range(0, 100)] int food;

    [Header("UI variables")]
    public GameObject AliveUI;
    public GameObject DeathUI;
    public Slider healthUI;
    public Slider foodUI;


    [Header("GameObject variables")]
    public GameObject clawArea;
    public GameObject screamArea;
    public Transform throwOrigin;
    public GameObject eggPrefab;
    
    [Header("Moving variables")]
    public float moveSpeed;
    public bool canMove;
    public float jumpForce;

    bool moveLeft = false; //This section has variables I added
    bool moveRight = true;
    bool Hit = false;
    bool Dash = false;
    Vector3 moveDir = Vector3.zero;
    Vector3 HitDir = Vector3.zero;
    public float hitSpeed;
    public bool hitMove;
    public bool dashing = false;
    public bool dash = false;
    public float dashSpeed;
    public float dashSlowDown;

    [SerializeField] float timeToMove;
    [SerializeField] bool isGrounded;

    Quaternion lookRight;
    Quaternion lookLeft;

    Animator anim;
    CharacterController controller;
    
    void Start()
    {
        healthUI.value = health;
        foodUI.value = food;

        // SET ROTATION AND GROUND
        isGrounded = true;
        canMove = true;
        lookRight = transform.rotation;
        lookLeft = lookRight * Quaternion.Euler(0, 180, 0);


        // GET RIGIDBODY AND ANIMATOR
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (this.gameObject != null)
        {
            if (canMove) MovePlayer();
            else timeToMove -= Time.deltaTime;
            if (timeToMove <= 0)
            {
                canMove = true;
                timeToMove = 0.7f;
            }

            Abilities();
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        // GET TAG
        string tag = other.gameObject.tag;
        // CHECK TAG
        if (tag == "Projectile" || tag == "Enemy" )
        {
            ModifyHealth(-20);
            Pushback(other.gameObject.transform);
        }
    }

    void MovePlayer()
    {
        // RIGHT
        if (controller.isGrounded)
        {
            if (!hitMove && !Hit)//If the hit check is over and you're still unable to move but you landed
            {
                hitMove = true;//Allow you to move
            }
            moveDir.y = -0.1f;//Apply a small gravity even when grounded just in case
            if (anim.GetBool("InAir"))
            {
                anim.SetBool("InAir", false);
                if (moveRight)
                {
                    transform.rotation = lookRight;
                }
                else if (moveLeft)
                {
                    transform.rotation = lookLeft;
                }
                Vector3 setDir = new Vector3(0, 0, 0);
                controller.Move(setDir);//Due to rotation during the jump the character ends up standing slightly in the air when landing, this is to compensate for it
            }
        }
        else
        {
            moveDir.y -= 9.81f * Time.deltaTime;//If you're not grounded apply normal gravity
        }
        if (hitMove && !dashing)
        {
            moveDir = new Vector3(0, moveDir.y, 0);//If you can move, reset the movement speed
        }
        if (Hit)//If you're hit
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);//Lift the character off the ground slightly
            moveDir.y = Mathf.Sqrt(jumpForce * -2f * -9.81f) / 2;//Apply a force upwards half of the regular jump
            moveDir = new Vector3(0, moveDir.y, HitDir.z * hitSpeed);//Apply a movement opposite of where you got hit, multiplied by hitSpeed
            Hit = false;//Hit check is over
        }
        if (dashing)//Slow down the dash over time
        {
            if (moveDir.z < 0)
            {
                moveDir.z += dashSlowDown;
                if(moveDir.z > 0)
                {
                    moveDir.z = 0;
                }
            }else if(moveDir.z > 0)
            {
                moveDir.z -= dashSlowDown;
                if (moveDir.z < 0)
                {
                    moveDir.z = 0;
                }
            }
            if (moveDir.z == 0)
            {
                dashing = false; //Once dash has stopped, resume normal movement
            }
        }
        if (dash)//At start of dash give the initial speed of the dash
        {
            dash = false;
            dashing = true;//Activate the slowdown of the dash
            if (moveLeft)
            {
                moveDir.z = -dashSpeed;
            }else if (moveRight)
            {
                moveDir.z = dashSpeed;
            }
        }
        if (Input.GetKey(KeyCode.D) && hitMove && !dashing || Input.GetKey(KeyCode.A) && hitMove && !dashing)
        {
            anim.SetBool("Walk", true);
            anim.SetBool("Idle", false);
            if (Input.GetKey(KeyCode.D))
            {
                if (moveLeft)
                {
                    transform.rotation = lookRight;
                    moveLeft = false;
                    moveRight = true;
                }
                moveDir.z += 1 * moveSpeed;
            }else if (Input.GetKey(KeyCode.A))
            {
                if (moveRight)
                {
                    transform.rotation = lookLeft;
                    moveLeft = true;
                    moveRight = false;
                }
                moveDir.z -= 1 * moveSpeed;
            }
        }
        if (controller.isGrounded && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Idle", true);
            if (moveRight)
            {
                transform.rotation = lookRight;
            }else if (moveLeft)
            {
                transform.rotation = lookLeft;
            }
            if (anim.GetBool("InAir") == true)
            {
                Vector3 setDir = new Vector3(0, 0, 0);
                controller.Move(setDir);//Compensating for rotation during jump so the character doesn't end up in air at end
            }
            anim.SetBool("InAir", false);
        }
         
        // JUMP
        if (Input.GetKey(KeyCode.Space) && controller.isGrounded)
        {

            GetComponent<BoxCollider>().enabled = false;
            //Animations and AddForce
            anim.Play("Jump");
            anim.SetBool("InAir", true);
            moveDir.y += Mathf.Sqrt(jumpForce * -2f * -9.81f);//Apply the jump force
            isGrounded = false;
        }
        if (anim.GetBool("InAir"))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, clawArea.transform.rotation, 0.75f * Time.deltaTime);
        }
        if(moveDir != Vector3.zero)
        {
            controller.Move(moveDir * Time.deltaTime);//Move the character for whatever direction it needs to move in
        }
    }

    void Abilities()
    {
        // CLAW
        if (Input.GetKeyDown(KeyCode.J)) 
        {
            if (isGrounded == true)
            {
                clawArea.SetActive(true);
                anim.Play("Claw");
                anim.SetBool("Walk", false);
            }
        }
        // THROW
        if (Input.GetKeyDown(KeyCode.K))
        {
            Instantiate(eggPrefab, throwOrigin.position, throwOrigin.rotation);
            anim.Play("Throw");
            if (isGrounded)
            {
                canMove = false;
                timeToMove = 0.5f;
            }
            anim.SetBool("Walk", false);
        }
        // SCREAM
        if (Input.GetKeyDown(KeyCode.L))
        {
            screamArea.SetActive(true);
            anim.Play("Scream");
            anim.SetBool("Walk", false);
        }
        // DASH
        if (Input.GetKeyDown(KeyCode.LeftShift) && !dashing && !dash)
        {
            anim.Play("Dash");
            anim.SetBool("Walk", false);
            dash = true; //Start the dashing in movement
        }
    } 

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            if (other.gameObject.tag != "Projectile")
            {
                if (moveRight) transform.rotation = lookRight;
                else transform.rotation = lookLeft;
                GetComponent<BoxCollider>().enabled = true;
                isGrounded = true;
                anim.SetBool("InAir", false);
            }
        }
    }

    public void ModifyHealth(int value)
    {
        health += value;
        if (health <= 0) Death();
        if (health >= 100) health = 100;
        healthUI.value = health;
    }

    public void ModifyFood(int value)
    {
        food += value;
        if (food <= 0) Death();
        if (food >= 100) food = 100;
        foodUI.value = food;
    }

    void Death()
    {

        Destroy(this.gameObject);
        AliveUI.SetActive(false);
        DeathUI.SetActive(true);
    }

    public void Pushback(Transform target)
    {
        anim.SetBool("InAir", true);
        HitDir = transform.position - target.position;
       // rb.AddForce(dir * 2f, ForceMode.Impulse);
        hitMove = false; //Block movement when hit
        Hit = true; //Initiate the hit check in movement
    }
}
