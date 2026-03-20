using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;
using URandom = UnityEngine.Random;

[System.Flags]
public enum DoorMask
{
    None = 0,
    Up = 1 << 0,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,

    All = Up | Down | Left | Right
}

public class ModularLevelGeneration : MonoBehaviour
{
    [Serializable]
    public struct CellData
    {
        public bool occupied;
        public LevelGeneration.Rooms type;
        public DoorMask doors;
    }

    [Header("Content")]
    [SerializeField] GameContentDatabase contentDatabase;

    [Header("Grid")]
    [SerializeField] int width = 9;
    [SerializeField] int height = 9;
    [SerializeField, Range(5, 15)] int targetNormalRooms = 8;

    [Header("Special Rooms")]
    [SerializeField, Range(0f, 1f)] float merchantChance = 0.4f;
    [SerializeField, Range(0f, 1f)] float hiddenChance = 0.3f;

    [Header("World Placement")]
    [SerializeField] Vector2 roomSize = new(16f, 16f);
    [SerializeField] float roomScale = 0.75f;

    [Header("Room Modules")]
    [SerializeField] List<RoomModuleDefinition> roomModules = new();

    [Header("Debug")]
    [SerializeField] bool generateOnStart = true;
    [SerializeField] bool printToConsole = true;

    [Header("Marker Debug")]
    [SerializeField] Vector2 markerSize = new(2f, 2f);
    [SerializeField] float markerDoorOffset = 1.5f;
    [SerializeField] float markerDoorSize = 0.4f;
    [SerializeField] Vector2Int markerMapOffset = new(20, 0);

    [Header("Pathfinder")]
    [SerializeField] AstarPath pathfinderPrefab;

    readonly Dictionary<Vector2Int, CellData> cells = new();
    readonly Dictionary<Vector2Int, RoomModuleDefinition> spawned = new();

    Vector2Int startCell;
    AstarPath pathfinderInstance;

    static readonly Vector2Int[] DirVecs =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    void Start()
    {
        pathfinderInstance = Instantiate(pathfinderPrefab);
        if (!pathfinderInstance)
        {
            Debug.LogError("Pathfinder has no instance");
        }

        if (generateOnStart)
            Generate();
    }

    DoorMask NormalizeMask(DoorMask mask)
    {
        return mask & DoorMask.All;
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        Clear();

        startCell = new Vector2Int(width / 2, height / 2);

        GenerateTopology(startCell);
        AssignSpecialRooms(startCell);
        SpawnRooms();
        SpawnMarkers(markerMapOffset);
        FitPathfinderToLevel();

        if (printToConsole)
            DebugPrint();
    }

    [ContextMenu("Generate Marker Map")]
    public void GenerateMarkerMap()
    {
        Clear();

        startCell = new Vector2Int(width / 2, height / 2);

        GenerateTopology(startCell);
        AssignSpecialRooms(startCell);
        SpawnMarkers();
        FitPathfinderToLevel();

        if (printToConsole)
            DebugPrint();
    }

    void GenerateTopology(Vector2Int start)
    {
        cells.Clear();

        AddCell(start, LevelGeneration.Rooms.Start);

        List<Vector2Int> frontier = new() { start };
        int normalsPlaced = 0;

        while (normalsPlaced < targetNormalRooms && frontier.Count > 0)
        {
            Vector2Int parent = frontier[URandom.Range(0, frontier.Count)];

            List<Vector2Int> candidates = DirVecs
                .Select(d => parent + d)
                .Where(IsInBounds)
                .Where(p => !cells.ContainsKey(p))
                .ToList();

            if (candidates.Count == 0)
            {
                frontier.Remove(parent);
                continue;
            }

            Vector2Int next = candidates[URandom.Range(0, candidates.Count)];

            AddCell(next, LevelGeneration.Rooms.Normal);
            Connect(parent, next);

            frontier.Add(next);
            normalsPlaced++;
        }

        Vector2Int? exitCell = cells.Keys
            .Where(p => p != start)
            .Where(p => CountConnections(p) == 1)
            .OrderByDescending(p => Manhattan(start, p))
            .Cast<Vector2Int?>()
            .FirstOrDefault();

        if (exitCell.HasValue)
            SetRoomType(exitCell.Value, LevelGeneration.Rooms.Exit);
    }

