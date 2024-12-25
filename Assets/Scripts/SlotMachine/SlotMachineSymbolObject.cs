using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotMachineSymbolObject", menuName = "Scriptable Objects/SlotMachineSymbolObject")]
public class SlotMachineSymbolObject : ScriptableObject
{
    public int id;
    public Sprite defaultSprite;
    public Sprite[] idleSprites;
    public Sprite[] breakingSprites;
}
