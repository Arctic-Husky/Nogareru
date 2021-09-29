using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private float speedVertical = 5.0f;
    [SerializeField]
    private float speedHorizontal = 5.0f;

    private float movimentoVertical;
    private float movimentoHorizontal;

    void Start()
    {
        
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        movimentoVertical = Input.GetAxis("Vertical") * speedVertical * Time.fixedDeltaTime;
        movimentoHorizontal = Input.GetAxis("Horizontal") * speedHorizontal * Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        transform.Translate(movimentoHorizontal, movimentoVertical, 0);
    }
}
