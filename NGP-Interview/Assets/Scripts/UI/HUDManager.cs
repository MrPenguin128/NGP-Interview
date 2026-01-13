using Entities.Player;
using Entities.StatsSystem;
using InventorySystem.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class HUDManager : Singleton<HUDManager>
    {
        [Header("References")]
        [SerializeField] InventoryUI inventoryUI;
        [SerializeField] Transform statsHolder;
        [Header("HUD Components")]
        [SerializeField] TextMeshProUGUI waveText;
        [SerializeField] GameObject savingText;
        [SerializeField] GameObject endingHUD;
        [Header("Player")]
        [Header("   Player Health Bar")]
        [SerializeField] Slider playerHealthBarSlider;
        [Header("   Player Stats")]
        [SerializeField] TextMeshProUGUI playerStatItem;
        Player player;
        private void Start()
        {
            player = GameManager.Player;
            player.GetStat(StatType.MaxHealth).OnValueChanged += UpdatePlayerMaxHealth;
            player.OnChangeCurrentHealth += UpdatePlayerHealth;
            UpdatePlayerMaxHealth(player.MaxHealth);
            playerHealthBarSlider.value = player.MaxHealth;
            SubscribeToAllStats();
        }

        #region PlayerStats
        void SubscribeToAllStats()
        {
            var stats = player.GetAllStats();
            for (int i = 0; i < stats.Count; i++)
            {
                var statText = Instantiate(playerStatItem, statsHolder);
                var stat = stats[i];
                stat.Value.OnValueChanged += SetStatText;
                SetStatText(stat.Value.Value);

                void SetStatText(float statValue)
                {
                    statText.text = $"{stat.Type}: {ItemDescriptionParser.FormatValue(stat.Type, statValue)}";
                }
            }
        }
        #endregion
        #region Player Health Bar
        private void UpdatePlayerMaxHealth(float obj)
        {
            playerHealthBarSlider.maxValue = obj;
        }

        private void UpdatePlayerHealth(float obj)
        {
            playerHealthBarSlider.value = GameManager.Player.CurrentHealth;
        }
        #endregion

        public void GameEnding()
        {
            endingHUD.SetActive(true);
        }

        public void ToggleInventory(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;
            if (inventoryUI.IsActive)
                inventoryUI.Hide();
            else
                inventoryUI.Show();
        }
        public void UpdateCurrentWave(int wave)
        {
            waveText.text = $"Current Wave: {wave:00}";
        }

        public void OnSave()
        {
            savingText.SetActive(true);
            Invoke(nameof(DisableSavingText), 3f);
        }
        void DisableSavingText() => savingText.SetActive(false);
    }

    public static class UIUtils
    {
        public static bool IsPointerOverUI()
        {
            return EventSystem.current != null &&
                   EventSystem.current.IsPointerOverGameObject();
        }
    }
}