    void AssignSpecialRooms(Vector2Int start)
    {
        List<Vector2Int> normalLeaves = cells
            .Where(kv => kv.Value.type == LevelGeneration.Rooms.Normal && CountConnections(kv.Key) == 1)
            .Select(kv => kv.Key)
            .Where(p => p != start)
            .ToList();

        if (normalLeaves.Count > 0 && URandom.value < merchantChance)
        {
            Vector2Int merchant = normalLeaves[URandom.Range(0, normalLeaves.Count)];
            SetRoomType(merchant, LevelGeneration.Rooms.Merchant);
            normalLeaves.Remove(merchant);
        }

        if (normalLeaves.Count > 0 && URandom.value < hiddenChance)
        {
            Vector2Int hidden = normalLeaves[URandom.Range(0, normalLeaves.Count)];
            SetRoomType(hidden, LevelGeneration.Rooms.Hidden);
        }
    }

    void SpawnRooms()
    {
        spawned.Clear();

        RoomContext roomContext = new()
        {
            content = contentDatabase,
            overallScale = roomScale
        };

        float scaledRoomSizeX = roomSize.x * roomScale;
        float scaledRoomSizeY = roomSize.y * roomScale;

        foreach (var kv in cells)
        {
            Vector2Int pos = kv.Key;
            CellData cell = kv.Value;

            RoomModuleDefinition prefab = PickModule(cell.type, cell.doors);
            if (!prefab)
            {
                Debug.LogError($"No module found for type={cell.type}, doors={cell.doors}");
                continue;
            }

            Vector3 worldPos = new(pos.x * scaledRoomSizeX, pos.y * scaledRoomSizeY, 0f);
            RoomModuleDefinition room = Instantiate(prefab, worldPos, Quaternion.identity, transform);

            room.transform.localScale *= roomScale;

            spawned[pos] = room;

            RoomRuntime runtime = room.GetComponent<RoomRuntime>();
            if (runtime != null)
                runtime.Initialize(roomContext);
        }
    }

    void SpawnMarkers(Vector2Int offset = new Vector2Int())
    {
        foreach (var kv in cells)
        {
            Vector2Int pos = kv.Key + offset;
            CellData cell = kv.Value;

            Vector3 worldPos = new(pos.x * roomSize.x, pos.y * roomSize.y, 0f);

            GameObject markerRoot = new GameObject($"Marker_{pos.x}_{pos.y}_{cell.type}");
            markerRoot.transform.SetParent(transform);
            markerRoot.transform.position = worldPos;

            CreateMarkerBody(markerRoot.transform, cell);
            CreateDoorMarkers(markerRoot.transform, cell.doors);
        }
    }

    void CreateMarkerBody(Transform parent, CellData cell)
    {
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Quad);
        body.name = "Body";
        body.transform.SetParent(parent, false);
        body.transform.localPosition = Vector3.zero;
        body.transform.localRotation = Quaternion.identity;
        body.transform.localScale = new Vector3(markerSize.x, markerSize.y, 1f);

        var renderer = body.GetComponent<MeshRenderer>();
        var collider = body.GetComponent<Collider>();
        if (collider) DestroyImmediate(collider);

