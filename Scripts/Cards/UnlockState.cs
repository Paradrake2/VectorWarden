using UnityEngine;
using System.Collections.Generic;
public class UnlockState : MonoBehaviour
{
    public static UnlockState Instance { get; private set; }
    private readonly HashSet<string> unlockedNodes = new();
    public readonly HashSet<SkillCard> unlockedCards = new();
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        ResetNodeUnlocks(); // should only be called once ever because this is a static singleton. Probably bad practice though
        ResetCardUnlocks(); // should only be called once ever because this is a static singleton. Probably bad practice though
    }

    public bool IsNodeUnlocked(CardUnlockNode node) => unlockedNodes.Contains(node.unlockID);
    public void NodeUnlock(CardUnlockNode node)
    {
        if (node == null) return;
        unlockedNodes.Add(node.unlockID);
    }
    public void UnlockCard(SkillCard card)
    {
        if (card == null) return;
        if (!unlockedCards.Contains(card)) unlockedCards.Add(card);
    }
    public bool IsCardUnlocked(SkillCard card) => unlockedCards.Contains(card);

    public void ResetNodeUnlocks()
    {
        unlockedNodes.Clear();
    }
    public void ResetCardUnlocks()
    {
        unlockedCards.Clear();
    }
}
