using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class Player : MonoBehaviour
{
    //Player Movement RigidBody
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
    private bool canDoubleJump = true;

    //Player Input
    private PlayerInput _playerInputs;
    private InputAction move;

    //Camera
    [SerializeField]
    private Camera playerCamera;

    //Weapon
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform bulletSpawn;

    //Health and respawn
    private Vector3 respawnPoint;
    private int currentHealth;
    [SerializeField]
    private int maxHealth;


    //Animation
    [SerializeField]
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _playerInputs = new PlayerInput();
        respawnPoint = GetComponent<Transform>().position;

        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        _playerInputs.Player.Jump.started += DoJump;
        _playerInputs.Player.Fire.started += Fire;
        _playerInputs.Player.Respawn.started += Reset;
        move = _playerInputs.Player.Move;
        _playerInputs.Player.Enable();
    }

    private void Reset(InputAction.CallbackContext obj)
    {
        this.gameObject.transform.position = respawnPoint;
        currentHealth = maxHealth;
    }

    private void Reset()
    {
        this.gameObject.transform.position = respawnPoint;
        currentHealth = maxHealth;
    }
    private void Fire(InputAction.CallbackContext obj)
    {
        GameObject bulletClone = Instantiate(bullet, bulletSpawn);
    }

    private void OnDisable()
    {
        _playerInputs.Player.Jump.started += DoJump;
        _playerInputs.Player.Fire.started += Fire;
        _playerInputs.Player.Respawn.started += Reset;
        _playerInputs.Player.Disable();
    }
    private void Update()
    {
        if (currentHealth <= 0)
            Reset();
    }

    private void FixedUpdate()
    {
        forceDirection += move.ReadValue<Vector2>().x * GetCameeraRight(playerCamera) * _movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameeraForward(playerCamera) * _movementForce;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if (rb.velocity.y < 0f && !IsGrounded())
        {
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.deltaTime * 4;
        }
        Vector3 horizontalVel = rb.velocity;
        horizontalVel.y = 0;
        if (horizontalVel.sqrMagnitude > maxSpeed * maxSpeed)
            rb.velocity = horizontalVel.normalized * maxSpeed + Vector3.up * rb.velocity.y;

        bool isIdle = move.ReadValue<Vector2>().x == 0 && move.ReadValue<Vector2>().y == 0;
        Debug.Log(isIdle);

        if (isIdle)
        {
            Debug.Log("Idle");
            rb.velocity = Vector3.zero;
            animator.SetFloat("Forward", 0);
            animator.SetFloat("Turn", 0);
        }
        else
        {
            animator.SetFloat("Forward", move.ReadValue<Vector2>().x);
            animator.SetFloat("Turn", move.ReadValue<Vector2>().y);
        }

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

        //bool isIdle = direction.x == 0 && direction.z == 0;
        //Debug.Log(isIdle);
        //if (isIdle)
        //{
        //    Debug.Log("Idle");
        //    rb.velocity = Vector3.zero;
        //    animator.SetFloat("Forward", 0);
        //}
        //else
        //{
        //    animator.SetFloat("Forward", direction.x);
        //    animator.SetFloat("Turn", direction.z);
        //}
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
            canDoubleJump = true;
            forceDirection += Vector3.up * _jumpForce;
            animator.SetTrigger("Jump");
        }
        else if(canDoubleJump)
        {
            canDoubleJump = false;
            forceDirection += Vector3.up * _jumpForce;
            animator.SetTrigger("Jump");
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction, Color.black);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
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

    public void SetRespawnPoint(Transform respawn)
    {
        respawnPoint = respawn.position;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

    }
}
