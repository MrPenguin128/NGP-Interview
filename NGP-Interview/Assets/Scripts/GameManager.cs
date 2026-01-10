using Entities.Player;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static Player Player { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Static reference to the Player entity to maintain easy and simple access.
    #region Player Reference
    public static void RegisterPlayer(Player player)
    {
        if (Player != null && Player != player)
        {
            Debug.LogWarning($"Trying to register a second player {player.gameObject?.name}.");
            return;
        }
        Player = player;
    }
    public static void Unregister(Player player)
    {
        if (Player == player)
            Player = null;
    }
    #endregion
}
