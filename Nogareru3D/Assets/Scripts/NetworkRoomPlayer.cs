using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TMPro;
using UnityEngine.UI;

public class NetworkRoomPlayer : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[10];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[10]; // Mudar este para imagem depois
    [SerializeField] private Button startGameButton = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))] // O hook chama o metodo quando a variavel sincronizada se altera
    public string DisplayName = "Carregando...";

    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool isLeader;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    private NetworkManagerNogareru room;

    public NetworkManagerNogareru Room
    {
        get
        {
            if (room != null)
            {
                return room;
            }
            return room = NetworkManager.singleton as NetworkManagerNogareru;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(InputNomeJogador.NomeDisplay);

        lobbyUI.SetActive(true);

    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);

        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if(!isLeader)
        {
            return;
        }

        startGameButton.interactable = readyToStart;
    }

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Esperando pelo jogador...";
            playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ? "<color=green>Pronto!</color>" : "<color=red>Não pronto.</color>";
        }
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if(Room.RoomPlayers[0].connectionToClient != connectionToClient)
        {
            return;
        }

        //Start game
        Debug.Log("Jogo comecou!");
    }
}