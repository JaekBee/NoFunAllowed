using UnityEngine;
using System.Collections;

public class Third_Person_Controller : MonoBehaviour
{
    public AnimationClip idleAnimation;
    public AnimationClip walkAnimation;
    public AnimationClip runAnimation;
    public AnimationClip jumpAnimation;
    public AnimationClip onDeathAnimation;
    public AnimationClip runSprayAnimation;
    public AnimationClip walkSprayAnimation;

    public GameObject waterPrefab;
    public Transform waterSpawn;

    public Camera _camera;
    private CharacterController _characterController;
    private Animation _animation;

    public float speed = 5.0f;
    public float turnSpeed = 3.0f;
    public float walkSpeed = 2.0f;
    public float runSpeed = 15.0f;
    public float gravity = 20.0f;
    public float jumpSpeed = 8.0F;
    public float wallSlide = 1.0f;
    public float timer = 1.0f;

    private bool spraying = false;

    private Vector3 moveDirection = Vector3.zero;

    public enum CharacterState
    {
        Idle = 0,
        Walking = 1,
        Running = 2,
        Spraying = 3,
        Death = 4,
    }

    public enum JumpState
    {
        Jump = 0,
        SingleJumping = 1,
        DoubleJumping = 2,
        TripleJumping = 3,
        SpinJumping = 4,
        SideJumping = 5,
        WallJumping = 6,
        NotJumping = 7,
    }

    public CharacterState _characterState;
    public JumpState _jumpState;

    void Start()
    {
        moveDirection = transform.TransformDirection(Vector3.forward);
        _animation = GetComponent<Animation>();
        _characterState = CharacterState.Idle;
        _characterController = GetComponent<CharacterController>();
        _jumpState = JumpState.NotJumping;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            spray();
        }

