using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using URandom = UnityEngine.Random;

public class LevelGeneration : MonoBehaviour
{
    public enum Rooms
    {
        Empty,
        Start,
        Normal,
        Merchant,
        Hidden,
        Exit
    }

    public readonly struct LevelData
    {
        public readonly Rooms[,] Map;
        public readonly Vector2Int Exit;

        public LevelData(Rooms[,] map, Vector2Int exit)
        {
            Map = map;
            Exit = exit;
        }
    }

    [Header("Grid Settings")]
    [SerializeField] int width;
    [SerializeField] int height;

    public int LevelWidth => width;
    public int LevelHeight => height;
    [SerializeField, Range(5, 10)] int maxNormalRooms;

    [Header("Hidden Room Settings")]
    [SerializeField, Range(0f, 1f)] float hiddenRoomRate;
    [SerializeField, Range(0f, 1f)] float hiddenRoomRateFalloff;
    [SerializeField, Range(0, 10)] int maxHiddenRooms;

    [Header("Merchant Room Settings")]
    [SerializeField, Range(0f, 1f)] float merchantRoomRate;

    [Header("Room Prefabs")]
    [SerializeField] GameObject StartRoom;
    [SerializeField] GameObject NormalRoom;
    [SerializeField] GameObject HiddenRoom;
    [SerializeField] GameObject MerchantRoom;
    [SerializeField] GameObject ExitRoom;

    [Header("Debug")]
    [SerializeField] bool printToConsole;

    void Start()
    {
        GenerateFromMenu();
    }

    public void GenerateFromMenu()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        Vector2Int startRoom = new(width / 2, height / 2);
        LevelData level = GenerateLevel(maxNormalRooms, startRoom);

        ClearPreviousLevel();
        SpawnLevel(level.Map);

