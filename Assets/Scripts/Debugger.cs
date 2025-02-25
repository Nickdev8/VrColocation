﻿using System;
using System.Collections.Generic;
using SaintsField;
using SaintsField.Playa;
using Unity.Netcode;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    public HostList hostList;
    public int previewNumberPlayers;
    
    private void Awake()
    {
        // _networkObject = GetComponent<NetworkObject>();
    }
    
    [Button]
    void tome()
    {
        WriteTestToMeRpc();
    }
    
    [Button]
    void tonotme()
    {
        WriteTestNotMeRpc();
    }
    
    [Button]
    void toserver()
    {
        WriteTestNotMeRpc();
    }

    public void SpawnSpawnPoints()
    {
        if (hostList.isActiveAndEnabled || !NetworkManager.Singleton.IsHost)
            return;
        
        // only runs when there is a hostList and its active
        foreach (PlayerNetwork player in FindObjectsOfType<PlayerNetwork>())
        {
            if (player.NetworkObjectId == NetworkManager.Singleton.LocalClientId)
                player.logger.LogErrorText($"HostList is {hostList} with {previewNumberPlayers}");
        }
        hostList.spawnPointMaker.SpawnSpawnPoint(true, previewNumberPlayers, true);
    }
    
    
    [Rpc(SendTo.Me, RequireOwnership = false)]
    private void WriteTestToMeRpc()
    {
        foreach (PlayerNetwork player in FindObjectsOfType<PlayerNetwork>())
        {
            player.logger.LogErrorText("this is the host");
        }
    }
    
    [Rpc(SendTo.NotMe, RequireOwnership = false)]
    private void WriteTestToHostRpc()
    {
        foreach (PlayerNetwork player in FindObjectsOfType<PlayerNetwork>())
        {
            player.logger.LogErrorText("this is the host");
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void WriteTestNotMeRpc()
    {
        // only runs when there is a hostList and its active
        foreach (PlayerNetwork player in FindObjectsOfType<PlayerNetwork>())
        {
            player.logger.LogErrorText("this is not the sender");
        }
    }
}