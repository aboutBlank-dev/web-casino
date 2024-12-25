using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineReel : MonoBehaviour
{
    [SerializeField] SlotMachineSymbol symbolPrefab;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] VerticalLayoutGroup layoutGroup;
    [SerializeField] int symbolCount = 3;

    private List<SlotMachineSymbol> symbols;
    private float initialPosition;
    private float distanceScrolled;

    private bool spinning = false;

    void Awake()
    {
        symbols = new List<SlotMachineSymbol>();
    }

    void Start()
    {
        AddBaseSymbols();
    }

    public void BreakAllSymbols()
    {
        foreach (var symbol in symbols)
        {
            symbol.PlayBreakAnimation();
        }
    }


    void AddBaseSymbols()
    {
        float viewportHeight = scrollRect.viewport.rect.height;
        float symbolHeight = viewportHeight / symbolCount;

        // +1 for extra symbols above and below
        for (int i = 0; i < symbolCount + 1; i++)
        {
            SlotMachineSymbol symbol = Instantiate(symbolPrefab, scrollRect.content);
            RectTransform rectTransform = symbol.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, symbolHeight);

            symbols.Add(symbol);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        //Make sure scroll rect is all the way at the bottom
        scrollRect.verticalNormalizedPosition = 0;
    }

    public void Spin()
    {
        spinning = true;
        StartCoroutine(SpinRoutine());
    }

    public void StopSpinning()
    {
        spinning = false;
    }

    public void ToggleSpinning()
    {
        spinning = !spinning;
        if (spinning) StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine()
    {
        float viewportHeight = scrollRect.viewport.rect.height;
        float symbolHeight = viewportHeight / symbolCount;

        while (spinning)
        {
            initialPosition = scrollRect.content.anchoredPosition.y;
            distanceScrolled = 0;

            while (distanceScrolled < symbolHeight)
            {
                float scrollSpeed = 20f;

                scrollRect.verticalNormalizedPosition += scrollSpeed * Time.deltaTime;
                distanceScrolled = Mathf.Abs(scrollRect.content.anchoredPosition.y - initialPosition);

                // Wait for the next frame
                yield return null;
            }

            //Offset the position of the reel 
            scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, initialPosition);

            OffsetSymbols();
        }

    }

    private void OffsetSymbols()
    {
        //Offset all the symbols despite the location having been "reset" back up
        for (int i = symbols.Count - 1; i > 0; i--)
        {
            var currentElement = symbols[i];
            var nextElement = (i == 0) ? symbols[symbols.Count - 1] : symbols[i - 1];

            currentElement.SetSymbol(nextElement.symbol);
        }

        //Manually change the last (first) symbol to be a new random one.
        if (symbols[0] != null)
            symbols[0].SetRandomSprite();
    }

    //Return the list of all "Shown" symbols.
    //Essentially, removes the one that sits "above" that is just used for visual purposes.
    public List<SlotMachineSymbol> GetShowingSymbols()
    {
        //Make a copy of symbols list and remove first element.

        List<SlotMachineSymbol> newSymbols = new List<SlotMachineSymbol>(symbols);
        newSymbols.RemoveAt(0);
        return newSymbols;
    }
}


