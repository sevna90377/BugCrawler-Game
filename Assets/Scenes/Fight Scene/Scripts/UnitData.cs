using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Unit/UnitData")]
public class UnitData : ScriptableObject
{
    public string unitName = "NewUnit";

    public int tissue;
    public int speed;
    public int strength;
    public int power;
    public int aggression;
    public int preservation;

    public bool isFriendly;
    public bool isFrontlane;
    public bool isBacklane;

    public Sprite unitSprite;
    public Ability[] abilities;
}