using UnityEngine;
using UnityEngine.UI;

public class SlotMachineSymbol : MonoBehaviour
{
    public Image image;

    [SerializeField] SlotMachineSymbolObject[] symbolObjects;
    [SerializeField] UISpriteAnimation breakAnimation;
    [SerializeField] UISpriteAnimation idleAnimation;

    public SlotMachineSymbolObject symbol;
    public int symboldID => symbol.id;

    private bool broken = false;
    public bool IsBroken => broken;

    [HideInInspector] public SlotMachineReel reel;


    public void Init(SlotMachineReel reel)
    {
        this.reel = reel;
        SetRandomSymbol();
    }

    public void SetSymbol(SlotMachineSymbolObject symbol)
    {
        this.symbol = symbol;
        broken = false;

        image.enabled = true;
        image.sprite = symbol.idleSprites[0];

        breakAnimation.sprites = symbol.breakingSprites;
        idleAnimation.sprites = symbol.idleSprites;

        breakAnimation.Stop();
        idleAnimation.PlayLoop();
    }

    public void SetRandomSymbol()
    {
        SlotMachineSymbolObject randomSymbol = symbolObjects[Random.Range(0, symbolObjects.Length)];
        SetSymbol(randomSymbol);
    }

    public void PlayBreakAnimation()
    {
        idleAnimation.Stop();
        breakAnimation.PlayOnce(OnBreakAnimationFinish);
        broken = true;
    }

    private void OnBreakAnimationFinish()
    {
        image.enabled = false;
    }
}
