using Entities.Player;
using InventorySystem.UI;
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
        [Header("Player Health Bar")]
        [SerializeField] Slider playerHealthBarSlider;
        Player player;
        private void Start()
        {
            player = GameManager.Player;
            player.GetStat(Entities.StatsSystem.StatType.MaxHealth).OnValueChanged += UpdatePlayerMaxHealth;
            player.OnChangeCurrentHealth += UpdatePlayerHealth;
            UpdatePlayerMaxHealth(player.MaxHealth);
            playerHealthBarSlider.value = player.MaxHealth;
        }

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

        public void ToggleInventory(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;
            if (inventoryUI.IsActive)
                inventoryUI.Hide();
            else
                inventoryUI.Show();
        }
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