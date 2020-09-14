using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameManager _gm;
    public Transform camera;
    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float turnSmooth = 0.1f;
    private float _turnSmoothVelocity;
    private CharacterController _characterController;
    private Vector3 _velocity;
    private Animator _animator;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _characterController.isTrigger = false;
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_gm.moveEnabled)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity,
                    turnSmooth);
                transform.rotation = Quaternion.Euler(0, angle, 0);
                Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                _characterController.Move(moveDirection.normalized * Speed * Time.deltaTime);
                _animator.SetBool("isRunning", true);
            }
            else _animator.SetBool("isRunning", false);

            if (Input.GetButtonDown("Jump"))
            {
                var _onFloor = Physics.Raycast(transform.position, Vector3.down, 1f);
                if (_onFloor) _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }

            if (_characterController.isGrounded) _velocity.y = 0;

            _velocity.y += Gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }
}