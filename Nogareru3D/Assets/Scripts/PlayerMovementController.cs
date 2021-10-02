using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private CharacterController characterController = null;

    // Input funciona assim: Aperta tecla, da informacao que apertou, ai n acontece nada ate soltar
    private Vector2 previousInput;

    private Controls controls;

    public Controls Controls
    {
        get
        {
            if (controls != null)
            {
                return controls;
            }

            return controls = new Controls();
        }
    }

    public override void OnStartAuthority()
    {
        enabled = true;

        Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>()); // Quando apertar, seta
        Controls.Player.Move.canceled += ctx => ResetMovement(); // Quando soltar, reseta
    }



    [ClientCallback]
    private void OnEnable() => Controls.Enable();
    [ClientCallback]
    private void OnDisable() => Controls.Disable();
    [ClientCallback]
    private void Update() => Move();

    [Client]
    private void ResetMovement()
    {
        previousInput = Vector2.zero;
    }

    [Client]
    private void SetMovement(Vector2 movement)
    {
        previousInput = movement;
    }

    [Client]
    private void Move()
    {
        Vector3 right = characterController.transform.right;
        Vector3 forward = characterController.transform.forward;

        right.y = 0f;
        forward.y = 0f;

        Vector3 movement = right.normalized * previousInput.x + forward.normalized * previousInput.y;

        characterController.Move(movement * velocidade * Time.deltaTime);
    }
}
