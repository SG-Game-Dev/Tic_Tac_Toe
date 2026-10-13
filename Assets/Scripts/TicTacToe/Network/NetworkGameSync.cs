using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TicTacToe
{
    // Online game का एकमात्र NetworkObject. सिर्फ संदेश पहुँचाता है, नियम नहीं जानता.
    // Host = X (पहली चाल), Client = O. Host हर चाल को जाँचकर दोनों को भेजता है.
    public class NetworkGameSync : NetworkBehaviour
    {
        public static NetworkGameSync Instance { get; private set; }

        TicTacToeGameController controller;
        readonly HashSet<ulong> readyPlayers = new HashSet<ulong>();

        void Awake()
        {
            DontDestroyOnLoad(gameObject); // menu scene -> game scene में साथ चले
        }

        public override void OnNetworkSpawn() { Instance = this; }

        public override void OnNetworkDespawn()
        {
            if (Instance == this) Instance = null;
        }

        // ================= Game शुरू करना (सिर्फ Host) =================
        public void StartGame(int sceneIndex)
        {
            if (!IsServer) return;
            LoadGameClientRpc(sceneIndex);
        }

        [Rpc(SendTo.ClientsAndHost)]
        void LoadGameClientRpc(int sceneIndex)
        {
            UiManager.isOnline = true;
            UiManager.isHuman = true;
            SceneManager.LoadScene(sceneIndex);
        }

        // ================= Game scene तैयार होने की खबर =================
        // Controller अपने scene में आकर खुद को register करता है
        public void Register(TicTacToeGameController c)
        {
            controller = c;
            PlayerReadyServerRpc();
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        void PlayerReadyServerRpc(RpcParams rpcParams = default)
        {
            readyPlayers.Add(rpcParams.Receive.SenderClientId);

            // दोनों तैयार हों तभी match शुरू
            if (readyPlayers.Count >= OnlineSession.MaxPlayers)
            {
                readyPlayers.Clear();
                BeginMatchClientRpc();
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        void BeginMatchClientRpc()
        {
            if (controller != null) controller.BeginOnlineMatch();
        }

        // ================= चाल =================
        public void RequestMove(int index) => MoveServerRpc(index);

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        void MoveServerRpc(int index, RpcParams rpcParams = default)
        {
            bool senderIsHost = rpcParams.Receive.SenderClientId == NetworkManager.ServerClientId;

            // Host जाँचता है: बारी सही है? खाना खाली है? game चल रहा है?
            if (controller == null || !controller.IsValidOnlineMove(index, senderIsHost)) return;

            MoveClientRpc(index); // दोनों (host समेत) पर चाल चलेगी
        }

        [Rpc(SendTo.ClientsAndHost)]
        void MoveClientRpc(int index)
        {
            if (controller != null) controller.ApplyNetworkMove(index);
        }

        // ================= Restart =================
        public void RequestRestart() => RestartServerRpc();

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        void RestartServerRpc()
        {
            RestartClientRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        void RestartClientRpc()
        {
            if (controller != null) controller.ResetMatch();
        }
    }
}