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

    void Awake()
    {
        SetRandomSprite();
    }

    public void SetSymbol(SlotMachineSymbolObject symbol)
    {
        this.symbol = symbol;

        breakAnimation.sprites = symbol.breakingSprites;
        idleAnimation.sprites = symbol.idleSprites;

        breakAnimation.Stop();
        idleAnimation.PlayLoop();
    }

    public void SetRandomSprite()
    {
        SlotMachineSymbolObject randomSymbol = symbolObjects[Random.Range(0, symbolObjects.Length)];
        SetSymbol(randomSymbol);
    }

    public void PlayBreakAnimation()
    {
        idleAnimation.Stop();
        breakAnimation.PlayOnce(() => this.image.enabled = false);
    }
}