        watch.Stop();
        Debug.Log($"Generation time (ms): {watch.Elapsed.TotalMilliseconds:F3} (ticks): {watch.ElapsedTicks}");
    }

    LevelData GenerateLevel(int targetRooms, Vector2Int startRoom)
    {
        Rooms[,] map = new Rooms[width, height];
        List<Vector2Int> placedRooms = new();

        // Start
        map[startRoom.x, startRoom.y] = Rooms.Start;
        placedRooms.Add(startRoom);

        // Normal
        int roomsCreated = 0;
        while (roomsCreated < targetRooms)
        {
            bool normalPlaced = TryPlaceRoom(
                map, placedRooms, Rooms.Normal, 200, out Vector2Int normalRoomPosition,
                _ =>
                {
                    return true;
                }
            );

            if (normalPlaced) roomsCreated++;
        }

        if (printToConsole) Debug.Log($"Normal rooms: {roomsCreated}");

        // Merchant
        int merchantCreated = 0;
        if (URandom.Range(0f, 1f) < merchantRoomRate)
        {
            bool merchantPlaced = TryPlaceRoom(
                map, placedRooms, Rooms.Merchant, 200, out Vector2Int merchantRoomPosition,
                neighbors =>
                {
                    return !neighbors.Contains(Rooms.Exit)
                        && !neighbors.Contains(Rooms.Hidden)
                        && !neighbors.Contains(Rooms.Start);
                }
            );

            if (merchantPlaced) merchantCreated++;
        }

        if (printToConsole) Debug.Log("Merchant room: " + merchantCreated);

        // Hidden
        int hiddenCreated = 0;
        float localHiddenRoomRate = hiddenRoomRate;
        for (int i = 0; i < 6; i++)
        {
            if (hiddenCreated >= maxHiddenRooms)
                break;

            if (URandom.Range(0f, 1f) > localHiddenRoomRate)
                continue;

            bool hiddenPlaced = TryPlaceRoom(
                map, placedRooms, Rooms.Hidden, 200, out Vector2Int hiddenRoomPosition,
                neighbors =>
                {
                    return !neighbors.Contains(Rooms.Hidden)
                        && !neighbors.Contains(Rooms.Start)
                        && !neighbors.Contains(Rooms.Merchant);
                }
            );

            if (hiddenPlaced)
            {
                hiddenCreated++;
                localHiddenRoomRate *= hiddenRoomRateFalloff;
            }
        }

        if (printToConsole) Debug.Log($"Hidden rooms: {hiddenCreated}");

        // Exit
        bool exitPlaced = TryPlaceRoom(
            map, placedRooms, Rooms.Exit, 500, out Vector2Int exitRoomPosition,
            neighbors =>
            {
                int emptyCount = neighbors.Count(n => n == Rooms.Empty);
                return !neighbors.Contains(Rooms.Start)
                    && !neighbors.Contains(Rooms.Hidden)
                    && !neighbors.Contains(Rooms.Merchant)
                    && emptyCount >= 2;
            }
        );

        if (printToConsole) Debug.Log(exitPlaced ? "Exit room placed!" : "Failed to place exit room!");
        if (printToConsole) Debug.Log($"Total rooms placed: {placedRooms.Count}");

        return new LevelData(map, exitRoomPosition);
    }

    bool TryPlaceRoom(
        Rooms[,] map,
        List<Vector2Int> placedRooms,
        Rooms roomType,
        int maxAttempts,
        out Vector2Int placedRoomPosition,
        Func<List<Rooms>, bool> isValidNeighborCondition)
    {
        for (int attempts = 0; attempts < maxAttempts; attempts++)
        {
            Vector2Int parent = placedRooms[URandom.Range(0, placedRooms.Count)];
            Vector2Int candidate = GenerateRoom(parent);

            if (!IsInBounds(candidate, width, height))
                continue;

            if (map[candidate.x, candidate.y] != Rooms.Empty)
                continue;

            List<Rooms> neighbors = GetNeighbors(map, candidate.x, candidate.y);
            if (!isValidNeighborCondition(neighbors))
                continue;

            placedRoomPosition = candidate;

            map[candidate.x, candidate.y] = roomType;
            placedRooms.Add(candidate);
            return true;
        }

        placedRoomPosition = default;
        return false;
    }

    static Vector2Int GenerateRoom(Vector2Int parentRoom)
    {
        Vector2Int dir = RandomDirection();
        return parentRoom + dir;
    }

    static Vector2Int RandomDirection()
    {
        int value = URandom.Range(0, 4);
        return value switch
        {
            0 => new Vector2Int(0, 1),   // up
            1 => new Vector2Int(0, -1),  // down
            2 => new Vector2Int(1, 0),   // right
            3 => new Vector2Int(-1, 0),  // left
            _ => Vector2Int.zero
        };
    }

    static bool IsInBounds(Vector2Int p, int width, int height)
        => p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;

    static List<Rooms> GetNeighbors(Rooms[,] map, int x, int y)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        List<Rooms> neighbors = new(4);

        Vector2Int[] dirs =
        {
            new(0, 1),
            new(0, -1),
            new(1, 0),
            new(-1, 0),
        };

        foreach (var d in dirs)
        {
            int nx = x + d.x, ny = y + d.y;
            if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                neighbors.Add(map[nx, ny]);
            else
                neighbors.Add(Rooms.Empty);
        }

        return neighbors;
    }

    void SpawnLevel(Rooms[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                GameObject roomToSpawn = map[x, y] switch
                {
                    Rooms.Start => StartRoom,
                    Rooms.Normal => NormalRoom,
                    Rooms.Merchant => MerchantRoom,
                    Rooms.Hidden => HiddenRoom,
                    Rooms.Exit => ExitRoom,
                    _ => null
                };

                if (roomToSpawn == null)
                    continue;

                Vector3 spawnPos = new Vector3(x * 1f, y * 1f, 0);
                Instantiate(roomToSpawn, spawnPos, Quaternion.identity, transform);
            }
    }

    public void ClearPreviousLevel()
    {
        if (transform == null) return;

        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }
}
