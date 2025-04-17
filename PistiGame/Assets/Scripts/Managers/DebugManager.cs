using System;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class DebugManager : MonoBehaviour
    {
        public Text logText;
        public GameObject openPanelButton, closePanelButton;
        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        public void ShowConsole()
        {
            openPanelButton.SetActive(false);
            closePanelButton.SetActive(true);
        }

        public void HideConsole()
        {
            openPanelButton.SetActive(true);
            closePanelButton.SetActive(false);
        }

        public void DeleteConsole() => logText.text = String.Empty;

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            logText.text += logString + "\n";
        }
    }
}
