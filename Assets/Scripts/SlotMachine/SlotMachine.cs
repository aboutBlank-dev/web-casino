using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    [SerializeField] SlotMachineReel reelPrefab;
    [SerializeField] int reelAmount;

    List<SlotMachineReel> reels;

    void Awake()
    {
        reels = new List<SlotMachineReel>();
    }

    void Start()
    {
        for (int i = 0; i < reelAmount; i++)
        {
            SlotMachineReel reel = Instantiate(reelPrefab, transform);
            reels.Add(reel);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            CheckForWins();
    }

    public void ToggleSpinning()
    {
        StartCoroutine(OffsetToggleReels());
    }

    void CheckForWins(int minGroupAmount = 5)
    {
        List<List<SlotMachineSymbol>> allSymbols = new List<List<SlotMachineSymbol>>();

        //get symbols as 2D array.
        foreach (SlotMachineReel reel in reels)
        {
            allSymbols.Add(reel.GetShowingSymbols());
        }

        int rows = allSymbols.Count;
        int cols = allSymbols[0].Count;

        var visited = new HashSet<(int, int)>(); // Track visited cells as a set of coordinates
        var groupings = new List<List<SlotMachineSymbol>>(); // List to store all groupings

        // Directions for adjacent cells (up, down, left, right)
        int[] dr = { -1, 1, 0, 0 };
        int[] dc = { 0, 0, -1, 1 };

        // Helper method to perform DFS and get a group
        List<SlotMachineSymbol> FloodFill(int r, int c, int symbolID)
        {
            var stack = new Stack<(int, int)>();
            var group = new List<SlotMachineSymbol>();

            stack.Push((r, c));
            visited.Add((r, c));

            while (stack.Count > 0)
            {
                var (curR, curC) = stack.Pop();
                group.Add(allSymbols[curR][curC]); //Change these if it does not work

                // Explore all neighbors
                for (int i = 0; i < 4; i++)
                {
                    int newR = curR + dr[i];
                    int newC = curC + dc[i];

                    // Check bounds and conditions
                    if (newR >= 0 && newR < rows && newC >= 0 && newC < cols &&
                        !visited.Contains((newR, newC)) && allSymbols[newR][newC].symboldID == symbolID)
                    {
                        visited.Add((newR, newC));
                        stack.Push((newR, newC));
                    }
                }
            }

            return group;
        }

        // Main logic to iterate through the grid
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (!visited.Contains((r, c)))
                {
                    List<SlotMachineSymbol> group = FloodFill(r, c, allSymbols[r][c].symboldID);

                    if (group.Count >= minGroupAmount)
                        groupings.Add(group);
                }
            }
        }

        //Break symbols
        foreach (List<SlotMachineSymbol> group in groupings)
        {
            foreach (SlotMachineSymbol symbol in group)
            {
                symbol.PlayBreakAnimation();
            }
        }
    }

    IEnumerator OffsetToggleReels()
    {
        float timeBetweenReels = 0.1f;
        foreach (SlotMachineReel reel in reels)
        {
            reel.ToggleSpinning();
            yield return new WaitForSeconds(timeBetweenReels);
        }
    }
}
