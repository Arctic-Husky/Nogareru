using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private Vector2 cameraVelocity = new Vector2(4f, 0.25f);
    [SerializeField] private Transform playerTransform = null;
    [SerializeField] private Transform playerCamera = null;
    [SerializeField] private float xClamp = 90f;

    /*[SerializeField] private GameObject visor = null;
    [SerializeField] private Transform visorOffset = null;*/

    private float xRotacao = 0f;
    private Controls controls;
    private Camera cameraComponent;
    private AudioListener audioListener;

    public override void OnStartClient()
    {
        cameraComponent = playerCamera.GetComponent<Camera>();
        audioListener = playerCamera.GetComponent<AudioListener>();

        if(!hasAuthority)
        {
            cameraComponent.enabled = false;
            audioListener.enabled = false;
        }
    }

    private Controls Controls
    {
        get
        {
            if(controls != null)
            {
                return controls;
            }

            return controls = new Controls();
        }
    }

    public override void OnStartAuthority()
    {
        
        //playerCamera.gameObject.SetActive(true);

        enabled = true;

        Cursor.lockState = CursorLockMode.Locked;

        Controls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
    }

    [ClientCallback]
    private void OnEnable()
    {
        Controls.Enable();
    }

    [ClientCallback]

    private void OnDisable()
    {
        Controls.Disable();
    }

    private void Look(Vector2 lookAxis)
    {
        float deltaTime = Time.deltaTime;

        xRotacao -= lookAxis.y * cameraVelocity.y * deltaTime;
        xRotacao = Mathf.Clamp(xRotacao, -xClamp, xClamp);
        Vector2 rotacaoAlvo = transform.eulerAngles;
        rotacaoAlvo.x = xRotacao;

        playerCamera.eulerAngles = rotacaoAlvo;

        //visor.transform.position = transform.TransformPoint(playerCamera.transform.localPosition.x, playerCamera.transform.localPosition.y ,visorOffset.localPosition.z);
        //visor.transform.rotation = Quaternion.Euler(rotacaoAlvo.x,0,0);

        playerTransform.Rotate(0, lookAxis.x * cameraVelocity.x * deltaTime, 0f);
    }
}