        renderer.sharedMaterial = CreateDebugMaterial(GetRoomTypeColor(cell.type));
    }

    void CreateDoorMarkers(Transform parent, DoorMask doors)
    {
        if ((doors & DoorMask.Up) != 0)
            CreateDoorMarker(parent, "Door_Up", new Vector3(0f, markerDoorOffset, 0f), Color.white);

        if ((doors & DoorMask.Down) != 0)
            CreateDoorMarker(parent, "Door_Down", new Vector3(0f, -markerDoorOffset, 0f), Color.white);

        if ((doors & DoorMask.Left) != 0)
            CreateDoorMarker(parent, "Door_Left", new Vector3(-markerDoorOffset, 0f, 0f), Color.white);

        if ((doors & DoorMask.Right) != 0)
            CreateDoorMarker(parent, "Door_Right", new Vector3(markerDoorOffset, 0f, 0f), Color.white);
    }

    void CreateDoorMarker(Transform parent, string name, Vector3 localPos, Color color)
    {
        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Quad);
        door.name = name;
        door.transform.SetParent(parent, false);
        door.transform.localPosition = localPos;
        door.transform.localRotation = Quaternion.identity;
        door.transform.localScale = new Vector3(markerDoorSize, markerDoorSize, 1f);

        var renderer = door.GetComponent<MeshRenderer>();
        var collider = door.GetComponent<Collider>();
        if (collider) DestroyImmediate(collider);

        renderer.sharedMaterial = CreateDebugMaterial(color);
    }

    Material CreateDebugMaterial(Color color)
    {
        Shader shader = Shader.Find("Sprites/Default");
        Material mat = new Material(shader);
        mat.color = color;
        return mat;
    }

    Color GetRoomTypeColor(LevelGeneration.Rooms type)
    {
        return type switch
        {
            LevelGeneration.Rooms.Start => new Color(0.3f, 1f, 0.3f),
            LevelGeneration.Rooms.Normal => new Color(0.7f, 0.7f, 0.7f),
            LevelGeneration.Rooms.Merchant => new Color(0.2f, 0.9f, 1f),
            LevelGeneration.Rooms.Hidden => new Color(0.7f, 0.3f, 1f),
            LevelGeneration.Rooms.Exit => new Color(1f, 0.3f, 0.3f),
            _ => Color.black
        };
    }

    RoomModuleDefinition PickModule(LevelGeneration.Rooms type, DoorMask requiredDoors)
    {
        List<RoomModuleDefinition> candidates = roomModules
            .Where(m => m != null)
            .Where(m => m.roomType == type)
            .Where(m => NormalizeMask(m.doors) == NormalizeMask(requiredDoors))
            .ToList();

        if (candidates.Count == 0)
            return null;

        int totalWeight = candidates.Sum(c => Mathf.Max(1, c.weight));
        int roll = URandom.Range(0, totalWeight);

        foreach (var c in candidates)
        {
            roll -= Mathf.Max(1, c.weight);
            if (roll < 0)
                return c;
        }

        return candidates[0];
    }

    void AddCell(Vector2Int pos, LevelGeneration.Rooms type)
    {
        cells[pos] = new CellData
        {
            occupied = true,
            type = type,
            doors = DoorMask.None
        };
    }

    void SetRoomType(Vector2Int pos, LevelGeneration.Rooms type)
    {
        CellData c = cells[pos];
        c.type = type;
        cells[pos] = c;
    }

    void Connect(Vector2Int a, Vector2Int b)
    {
        Vector2Int d = b - a;

        CellData ca = cells[a];
        CellData cb = cells[b];

        if (d == Vector2Int.up)
        {
            ca.doors |= DoorMask.Up;
            cb.doors |= DoorMask.Down;
        }
        else if (d == Vector2Int.down)
        {
            ca.doors |= DoorMask.Down;
            cb.doors |= DoorMask.Up;
        }
        else if (d == Vector2Int.left)
        {
            ca.doors |= DoorMask.Left;
            cb.doors |= DoorMask.Right;
        }
        else if (d == Vector2Int.right)
        {
            ca.doors |= DoorMask.Right;
            cb.doors |= DoorMask.Left;
        }

        cells[a] = ca;
        cells[b] = cb;
    }

    int CountConnections(Vector2Int pos)
    {
        DoorMask d = cells[pos].doors;
        int count = 0;
        if ((d & DoorMask.Up) != 0) count++;
        if ((d & DoorMask.Down) != 0) count++;
        if ((d & DoorMask.Left) != 0) count++;
        if ((d & DoorMask.Right) != 0) count++;
        return count;
    }

    bool IsInBounds(Vector2Int p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;
    }

    static int Manhattan(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public bool TryGetBoundsRelativeToStart(out Vector2Int minOffset, out Vector2Int maxOffset)
    {
        minOffset = Vector2Int.zero;
        maxOffset = Vector2Int.zero;

        if (cells.Count == 0)
            return false;

        bool first = true;

        foreach (var pos in cells.Keys)
        {
            Vector2Int offset = pos - startCell;

            if (first)
            {
                minOffset = offset;
                maxOffset = offset;
                first = false;
                continue;
            }

            minOffset = Vector2Int.Min(minOffset, offset);
            maxOffset = Vector2Int.Max(maxOffset, offset);
        }

        return true;
    }

    public bool TryGetSymmetricWorldBoundsFromStart(out Vector2 center, out Vector2 size)
    {
        center = Vector2.zero;
        size = Vector2.zero;

        if (!TryGetBoundsRelativeToStart(out Vector2Int minOffset, out Vector2Int maxOffset))
            return false;

        float cellWidth = roomSize.x * roomScale;
        float cellHeight = roomSize.y * roomScale;

        center = new Vector2(startCell.x * cellWidth, startCell.y * cellHeight);

        int maxHorizontal = Mathf.Max(Mathf.Abs(minOffset.x), Mathf.Abs(maxOffset.x));
        int maxVertical = Mathf.Max(Mathf.Abs(minOffset.y), Mathf.Abs(maxOffset.y));

        int roomsWide = maxHorizontal * 2 + 1;
        int roomsHigh = maxVertical * 2 + 1;

        size = new Vector2(
            roomsWide * cellWidth * 2f,
            roomsHigh * cellHeight * 2f
        );

        return true;
    }

    public void FitPathfinderToLevel()
    {
        if (pathfinderInstance == null)
        {
            Debug.LogWarning("Pathfinder is not assigned.");
            return;
        }

        GridGraph grid = pathfinderInstance.data.gridGraph;
        if (grid == null)
        {
            Debug.LogWarning("No GridGraph found on AstarPath.");
            return;
        }

        if (!TryGetSymmetricWorldBoundsFromStart(out Vector2 center, out Vector2 size))
        {
            Debug.LogWarning("No generated cells to fit pathfinder to.");
            return;
        }

        grid.center = new Vector3(center.x, center.y, 0f);

        int nodesX = Mathf.CeilToInt(size.x / grid.nodeSize);
        int nodesY = Mathf.CeilToInt(size.y / grid.nodeSize);

        grid.SetDimensions(nodesX, nodesY, grid.nodeSize);

        pathfinderInstance.Scan();
    }

    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        cells.Clear();
        spawned.Clear();
    }

    void DebugPrint()
    {
        Debug.Log($"Generated cells: {cells.Count}");

        if (TryGetBoundsRelativeToStart(out Vector2Int minOffset, out Vector2Int maxOffset))
        {
            Debug.Log($"Relative bounds from start: min={minOffset}, max={maxOffset}");
        }

        if (TryGetSymmetricWorldBoundsFromStart(out Vector2 center, out Vector2 size))
        {
            Debug.Log($"Pathfinder center: {center}, size: {size}");
        }

        foreach (var kv in cells.OrderBy(k => k.Key.y).ThenBy(k => k.Key.x))
            Debug.Log($"{kv.Key} | {kv.Value.type} | {kv.Value.doors}");
    }
}