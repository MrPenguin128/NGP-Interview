using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField] Color damageColor;
    [SerializeField] Color criticalColor;
    [SerializeField] Color healColor;
    Color textColor;
    float disappearTimer;
    public static DamagePopup Create(Vector3 position, float damage, bool isCritical)
    {
        var popup = Instantiate(GameAssetsObject.Instance.DamagePopupPrefab, position, GameAssetsObject.Instance.DamagePopupPrefab.transform.rotation);
        popup.SetupDamage(damage, isCritical);
        return popup;
    }
    public static DamagePopup CreateHealing(Vector3 position, float heal)
    {
        var popup = Instantiate(GameAssetsObject.Instance.DamagePopupPrefab, position, GameAssetsObject.Instance.DamagePopupPrefab.transform.rotation);
        popup.SetupHealing(heal);
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
    public void SetupHealing(float heal) => Setup(heal, 1, healColor);
    public void SetupDamage(float damage, bool isCritical)
    {
        if(isCritical)
            Setup(damage, 1.2f, criticalColor);
        else
            Setup(damage, 1.2f, damageColor);
    }
    void Setup(float value, float size, Color color)
    {
        text.fontSize *= size;
        textColor = color;
        text.color = textColor;
        text.text = value.ToString("#.#").Replace(',', '.');
        disappearTimer = 0.5f;
    }

}
