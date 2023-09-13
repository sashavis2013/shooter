using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    public Animator rigController;
    public float jumpHeight = 3;
    public float gravity = 20;
    public float stepDown = 0.3f;
    public float airControl = 0.5f;
    public float jumpDamp = 0.5f;
    public float groundSpeed = 1;
    public float pushPower = 2;

    Animator _animator;
    CharacterController _cc;
    ActiveWeapon _activeWeapon;
    ReloadWeapon _reloadWeapon;
    CharacterAiming _characterAiming;
    Vector2 _input;

    Vector3 _rootMotion;
    Vector3 _velocity;
    bool _isJumping;

    int _isSprintingParam = Animator.StringToHash("isSprinting");
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        _cc = GetComponent<CharacterController>();
        _activeWeapon = GetComponent<ActiveWeapon>();
        _reloadWeapon = GetComponent<ReloadWeapon>();
        _characterAiming = GetComponent<CharacterAiming>();
    }
    void Update()
    {
        _input.x = Input.GetAxis("Horizontal");
        _input.y = Input.GetAxis("Vertical");

        _animator.SetFloat("InputX", _input.x);
        _animator.SetFloat("InputY", _input.y);

        UpdateIsSprinting();
 
        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }
    }

    bool IsSprinting() {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isFiring = _activeWeapon.IsFiring();
        bool isReloading = _reloadWeapon.isReloading;
        bool isChangingWeapon = _activeWeapon.isChangingWeapon;
        bool isAiming = _characterAiming.isAiming;
        return isSprinting && !isFiring && !isReloading && !isChangingWeapon && !isAiming;
    }

    private void UpdateIsSprinting() {
        bool isSprinting = IsSprinting();
        _animator.SetBool(_isSprintingParam, isSprinting);
        rigController.SetBool(_isSprintingParam, isSprinting);
    }

    private void OnAnimatorMove() {
        _rootMotion += _animator.deltaPosition;
        MoveCharacter(Time.deltaTime);
    }

    void MoveCharacter(float deltaTime) {
        if (_isJumping) { // IsInAir state
            UpdateInAir(deltaTime);
        } else { // IsGrounded state
            UpdateOnGround();
        }
    }

    private void UpdateOnGround() {
        Vector3 stepForwardAmount = _rootMotion * groundSpeed;
        Vector3 stepDownAmount = Vector3.down * stepDown;

        _cc.Move(stepForwardAmount + stepDownAmount);
        _rootMotion = Vector3.zero;

        if (!_cc.isGrounded) {
            _cc.Move(-stepDownAmount);
            SetInAir(0);
        }
    }

    private void UpdateInAir(float deltaTime) {
        _velocity.y -= gravity * deltaTime;
        Vector3 displacement = _velocity * deltaTime;
        displacement += CalculateAirControl();
        _cc.Move(displacement);
        _isJumping = !_cc.isGrounded;
        _rootMotion = Vector3.zero;
        _animator.SetBool("isJumping", _isJumping);
    }

    Vector3 CalculateAirControl() {
        return ((transform.forward * _input.y) + (transform.right * _input.x)) * (airControl / 100);
    }

    void Jump() {
        if (!_isJumping) {
            float jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
            SetInAir(jumpVelocity);
        }
    }

    private void SetInAir(float jumpVelocity) {
        _isJumping = true;
        _velocity = _animator.velocity * (jumpDamp * groundSpeed);
        _velocity.y = jumpVelocity;
        _animator.SetBool("isJumping", true);
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
            return;

        // to not push objects below us
        if (hit.moveDirection.y < -0.3f)
            return;

        // Calculate push direction from move direction,
        // only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // Apply the push
        body.velocity = pushDir * pushPower;
    }
}
