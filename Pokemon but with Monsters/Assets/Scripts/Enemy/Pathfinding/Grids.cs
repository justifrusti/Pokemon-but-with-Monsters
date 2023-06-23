using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grids : MonoBehaviour
{
    public DungeonGenerator generator;
    public Vector2 gridSize;
    public Vector3 nodeSize;
    public bool calculateGridSizeBasedOnIslandFormat;
    public Node[,] nodes;
    public LayerMask unwalkable;
    public GameObject loadingScreen;

    public void CreateGrid(DungeonGenerator n_generator)
    {
        generator = n_generator;

        if (calculateGridSizeBasedOnIslandFormat)
        {
            gridSize.x = (generator.size.x * generator.offset.x) / nodeSize.x;
            gridSize.y = (generator.size.y * generator.offset.y) / nodeSize.z;
        }

        nodes = new Node[Mathf.RoundToInt(gridSize.x + 2), Mathf.RoundToInt(gridSize.y + 2)];

        //StartCoroutine(GenerateNodesCoroutine());

        //for-loop that does the same as the Coroutine, but freezes the game
        //   vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv

        for (int i = 0; i < gridSize.x + 2; i++)
        {
            for (int j = 0; j < gridSize.y + 2; j++)
            {
                Vector3 newPosition;
                bool newWalkable;
                newPosition.x = i * nodeSize.x;
                newPosition.y = transform.position.y;
                newPosition.z = j * nodeSize.y;
                newWalkable = IsNodeWalkable(newPosition);
                newPosition.y = FindHeight(newPosition);
                nodes[i, j] = new(newPosition, newWalkable);
            }
        }
    }

    IEnumerator GenerateNodesCoroutine()
    {
        for (int i = 0; i < gridSize.x + 2; i++)
        {
            for (int j = 0; j < gridSize.y + 2; j++)
            {
                Vector3 newPosition;
                bool newWalkable;
                newPosition.x = i * nodeSize.x;
                newPosition.y = transform.position.y;
                newPosition.z = j * nodeSize.y;
                newWalkable = IsNodeWalkable(newPosition);
                newPosition.y = FindHeight(newPosition);
                nodes[i, j] = new Node(newPosition, newWalkable);
            }

            // Yield after each iteration to allow the game to continue running
            yield return new WaitForEndOfFrame();
        }

        // Execution completed
        Debug.Log("Nodes generation completed");
        StartCoroutine(GetNeighoursCoroutine());
    }

    IEnumerator GetNeighoursCoroutine()
    {
        for (int i = 0; i < gridSize.x + 2; i++)
        {
            for (int j = 0; j < gridSize.y + 2; j++)
            {
                nodes[i, j].neighbors = GetNeighbours(nodes, i, j);
                nodes[i, j].gCost = 9999;
                nodes[i, j].hCost = 9999;
            }
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Nodes neighbouring completed");
        //loadingScreen.SetActive(false);
    }

    public List<Vector3> FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Node startNode = GetClosestNode(startPosition);
        Node targetNode = GetClosestNode(targetPosition);

        List<Node> openSet = new();
        HashSet<Node> closedSet = new();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
                return GeneratePath(startNode, targetNode);

            foreach (Node neighbor in currentNode.neighbors)
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                float moveCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (moveCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = moveCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // No path found
        return null;
    }

    private List<Vector3> GeneratePath(Node startNode, Node targetNode)
    {
        LinkedList<Vector3> path = new();
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.AddFirst(currentNode.position);
            currentNode = currentNode.parent;
        }

        return new List<Vector3>(path);
    }

    public float FindHeight(Vector3 position)
    {
        if (Physics.Raycast(position, -transform.up, out RaycastHit hit, 1000))
        {
            return hit.point.y;
        }

        return position.y;
    }

    public bool IsNodeWalkable(Vector3 n_position)
    {
        if (Physics.Raycast(n_position, -transform.up, out RaycastHit hit, 1000))
        {
            return !(Physics.CheckSphere(hit.point, nodeSize.x, unwalkable));
        }

        return false;
    }

    public List<Node> GetNeighbours(Node[,] nodes, int x_index, int y_index)
    {
        List<Node> returnList = new() { };

        for (int i = x_index - 1; i <= x_index + 1; i++)
        {
            if (i >= 0 && i < gridSize.x + 1)
            {
                for (int j = y_index - 1; j <= y_index + 1; j++)
                {
                    if (j >= 0 && j < gridSize.y + 1)
                    {
                        if (nodes[i, j].walkable && nodes[i, j] != nodes[x_index, y_index])
                        {
                            returnList.Add(nodes[i, j]);
                        }
                    }
                }
            }
        }

        return returnList;
    }

    public Node GetClosestNode(Vector3 position)
    {
        float distanceToCurrentClosest = 9999;
        Vector2 index;
        index.x = 0;
        index.y = 0;

        for (int i = 0; i <= gridSize.x + 1; i++)
        {
            for (int j = 0; j <= gridSize.y + 1; j++)
            {
                if (Vector3.Distance(position, nodes[i, j].position) < distanceToCurrentClosest && nodes[i, j].walkable)
                {
                    distanceToCurrentClosest = Vector3.Distance(position, nodes[i, j].position);
                    index = new Vector2(i, j);
                }
            }
        }
        return nodes[Mathf.RoundToInt(index.x), Mathf.RoundToInt(index.y)];
    }

    private float GetDistance(Node nodeA, Node nodeB)
    {
        Vector3 nodeApos = nodeA.position;
        Vector3 nodeBpos = nodeB.position;
        nodeApos.y = 0;
        nodeBpos.y = 0;

        return Vector3.Distance(nodeApos, nodeBpos);
    }

    public Node RandomNode()
    {
        List<Node> walkableNodes = new() { };
        for (int i = 0; i <= gridSize.x + 1; i++)
        {
            for (int j = 0; j <= gridSize.y + 1; j++)
            {
                if (nodes[i, j].walkable)
                {
                    walkableNodes.Add(nodes[i, j]);
                }
            }
        }

        return walkableNodes[Random.Range(0, walkableNodes.Count - 1)];
    }

    public void OnDrawGizmos()
    {
        if (nodes == null)
        {
            return;
        }

        for (int i = 0; i < gridSize.x + 2; i++)
        {
            for (int j = 0; j < gridSize.y + 2; j++)
            {
                if (nodes[i, j].walkable)
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawCube(nodes[i, j].position, nodeSize);
            }
        }
    }
}

public class Node
{
    public Vector3 position;
    public bool walkable;
    public float gCost;
    public float hCost;
    public Node parent;
    public List<Node> neighbors;

    public Node(Vector3 n_position, bool n_walkable)
    {
        position = n_position;
        walkable = n_walkable;
        neighbors = new List<Node>();
    }

    public float FCost => gCost + hCost;
}