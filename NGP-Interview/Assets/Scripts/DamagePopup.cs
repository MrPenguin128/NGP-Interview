using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField] Color damageColor;
    [SerializeField] Color criticalColor;
    Color textColor;
    float disappearTimer;
    public static DamagePopup Create(Vector3 position, float damage, bool isCritical)
    {
        var popup = Instantiate(GameAssetsObject.Instance.DamagePopupPrefab, position, GameAssetsObject.Instance.DamagePopupPrefab.transform.rotation);
        popup.Setup(damage, isCritical);
        return popup;
    }

    private void Update()
    {
        float moveYSpeed = 2f;
        transform.position += moveYSpeed * transform.up * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            text.color = textColor;
            if (textColor.a <= 0)
                Destroy(gameObject);
        }
    }
    public void Setup(float damage, bool isCritical)
    {
        if (isCritical)
        {
            textColor = criticalColor;
            text.fontSize *= 1.2f;
        }
        else
            textColor = damageColor;
        text.color = textColor;
        text.text = damage.ToString("#.#").Replace(',','.');
        disappearTimer = 0.5f;
    }

}
