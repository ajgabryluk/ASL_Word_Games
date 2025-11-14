using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class CrosswordGenerator : MonoBehaviour
{
    public enum Direction { Horizontal, Vertical }

    public int gridSize = 30; // initial size, can expand
    private char[,] grid;
    public List<string> words;
    public int maxWords = 10;

    public class WordLocation
    {
        public int x;
        public int y;
        public string word;
        public Direction direction;
    }
    public List<WordLocation> answerKey = new List<WordLocation>();
    public GameObject empty_box;
    public GameObject default_box;
    private List<WordLocation> currentBest = new List<WordLocation>();

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "signsList.txt");
        words = new List<string>(File.ReadAllLines(path));

        char[,] best = RepeatedGeneration(words, maxWords: maxWords, iterations: 100);
        best = ShrinkGrid(best);


        //Fix word locations after shrinking
        int rows = best.GetLength(0);
        int cols = best.GetLength(1);
        GridLayoutGroup layout = GetComponent<GridLayoutGroup>();
        layout.constraintCount = cols;
        float cellSize = Mathf.Floor((GetComponent<RectTransform>().rect.width / cols) - 1);
        layout.cellSize = new Vector2(cellSize, cellSize);
        layout.padding = new RectOffset((int)((GetComponent<RectTransform>().rect.width - (cellSize * cols + cols)) / 2), 0, 2, 0);
        foreach (var wl in answerKey)
        {
            if (FindStartIndex(best, wl.word, wl.direction, out int sx, out int sy))
            {
                wl.x = sx;
                wl.y = sy;
            }
        }

        //Set up puzzle
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if(best[y, x].Equals(' '))
                {
                    GameObject box = Instantiate(empty_box, Vector2.zero, Quaternion.identity, transform);
                    box.name = $"{y},{x}";
                }
                else
                {
                    GameObject box = Instantiate(default_box, Vector2.zero, Quaternion.identity, transform);
                    box.GetComponent<CrossWordBox>().SetLetter(best[y, x]);
                    List<WordLocation> loc = answerKey.FindAll(word => word.x == x && word.y == y);
                    if(loc.Count > 0)
                    {
                        foreach(WordLocation wl in loc)
                        {
                            Debug.Log($"{wl.x}, {wl.y}, {wl.word}, {wl.direction}");
                        }
                        box.GetComponent<CrossWordBox>().SetNumber((answerKey.IndexOf(loc[0]) + 1).ToString());
                        box.GetComponent<CrossWordBox>().wordLocation = loc;
                    }
                    else
                    {
                        box.GetComponent<CrossWordBox>().SetNumber("");
                    }
                    box.name = $"{y},{x}";
                }
            }     
        }


        // //PrintGrid(best);
        // foreach(WordLocation wl in answerKey)
        // {
        //     Debug.Log($"{wl.x}, {wl.y}, {wl.word}, {wl.direction}");
        // }
    }

    public char[,] IterativePlacement(List<string> words, int maxWords)
    {
        grid = CreateEmptyGrid(gridSize);

        // Shuffle the list
        Shuffle(words);

        // Place first word at center horizontally
        string first = words[0];
        words.RemoveAt(0);

        int mid = gridSize / 2;
        PlaceWord(first, 0, 7, Direction.Horizontal); 
        int count = 1;

        // Try placing remaining words
        while (count < maxWords && words.Count > 0)
        {
            string word = words[0];
            words.RemoveAt(0);

            bool placed = TryPlaceWord(word);

            if (placed)
                count++;
        }

        return grid;
    }

    // ---------------------------
    //   Placement Logic
    // ---------------------------

    private bool TryPlaceWord(string word)
    {
        for (int i = 0; i < word.Length; i++)
        {
            char letter = word[i];

            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    if (grid[y, x] == letter)
                    {
                        if (CanPlaceWord(word, i, x, y, Direction.Horizontal, out int px, out int py))
                        {
                            PlaceWord(word, px, py, Direction.Horizontal);
                            return true;
                        }

                        if (CanPlaceWord(word, i, x, y, Direction.Vertical, out px, out py))
                        {
                            PlaceWord(word, px, py, Direction.Vertical);
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if word can be placed with letterIndex aligned to (x,y)
    /// </summary>
    private bool CanPlaceWord(string word, int letterIndex, int x, int y, Direction dir, out int px, out int py)
    {
        px = x;
        py = y;

        // Compute starting coordinate
        if (dir == Direction.Horizontal)
            px = x - letterIndex;
        else
            py = y - letterIndex;

        // -------------------------------------------
        // ABSOLUTE CROSSWORD SIZE LIMITS
        // -------------------------------------------

        const int MAX_WIDTH = 13;   // across
        const int MAX_HEIGHT = 16;  // down

        // Horizontal width constraint
        if (dir == Direction.Horizontal)
        {
            if (px < 0) return false;
            if (px + word.Length > MAX_WIDTH) return false;
        }
        else // Vertical height constraint
        {
            if (py < 0) return false;
            if (py + word.Length > MAX_HEIGHT) return false;
        }

        // Note: underlying grid bounds also must be obeyed
        if (!CheckBounds(word, px, py, dir))
            return false;

        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // -------------------------------------------
        // Validate every cell of the placement
        // -------------------------------------------

        for (int i = 0; i < word.Length; i++)
        {
            int cx = (dir == Direction.Horizontal) ? px + i : px;
            int cy = (dir == Direction.Vertical) ? py + i : py;

            // HARD LIMITS (no writing outside 13x16 logical crossword)
            if (cx < 0 || cx >= MAX_WIDTH) return false;
            if (cy < 0 || cy >= MAX_HEIGHT) return false;

            char existing = grid[cy, cx];

            // Must either match or be empty
            if (existing != ' ' && existing != word[i])
                return false;

            bool isIntersection = (cx == x && cy == y);

            // -------------------------------------------
            // Perpendicular adjacency rule
            // -------------------------------------------

            if (dir == Direction.Horizontal)
            {
                // Above
                if (!isIntersection && cy > 0 && grid[cy - 1, cx] != ' ')
                    return false;

                // Below
                if (!isIntersection && cy < rows - 1 && grid[cy + 1, cx] != ' ')
                    return false;
            }
            else // Vertical
            {
                // Left
                if (!isIntersection && cx > 0 && grid[cy, cx - 1] != ' ')
                    return false;

                // Right
                if (!isIntersection && cx < cols - 1 && grid[cy, cx + 1] != ' ')
                    return false;
            }
        }

        // -------------------------------------------
        // Check BEFORE the word
        // -------------------------------------------

        if (dir == Direction.Horizontal)
        {
            if (px - 1 >= 0 && grid[py, px - 1] != ' ')
                return false;
        }
        else
        {
            if (py - 1 >= 0 && grid[py - 1, px] != ' ')
                return false;
        }

        // -------------------------------------------
        // Check AFTER the word
        // -------------------------------------------

        if (dir == Direction.Horizontal)
        {
            int endX = px + word.Length;
            if (endX < cols && endX < MAX_WIDTH && grid[py, endX] != ' ')
                return false;
        }
        else
        {
            int endY = py + word.Length;
            if (endY < rows && endY < MAX_HEIGHT && grid[endY, px] != ' ')
                return false;
        }

        return true;
    }

    private void PlaceWord(string word, int x, int y, Direction dir)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int i = 0; i < word.Length; i++)
        {
            int cx = (dir == Direction.Horizontal) ? x + i : x;
            int cy = (dir == Direction.Vertical) ? y + i : y;

            // SAFETY CHECK
            if (cx < 0 || cy < 0 || cx >= cols || cy >= rows)
            {
                Debug.LogError($"PlaceWord ERROR: word '{word}' out of bounds at {cx},{cy}");
                return; // Prevent crash
            }
            grid[cy, cx] = word[i];
        }
        currentBest.Add(new WordLocation() {word=word, direction=dir});
    }

    private bool CheckBounds(string word, int x, int y, Direction dir)
    {
        if (dir == Direction.Horizontal)
        {
            if (x < 0 || x + word.Length >= grid.GetLength(1)) return false;
            if (y < 0 || y >= grid.GetLength(0)) return false;
        }
        else
        {
            if (y < 0 || y + word.Length >= grid.GetLength(0)) return false;
            if (x < 0 || x >= grid.GetLength(1)) return false;
        }
        return true;
    }

    // ---------------------------
    //   Grid + Helpers
    // ---------------------------

    private char[,] CreateEmptyGrid(int size)
    {
        char[,] g = new char[size, size];
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                g[y, x] = ' ';
        return g;
    }

    private void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    private void PrintGrid(char[,] g)
    {
        string output = "";
        int rows = g.GetLength(0);
        int cols = g.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            string row = "";
            for (int x = 0; x < cols; x++)
                row += g[y, x] == ' ' ? "." : g[y, x].ToString();
            output += row + "\n";
        }

        Debug.Log(output);
    }

    public char[,] RepeatedGeneration(List<string> words, int maxWords, int iterations)
    {
        int bestScore = int.MinValue;
        char[,] bestGrid = null;

        for (int i = 0; i < iterations; i++)
        {
            // Create a *fresh* list of words so each iteration starts from the same state
            List<string> copy = new List<string>(words);

            char[,] crossword = IterativePlacement(copy, maxWords);
            int score = GenerateScore(crossword);

            if (score > bestScore)
            {
                bestScore = score;
                bestGrid = crossword;
                if(i == 0)
                {
                    answerKey = currentBest.GetRange(0, maxWords);
                }
                else
                {
                    answerKey = currentBest.GetRange(currentBest.Count-(maxWords), maxWords);
                }
            }
        }
        return bestGrid;
    }

    public int GenerateScore(char[,] grid)
    {
        int score = 0;
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (grid[y, x] != ' ')
                {
                    score++; // basic filled-cell score

                    // Check intersection: cell has both a horizontal and vertical neighbor
                    bool horiz = (x > 0 && grid[y, x - 1] != ' ') ||
                                (x < cols - 1 && grid[y, x + 1] != ' ');

                    bool vert = (y > 0 && grid[y - 1, x] != ' ') ||
                                (y < rows - 1 && grid[y + 1, x] != ' ');

                    if (horiz && vert)
                        score += 3; // bonus for intersection
                }
            }
        }

        return score;
    }

    public char[,] ShrinkGrid(char[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        int minRow = rows, maxRow = -1;
        int minCol = cols, maxCol = -1;

        // Find bounding box of used area
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (grid[y, x] != ' ')
                {
                    if (y < minRow) minRow = y;
                    if (y > maxRow) maxRow = y;

                    if (x < minCol) minCol = x;
                    if (x > maxCol) maxCol = x;
                }
            }
        }

        // No letters? Return empty 1x1 space
        if (maxRow == -1)
            return new char[1, 1] { { ' ' } };

        int newRows = maxRow - minRow + 1;
        int newCols = maxCol - minCol + 1;

        char[,] shrunk = new char[newRows, newCols];

        for (int y = 0; y < newRows; y++)
            for (int x = 0; x < newCols; x++)
                shrunk[y, x] = grid[minRow + y, minCol + x];

        return shrunk;
    }

    public bool FindStartIndex(char[,] grid, string word, Direction dir, out int startX, out int startY)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        startX = -1;
        startY = -1;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                // First letter must match
                if (grid[y, x] != word[0])
                    continue;

                // Check HORIZONTAL
                if (dir == Direction.Horizontal)
                {
                    if (x + word.Length > cols)
                        continue; // would overflow grid

                    bool match = true;
                    for (int i = 0; i < word.Length; i++)
                    {
                        if (grid[y, x + i] != word[i])
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        startX = x;
                        startY = y;
                        return true;
                    }
                }

                // Check VERTICAL
                else
                {
                    if (y + word.Length > rows)
                        continue; // would overflow grid

                    bool match = true;
                    for (int i = 0; i < word.Length; i++)
                    {
                        if (grid[y + i, x] != word[i])
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        startX = x;
                        startY = y;
                        return true;
                    }
                }
            }
        }

        return false;
    }
}




public static class ListExtensions
{
    public static T Pop<T>(this List<T> list)
    {
        if (list.Count == 0)
        {
            throw new InvalidOperationException("List is empty.");
        }

        int lastIndex = list.Count - 1;
        T lastItem = list[lastIndex];
        list.RemoveAt(lastIndex);
        return lastItem;
    }
}
