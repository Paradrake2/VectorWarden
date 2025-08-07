using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardUnlockNodeIngredients
{
    public Items item;
    public int amount;
}
[CreateAssetMenu(fileName = "CardUnlockNode", menuName = "Scriptable Objects/CardUnlockNode")]
public class CardUnlockNode : ScriptableObject
{
    public string unlockID;
    public string cardUnlockname;
    public Sprite icon;
    public List<CardUnlockNode> prerequisites = new();
    public CardUnlockNodeIngredients[] requirements;
    public float requiredXP;
    public bool isUnlocked = false;
    public List<SkillCard> unlockedCards = new();
    public List<SkillCard> disabledCards = new();
}
