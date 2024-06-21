using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameManagement gameManagement;
    public TileController tilePrefab;
    private TileGrid grid;
    public TileState[] tileStates;
    private List<TileController> tiles;
    private bool waiting = false;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<TileController>(16);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!waiting)
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveTiles(Vector2Int.up, 0, 1, grid.height - 2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveTiles(Vector2Int.down, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
        }
    }

    public void ClearBoard()
    {
        foreach(TileCell cell in grid.cells)
        {
            cell.tileController = null;
        }
        foreach (TileController tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        tiles.Clear();
    }

    public void CreateTile()
    {
        TileController tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }

    private void MoveTiles(Vector2Int direction, int startX, int increX, int startY, int increY)
    {
        bool moved = false;
        for (int x = startX; x >= 0 && x < grid.width; x += increX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += increY)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.occupied)
                {
                   if (MoveTile(cell.tileController, direction))
                   {
                        moved = true;
                   }
                }
            }
        }
        if (moved)
        {
            StartCoroutine(WaitForMove());
        }
    }

    private bool MoveTile(TileController tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);
        while (adjacent != null)
        {
            if (adjacent.occupied)
            {
                if (CanMerge(tile, adjacent.tileController))
                {
                    Merge(tile, adjacent.tileController);
                }
                break;
            }
            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }
        return false;
    }

    private bool CanMerge(TileController a, TileController b)
    {
        return a.number == b.number && !b.locked;
    }

    private void Merge(TileController a, TileController b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int number = b.number * 2;
        b.SetState(tileStates[index], number);
        gameManagement.IncreaseScore(number);
    }

    private int IndexOf(TileState state)
    {
        for(int i = 0; i < tileStates.Length; i++)
        {
            if(state == tileStates[i])
            {
                return i;
            }
        }
        return -1;
    }
    private IEnumerator WaitForMove()
    {
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;
        foreach(TileController t in tiles)
        {
            t.locked = false;
        }
        if (tiles.Count != grid.size)
        {
            CreateTile();
        }
        if (CheckGameOver())
        {
            gameManagement.GameOver();
        }
        
    }

    private bool CheckGameOver()
    {
        if(tiles.Count != grid.size)
        {
            return false;
        }

        foreach(var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);
            if (up != null && CanMerge(tile, up.tileController))
            {
                return false;
            }
            if (down != null && CanMerge(tile, down.tileController))
            {
                return false;
            }
            if (left != null && CanMerge(tile, left.tileController))
            {
                return false;
            }
            if (right != null && CanMerge(tile, right.tileController))
            {
                return false;
            }
        }
        return true;
    }
}