        if (Input.GetButton("Run"))
        {
            _characterState = CharacterState.Running;
            if (Input.GetKey("w") && Input.GetKey("d"))
            {
                getRotation(new Vector3(-1f, 0f, -1f));
            } else if (Input.GetKey("w") && Input.GetKey("a"))
            {
                getRotation(new Vector3(1f, 0f, -1f));
            } else if (Input.GetKey("s") && Input.GetKey("d"))
            {
                getRotation(new Vector3(-1f, 0f, 1f));
            } else if (Input.GetKey("s") && Input.GetKey("a"))
            {
                getRotation(new Vector3(1f, 0f, 1f));
            } else if (Input.GetKey("w"))
            {
                getRotation(new Vector3(0f, 0f, -1f));
            } else if (Input.GetKey("s"))
            {
                getRotation(new Vector3(0f, 0f, 1f));
            } else if (Input.GetKey("d"))
            {
                getRotation(new Vector3(-1f, 0f, 0f));
            } else if (Input.GetKey("a"))
            {
                getRotation(new Vector3(1f, 0f, 0f));
            }
        } else if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            _characterState = CharacterState.Walking;
            if (Input.GetKey("w") && Input.GetKey("d"))
            {
                getRotation(new Vector3(-1f, 0f, -1f));
            } else if (Input.GetKey("w") && Input.GetKey("a"))
            {
                getRotation(new Vector3(1f, 0f, -1f));
            } else if (Input.GetKey("s") && Input.GetKey("d"))
            {
                getRotation(new Vector3(-1f, 0f, 1f));
            } else if (Input.GetKey("s") && Input.GetKey("a"))
            {
                getRotation(new Vector3(1f, 0f, 1f));
            } else if (Input.GetKey("w"))
            {
                getRotation(new Vector3(0f, 0f, -1f));
            } else if (Input.GetKey("s"))
            {
                getRotation(new Vector3(0f, 0f, 1f));
            } else if (Input.GetKey("d"))
            {
                getRotation(new Vector3(-1f, 0f, 0f));
            } else if (Input.GetKey("a"))
            {
                getRotation(new Vector3(1f, 0f, 0f));
            }
        }

        if (_characterController.isGrounded)
        {
            //timer -= Time.deltaTime;
            
            //updateJumpState(JumpState.NotJumping); // gotta find a way to wait before setting this
            Vector3 forward = _camera.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;
            Vector3 right = new Vector3(forward.z, 0, -forward.x);
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            moveDirection = (h * right + v * forward);

            if (_characterState == CharacterState.Walking)
            {
                moveDirection *= walkSpeed;
            } else if (_characterState == CharacterState.Running)
            {
                moveDirection *= runSpeed;
            }

            if (Input.GetButtonDown("Jump"))
            {
                updateJumpState(JumpState.Jump);
                moveDirection.y = jumpSpeed;
            }

            if (Input.GetButtonDown("Jump") && Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") > 0)
            {
                updateJumpState(JumpState.SideJumping);
                moveDirection.y = jumpSpeed;
                moveDirection.x = 3.0f * forward.z;
            }
            else if (Input.GetButtonDown("Jump") && Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") < 0)
            {
                updateJumpState(JumpState.SideJumping);
                moveDirection.y = jumpSpeed;
                moveDirection.x = -3.0f * forward.z;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        _characterController.Move(moveDirection * Time.deltaTime);

        if (_animation)
        {
            if (_characterState == CharacterState.Spraying)
            {
                spraying = true;
            } else
            {
                if (_characterController.velocity.sqrMagnitude < 0.1f)
                {
                    if (spraying == false)
                    {
                        _animation.CrossFade(idleAnimation.name);
                    }
                } else
                {
                    if (_characterState == CharacterState.Running)
                    {
                        if (spraying == false)
                        {
                            _animation[runAnimation.name].speed = Mathf.Clamp(_characterController.velocity.magnitude, 0.0f, 1.0f);
                            _animation.CrossFade(runAnimation.name);
                        } else
                        {
                            _animation[runSprayAnimation.name].speed = Mathf.Clamp(_characterController.velocity.magnitude, 0.0f, 1.0f);
                            _animation.CrossFade(runSprayAnimation.name);
                        }
                    } else if (_characterState == CharacterState.Death)
                    {
                        _animation[onDeathAnimation.name].speed = Mathf.Clamp(_characterController.velocity.magnitude, 0.0f, 1.0f);
                        _animation.Play(onDeathAnimation.name);
                    } else if (_characterState == CharacterState.Walking)
                    {
                        if (spraying == false)
                        {
                            _animation[walkAnimation.name].speed = Mathf.Clamp(_characterController.velocity.magnitude, 0.0f, 1.0f);
                            _animation.CrossFade(walkAnimation.name);
                        } else
                        {
                            _animation[walkSprayAnimation.name].speed = Mathf.Clamp(_characterController.velocity.magnitude, 0.0f, 1.0f);
                            _animation.CrossFade(walkSprayAnimation.name);
                        }
                    }
                }
            }
        }
    }

    void spray()
    {

        GameObject water = (GameObject)Instantiate(waterPrefab, waterSpawn.position, waterSpawn.rotation);

        water.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 600));

        Destroy(water, 2);

    }

    void getRotation(Vector3 toRotation)
    {
        Vector3 relativePos = _camera.transform.TransformDirection(toRotation);
        relativePos.y = 0.0f;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
    }

    void updateJumpState(JumpState newJumpState)
    {
        if (newJumpState == JumpState.SpinJumping)
        {
            _jumpState = JumpState.SpinJumping;
            jumpSpeed = 18.0f;
        } else if (newJumpState == JumpState.SideJumping)
        {
            _jumpState = JumpState.SideJumping;
            jumpSpeed = 12.0f;
        } else if (newJumpState == JumpState.WallJumping)
        {
            _jumpState = JumpState.WallJumping;
            jumpSpeed = 12.0f;
        } else if (newJumpState == JumpState.NotJumping)
        {
            _jumpState = JumpState.NotJumping;
            jumpSpeed = 10.0f;
        } else if (newJumpState == JumpState.Jump)
        {
            if(_jumpState == JumpState.NotJumping)
            {
                _jumpState = JumpState.SingleJumping;
                jumpSpeed = 8.0f;
            } else if(_jumpState == JumpState.SingleJumping)
            {
                _jumpState = JumpState.DoubleJumping;
                jumpSpeed = 12.0f;
            } else if (_jumpState == JumpState.DoubleJumping)
            {
                _jumpState = JumpState.TripleJumping;
                jumpSpeed = 18.0f;
            } else if (_jumpState == JumpState.TripleJumping)
            {
                _jumpState = JumpState.SingleJumping;
                jumpSpeed = 8.0f;
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit collider)
    {
        // finds when collider hits a wall while jumping
        if (!_characterController.isGrounded && collider.normal.y < 0.1f)
        {

            if (Input.GetButtonDown("Jump"))
            {
                updateJumpState(JumpState.WallJumping);
                moveDirection.x = collider.normal.x * runSpeed;
                moveDirection.y = jumpSpeed;
                moveDirection.z = collider.normal.z * runSpeed;
            }
        }
    }

	IEnumerator Wait(float waitTime) 
	{
		yield return new WaitForSeconds(waitTime);
	}
}