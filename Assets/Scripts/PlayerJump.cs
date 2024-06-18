using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] InputActionProperty jumpButton;
    [SerializeField] InputActionProperty runButton;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] CharacterController cc;
    [SerializeField] LayerMask groundLayers;
    [SerializeField] bool _isGrounded;
    [SerializeField] bool _isRun = false;

    float gravity = Physics.gravity.y;
    [SerializeField] Vector3 movement;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        _isGrounded = IsGrounded();
        _isRun = IsRun();
    }

    void FixedUpdate()
    {
        if(jumpButton.action.WasPressedThisFrame() && _isGrounded)
        {
            Jump();
        }
        // 달리기 코드 여유있으면 수정
        if(runButton.action.WasPressedThisFrame() && _isGrounded)
        {
            Debug.Log("run");
            Run();
        }
        movement.y += gravity * Time.deltaTime;

        cc.Move(movement * Time.deltaTime);
    }

    void Jump()
    {
        movement.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }

    void Run()
    {

    }

    bool IsRun()
    {
        return movement.x == 2f;
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.2f, groundLayers);
    }
}
