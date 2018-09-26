using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MazeGen : MonoBehaviour {
    public int mazeX = 30;
    public int mazeY = 20;

    public bool endPlaced;

    public GameObject wallPrefab;
    public GameObject borderPrefab;
    public GameObject endTilePrefab;
    public GameObject floorPrefab;

    bool[,] maze; // 0 = open, 1 = closed.
    public HashSet<Vector2Int> frontier = new HashSet<Vector2Int>(); 

    private void Start(){
        maze = new bool[mazeX, mazeY];
        endPlaced = false;
        initMaze();
        constructPaths();
        buildMaze();
        buildBorder();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R)) {
            SceneManager.LoadScene("SampleScene");
            Time.timeScale = 1;
        }
    }

    // Construct the border of the maze.
    void buildBorder() {
        for (int x = 0; x < mazeX; x++) {
            for (int y = 0; y < mazeY; y++) {
                if (x == 0) Instantiate(borderPrefab, new Vector3(x - 1, 0, y), Quaternion.identity);
                if (x == mazeX - 1) Instantiate(borderPrefab, new Vector3(mazeX, 0, y), Quaternion.identity);
                if (y == 0) Instantiate(borderPrefab, new Vector3(x, 0, y - 1), Quaternion.identity);
                if (y == mazeY - 1) Instantiate(borderPrefab, new Vector3(x, 0, mazeY), Quaternion.identity);           
            }
        }

        // Corners
        Instantiate(borderPrefab, new Vector3(-1, 0, -1), Quaternion.identity);
        Instantiate(borderPrefab, new Vector3(-1, 0, mazeY), Quaternion.identity);
        Instantiate(borderPrefab, new Vector3(mazeX, 0, -1), Quaternion.identity);
        Instantiate(borderPrefab, new Vector3(mazeX, 0, mazeY), Quaternion.identity);

    }


    // Instantiate our maze into the world using the maze array.
    void buildMaze() {
        for (int x = 0; x < mazeX; x++) {
            for (int y = 0; y < mazeY; y++) {
                if (maze[x, y])
                {
                    Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity);
                }
                else
                {
                    if (!endPlaced && x > mazeX - 3 && y > mazeY - 3)
                    {
                        Instantiate(endTilePrefab, new Vector3(x, -1, y), Quaternion.identity);
                        endPlaced = true;
                    }
                    else {
                        Instantiate(floorPrefab, new Vector3(x, -1, y), Quaternion.identity);
                    }
                }
            }
        }
    }

    // Use randomised Prims algorithm to generate paths through our maze.
    void constructPaths() {
        // Set initial node.
        maze[0, 0] = false;
        addNeighboursToFrontier(0, 0);

        while (frontier.Count != 0) {
            // Select a random element from the frontier hashset.
            Vector2Int[] tempArray = new Vector2Int[frontier.Count];
            frontier.CopyTo(tempArray);
            int rnd = Random.Range(0, frontier.Count - 1);
            Vector2Int chosenFrontier = tempArray[rnd];

            // Choose a random open neighbour and create a path between the two.
            Vector2Int chosenNeighbour = chooseRandomNeighbour(chosenFrontier.x, chosenFrontier.y);
            maze[chosenFrontier.x, chosenFrontier.y] = false;
            maze[chosenNeighbour.x, chosenNeighbour.y] = false;
            removeMiddleTile(chosenFrontier.x, chosenFrontier.y, chosenNeighbour.x, chosenNeighbour.y);

            // Add new frontier tiles to list.
            addNeighboursToFrontier(chosenFrontier.x, chosenFrontier.y);

            // Remove our chosen element from our hashset.
            frontier.Remove(chosenFrontier);
        }

    }

    // Remove the wall tile between two chosen passages to create a pathway.
    void removeMiddleTile(int startX, int startY, int endX, int endY) {

        if (startX == endX)
        {
            int lowY = Mathf.Min(startY, endY);
            maze[startX, lowY + 1] = false;
        }
        else if (startY == endY)
        {
            int lowX = Mathf.Min(startX, endX);
            maze[lowX + 1, startY] = false;
        }
        else {
            Debug.Log("Error: Selected Tile For Removal is not Valid");
        }

    }

    // Choose and return a random open neighbour.
    Vector2Int chooseRandomNeighbour(int x, int y) {

        List<Vector2Int> candidates = new List<Vector2Int>();

        if (x - 2 >= 0 && !maze[x - 2, y])
            candidates.Add(new Vector2Int(x - 2, y));

        if (x + 2 < mazeX && !maze[x + 2, y])
            candidates.Add(new Vector2Int(x + 2, y));

        if (y - 2 >= 0 && !maze[x, y - 2])
            candidates.Add(new Vector2Int(x, y - 2));

        if (y + 2 < mazeY && !maze[x, y + 2])
            candidates.Add(new Vector2Int(x, y + 2));

        int rnd = Random.Range(0, candidates.Count - 1);

        return candidates[rnd];
    }

    // Add closed neighbours to our frontier list.
    void addNeighboursToFrontier(int x, int y) {
        if (x - 2 >= 0 && maze[x - 2, y])
            frontier.Add(new Vector2Int(x - 2, y));

        if (x + 2 < mazeX && maze[x + 2, y])
            frontier.Add(new Vector2Int(x + 2, y));

        if (y - 2 >= 0 && maze[x, y - 2]) 
            frontier.Add(new Vector2Int(x, y - 2));

        if (y + 2 < mazeY && maze[x, y + 2])
            frontier.Add(new Vector2Int(x, y + 2));
    }

    // Initialize the maze array.
    void initMaze() {
        for (int x = 0; x < mazeX; x++)
        {
            for (int y = 0; y < mazeY; y++)
            {
                maze[x, y] = true;
            }
        }
    }
}
