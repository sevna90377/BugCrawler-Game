using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Unit/UnitData")]
public class UnitData : ScriptableObject
{
    public string unitName = "NewUnit";
    public int maxHP;
    public Sprite unitSprite;
    public int energyGain;
    public Ability[] abilities;
}