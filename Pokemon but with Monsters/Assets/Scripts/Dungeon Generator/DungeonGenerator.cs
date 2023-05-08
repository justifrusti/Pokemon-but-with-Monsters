using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public enum GenerationState { Setup, Generating, Finished, Disabled };
    public GenerationState generationState;

    [Header("Generation Ruleset")]
    public Transform generatorPosition;

    public Vector2Int size;

    public int sizeCap;

    public int startPos = 0;
    public int maxItterations = 10000;
    private int currentRooms;
    public int maxRooms;

    public List<RuleSet> rooms;

    [HideInInspector]public List<GameObject> generatedRooms;

    public Vector2 offset;

    [SerializeField]private float[] roomWeights;

    List<Cell> board;

    [Header("Seed Value's")]
    public int dungeonSeed;
    public int fillInSeed;

    public bool useFillInSeed;

    //Standard is: 0 - Up, 1 - Down, 2 - Right, 3 - Left, Note: Only change in editor when rooms arent generating correctly
    [Header("Side Indexes")]
    public int up = 0;
    public int down = 1;
    public int right = 2;
    public int left = 3;

    [System.Serializable]
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class RuleSet
    {
        public GameObject room;

        public Vector2Int minPos, maxPos;

        [Range(0.00f, 1.00f)]
        public float weight;

        public bool mustBeSpawned;
        public bool obligatory;
        public bool isOneTimeUse;

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - Can't Spawn, 1 - Can Spawn, 2 - Has to spawn in this location, 3 - Must be spawned atleast once

            if(x >= minPos.x && x <= maxPos.x && y >= minPos.y && y <= maxPos.y)
            {
                if(mustBeSpawned)
                {
                    return obligatory ? 2 : 3;
                }else
                {
                    return obligatory ? 2 : 1;
                }
            }

            return 0;
        }
    }

    void Awake()
    {
        generatorPosition = gameObject.transform;

        roomWeights = new float[rooms.Count];
    }

    void Start()
    {
        if (useFillInSeed)
        {
            dungeonSeed = fillInSeed;
        }
        else
        {
            dungeonSeed = Random.Range(0, int.MaxValue);
        }

        Random.InitState(dungeonSeed);

        if (size.x >= sizeCap && size.y >= sizeCap)
        {
            size.x = sizeCap;
            size.y = sizeCap;
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            RuleSet r = rooms[i];

            if(r.maxPos.x > size.x && r.maxPos.y > size.y)
            {
                r.maxPos.x = size.x;
                r.maxPos.y = size.y;

                r.minPos.x = (r.minPos.x / 2);
                r.minPos.y = (r.minPos.y / 2);
            }
        }

        generationState = GenerationState.Generating;
    }

    void Update()
    {
        switch(generationState)
        {
            case GenerationState.Generating:
                GenerateDungeon();
                break;

            case GenerationState.Finished:

                generationState = GenerationState.Disabled;
                break;

            case GenerationState.Disabled:
                print ("disable");
                break;
        }
    }

    void GenerateRooms()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Cell currentCell = board[(x + y * size.x)];

                if (currentCell.visited /*&& currentRooms < maxRooms*/)
                {
                    int randomRoom = -1;

                    List<int> availableRooms = new List<int>();

                    for (int i = 0; i < rooms.Count; i++)
                    {
                        int probability = rooms[i].ProbabilityOfSpawning(x, y);

                        if(probability == 3)
                        {
                            int randomizer = Random.Range(0, 2);

                            if(x >= rooms[i].minPos.x && x < (rooms[i].maxPos.x - 1) && y >= rooms[i].minPos.y && y < (rooms[i].maxPos.y - 1))
                            {
                                if (randomizer == 0)
                                {
                                    availableRooms.Add(i);
                                }
                                else
                                {
                                    randomRoom = i;
                                }
                            }else
                            {
                                randomRoom = i;
                            }
                        }
                        else if (probability == 2)
                        {
                            randomRoom = i;

                            break;
                        }
                        else if (probability == 1)
                        {
                            availableRooms.Add(i);
                        }
                                
                    }

                    if(randomRoom == -1)
                    {
                        if(availableRooms.Count > 0)
                        {
                            randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
                        }else
                        {
                            randomRoom = 0;
                        }
                    }

                    var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(x * offset.x, 0, -y * offset.y), Quaternion.identity, generatorPosition).GetComponent<RoomManager>();

                    if(!generatedRooms.Contains(newRoom.gameObject))
                    {
                        generatedRooms.Add(newRoom.gameObject); 
                    }

                    newRoom.UpdateRoom(currentCell.status);

                    if(rooms[randomRoom].isOneTimeUse)
                    {
                        rooms.RemoveAt(randomRoom);

                        for (var i = rooms.Count - 1; i > -1; i--)
                        {
                            if (rooms[i] == null)
                            {
                                rooms.RemoveAt(i);
                            }
                        }
                    }

                    newRoom.name += " " + x + " - " + y;

                    currentRooms++;
                }else
                {
                    generationState = GenerationState.Finished;
                }
            }
        }
    }

    void GenerateDungeon()
    {
        board = new List<Cell>();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int itterations = 0;

        while (itterations < maxItterations)
        {
            itterations++;

            board[currentCell].visited = true;

            if(currentCell == board.Count - 1)
            {
                break;
            }

            List<int> neighbours = CheckNeighbours(currentCell);

            if (neighbours.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbours[Random.Range(0, neighbours.Count)];

                if (newCell > currentCell)
                {
                    //Down or Right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[right] = true;
                        currentCell = newCell;
                        board[currentCell].status[left] = true;
                    }
                    else
                    {
                        board[currentCell].status[down] = true;
                        currentCell = newCell;
                        board[currentCell].status[up] = true;
                    }
                }
                else
                {
                    //Up or Left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[left] = true;
                        currentCell = newCell;
                        board[currentCell].status[right] = true;
                    }
                    else
                    {
                        board[currentCell].status[up] = true;
                        currentCell = newCell;
                        board[currentCell].status[down] = true;
                    }
                }
            }
        }

        //SpawnWeightedRoom();
        GenerateRooms();
    }  
    
    public void SpawnWeightedRoom()
    {
        ResetRoomSpawnWeights();

        float value = Random.value;

        for (int i = 0; i < rooms.Count; i++)
        {
            if (value < roomWeights[i])
            {
                GenerateRooms();
                return;
            }

            value -= roomWeights[i];
        }
    }

    private void ResetRoomSpawnWeights()
    {
        float totalRoomWeight = 0;

        for (int i = 0; i < rooms.Count; i++)
        {
            roomWeights[i] = rooms[i].weight;
            totalRoomWeight += roomWeights[i];
        }

        for (int i = 0; i < roomWeights.Length; i++)
        {
            roomWeights[i] = roomWeights[i] / totalRoomWeight;
        }
    }

    List<int> CheckNeighbours(int cell)
    {
        List<int> neighbours = new List<int>();

        //Check Up Neighbour
        if(cell - size.x >= 0 && !board[(cell - size.x)].visited)
        {
            neighbours.Add((cell - size.x));
        }

        //Check Down Neighbour
        if(cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbours.Add((cell + size.x));
        }

        //Check Right Neighbour
        if ((cell + 1) % size.x != 0 && !board[(cell + 1)].visited)
        {
            neighbours.Add((cell + 1));
        }

        //Check Left Neighbour
        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbours.Add((cell - 1));
        }

        return neighbours;
    }
}
