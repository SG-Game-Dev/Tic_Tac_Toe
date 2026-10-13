using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace TicTacToe
{
    // WiFi LAN connection का सारा काम: Host बनना, Join करना, छोड़ना.
    // NetworkManager किसी scene में नहीं रखना है, यह इसे prefab से runtime पर बनाती है.
    public static class OnlineSession
    {
        public const int MaxPlayers = 2;

        public static event Action<string> StatusChanged;   // UI पर दिखाने का message
        public static event Action<bool> CanStartChanged;   // Host: दोस्त जुड़ा या नहीं
        public static event Action<string> Disconnected;    // connection टूटा (reason के साथ)

        static NetworkManager manager;
        static GameObject syncPrefab;

        public static bool IsHost => manager != null && manager.IsHost;

        // ---------------- Host ----------------
        public static bool StartHost(GameObject managerPrefab, GameObject gameSyncPrefab, ushort port)
        {
            if (!CreateManager(managerPrefab)) return false;
            syncPrefab = gameSyncPrefab;

            manager.ConnectionApprovalCallback = ApproveConnection;
            manager.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", port, "0.0.0.0");

            Subscribe();
            manager.OnServerStarted += OnServerStarted;

            if (!manager.StartHost())
            {
                Leave();
                StatusChanged?.Invoke("Could not start host (port busy?)");
                return false;
            }

            StatusChanged?.Invoke("Waiting for friend...");
            return true;
        }

        // ---------------- Client ----------------
        public static bool StartClient(GameObject managerPrefab, string ip, ushort port)
        {
            if (!IPAddress.TryParse(ip, out _))
            {
                StatusChanged?.Invoke("Invalid IP address");
                return false;
            }
            if (!CreateManager(managerPrefab)) return false;

            var transport = manager.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, port);
            transport.MaxConnectAttempts = 4;   // 60 बार की जगह जल्दी fail हो
            transport.ConnectTimeoutMS = 1000;

            Subscribe();

            if (!manager.StartClient())
            {
                Leave();
                StatusChanged?.Invoke("Could not start client");
                return false;
            }

            StatusChanged?.Invoke("Connecting...");
            return true;
        }

        // ---------------- Leave ----------------
        public static void Leave()
        {
            UiManager.isOnline = false;
            if (manager == null) return;

            manager.OnServerStarted -= OnServerStarted;
            manager.OnClientConnectedCallback -= OnClientConnected;
            manager.OnClientDisconnectCallback -= OnClientDisconnected;
            manager.ConnectionApprovalCallback = null;

            manager.Shutdown();
            UnityEngine.Object.Destroy(manager.gameObject);
            manager = null;
        }

        // ---------------- अंदर के helper ----------------
        static bool CreateManager(GameObject prefab)
        {
            if (manager != null) Leave();

            if (prefab == null)
            {
                StatusChanged?.Invoke("NetworkManager prefab is missing");
                return false;
            }

            manager = UnityEngine.Object.Instantiate(prefab).GetComponent<NetworkManager>();
            manager.NetworkConfig.ConnectionApproval = true;       // सिर्फ 2 players के लिए
            manager.NetworkConfig.EnableSceneManagement = false;   // scene हम खुद load करेंगे
            return true;
        }

        static void Subscribe()
        {
            manager.OnClientConnectedCallback += OnClientConnected;
            manager.OnClientDisconnectCallback += OnClientDisconnected;
        }

        static void ApproveConnection(NetworkManager.ConnectionApprovalRequest request,
                                      NetworkManager.ConnectionApprovalResponse response)
        {
            bool hasRoom = manager.ConnectedClientsIds.Count < MaxPlayers;
            response.Approved = hasRoom;
            response.CreatePlayerObject = false;
            response.Pending = false;
            if (!hasRoom) response.Reason = "Room is full";
        }

        static void OnServerStarted()
        {
            manager.OnServerStarted -= OnServerStarted;
            UnityEngine.Object.Instantiate(syncPrefab).GetComponent<NetworkObject>().Spawn();
        }

        static void OnClientConnected(ulong clientId)
        {
            if (manager == null) return;

            if (manager.IsServer)
            {
                if (manager.ConnectedClientsIds.Count >= MaxPlayers)
                {
                    StatusChanged?.Invoke("Friend joined! Press Start.");
                    CanStartChanged?.Invoke(true);
                }
            }
            else
            {
                StatusChanged?.Invoke("Connected! Waiting for host to start...");
            }
        }

        static void OnClientDisconnected(ulong clientId)
        {
            if (manager == null) return;

            if (manager.IsServer)
            {
                if (clientId == NetworkManager.ServerClientId) return; // host खुद
                CanStartChanged?.Invoke(false);
                Disconnected?.Invoke("Friend left");
            }
            else
            {
                Disconnected?.Invoke(manager.DisconnectReason);
            }
        }

        // इस phone/PC का WiFi IP (दोस्त को यही डालना है)
        public static string GetLocalIPv4()
        {
            List<string> found = new List<string>();

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up) continue;
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;

                foreach (var addr in ni.GetIPProperties().UnicastAddresses)
                {
                    if (addr.Address.AddressFamily != AddressFamily.InterNetwork) continue;

                    string ip = addr.Address.ToString();
                    if (ip.StartsWith("192.168.") || ip.StartsWith("10.") || ip.StartsWith("172."))
                        found.Add(ip);
                }
            }
            return found.Count > 0 ? string.Join(" / ", found) : "Unknown";
        }
    }
}
