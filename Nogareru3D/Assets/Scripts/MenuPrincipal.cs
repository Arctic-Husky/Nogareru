using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] private NetworkManagerNogareru networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    private void HostLobby()
    {
        networkManager.StartHost();

        landingPagePanel.SetActive(false);
    }
}
