using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class NetworkManagerNogareru : NetworkManager
{
    [Scene]
    [SerializeField] private string cenaMenu = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer roomPlayerPrefab = null;

    public static event Action onClientConnected;
    public static event Action onClientDisconnected;

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

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if ("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity"== cenaMenu)
        {
            NetworkRoomPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }
}
