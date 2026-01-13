using System.Collections;
using Entities;
using Entities.Player;
using InventorySystem;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractiveObject
{
    //------------------------------------
    //          PREMADE ASSET
    //------------------------------------

    [SerializeField] LayerMask groundMask;
    [Header("Item Setings")]
    [SerializeField] ItemObject item;
    [Header("Components")]
    [SerializeField] Canvas collectCanvas;
    int amount = 1;
    bool canBeCollected = false;

    public bool CanInteract => canBeCollected;

    private void Start()
    {
        StartCoroutine(DropMovement());
    }
    public void SetItem(ItemObject item, int amount)
    {
        this.item = item;
        this.amount = amount;
        Invoke(nameof(EnablePickup), 1f);
        transform.Translate(Random.insideUnitSphere);
    }
    IEnumerator DropMovement()
    {
        float dropTime = 1f;
        Vector3 pos;
        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position + Random.insideUnitSphere * 3;
        targetPos.y = transform.position.y;
        float time = 0;
        while (time < 1)
        {
            pos = Vector3.Lerp(startPos, targetPos, time / dropTime);
                transform.position = new Vector3(pos.x, pos.y + Mathf.Sin(time / dropTime * Mathf.PI) * 2, pos.z);
            if (Physics.Raycast(transform.position, -Vector3.up, 1.05f, groundMask) && time > 0.5f)
                time = 1;
            else
                time += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    void EnablePickup() => canBeCollected = true;
    public void Die() { Destroy(gameObject); }

    public void OnEnter()
    {
        collectCanvas.gameObject.SetActive(true);
    }

    public void OnExit()
    {
        collectCanvas.gameObject?.SetActive(false);
    }

    public void OnInteract(BaseEntity entity)
    {
        if (!CanInteract || entity is not Player player) return;
        player.Inventory.AddItem(item, amount);
        Die();
    }
}
