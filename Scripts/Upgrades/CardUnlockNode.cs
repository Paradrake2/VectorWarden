using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardUnlockNode", menuName = "Scriptable Objects/CardUnlockNode")]
public class CardUnlockNode : ScriptableObject
{
    public string unlockID;
    public string cardUnlockname;
    public Sprite icon;
    public List<CardUnlockNode> prerequisites = new();
    public Dictionary<Items, int> requirements = new();
    public float requiredXP;
    public bool isUnlocked = false;
    public List<SkillCard> unlockedCards = new();
    public List<SkillCard> disabledCards = new();
}
