
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]//Player Variables
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;
    public float Sensitivity = 1f;
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;
    public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]//For Grounded
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;

    [Header("Cinemachine")]//Cinemachine Camera
    public GameObject CinemachineCameraTarget;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;
    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    //Sounds
    public AudioSource audioSource;
    public AudioClip DeathAudioClip, ShootAudioClip, DamageAudioClip, WeaponEquipedClip;
    public AudioClip LandingAudioClip;
    public AudioClip FootstepAudioClip;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    // animation IDs
    private int _animIDSpeed, _animIDGrounded, _animIDJump;
    private int _animIDFreeFall, _animIDMotionSpeed;
    private int _animItemPickUp, _animWeaponAim, _animWeaponReload, _animWeaponShoot;
    private int _animTakeDamage, _animDeath;

    private Animator _animator;
    private CharacterController _controller;
    private PlayerStats playerStats;
    private PlayerInput _input;
    public CompassBar _compassBar;
    private GameObject _mainCamera;
    private const float _threshold = 0.01f;

    private bool _hasAnimator;
    public bool _isInteracting;
    public bool isDead;

    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        _isInteracting = false;
        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
        _compassBar = GetComponentInChildren<CompassBar>();
        playerStats = GetComponent<PlayerStats>();

        AssignAnimationIDs();

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
        if(_isInteracting)
            return;

        _hasAnimator = TryGetComponent(out _animator);//Get the animator Controller and comfirm there is an ani,ator
        
        JumpAndGravity();//Check Jump and Falls
        GroundedCheck();//Check when Player is Grounded
        Move();// Check for Player Movement
    }

    private void LateUpdate()
    {
        CameraRotation();//Control Player Rotation...
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        _animItemPickUp = Animator.StringToHash("PickUp");
        _animWeaponReload = Animator.StringToHash("Reload");
        _animWeaponShoot = Animator.StringToHash("Shoot");

        _animTakeDamage = Animator.StringToHash("TakeDamage");
        _animDeath = Animator.StringToHash("Dead");
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }

    private void CameraRotation()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        SetDirection(mouseWorldPosition);
        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            _cinemachineTargetYaw += _input.look.x * Time.deltaTime * Sensitivity;
            _cinemachineTargetPitch += _input.look.y * Time.deltaTime * Sensitivity;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = (_input.sprint && playerStats.energy > 0 && !_input.aim) ? SprintSpeed : MoveSpeed;
        
        if(targetSpeed == SprintSpeed && -_input.move.magnitude != 0) playerStats.energy -= 5f * Time.deltaTime;//Consume Player Energy
        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
        }
        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        // move the player
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    void SetDirection(Vector3 worldDir)
    {
        Vector3 worldAimTarget = worldDir;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                
                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;
            playerStats.energy -= 10f * Time.deltaTime;//Consume Player Energy
            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;
        
        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }

    public void SetSensitivity(float newSensitivity) 
    {
        Sensitivity = newSensitivity;//Sensitivity changes on aim and out of aim Input
    }

    public void OnFootstep()//Play Foot SoundFX
    {
        audioSource.PlayOneShot(FootstepAudioClip);
    }

    public void AnimShoot()//Play Shoot Animation and SoundFX
    {
        ShootSound();
        _animator.SetTrigger(_animWeaponShoot);
    }

    public void AnimReload()//Play Reload Anim
    {
        _animator.SetTrigger(_animWeaponReload);
    }

    private void OnLand()//Play Land SoundFX
    {
        audioSource.PlayOneShot(LandingAudioClip);
    }

    public void ShootSound()//Play Shoot SoundFX
    {
        audioSource.PlayOneShot(ShootAudioClip);
    }

    public void DeathSound()//Play Death SoundFX
    {
        audioSource.PlayOneShot(DeathAudioClip);
    }

    public void TakeDamageSound()//Play Take Damage Soundfx
    {
        audioSource.PlayOneShot(DamageAudioClip);
    }

    public void WeaponEquipedSound()//Play Weapon Equiped SoundFX 
    {
        audioSource.PlayOneShot(WeaponEquipedClip);
    }

    public void StopInteraction()//Stop All Movements And Animations
    {
        _isInteracting = true;
    }

    public void ResetInteraction()//Reset All Movements And Animations
    {
        _isInteracting = false;
    }

    public void AnimPickUp()//Play Pick Up Animation
    {
        _animator.SetTrigger(_animItemPickUp);
    }

    public void Die()//On Player Die
    {
        isDead = true;//Set Die to true
        _animator.SetTrigger(_animDeath);// Play Death Animation
        _isInteracting = true;//Is Interacting So player Doesn't Move on Death
        MenuManager.Instance.InMenu();
        MenuManager.Instance.OpenMenu("deathScreen");//Show Death Screen
        DeathSound();
        StopInteraction();
    }

    public void DeathScreen()
    {
        MenuManager.Instance.Pause();//Show Death Screen On DEATH
    }

    public void TakeDamage()//Make Player Take Damage
    {
        TakeDamageSound();//Play Damage Sounds
        _animator.SetTrigger(_animTakeDamage);//Play Damage Anim
    }
    
    void OnTriggerEnter(Collider other)
    {
        if(_isInteracting)
            return;
        if(other.gameObject.tag == "Collectibles")//Check To Pick Collectibles
        {
            Inventory.Instance.canReceiveInput = true;
            if(other.gameObject.GetComponent<InventoryItem>() != null)
            {
                Inventory.Instance.indicator.SetActive(true);//Show Pick Up Indicator
                Inventory.Instance.currentPickableItem = other.gameObject.GetComponent<InventoryItem>();
            }
        }

        if(other.gameObject.tag == "RootsTrigger")
        {
            if(other.gameObject.GetComponent<EnergyGlowTrigger>() != null)
            {
                if(!FindObjectOfType<FirstPersonShooterController>().radioActiveGO.activeSelf)
                    Inventory.Instance.needRadioActiveGO.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)//Reset All Collectibles and Inventory Variabes on Exit
    {
        if(other.gameObject.tag == "Collectibles")
        {
            Inventory.Instance.canReceiveInput = false;
            Inventory.Instance.currentPickableItem = null;
            Inventory.Instance.indicator.SetActive(false);
        }
        if(other.gameObject.tag == "RootsTrigger")
        {
            Inventory.Instance.canReceiveInput = false;
            Inventory.Instance.needRadioActiveGO.SetActive(false);
            Inventory.Instance.indicator.SetActive(false);
        }
    }
}