using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public int controlMode = 1;      //1鼠标控制，0坐标控制，在其他脚本中用作移动锁；
    public float moveSpeed = 5f;
    public LayerMask obstacleLayer;
    public float nodeRadius = 0.5f;
    public Vector2 targetPosition;
    public Vector2 MoveDirection => _currentDirection;
    public float CurrentSpeed => _currentSpeed;
    public bool IsPathfinding => currentPath != null;

    private Vector2 _currentDirection;
    private float _currentSpeed;

    private List<Vector2> currentPath;
    private int currentPathIndex;
    private Grid grid;

    private void Start()
    {
        obstacleLayer = LayerMask.NameToLayer("Ob");
        moveSpeed = 2f;
        targetPosition = transform.position;
        // 检查是否是玩家选择的英雄
        if (gameObject.name == GeneralDataManager.SelectedHeroName)
        {
            controlMode = 1; // 玩家控制模式
            Debug.Log($"{gameObject.name} 设置为玩家控制");
        }
        else
        {
            controlMode = 0; // AI控制模式
        }
    }
    void Update()
    {
        HandleInput();
        FollowPath();
    }

    public void HandleInput()
    {
        if (controlMode == 1 && Input.GetMouseButtonDown(0))
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            StartPathfinding(target);
        }
        else if (controlMode == 0)
        {
            StartPathfinding(targetPosition);
        }
    }

    public void StartPathfinding(Vector2 target)
    {
        grid = new Grid(transform.position, target, nodeRadius, obstacleLayer);
        currentPath = AStar.FindPath(transform.position, target, grid);
        currentPathIndex = 0;
    }

    public void FollowPath()
    {
        if (currentPath == null || currentPathIndex >= currentPath.Count)
        {
            currentPath = null;
            _currentSpeed = 0f;
            _currentDirection = Vector2.zero;
            return;
        }

        Vector2 currentWaypoint = currentPath[currentPathIndex];
        Vector2 newPosition = Vector2.MoveTowards(transform.position, currentWaypoint, moveSpeed * Time.deltaTime);

        // 计算实际移动方向
        _currentDirection = (newPosition - (Vector2)transform.position).normalized;
        _currentSpeed = _currentDirection.magnitude * moveSpeed;

        transform.position = newPosition;

        if (Vector2.Distance(transform.position, currentWaypoint) < 0.05f)
        {
            currentPathIndex++;
        }
    }
}

public class Node
{
    public bool walkable;
    public Vector2 worldPosition;
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public Node parent;

    public int fCost => gCost + hCost;

    public Node(bool walkable, Vector2 worldPos, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPos;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}

public class Grid
{
    public Node[,] nodes;
    public Vector2 bottomLeft;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    public Grid(Vector2 start, Vector2 end, float nodeRadius, LayerMask obstacleLayer)
    {
        nodeDiameter = nodeRadius * 2;
        CreateGrid(start, end, nodeRadius, obstacleLayer);
    }

    void CreateGrid(Vector2 start, Vector2 end, float nodeRadius, LayerMask obstacleLayer)
    {
        Vector2 size = new Vector2(
            Mathf.Abs(start.x - end.x) + 10f,
            Mathf.Abs(start.y - end.y) + 10f
        );

        bottomLeft = (Vector2)(start + end) * 0.5f - size * 0.5f;
        gridSizeX = Mathf.CeilToInt(size.x / nodeDiameter);
        gridSizeY = Mathf.CeilToInt(size.y / nodeDiameter);

        nodes = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = bottomLeft + new Vector2(
                    x * nodeDiameter + nodeRadius,
                    y * nodeDiameter + nodeRadius
                );

                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius, obstacleLayer);
                nodes[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX = (worldPosition.x - bottomLeft.x) / (gridSizeX * nodeDiameter);
        float percentY = (worldPosition.y - bottomLeft.y) / (gridSizeY * nodeDiameter);
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(Mathf.FloorToInt(gridSizeX * percentX), 0, gridSizeX - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(gridSizeY * percentY), 0, gridSizeY - 1);
        return nodes[x, y];
    }
}

public static class AStar
{
    public static List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos, Grid grid)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                   (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in GetNeighbours(currentNode, grid))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return null;
    }

    static List<Vector2> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    static int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);
        return (dstX > dstY) ?
            14 * dstY + 10 * (dstX - dstY) :
            14 * dstX + 10 * (dstY - dstX);
    }

    static List<Node> GetNeighbours(Node node, Grid grid)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < grid.nodes.GetLength(0) &&
                    checkY >= 0 && checkY < grid.nodes.GetLength(1))
                {
                    neighbours.Add(grid.nodes[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
}