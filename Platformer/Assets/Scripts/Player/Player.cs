using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _movementForce;
    [SerializeField]
    private float _jumpForce;
    [SerializeField]
    private float _gravityValue;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float maxSpeed = 5f;
    private Vector3 forceDirection = Vector3.zero;

    private PlayerInput _playerInputs;
    private InputAction move;

    [SerializeField]
    private Camera playerCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _playerInputs = new PlayerInput();

    }

    private void OnEnable()
    {
        _playerInputs.Player.Jump.started += DoJump;
        move = _playerInputs.Player.Move;
        _playerInputs.Player.Enable();
    }

    private void OnDisable()
    {
        _playerInputs.Player.Jump.started += DoJump;
        _playerInputs.Player.Disable();
    }

    private void FixedUpdate()
    {
        forceDirection += move.ReadValue<Vector2>().x * GetCameeraRight(playerCamera) * _movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameeraForward(playerCamera) * _movementForce;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if(rb.velocity.y < 0f)
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.deltaTime * 4;

        Vector3 horizontalVel = rb.velocity;
        horizontalVel.y = 0;
        if (horizontalVel.sqrMagnitude > maxSpeed * maxSpeed)
            rb.velocity = horizontalVel.normalized * maxSpeed + Vector3.up * rb.velocity.y;

        LookAt();
    }

    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            rb.angularVelocity = Vector3.zero;
    }

    private Vector3 GetCameeraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameeraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        if(IsGrounded())
        {
            forceDirection += Vector3.up * _jumpForce;
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction, Color.black);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            Debug.Log("Grounded");
            return true;
        }
        else
        {
            Debug.Log("Not floor");
            return false;
        }
    }
}
