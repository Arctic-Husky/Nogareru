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

    [Header("Game")]
    [SerializeField] private NetworkGamePlayer gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;
    [SerializeField] private GameObject roundSystem = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;

    public List<NetworkRoomPlayer> RoomPlayers { get; } = new List<NetworkRoomPlayer>();
    public List<NetworkGamePlayer> GamePlayers { get; } = new List<NetworkGamePlayer>();
    //public List<NetworkRoomPlayer> SpectatorPlayers { get; } = new List<NetworkRoomPlayer>();

    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }
    

    public override void OnStartClient()
    {
        /*if (spawnPrefabs != null)
            return;*/

        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        NetworkClient.ClearSpawners(); // Adicionado para tentar acabar com warnings de replace de prefab

        foreach (var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
            Debug.Log($"Registrou prefab {prefab.name}!");
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
        Debug.Log("Conectou client!");
        
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
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
        OnServerStopped?.Invoke();

        RoomPlayers.Clear();
        GamePlayers.Clear();
    }

    public void StartGame()
    {
        if("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" == cenaMenu)
        {
            if(!IsReadyToStart())
            {
                return;
            }

            ServerChangeScene("Assets/Scenes/Jogo_Mapa_01.unity"); // Colocar logica dps pra selecionar o mapa
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" == cenaMenu && newSceneName.StartsWith("Assets/Scenes/Jogo_Mapa"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gamePlayerInstance = Instantiate(gamePlayerPrefab);
                gamePlayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if(sceneName.Contains("Jogo_Mapa"))
        {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);

            GameObject roundSystemInstance = Instantiate(roundSystem);
            NetworkServer.Spawn(roundSystemInstance);
        }
    }

    public override void OnServerReady(NetworkConnection conn) // Quando um client carregar
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}
