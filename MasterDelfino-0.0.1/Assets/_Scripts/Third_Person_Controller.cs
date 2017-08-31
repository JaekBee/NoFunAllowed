using UnityEngine;
using System.Collections;

public class Third_Person_Controller : MonoBehaviour 
{
	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;
	public AnimationClip runAnimation;
	public AnimationClip jumpPoseAnimation;
	public AnimationClip onDeathAnimation;
	public AnimationClip runSprayAnimation;
	public AnimationClip walkSprayAnimation;

	public Camera _camera;
	private Animation _animation;

	public float speed = 5.0f;
	public float turnSpeed = 10.0f;

	public float walkSpeed = 2.0f;
	public float runSpeed = 15.0f;
	public float gravity = 20.0f;
	public float jumpSpeed = 8.0F;

	private bool Spraying = false;

	private Vector3 moveDirection = Vector3.zero;

	public enum CharacterState 
	{
		Idle = 0,
		Walking = 1,
		Running = 2,
		Jumping = 3,
		Death = 4,
		Spraying = 5,
	}

	public CharacterState _characterState;

	void Start()
	{
		moveDirection = transform.TransformDirection(Vector3.forward);
		_animation = GetComponent<Animation> ();
		_characterState = CharacterState.Idle;
	}
 	
	void Update()
	{
		CharacterController _characterController = GetComponent<CharacterController> ();
		if (Input.GetKey("f"))
		{
			_characterState = CharacterState.Spraying;
		}
		else if (Input.GetButton ("Run")) 
		{
			_characterState = CharacterState.Running;
			if (Input.GetKey ("w") && Input.GetKey ("d")) 
			{
				getRotation(new Vector3(-1f, 0f, -1f));
			}else if (Input.GetKey ("w") && Input.GetKey ("a")) 
			{
				getRotation(new Vector3(1f, 0f, -1f));
			}else if (Input.GetKey ("s") && Input.GetKey ("d")) 
			{
				getRotation(new Vector3(-1f, 0f, 1f));
			}else if (Input.GetKey ("s") && Input.GetKey ("a")) 
			{
				getRotation(new Vector3(1f, 0f, 1f));
			}else if (Input.GetKey ("w"))
			{
				getRotation(new Vector3(0f, 0f, -1f));
			} else if (Input.GetKey ("s")) 
			{
				getRotation(new Vector3(0f, 0f, 1f));
			} else if (Input.GetKey ("d")) 
			{
				getRotation(new Vector3(-1f, 0f, 0f));
			}else if (Input.GetKey ("a")) 
			{
				getRotation(new Vector3(1f, 0f, 0f));
			}
		} else if(Input.GetKey ("w") || Input.GetKey ("a") || Input.GetKey ("s") || Input.GetKey ("d"))
		{
			_characterState = CharacterState.Walking;
			if (Input.GetKey ("w") && Input.GetKey ("d")) 
			{
				getRotation(new Vector3(-1f, 0f, -1f));
			}else if (Input.GetKey ("w") && Input.GetKey ("a")) 
			{
				getRotation(new Vector3(1f, 0f, -1f));
			}else if (Input.GetKey ("s") && Input.GetKey ("d")) 
			{
				getRotation(new Vector3(-1f, 0f, 1f));
			}else if (Input.GetKey ("s") && Input.GetKey ("a")) 
			{
				getRotation(new Vector3(1f, 0f, 1f));
			}else if (Input.GetKey ("w"))
			{
				getRotation(new Vector3(0f, 0f, -1f));
			} else if (Input.GetKey ("s")) 
			{
				getRotation(new Vector3(0f, 0f, 1f));
			} else if (Input.GetKey ("d")) 
			{
				getRotation(new Vector3(-1f, 0f, 0f));
			}else if (Input.GetKey ("a")) 
			{
				getRotation(new Vector3(1f, 0f, 0f));
			}
		}

		if (_characterController.isGrounded) 
		{
			Vector3 forward = _camera.transform.TransformDirection(Vector3.forward);
			forward.y = 0;
			forward = forward.normalized;
			Vector3 right  = new Vector3(forward.z, 0, -forward.x);
			float h = Input.GetAxis("Horizontal");
			float v =Input.GetAxis("Vertical");
			
			moveDirection  = (h * right  + v * forward);

            if (_characterState == CharacterState.Walking)
			{
				moveDirection *= walkSpeed;
			}else if(_characterState == CharacterState.Running)
			{
				moveDirection *= runSpeed;
			}
			
			if (Input.GetButton ("Jump")) 
			{
				moveDirection.y = jumpSpeed;
			}
		}

		moveDirection.y -= gravity * Time.deltaTime;
		_characterController.Move (moveDirection * Time.deltaTime);

		if (_animation) 
		{
			if (_characterState == CharacterState.Jumping) 
			{
					_animation [jumpPoseAnimation.name].speed = 1.0f;
					_animation [jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade (jumpPoseAnimation.name);
			} else if (_characterState == CharacterState.Spraying) 
			{
				StartCoroutine(Wait(2.0F));
				_characterState = CharacterState.Idle;
				Spraying = true;
			}else
			{
				if (_characterController.velocity.sqrMagnitude < 0.1f) 
				{
					if(Spraying == false)
					{
						_animation.CrossFade (idleAnimation.name);
					}
				} else 
				{
					if (_characterState == CharacterState.Running) 
					{
						if(Spraying == false)
						{
							_animation [runAnimation.name].speed = Mathf.Clamp (_characterController.velocity.magnitude, 0.0f, 1.0f);
							_animation.CrossFade (runAnimation.name);
						}else
						{
							_animation [runSprayAnimation.name].speed = Mathf.Clamp (_characterController.velocity.magnitude, 0.0f, 1.0f);
							_animation.CrossFade (runSprayAnimation.name);
						}
					} else if (_characterState == CharacterState.Death) 
					{
						_animation [onDeathAnimation.name].speed = Mathf.Clamp (_characterController.velocity.magnitude, 0.0f, 1.0f);
						_animation.Play (onDeathAnimation.name);	
					} else if (_characterState == CharacterState.Walking) 
					{
						if(Spraying == false)
						{
							_animation [walkAnimation.name].speed = Mathf.Clamp (_characterController.velocity.magnitude, 0.0f, 1.0f);
							_animation.CrossFade (walkAnimation.name);
						}else
						{
							_animation [walkSprayAnimation.name].speed = Mathf.Clamp (_characterController.velocity.magnitude, 0.0f, 1.0f);
							_animation.CrossFade (walkSprayAnimation.name);
						}
					}
				}
			}
		}
	}
	void getRotation (Vector3 toRotation)
	{
		Vector3 relativePos = _camera.transform.TransformDirection(toRotation);
		relativePos.y = 0.0f;
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
	}
	void OnControllerColliderHit (ControllerColliderHit collider)
	{
		if(collider.gameObject.tag == "PickUp") // test death state
		{
			_characterState = CharacterState.Death;
		}
	}
	IEnumerator Wait(float waitTime) 
	{
		yield return new WaitForSeconds(waitTime);
	}
}