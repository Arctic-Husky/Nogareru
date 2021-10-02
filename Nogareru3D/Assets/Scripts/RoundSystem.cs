using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class RoundSystem : NetworkBehaviour
{
    [SerializeField] private Animator animator = null;

    private NetworkManagerNogareru room;

    public NetworkManagerNogareru Room
    {
        get
        {
            if(room != null)
            {
                return room;
            }

            return room = NetworkManager.singleton as NetworkManagerNogareru;
        }
    }

    public void CountdownEnded()
    {
        animator.enabled = false;
    }

    #region Server

    public override void OnStartServer()
    {
        NetworkManagerNogareru.OnServerStopped += CleanUpServer;
        NetworkManagerNogareru.OnServerReadied += CheckToStartRound;
    }

    [ServerCallback]
    private void OnDestroy()
    {
        CleanUpServer();
    }

    [Server]
    private void CleanUpServer()
    {
        NetworkManagerNogareru.OnServerStopped -= CleanUpServer;
        NetworkManagerNogareru.OnServerReadied -= CheckToStartRound;
    }

    [ServerCallback]
    private void StartRound()
    {
        //RpcStartRound();
    }

    [Server]
    private void CheckToStartRound(NetworkConnection conn)
    {
        //if(Room.GamePlayers.Count(x => x.connectionToClient.isReady) != Room.GamePlayers)
    }

    #endregion

    #region Client



    #endregion
}
