using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class New : MonoBehaviour
{

    [SerializeField] private float _accerlation, _drag, _rotationSpeed;
    [SerializeField] private float _MaximumXZVelocity = (5 * 1000) / (60 * 60); // 5km/h
    [SerializeField] private Vector2 _clampLimetCam;

    private float _clamp;
    private Transform _absoluteTransform, _cam;
    private CharacterController _playerControll;
    private Animator _anim;

    [HideInInspector] public Vector3 Velocity = Vector3.zero, InputMovement;

    //testing
    [SerializeField]
    Transform rightHand, target;
    Vector3 offset = new Vector3(0.2f, 0.4f, 0);
    Vector3 handPos, targetPos;
    Quaternion handRotation, targetRotation;

    void Start()
    {
        //components
        _playerControll = GetComponent<CharacterController>();
        _absoluteTransform = _playerControll.transform;

        //components from child objects
        _anim = transform.GetChild(0).GetComponent<Animator>();
        //_cam = transform.GetChild(1);
        _cam = transform.GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(3);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

#if DEBUG

        Assert.IsNotNull(_playerControll, "The player doesn't have charactercontroller! pls fix");
        Assert.IsNotNull(_anim, "The player does not have an animator on the first childobject! pls fix");

#endif

        //testing stuff
        //UpArm = _anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        //UnArm = _anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        //Hand = _anim.GetBoneTransform(HumanBodyBones.LeftHand);
    }

    void Update()
    {
        InputMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if (_playerControll.isGrounded)
        {
            Debug.Log("Player staat op de grond");
        }
        
    }


    private void OnAnimatorIK()
    {
        Vector3 targetPosistion = targetPos + targetRotation * offset;
        targetPosistion += handPos + handRotation * offset;

        _anim.SetIKPosition(AvatarIKGoal.RightHand, targetPosistion);
        _anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
    }

    private void LateUpdate()
    {
        //testing stuff
        //UpArm.LookAt(AimPoint.position);
        //UpArm.rotation = UpArm.rotation * Quaternion.Euler(Offset);
        //UnArm.LookAt(AimPoint.position);
        //UnArm.rotation = UnArm.rotation * Quaternion.Euler(Offset1);
        //Hand.LookAt(AimPoint.position);
        //Hand.rotation = UnArm.rotation * Quaternion.Euler(Offset2);

        handPos = rightHand.position;
        targetPos = target.position;
        handRotation = rightHand.rotation;
        targetRotation = target.rotation;
        
    }



    void FixedUpdate()
    {
        ApplyGround();          //When char is standing on the ground
        ApplyGravity();         //When the char isn't standing on anything
        ApplyMovement();        //to calculate the movement
        ApplyDragOnGround();    //To apply drag on the player

        LimitXZVelocity();

        //for the animations
        Vector3 XZvel = Vector3.Scale(Velocity, new Vector3(1, 0, 1));
        Vector3 localVelXZ = gameObject.transform.InverseTransformDirection(XZvel);
        _anim.SetFloat("VerticalInput", (localVelXZ.z * (_drag)) / _MaximumXZVelocity);
        _anim.SetFloat("HorizontalInput", (localVelXZ.x * (_drag)) / _MaximumXZVelocity);

        //to clamp the camera
        _clamp += -Input.GetAxis("Mouse Y") * _rotationSpeed * Time.deltaTime;
        _clamp = Mathf.Clamp(_clamp, _clampLimetCam.x, _clampLimetCam.y);
        _cam.eulerAngles = new Vector3(_clamp, _cam.eulerAngles.y, 0);

        _absoluteTransform.Rotate(0, Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime, 0);
        DoMovement();

    }

    private void ApplyGround()
    {
        if (_playerControll.isGrounded)
        {
            Velocity -= Vector3.Project(Velocity, Physics.gravity);
        }
    }

    private void ApplyGravity()
    {
        if (!_playerControll.isGrounded)
        {
            Velocity += Physics.gravity * Time.deltaTime;
        }
    }

    private void ApplyMovement()
    {
        //for the rotation, that were you look is forward
        Vector3 xzForward = Vector3.Scale(_absoluteTransform.forward, new Vector3(1, 0, 1));
        Quaternion Rotation = Quaternion.LookRotation(xzForward);

        //to actual move
        Vector3 movement = Rotation * InputMovement;
        Velocity += movement * _accerlation * Time.deltaTime;


    }

    private void ApplyDragOnGround()
    {
        if (_playerControll.isGrounded)
        {
            Velocity = Velocity * (1 - _drag * Time.deltaTime);
        }


    }

    private void LimitXZVelocity()
    {
        Vector3 yVel = Vector3.Scale(Velocity, Vector3.up);
        Vector3 xzVel = Vector3.Scale(Velocity, new Vector3(1, 0, 1));

        xzVel = Vector3.ClampMagnitude(xzVel, _MaximumXZVelocity);

        Velocity = xzVel + yVel;

    }

    private void DoMovement()
    {
        Vector3 movement = Velocity * Time.deltaTime;
        _playerControll.Move(movement);
    }

}