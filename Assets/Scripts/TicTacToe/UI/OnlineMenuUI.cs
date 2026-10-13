using TMPro;
using UnityEngine;

namespace TicTacToe
{
    // Menu scene का Online panel: Host / Join / Start / Cancel
    public class OnlineMenuUI : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] GameObject networkManagerPrefab;
        [SerializeField] GameObject gameSyncPrefab;

        [Header("Settings")]
        [SerializeField] int gameSceneIndex = 1;   // Build Settings में game scene का index
        [SerializeField] ushort port = 7777;

        [Header("UI")]
        [SerializeField] TMP_InputField ipInput;
        [SerializeField] TextMeshProUGUI statusText;
        [SerializeField] TextMeshProUGUI myIpText;
        [SerializeField] GameObject startGameButton;

        void OnEnable()
        {
            OnlineSession.StatusChanged += ShowStatus;
            OnlineSession.CanStartChanged += OnCanStartChanged;
            OnlineSession.Disconnected += OnDisconnected;
        }

        void OnDisable()
        {
            OnlineSession.StatusChanged -= ShowStatus;
            OnlineSession.CanStartChanged -= OnCanStartChanged;
            OnlineSession.Disconnected -= OnDisconnected;
        }

        void Start()
        {
            startGameButton.SetActive(false);
            if (ipInput != null) ipInput.text = PlayerPrefs.GetString("lastIP", "");
        }

        // ---- Buttons के OnClick में जोड़ें ----
        public void OnHostClicked()
        {
            if (OnlineSession.StartHost(networkManagerPrefab, gameSyncPrefab, port) && myIpText != null)
                myIpText.text = "Your IP: " + OnlineSession.GetLocalIPv4();
        }

        public void OnJoinClicked()
        {
            string ip = ipInput.text.Trim();
            PlayerPrefs.SetString("lastIP", ip);
            OnlineSession.StartClient(networkManagerPrefab, ip, port);
        }

        public void OnStartGameClicked()
        {
            if (NetworkGameSync.Instance != null)
                NetworkGameSync.Instance.StartGame(gameSceneIndex);
        }

        public void OnCancelClicked()
        {
            OnlineSession.Leave();
            ShowStatus("");
            if (myIpText != null) myIpText.text = "";
            startGameButton.SetActive(false);
        }

        // ---- Events ----
        void ShowStatus(string message)
        {
            if (statusText != null) statusText.text = message;
        }

        void OnCanStartChanged(bool canStart)
        {
            startGameButton.SetActive(canStart && OnlineSession.IsHost);
        }

        void OnDisconnected(string reason)
        {
            if (OnlineSession.IsHost)
            {
                ShowStatus("Friend left. Waiting...");
                startGameButton.SetActive(false);
                return;
            }

            OnlineSession.Leave();
            ShowStatus(string.IsNullOrEmpty(reason) ? "Could not connect" : reason);
        }
    }
}
