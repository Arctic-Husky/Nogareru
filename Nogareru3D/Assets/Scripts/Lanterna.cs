using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Lanterna : NetworkBehaviour
{
    [SerializeField] private Light luz = null;
    [SerializeField] private GameObject indicadorDeLigado = null;

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

    [ClientCallback]
    private void OnEnable() => Controls.Enable();
    [ClientCallback]
    private void OnDisable() => Controls.Disable();

    public override void OnStartAuthority()
    {
        Controls.Player.Flashlight.started += ctx => CmdLigar();
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
