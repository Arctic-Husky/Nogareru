using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private GameObject cacadorPrefab = null;

    private static List<Transform> spawnPoints = new List<Transform>();
    //private static List<Transform> cacadorSpawnPoints = new List<Transform>();

    private int nextIndex = 0;
    private int escolhido;

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

    public override void OnStartServer() => NetworkManagerNogareru.OnServerReadied += SpawnPlayer;

    [ServerCallback]
    private void OnDestroy() => NetworkManagerNogareru.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        if(nextIndex == 0)
        {
            SelecionaCacador();
        }

        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        if(spawnPoint == null)
        {
            Debug.LogError($"Sem spawn point para o player {nextIndex}");
            return;
        }

        if(nextIndex == escolhido)
        {
            GameObject cacadorInstance = Instantiate(cacadorPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
            NetworkServer.Spawn(cacadorInstance, conn);

            nextIndex = (nextIndex + 1) % spawnPoints.Count;
        }
        else
        {
            GameObject playerInstance = Instantiate(playerPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
            NetworkServer.Spawn(playerInstance, conn);

            nextIndex = (nextIndex + 1) % spawnPoints.Count;
        }
    }


    /// <summary>
    /// Metodo que seleciona que jogador vai ser o Cacador
    /// </summary>
    private void SelecionaCacador()
    {
        int escolhido = Random.Range(0,NetworkManagerNogareru.singleton.numPlayers);
        Debug.Log($"{escolhido+1}/{NetworkManagerNogareru.singleton.numPlayers}");

        this.escolhido = escolhido;
    }
}
