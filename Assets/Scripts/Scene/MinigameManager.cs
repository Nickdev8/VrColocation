﻿using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using SaintsField;
using UnityEngine.Serialization;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;
    
    public List<GameObject> minigameQueue;
    [NonReorderable] [ReadOnly] public List<GameObject> doneGames;
    
    [ReadOnly] public GameObject currentMinigame;
    [ReadOnly] public MinigameController currentController;

    private void Awake() => Instance = this;

    [Rpc(SendTo.Server)]
    public void StartNextGameRpc()
    {
        if (minigameQueue.Count == 0)
        {
            ResetAllMinigames(); return;
        }

        // Destroy previous minigame
        if (currentMinigame != null)
        {
            var oldNetworkObject = currentMinigame.GetComponent<NetworkObject>();
            if (oldNetworkObject != null)
            {
                oldNetworkObject.Despawn();
            }
            Destroy(currentMinigame);
            minigameQueue.RemoveAt(0);
        }
        
        
        // Cancels miniGame checks are not sufficient
        if (!CheckMinigame())
        {
            CancelGameServerRpc(); 
            doneGames.Add(minigameQueue[0]);
            minigameQueue.RemoveAt(0);
            StartNextGameRpc(); 
            return;
        }

        // Spawn new minigame
        currentMinigame = Instantiate(minigameQueue[0]);
        var networkObject = currentMinigame.GetComponent<NetworkObject>();
        networkObject.Spawn();
        currentController = currentMinigame.GetComponent<MinigameController>();

        doneGames.Add(minigameQueue[0]);


        
        // Initialize game
        currentController.InitializeGame();
    }

    bool CheckMinigame()
    {
        //checks if the minigame can be played with the current player count
        if (SceneNetworkManager.Instance.ConnectedClientsCount() < currentController.minimumPlayerCount) {
            Debug.LogError("MinigameManager::StartNextGameServerRpc: Players count is too small");
            return false;
        }
        //
        // add more checks if needed
        //
        return true;
    }
    
    private void CancelGameServerRpc()
    {
        var oldNetworkObject = currentController.GetComponent<NetworkObject>();
        if (oldNetworkObject != null)
        {
            oldNetworkObject.Despawn();
        }
        
        Destroy(currentController.gameObject);
    }

    private void ResetAllMinigames()
    {
        foreach (GameObject minigame in doneGames)
        {
            minigameQueue.Add(minigame);
        }
        doneGames.Clear();
    }

    public MinigameController GetCurrentController()
    {
        if (currentController == null)
            return null;
        
        return currentController;
        
    }
}