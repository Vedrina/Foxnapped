using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTest : MonoBehaviour
{
    public float speed;
    public float runSpeed;
    public float jumpHeight;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public float gravity;
    public float rotSpeed;

    public GameObject cam;

    private float rot = 0f;
    private Vector3 moveDir = Vector3.zero;
    private Vector3 inputDir = Vector3.zero;

    private CharacterController controller;
    private Animator anim;
    private bool jumpStart = false;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void FixedUpdate()
    {
        Movement();
        AnimationCheck();
        /*float camY = cam.transform.eulerAngles.y;
        Vector3 rotN = new Vector3(0, camY, 0);
        Quaternion rotationN = Quaternion.Euler(rotN);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationN, 180);*/
    }

    private void LateUpdate()
    {
        Vector3 rotDir = new Vector3(moveDir.x, 0, moveDir.z);
        if(rotDir != Vector3.zero){
            transform.rotation = Quaternion.LookRotation(rotDir);
        }
    }

    private void Movement()
    {
        if (controller.isGrounded)
        {
            moveDir.y = 0f;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            moveDir = new Vector3(0, moveDir.y, 0);
            if (Input.GetKey(KeyCode.W))
            {
                inputDir += cam.transform.forward;
                moveDir += new Vector3(inputDir.x, 0, inputDir.z);
                inputDir = Vector3.zero;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputDir += -cam.transform.right;
                moveDir += new Vector3(inputDir.x, 0, inputDir.z);
                inputDir = Vector3.zero;
            }
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
            {
                inputDir += -cam.transform.forward;
                moveDir += new Vector3(inputDir.x, 0, inputDir.z);
                inputDir = Vector3.zero;
            }
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                inputDir += cam.transform.right;
                moveDir += new Vector3(inputDir.x, 0, inputDir.z);
                inputDir = Vector3.zero;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveDir.x *= runSpeed;
                moveDir.z *= runSpeed;
            }
            else
            {
                moveDir.x *= speed;
                moveDir.z *= speed;
            }
        }
        else
        {
            moveDir = new Vector3(0, moveDir.y, 0);
        }
        if (Input.GetKey(KeyCode.Space) && controller.isGrounded)
        {
            moveDir.y += Mathf.Sqrt(jumpHeight * -2f * -gravity);
        }
        if (moveDir.y < 0)
        {
            moveDir -= Vector3.up * gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (moveDir.y > 0 && !Input.GetButton("Jump"))
        {
            moveDir -= Vector3.up * gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        moveDir.y -= gravity * Time.deltaTime;
        controller.Move(moveDir * Time.deltaTime);
    }

    private void AnimationCheck()
    {
        if (Input.GetKey(KeyCode.W) && controller.isGrounded && !anim.GetBool("jumping") || Input.GetKey(KeyCode.A) && controller.isGrounded && !anim.GetBool("jumping") || Input.GetKey(KeyCode.S) && controller.isGrounded && !anim.GetBool("jumping") || Input.GetKey(KeyCode.D) && controller.isGrounded && !anim.GetBool("jumping"))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                anim.SetInteger("condition", 2);
            }
            else
            {
                anim.SetInteger("condition", 1);
            }
        }
        else if (controller.isGrounded && !anim.GetBool("jumping"))
        {
            anim.SetInteger("condition", 0);
        }
        if (Input.GetKey(KeyCode.Space) && anim.GetBool("jumping") == false)
        {
            anim.SetInteger("condition", 3);
            anim.SetBool("jumping", true);
        }
        if (controller.isGrounded && anim.GetBool("jumping") == true)
        {
            anim.SetInteger("condition", 5);
        }
        if (!controller.isGrounded)
        {
            if (moveDir.y >= 3.5f && !jumpStart)
            {
                jumpStart = true;
            }
            else if (jumpStart && moveDir.y < 3.5f)
            {
                anim.SetInteger("condition", 4);
                jumpStart = false;
            }
        }
        if (anim.GetBool("jumping"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                anim.SetBool("jumping", false);
                anim.SetInteger("condition", 0);
            }
        }
    }
}
