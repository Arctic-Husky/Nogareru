using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private CharacterController characterController = null;
    [SerializeField] private float gravidade = 9.8f;

    // Input funciona assim: Aperta tecla, da informacao que apertou, ai n acontece nada ate soltar
    private Vector2 previousInput;

    private float velVertical = 0f;

    private Controls controls;

    public override void OnStartAuthority()
    {
        enabled = true;

        InputManager.Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>()); // Quando apertar, seta
        InputManager.Controls.Player.Move.canceled += ctx => ResetMovement(); // Quando soltar, reseta
    }

    [ClientCallback]
    private void Update() => Move();
    /*[ClientCallback]
    private void FixedUpdate() => Gravity();*/

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

    /*[Client]
    private void Gravity()
    {
        
    }*/

    /// <summary>
    /// Metodo de movimentacao, por enquanto confiando no client
    /// </summary>
    [Client]
    private void Move()
    {
        Vector3 right = characterController.transform.right;
        Vector3 forward = characterController.transform.forward;
        Vector3 up = characterController.transform.up;
        
        if(characterController.isGrounded)
        {
            velVertical = 0;
        }

        velVertical -= gravidade * Time.deltaTime;

        right.y = 0f;
        forward.y = 0f;
        up.y = velVertical;

        Vector3 movement = right.normalized * previousInput.x + forward.normalized * previousInput.y + up;

        characterController.Move(movement * velocidade * Time.deltaTime);
    }
}
