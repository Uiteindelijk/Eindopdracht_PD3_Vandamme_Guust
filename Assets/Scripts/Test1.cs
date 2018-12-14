using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Test1 : MonoBehaviour
{

    [SerializeField] private float _acceleration, _drag, _rotationSpeed;
    [SerializeField] private float _maximumXZVelocity = (30 * 1000) / (60 * 60); // [m/s]

    private Transform _absoluteTransform, _aimPoint, _aimTarget;
    private CharacterController _charControll;
    private Animator _anim;
    private Vector3 _aim;

    private bool _isMoving;

    [HideInInspector] public Vector3 Velocity = Vector3.zero;
    [HideInInspector] public Vector3 InputMovement;

    void Start()
    {
        //components

        _charControll = GetComponent<CharacterController>();
        _absoluteTransform = _charControll.transform;

        //we take the animation controller from our first child object
        _anim = transform.GetChild(0).GetComponent<Animator>();

        //we take the transform from our second child object and then the first child object of the second child object
        _aimPoint = transform.GetChild(1);
        _aimTarget = _aimPoint.GetChild(0);

        //hiding the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


#if DEBUG

        Assert.IsNotNull(_charControll, "Error: CharacterController is missing from script CharControll");
        Assert.IsNotNull(_anim, "Error: Animator is missing from script CharControll");

#endif

    }

    void Update()
    {

        InputMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        _aim = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        
    }

    void FixedUpdate()
    {

        ApplyGround();          //when char is standing on the ground
        ApplyGravity();         //when char is not standing on anything
        ApplyMovement();        //to calculate to where and how fast the player has to move
        ApplyDragOnGround();    //to apply a drag on the player
        LimitXZVelocity();

        Vector3 XZvel = Vector3.Scale(Velocity, new Vector3(1, 0, 1));
        Vector3 localVelXZ = gameObject.transform.InverseTransformDirection(XZvel);
        _anim.SetFloat("VerticalInput", (localVelXZ.z * (_drag)) / _maximumXZVelocity);
        _anim.SetFloat("HorizontalInput", (localVelXZ.x * (_drag)) / _maximumXZVelocity);
        _absoluteTransform.Rotate(0, Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime, 0);

        DoMovement();           //do velocity / movement on character controller

    }

    private void ApplyGround()
    {

        if (_charControll.isGrounded)
        {
            Velocity -= Vector3.Project(Velocity, Physics.gravity);
        }

    }

    private void ApplyGravity()
    {

        if (!_charControll.isGrounded)
        {
            Velocity += Physics.gravity * Time.deltaTime;
        }

    }

    private void ApplyMovement()
    {

        if (_charControll.isGrounded)
        {
            //get the relative rotation
            Vector3 xzForward = Vector3.Scale(_absoluteTransform.forward, new Vector3(1, 0, 1));
            Quaternion relativeRotation = Quaternion.LookRotation(xzForward);

            //move in relative direction
            Vector3 relativeMovement = relativeRotation * InputMovement;
            Velocity += relativeMovement * _acceleration * Time.deltaTime;
        }

    }

    private void ApplyDragOnGround()
    {
        if (_charControll.isGrounded)
        {
            Velocity = Velocity * (1 - _drag * Time.deltaTime);
        }
    }
    
    private void LimitXZVelocity()
    {
        Vector3 yVel = Vector3.Scale(Velocity, Vector3.up);
        Vector3 xzVel = Vector3.Scale(Velocity, new Vector3(1, 0, 1));

        xzVel = Vector3.ClampMagnitude(xzVel, _maximumXZVelocity);

        Velocity = xzVel + yVel;
    }

    private void DoMovement()
    {
        Vector3 movement = Velocity * Time.deltaTime;
        _charControll.Move(movement);
    }

}
