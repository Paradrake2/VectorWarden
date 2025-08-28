using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "AutoAttackBehaviour", menuName = "Scriptable Objects/AutoAttackBehaviour")]
public abstract class AutoAttackBehaviour : ScriptableObject
{
    public float baseCooldown = 5f;
    public AutoAttackData attackData;


    
}
