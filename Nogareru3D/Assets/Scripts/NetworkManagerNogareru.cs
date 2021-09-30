using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class NetworkManagerNogareru : NetworkManager
{
    [SerializeField] private int minJogadores = 2;

    [Scene]
    [SerializeField] private string cenaMenu = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer roomPlayerPrefab = null;

    public static event Action onClientConnected;
    public static event Action onClientDisconnected;

    public List<NetworkRoomPlayer> RoomPlayers { get; } = new List<NetworkRoomPlayer>();

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        onClientConnected?.Invoke();
        Debug.Log("Conectou!");
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        onClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        // Impede que um jogador se conecte em uma partida em andamento

        if("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" != cenaMenu) // Caralho porra
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayer>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if(numPlayers < minJogadores)
        {
            return false;
        }

        foreach (var player in RoomPlayers)
        {
            if(!player.IsReady)
            {
                return false;
            }
        }

        return true;
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if ("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity"== cenaMenu)
        {
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
    }
}
