using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerMovement : MonoBehaviour
{
    //normal state
    [SerializeField] private float _acceleration, _drag, _normalRotateSpeed, _walkSpeed;
    [SerializeField] private Vector2 _clampLimetCam;
    [SerializeField] private Transform _ikTargetLeftHand, _ikTargetRightHand;
    private Vector3 Velocity = Vector3.zero, _inputMovement;
    private float _clampCamInput, _maxWalkXZVel, _inputLeftRight, _rotationSpeed, _inputForBack;
    private Transform _absoluteTransform, _cam, _ik;
    private CharacterController _playerControll;
    private Animator _anim;
    
    //Running state
    [SerializeField] private float _runSpeed;
    private bool _isRunning = false, _playerIsMoving = false;

    //Moving box state
    [SerializeField] private float _movingBoxSpeed, _boxRayRange, _grabRotateSpeed;
    private bool _movingABox = false;
    [SerializeField] private Transform _boxRayLeft, _boxRayRight;
    private MovebleBox _targetBox;
    private float _boxDistance;

    //Looking over shoulder state
    [SerializeField] private Vector2 _LookShoulderLimit;
    private float _clampLookShoulder;
    private bool _lookingOverSchoulder = false;

    //gun vars
    [SerializeField] private int _gunDamage;
    [SerializeField] private float _gunRange, _reloadTime;
    [SerializeField] private int _maxBullets, _currentBullets;
    [SerializeField] private Transform _gunBarrel;
    private float _reloadTimer;

    //button vars
    [SerializeField] private float _buttonRayRange;
    private bool _isButtonPressed = false;
    private Transform _buttonRayStart;

    //getting stabbed vars
    private bool _stabbed = false;
    private int _playerLives = 100;
    private float _shokTimer = 0;

    //player dies and player won vars
    private bool _playerIsDead = false;
    private bool _playerWon = false;

    //to pause the game vars
    private bool _isPaused = false;

    //Inverse Kinematics
    private IKControllerPlayer _ikPlayer;
    private ButtonIK _buttonIK;
    private BoxIk _boxIK;

    //main voids
    private void Start()
    {
        SetComponentsAndBehaviours();
        SetTransformsAndMouse();

#if DEBUG
        //debug stuf
        Assert.IsNotNull(_playerControll, "The Player doesn't contain a CharacterController");
        Assert.IsNotNull(_anim, "The player doesn't contain a animator");
        Assert.IsNotNull(_ikPlayer, "The Animator on the player doesn't contain script IKControllerPlayer");
#endif

    }

    private void Update()
    {
        InputMovement();
        CamRotation();
        InverseKinematicsHands();
        Animations();
        NormalState();
        IsMoving();
        RunningState();
        MovingBoxState();
        MovingBox();
        DroppingBox();
        LookingOverShoulderState();
        PressingButtonState();
        GunState();
        Reload();
        PlayerGotStabbed();
        PlayerDies();
        PauseGame();
        //DebugState();
    }

    private void FixedUpdate()
    {
        //physics
        ApplyGround();
        ApplyGravity();
        CalculatingMovement();
        LimitXZVelocity();
        ApplyDragOnGround();
        DoMovement();

    }
    
    //set conpoments and behaviours
    private void SetComponentsAndBehaviours()
    {
        _playerControll = GetComponent<CharacterController>();
        _anim = transform.GetChild(0).GetComponent<Animator>();

        _ikPlayer = _anim.GetBehaviour<IKControllerPlayer>();
        _buttonIK = _anim.GetBehaviour<ButtonIK>();
        _boxIK = _anim.GetBehaviour<BoxIk>();
    }

    //set transforms and mouse
    private void SetTransformsAndMouse()
    {
        _absoluteTransform = _playerControll.transform;
        _cam = transform.GetChild(1);
        _ik = transform.GetChild(2);
        _buttonRayStart = transform.GetChild(3);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //to get the input
    private void InputMovement()
    {
        _inputMovement = new Vector3(_inputLeftRight, 0, _inputForBack).normalized;
    }

    //to set the rotation of the cam
    private void CamRotation()
    {
        _clampCamInput += -Input.GetAxis("Mouse Y") * _rotationSpeed * Time.deltaTime;
        _clampCamInput = Mathf.Clamp(_clampCamInput, _clampLimetCam.x, _clampLimetCam.y);
        if (!_lookingOverSchoulder)
        {
            _absoluteTransform.Rotate(0, Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime, 0);
        }
    }

    //to set the ik of the hands
    private void InverseKinematicsHands()
    {
        //for more information check document
        _ikPlayer.IkTargetLeftHand = _ikTargetLeftHand;
        _ikPlayer.IkTargetRightHand = _ikTargetRightHand;
    }

    //to set all the animation vars
    private void Animations()
    {
        Vector3 XZvel = Vector3.Scale(Velocity, new Vector3(1, 0, 1));
        Vector3 localVelXZ = gameObject.transform.InverseTransformDirection(XZvel);
        _anim.SetFloat("VerticalInput", localVelXZ.z / _maxWalkXZVel);
        _anim.SetFloat("HorizontalInput", localVelXZ.x / _maxWalkXZVel);
        _anim.SetBool("Run", _isRunning);
        _anim.SetBool("MoveBox", _movingABox);
        _anim.SetBool("PressedButton", _isButtonPressed);
        _anim.SetBool("PlayerIsDead", _playerIsDead);
        _anim.SetBool("PlayerWon", _playerWon);
    }

    //when the player is in the normal state
    private void NormalState()
    {
        if (!_isRunning && !_movingABox && !_lookingOverSchoulder && !_isButtonPressed && !_stabbed && !_playerIsDead)
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

    //to check if the player is moving
    private void IsMoving()
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

    //when the player is running
    private void RunningState()
    {
        if (Input.GetButton("Run") && !_movingABox && _playerIsMoving && !_playerIsDead)
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

    //to grab a box
    private void MovingBoxState()
    {
        if (Input.GetButtonDown("Grab") && !_playerIsDead)
        {
            GrabBox();
        }
        
        
    }

    //when player is moving a box
    private void MovingBox()
    {
        if (_movingABox)
        {
            _inputLeftRight = 0;
            _maxWalkXZVel = (_movingBoxSpeed * 1000) / (60 * 60);
            _rotationSpeed = _grabRotateSpeed;
            _inputForBack = Input.GetAxisRaw("Vertical");
            _cam.transform.rotation = Quaternion.Lerp(_cam.transform.rotation, _absoluteTransform.transform.rotation, Time.deltaTime * 10f);
            _boxDistance = Vector3.Distance(_absoluteTransform.position, _targetBox.transform.position);
        }
    }

    //when the player drops a box
    private void DroppingBox()
    {
        if (Input.GetButtonDown("Drop") && !_lookingOverSchoulder && _movingABox || _playerIsDead && _movingABox)
        {
            DropBox();
        }
        if (_movingABox && _boxDistance > 1.4f && _boxDistance < 11.8f)
        {
            //for more information check document
            DropBox();
        }
    }

    //player grabs box
    private void GrabBox()
    {
        //for more information check document
        RaycastHit hit;
        if (Physics.Raycast(_boxRayLeft.transform.position, _boxRayLeft.transform.forward, out hit, _boxRayRange) &&
            Physics.Raycast(_boxRayRight.transform.position, _boxRayRight.transform.forward, out hit, _boxRayRange))
        {

            if (hit.transform.tag == "Box")
            {
                _targetBox = hit.transform.GetComponent<MovebleBox>();
                _targetBox.BoxIsGrabbed(transform.gameObject);
                _movingABox = true;
                Debug.Log("Grabbing Box: " + hit.transform.name);

                _boxIK.RayRange = _boxRayRange;
                _boxIK._leftRay = _boxRayLeft;
                _boxIK._rightRay = _boxRayRight;
            }
        }
    }

    //player drops box
    private void DropBox()
    {
        //to drop the box
        _targetBox.BoxIsDropped();
        _movingABox = false;
        _boxDistance = 0;
        Debug.Log("Dropping Box");

    }

    //looking over shcould state
    private void LookingOverShoulderState()
    {
        if (Input.GetButton("LookBack") && _movingABox)
        {
            LookingOverShoulder();
        }
        else if (Input.GetButtonUp("LookBack") || _playerIsDead)
        {
            _clampLookShoulder = 0;
            _lookingOverSchoulder = false;
        }
    }

    //when player is looking over shoulder
    private void LookingOverShoulder()
    {
        //for more information check document
        //Debug.Log("Looking over Schoulder");
        _rotationSpeed = _normalRotateSpeed;
        _inputForBack = 0;
        _absoluteTransform.Rotate(0, 0, 0);
        _lookingOverSchoulder = true;

        _clampLookShoulder += Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime;
        _clampLookShoulder = Mathf.Clamp(_clampLookShoulder, _LookShoulderLimit.x, _LookShoulderLimit.y);
        _cam.localEulerAngles = new Vector3(_clampCamInput, _clampLookShoulder, 0);
    }

    //when the player wants to press a button
    private void PressingButtonState()
    {
        if (Input.GetButtonDown("Button") && !_isRunning && !_movingABox && !_isButtonPressed && !_playerIsDead)
        {
            PressButton();
        }
        else
        {
            _isButtonPressed = transform.GetChild(0).GetComponent<ButtonAnimationCheck>().ButtonPressAnim;
        }
    }

    //when button actual gets pressed
    private void PressButton()
    {
        //for more information check document
        RaycastHit hit;
        if (Physics.Raycast(_buttonRayStart.transform.position, _buttonRayStart.transform.forward, out hit, _buttonRayRange))
        {
            if (hit.transform.tag == "Button")
            {
                Button button = hit.transform.GetComponent<Button>();
                if (button != null)
                {
                    button.ButtonPressed();
                    _inputForBack = 0;
                    _inputLeftRight = 0;
                    _buttonIK.ButtonTarget = button.transform;
                    _isButtonPressed = true;
                }
            }
        }
    }

    //player shoot gun
    private void GunState()
    {
        if (Input.GetButtonDown("Fire1") && !_isRunning && !_movingABox && _currentBullets > 0 && !_playerIsDead)
        {
            Shoot();
            _currentBullets--;
            Debug.Log("Gun has shot");
        }
    }

    //reloading gun
    private void Reload()
    {
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

    //gun has shot andhits npc
    private void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(_gunBarrel.transform.position, _gunBarrel.transform.right, out hit, _gunRange))
        {
            Debug.Log("Gun hitted " + hit.transform.name);
            if (hit.transform.tag == "Enemy")
            {
                EnemyAi target = hit.transform.GetComponent<EnemyAi>();
                if (target != null)
                {
                    target.NpcRecivesDamage(_gunDamage);
                }
            }
        }
    }

    //when the player gets stabbed by the npc
    private void PlayerGotStabbed()
    {
        if (_stabbed)
        {
            _inputForBack = 0;
            _inputLeftRight = 0;
            _rotationSpeed = 0;
            //Debug.Log("in schok");
            InShok();
        }
    }

    //when the player recives the stab
    public void RecivingTheStab(int amountDamage)
    {
        _stabbed = true;
        _playerLives -= amountDamage;
        Debug.Log("Player recived a stab");
    }

    //when the player is in shok
    private void InShok()
    {
        _shokTimer += Time.deltaTime;
        if (_shokTimer > 4)
        {
            _stabbed = false;
            _shokTimer = 0;
            //Debug.Log("shok is over");
        }
    }

    //when the player dies
    private void PlayerDies()
    {
        if (_playerLives <= 0)
        {
            Debug.Log("The Player is dead");
            _playerIsDead = true;
            _inputForBack = 0;
            _inputLeftRight = 0;
            _rotationSpeed = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                enemy.GetComponent<EnemyAi>().PlayerDied();
            }

        }
    }
    
    //when the player wins
    public void PlayerWon()
    {
        Debug.Log("The Player won the game");
        _playerWon = true;
        _inputForBack = 0;
        _inputLeftRight = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //to pause or resume the game
    private void PauseGame()
    {
        if (!_isPaused && Input.GetButtonDown("PauseButton"))
        {
            GameIsPaused();
        }
        else if (_isPaused && Input.GetButtonDown("PauseButton"))
        {
            ResumeGame();
        }
    }

    //when the game is paused
    private void GameIsPaused()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _isPaused = true;
        Debug.Log("game is paused");
    }

    //when the game is resumed
    private void ResumeGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _isPaused = false;
        Debug.Log("Game Unpaused");
    }

    //if the player is standing on the ground
    private void ApplyGround()
    {
        if (_playerControll.isGrounded)
        {
            Velocity -= Vector3.Project(Velocity, Physics.gravity);
        }

    }

    //if the player isn't standing on the ground
    private void ApplyGravity()
    {
        if (!_playerControll.isGrounded)
        {
            Velocity += Physics.gravity * Time.deltaTime;
        }
    }

    //to calculate the movement
    private void CalculatingMovement()
    {
        //for the rotation, that were you look is forward
        Vector3 xzForward = Vector3.Scale(_absoluteTransform.forward, new Vector3(1, 0, 1));
        Quaternion Rotation = Quaternion.LookRotation(xzForward);

        //to actual move
        Vector3 movement = Rotation * _inputMovement;
        Velocity += movement * _acceleration * Time.deltaTime;

    }

    //to apply drag
    private void ApplyDragOnGround()
    {
        if (_playerControll.isGrounded)
        {
            Velocity = Velocity * (1 - _drag * Time.deltaTime);
        }
    }

    //to limit the top speed
    private void LimitXZVelocity()
    {
        //to give the player an max speed
        Vector3 yVel = Vector3.Scale(Velocity, Vector3.up);
        Vector3 xzVel = Vector3.Scale(Velocity, new Vector3(1, 0, 1));
        xzVel = Vector3.ClampMagnitude(xzVel, _maxWalkXZVel);
        Velocity = xzVel + yVel;
    }

    //to do the movement
    private void DoMovement()
    {
        //to do the actual movement of the player
        Vector3 movement = Velocity * Time.deltaTime;
        _playerControll.Move(movement);

    }
    
    //debug void for when you need to debug
    void DebugState()
    {
        Vector3 boxRayLeft = _boxRayLeft.TransformDirection(Vector3.forward) * _boxRayRange;
        Debug.DrawRay(_boxRayLeft.transform.position, boxRayLeft, Color.red);

        Vector3 boxRayRight = _boxRayRight.TransformDirection(Vector3.forward) * _boxRayRange;
        Debug.DrawRay(_boxRayRight.transform.position, boxRayRight, Color.red);

        Vector3 buttonRay = _buttonRayStart.TransformDirection(Vector3.forward) * _buttonRayRange;
        Debug.DrawRay(_buttonRayStart.transform.position, buttonRay, Color.yellow);

        Vector3 gunray = _gunBarrel.TransformDirection(Vector3.right) * _gunRange;
        Debug.DrawRay(_gunBarrel.transform.position, gunray, Color.cyan);
    }

}