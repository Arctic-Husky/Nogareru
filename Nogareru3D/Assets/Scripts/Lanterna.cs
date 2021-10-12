using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Lanterna : NetworkBehaviour
{
    [SerializeField] private Light luz = null;
    [SerializeField] private GameObject indicadorDeLigado = null;

    public override void OnStartAuthority()
    {
        InputManager.Controls.Player.Flashlight.started += ctx => CmdLigar();
    }

    [Command]
    public void CmdLigar()
    {
        RpcLigar();
    }

    [ClientRpc]
    private void RpcLigar()
    {
        luz.enabled = !luz.enabled;
        indicadorDeLigado.SetActive(!indicadorDeLigado.activeSelf);
    }
}
