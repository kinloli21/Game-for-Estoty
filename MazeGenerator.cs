using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private GameObject _treasurePrefab;

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    private MazeCell[,] _mazeGrid;
    private MazeCell _lastVisitedCell;

    void Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        float cellScale = 16.0f; // Visual scale
        float internalScale = 1.0f; // Internal scale for calculations

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x * cellScale, 0, z * cellScale), Quaternion.identity);
            }
        }

        GenerateMaze(null, _mazeGrid[0, 0]);

        // After maze generation, spawn the Treasure at the last visited cell
        if (_lastVisitedCell != null && _treasurePrefab != null)
        {
            Instantiate(_treasurePrefab, new Vector3(_lastVisitedCell.transform.position.x * internalScale, 0, _lastVisitedCell.transform.position.z * internalScale), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Treasure prefab or last visited cell is null. Treasure not spawned.");
        }

        // Spawn enemies at random locations in the maze
        SpawnEnemies(2, internalScale);
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        _lastVisitedCell = currentCell;

        ClearWalls(previousCell, currentCell);
        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCell = GetUnvisitedCells(currentCell);

        return unvisitedCell.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x / 16; // Adjust for cell scale
        int z = (int)currentCell.transform.position.z / 16; // Adjust for cell scale

        List<MazeCell> unvisitedCells = new List<MazeCell>();

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];
            if (!cellToRight.IsVisited)
            {
                unvisitedCells.Add(cellToRight);
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];
            if (!cellToLeft.IsVisited)
            {
                unvisitedCells.Add(cellToLeft);
            }
        }

        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];
            if (!cellToFront.IsVisited)
            {
                unvisitedCells.Add(cellToFront);
            }
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];
            if (!cellToBack.IsVisited)
            {
                unvisitedCells.Add(cellToBack);
            }
        }

        return unvisitedCells;
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        int x = (int)currentCell.transform.position.x / 16; // Adjust for cell scale
        int z = (int)currentCell.transform.position.z / 16; // Adjust for cell scale

        float cellScale = 16.0f; // Adjust this based on your actual cell scale

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
        }
        else if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
        }
        else if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
        }
        else if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
        }

        // Additional logic for adjusting positions based on the cell scale
        Vector3 adjustedPosition = new Vector3(x * cellScale, 0, z * cellScale);
        currentCell.transform.position = adjustedPosition;
    }

    private void SpawnEnemies(int numberOfEnemies, float internalScale)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Get a random cell inside the maze with walls on all sides
            MazeCell randomCell = GetRandomInnerCell();

            if (randomCell != null && _enemyPrefab != null)
            {
                Instantiate(_enemyPrefab, new Vector3(randomCell.transform.position.x * internalScale, 0, randomCell.transform.position.z * internalScale), Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Enemy prefab or selected cell is null. Enemy not spawned.");
            }
        }
    }

    private MazeCell GetRandomInnerCell()
    {
        var innerCells = _mazeGrid.Cast<MazeCell>().Where(cell => IsInnerCell(cell)).ToList();

        if (innerCells.Count > 0)
        {
            return innerCells[Random.Range(0, innerCells.Count)];
        }

        return null;
    }

    private bool IsInnerCell(MazeCell cell)
    {
        int x = (int)cell.transform.position.x / 16; // Adjust for cell scale
        int z = (int)cell.transform.position.z / 16; // Adjust for cell scale

        return x > 0 && x < _mazeWidth - 1 && z > 0 && z < _mazeDepth - 1;
    }
}
