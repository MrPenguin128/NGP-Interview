using UnityEngine;

[CreateAssetMenu(fileName = "New Combo Data", menuName = "Scriptable Objects/Combo Data")]
public class ComboDataObject : ScriptableObject
{
    public ComboHit[] hits;
    public float ComboResetTime = 1f;

}
[System.Serializable]
public class ComboHit
{
    [Range(0, 360)]
    public float AttackAngle;

    public float AttackRangeMultiplier = 1f;
    public float DamageMultiplier = 1f;
    public float MoveSpeedModifier = 1f;
    public float HitDelay = 0.1f;
}

