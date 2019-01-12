using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class New : MonoBehaviour
{
    //normal state
    [SerializeField] private float _acceleration, _drag, _normalRotateSpeed, _walkSpeed;
    [SerializeField] private Vector2 _clampLimetCam;
    [SerializeField] private Transform _ikTargetLeftHand, _ikTargetRightHand;
    private Vector3 Velocity = Vector3.zero, InputMovement;
    private float _clampCamInput, _maxWalkXZVel, _inputLeftRight, _rotationSpeed, _inputForBack;
    private Transform _absoluteTransform, _cam, _ik;
    private CharacterController _playerControll;
    private Animator _anim;
    private IKControllerPlayer _ikPlayer;

    //Running state
    [SerializeField] private float _runSpeed;
    private bool _isRunning = false, _playerIsMoving = false;

    //Moving box state
    [SerializeField] private float _movingBoxSpeed, _boxRayRange, _grabRotateSpeed;
    private bool _movingABox = false;
    private Transform _boxRayStart;
    private MovebleBox _targetBox;
    private float _boxDistance;

    //Looking over shoulder state
    [SerializeField] private Vector2 _LookShoulderLimit;
    private float _clampLookShoulder;
    private bool _lookingOverSchoulder = false;

    //gun vars
    [SerializeField] private float _gunDamage, _gunRange, _reloadTime;
    [SerializeField] private int _maxBullets, _currentBullets;
    [SerializeField] private Transform _gunBarrel;
    private float _reloadTimer;

    //button vars
    [SerializeField] private float  _buttonRayRange;
    private bool  IsButtonPressed = false;
    private Transform  _buttonRayStart;

    //getting shanked vars
    private bool _stabbed = false;
    private int _playerLives = 100;
    private float _shokTimer = 0;

    //main voids
    void Start()
    {
        //components
        _playerControll = GetComponent<CharacterController>();
        _absoluteTransform = _playerControll.transform;

        //components from child objects
        _anim = transform.GetChild(0).GetComponent<Animator>();
        _cam = transform.GetChild(1);
        _boxRayStart = transform.GetChild(2);
        _ik = transform.GetChild(3);
        _buttonRayStart = transform.GetChild(4);

        //getting behaviour from the animator
        _ikPlayer = _anim.GetBehaviour<IKControllerPlayer>();

        //to lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

#if DEBUG
        //debug stuf
        Assert.IsNotNull(_playerControll, "The Player doesn't contain a CharacterController");
        Assert.IsNotNull(_anim, "The player doesn't contain a animator");
        Assert.IsNotNull(_ikPlayer, "The Animator on the player doesn't contain script IKControllerPlayer");
#endif

    }

    void Update()
    {
        //input movement
        InputMovement = new Vector3(_inputLeftRight, 0, _inputForBack).normalized;

        //to rotate and clamp the cam
        CamRotation();
        
        //Inverse Kinematics
        _ikPlayer.IkTargetLeftHand = _ikTargetLeftHand;
        _ikPlayer.IkTargetRightHand = _ikTargetRightHand;

        //Animations
        Animations();

        //walking
        NormalState();

        //RunningMode
        IsMoving();
        RunningState();

        //BoxMode
        MovingBoxState();

        //LookOverShoulderMode
        LookingOverShoulderState();

        //Pressing button
        PressingButtonState();

        //Gun
        GunState();

        //getting stabbed yeet
        PlayerGotStabbed();

        //player dies
        PlayerDies();

        //debug
        DebugState();
        
    }
    
    void FixedUpdate()
    {
        //physics
        ApplyGround();
        ApplyGravity();
        CalculatingMovement();
        LimitXZVelocity();
        ApplyDragOnGround();
        DoMovement();

    }

    //voids in normal update
    void CamRotation()
    {
        _clampCamInput += -Input.GetAxis("Mouse Y") * _rotationSpeed * Time.deltaTime;
        _clampCamInput = Mathf.Clamp(_clampCamInput, _clampLimetCam.x, _clampLimetCam.y);
        if (!_lookingOverSchoulder)
        {
            _absoluteTransform.Rotate(0, Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime, 0);
        }
    }

    void Animations()
    {
        Vector3 XZvel = Vector3.Scale(Velocity, new Vector3(1, 0, 1));
        Vector3 localVelXZ = gameObject.transform.InverseTransformDirection(XZvel);
        _anim.SetFloat("VerticalInput", localVelXZ.z / _maxWalkXZVel);
        _anim.SetFloat("HorizontalInput", localVelXZ.x / _maxWalkXZVel);
        _anim.SetBool("Run", _isRunning);
        _anim.SetBool("MoveBox", _movingABox);
        _anim.SetBool("PressedButton", IsButtonPressed);
    }

    void NormalState()
    {
        if (!_isRunning && !_movingABox && !_lookingOverSchoulder && !IsButtonPressed && !_stabbed)
        {
            //Debug.Log("Normal state");
            _maxWalkXZVel = (_walkSpeed * 1000) / (60 * 60);
            _inputLeftRight = Input.GetAxisRaw("Horizontal");
            _inputForBack = Input.GetAxisRaw("Vertical");
            _rotationSpeed = _normalRotateSpeed;
            _cam.eulerAngles = new Vector3(_clampCamInput, _cam.eulerAngles.y, 0);
            _ik.eulerAngles = _cam.eulerAngles;
        }
    }

    void IsMoving()
    {
        if (Input.GetAxis("Horizontal") > 0.3 || Input.GetAxis("Vertical") > 0.3 || Input.GetAxis("Horizontal") < -0.3 || Input.GetAxis("Vertical") < -0.3)
        {
            _playerIsMoving = true;
        }
        else
        {
            _playerIsMoving = false;
        }
    }

    void RunningState()
    {
        if (Input.GetButton("Run") && !_movingABox && _playerIsMoving)
        {
            //Debug.Log("Running state");
            _isRunning = true;
            _maxWalkXZVel = (_runSpeed * 1000) / (60 * 60);
            _cam.eulerAngles = new Vector3(_clampCamInput, _cam.eulerAngles.y, 0);

        }
        else
        {
            _isRunning = false;
        }
    }

    void MovingBoxState()
    {
        if (Input.GetButtonDown("Grab"))
        {
            GrabBox();
        }
        if (Input.GetButtonDown("Drop") && !_lookingOverSchoulder && _movingABox)
        {
            DropBox();
        }
        if (_movingABox && _boxDistance > 1.4f && _boxDistance < 11.8f)
        {
            //Debug.Log("box is out of range");
            DropBox();
        }
        if (_movingABox)
        {
            //Debug.Log("Moving Box state");
            _inputLeftRight = 0;
            _maxWalkXZVel = (_movingBoxSpeed * 1000) / (60 * 60);
            _rotationSpeed = _grabRotateSpeed;
            _inputForBack = Input.GetAxisRaw("Vertical");
            _cam.transform.rotation = Quaternion.Lerp(_cam.transform.rotation, _absoluteTransform.transform.rotation, Time.deltaTime * 10f);
            _boxDistance = Vector3.Distance(_absoluteTransform.position, _targetBox.transform.position);
        }
    }

    void LookingOverShoulderState()
    {
        if (Input.GetButton("LookBack") && _movingABox)
        {
            //Debug.Log("Looking over Schoulder");
            _rotationSpeed = _normalRotateSpeed;
            _inputForBack = 0;
            _absoluteTransform.Rotate(0, 0, 0);
            _lookingOverSchoulder = true;

            _clampLookShoulder += Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime;
            _clampLookShoulder = Mathf.Clamp(_clampLookShoulder, _LookShoulderLimit.x, _LookShoulderLimit.y);
            //_cam.eulerAngles = new Vector3(_clampCamInput, _clampLookShoulder, 0);
            _cam.localEulerAngles = new Vector3(_clampCamInput, _clampLookShoulder, 0);

        }
        else if (Input.GetButtonUp("LookBack"))
        {
            _clampLookShoulder = 0;
            _lookingOverSchoulder = false;
        }
    }

    void PressingButtonState()
    {
        
        if (Input.GetButtonDown("Button") && !_isRunning && !_movingABox)
        {
            PressButton();
            _inputForBack = 0;
            _inputForBack = 0;
            IsButtonPressed = true;
        }
        else
        {
            IsButtonPressed = transform.GetChild(0).GetComponent<ButtonAnimationCheck>().ButtonPressAnim;
        }
        

    }

    void GunState()
    {
        if (Input.GetButtonDown("Fire1") && !_isRunning && !_movingABox && _currentBullets > 0)
        {
            Shoot();
            _currentBullets--;
            Debug.Log("Gun has shot");
        }

        if (_currentBullets <= _maxBullets)
        {
            _reloadTimer += Time.deltaTime;
            if (_reloadTimer >= _reloadTime)
            {
                _reloadTimer = 0;
                _currentBullets++;
            }
        }
    }

    void PlayerGotStabbed()
    {
        if(_stabbed)
        {
            _inputForBack = 0;
            _inputLeftRight = 0;
            _rotationSpeed = 0;
            //Debug.Log("in schok");

            _shokTimer += Time.deltaTime;
            if(_shokTimer > 2)
            {
                _stabbed = false;
                _shokTimer = 0;
                //Debug.Log("shok is over");
            }

        }
    }

    void PlayerDies()
    {
        if(_playerLives <= 0)
        {
            Debug.Log("The Player is dead");
        }
    }

    void DebugState()
    {
        Vector3 interactRay = _boxRayStart.TransformDirection(Vector3.forward) * _boxRayRange;
        Debug.DrawRay(_boxRayStart.transform.position, interactRay, Color.red);

        Vector3 butRay = _buttonRayStart.TransformDirection(Vector3.forward) * _buttonRayRange;
        Debug.DrawRay(_buttonRayStart.transform.position, butRay, Color.yellow);

        Vector3 gunray = _gunBarrel.TransformDirection(Vector3.right) * _gunRange;
        Debug.DrawRay(_gunBarrel.transform.position, gunray, Color.cyan);
    }

    //voids in fixed update
    void ApplyGround()
    {
        //if the player is standing on the ground
        if (_playerControll.isGrounded)
        {
            Velocity -= Vector3.Project(Velocity, Physics.gravity);
        }

    }

    void ApplyGravity()
    {
        //if the player isn't standing on the ground
        if (!_playerControll.isGrounded)
        {
            Velocity += Physics.gravity * Time.deltaTime;
        }
    }

    void CalculatingMovement()
    {
        //for the rotation, that were you look is forward
        Vector3 xzForward = Vector3.Scale(_absoluteTransform.forward, new Vector3(1, 0, 1));
        Quaternion Rotation = Quaternion.LookRotation(xzForward);

        //to actual move
        Vector3 movement = Rotation * InputMovement;
        Velocity += movement * _acceleration * Time.deltaTime;

    }

    void ApplyDragOnGround()
    {
        if (_playerControll.isGrounded)
        {
            Velocity = Velocity * (1 - _drag * Time.deltaTime);
        }
    }

    void LimitXZVelocity()
    {
        //to give the player an max speed
        Vector3 yVel = Vector3.Scale(Velocity, Vector3.up);
        Vector3 xzVel = Vector3.Scale(Velocity, new Vector3(1, 0, 1));
        xzVel = Vector3.ClampMagnitude(xzVel, _maxWalkXZVel);
        Velocity = xzVel + yVel;
    }

    void DoMovement()
    {
        //to do the actual movement of the player
        Vector3 movement = Velocity * Time.deltaTime;
        _playerControll.Move(movement);

    }

    //voids in states
    void GrabBox()
    {
        //to grab a box and move it
        RaycastHit hit;
        if (Physics.Raycast(_boxRayStart.transform.position, _boxRayStart.transform.forward, out hit, _boxRayRange))
        {
            
            if (hit.transform.tag == "Box")
            {
                _targetBox = hit.transform.GetComponent<MovebleBox>();
                _targetBox.BoxIsGrabbed(transform.gameObject);
                _movingABox = true;
                Debug.Log("Grabbing Box");
            }
        }
    }

    void DropBox()
    {
        //to drop the box
        _targetBox.BoxIsDropped();
        _movingABox = false;
        _boxDistance = 0;
        Debug.Log("Dropping Box");
        
    }

    void Shoot()
    {
        RaycastHit hit;
        
        if(Physics.Raycast(_gunBarrel.transform.position, _gunBarrel.transform.right, out hit, _gunRange))
        {
            Debug.Log("Gun hitted " + hit.transform.name);
            if (hit.transform.tag == "Enemy")
            {
                EnemyLives target = hit.transform.GetComponent<EnemyLives>();
                if(target != null)
                {
                    target.TakeDamage(_gunDamage);
                }
            }
        }


    }

    void PressButton()
    {
        RaycastHit hit;
        if (Physics.Raycast(_buttonRayStart.transform.position, _buttonRayStart.transform.forward, out hit, _buttonRayRange))
        {
            if(hit.transform.tag == "Button")
            {
                
                Button button = hit.transform.GetComponent<Button>();
                if (button != null)
                {
                    button.ButtonPressed();
                    Debug.Log("knop is true");
                }

            }

        }
    }
    
    //voids enemy activates

    public void RecivingTheStab(int amountDamage)
    {
        _stabbed = true;
        _playerLives -= amountDamage;
        Debug.Log("Player recived a stab");
    }
    
}