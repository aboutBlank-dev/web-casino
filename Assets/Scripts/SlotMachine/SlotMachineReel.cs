using System.Collections;
using System.Collections.Generic;
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

    private Vector2 spawnPosition; //Position of the top most symbol (out of vision)
    private float symbolHeight;

    void Awake()
    {
        symbols = new List<SlotMachineSymbol>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            symbols[0].SetRandomSymbol();
        }
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
        symbolHeight = viewportHeight / symbolCount;

        // +1 for extra symbols above
        for (int i = 0; i < symbolCount + 1; i++)
        {
            SlotMachineSymbol symbol = Instantiate(symbolPrefab, scrollRect.content);
            symbol.Init(this);

            RectTransform rectTransform = symbol.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, symbolHeight);

            symbols.Add(symbol);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        //Make sure scroll rect is all the way at the bottom
        scrollRect.verticalNormalizedPosition = 0;

        spawnPosition = symbols[0].transform.position;
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
            scrollRect.verticalNormalizedPosition = 0;

            OffsetSymbols();
        }

    }

    private void OffsetSymbols()
    {
        //Offset symbols down
        for (int i = symbols.Count - 1; i >= 1; i--)
        {
            var currentElement = symbols[i];
            var elementAbove = symbols[i - 1];

            currentElement.SetSymbol(elementAbove.symbol);
        }

        //Manually change the last (first) symbol to be a new random one.
        if (symbols[0] != null)
            symbols[0].SetRandomSymbol();
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

    //Drop the symbols and add remaining ones to fill the gaps.
    public void DropMissingSymbols()
    {
        //loop backwards through symbols array  
        //This is looping BOTTOM -> TOP
        Queue<SlotMachineSymbol> validSymbols = new Queue<SlotMachineSymbol>();
        for (int i = symbols.Count - 1; i >= 0; i--)
        {
            if (symbols[i].IsBroken)
                continue;

            validSymbols.Enqueue(symbols[i]);
        }

        //There are no "gaps" in this reel
        if (validSymbols.Count == symbols.Count) return;
        for (int i = symbols.Count - 1; i >= 0; i--)
        {
            if (validSymbols.Count > 0)
                symbols[i].SetSymbol(validSymbols.Dequeue().symbol);
            else
                symbols[i].SetRandomSymbol();
        }
    }

    void OnDrawGizmos()
    {
        float size = 0.1f; // Adjust size as needed

        Gizmos.color = Color.red; // Choose your color
        Gizmos.DrawCube(spawnPosition, new Vector3(size, size, size));
    }
}


