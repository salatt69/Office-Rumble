using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using URandom = UnityEngine.Random;

public class LevelGeneration : MonoBehaviour
{
    public enum RoomType
    {
        Empty,
        Start,
        Normal,
        Merchant,
        Hidden,
        Exit
    }

    [Header("Grid Settings")]
    [SerializeField] int width;
    [SerializeField] int height;
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

        (int x, int y) startRoom = (width / 2, height / 2);
        var (map, exitRoom) = GenerateLevel(width, height, maxNormalRooms, startRoom);

        ClearPreviousLevel();
        SpawnLevel(map);

        watch.Stop();
        Debug.Log($"Generation time (ms): {watch.Elapsed.TotalMilliseconds:F3} (ticks): {watch.ElapsedTicks}");
    }

    (RoomType[,] map, (int x, int y) levelExit) GenerateLevel(
        int width, int height, int targetRooms, (int x, int y) startRoom)
    {
        RoomType[,] map = new RoomType[width, height];
        List<(int x, int y)> placedRooms = new();

        // Start
        map[startRoom.x, startRoom.y] = RoomType.Start;
        placedRooms.Add(startRoom);

        // Normal
        int roomsCreated = 0;
        while (roomsCreated < targetRooms)
        {
            bool normalPlaced = TryPlaceRoom(
                map, placedRooms, width, height,
                RoomType.Normal, 200, _ => true
            );

            if (!normalPlaced) break;
            roomsCreated++;
        }

        if (printToConsole) Debug.Log($"Normal rooms: {roomsCreated}");

        // Merchant
        int merchantCreated = 0;
        if (URandom.Range(0f, 1f) < merchantRoomRate)
        {
            bool merchantPlaced = TryPlaceRoom(
                map, placedRooms, width, height,
                RoomType.Merchant, 200,
                neighbors => !neighbors.Contains(RoomType.Exit)
                          && !neighbors.Contains(RoomType.Hidden)
                          && !neighbors.Contains(RoomType.Start)
            );
            merchantCreated++;
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
                map, placedRooms, width, height,
                RoomType.Hidden, 200,
                neighbors => !neighbors.Contains(RoomType.Hidden)
                          && !neighbors.Contains(RoomType.Start)
                          && !neighbors.Contains(RoomType.Merchant)
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
            map, placedRooms, width, height,
            RoomType.Exit, 500,
            neighbors =>
            {
                int emptyCount = neighbors.Count(n => n == RoomType.Empty);
                return !neighbors.Contains(RoomType.Start)
                    && !neighbors.Contains(RoomType.Hidden)
                    && !neighbors.Contains(RoomType.Merchant)
                    && emptyCount >= 2;
            }
        );

        if (printToConsole) Debug.Log(exitPlaced ? "Exit room placed!" : "Failed to place exit room!");

        if (printToConsole) Debug.Log($"Total rooms placed: {placedRooms.Count}");

        // Locate Exit 
        (int x, int y) exit = (-1, -1);
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (map[x, y] == RoomType.Exit)
                    exit = (x, y);

        return (map, exit);
    }

    bool TryPlaceRoom(
        RoomType[,] map,
        List<(int x, int y)> placedRooms,
        int width,
        int height,
        RoomType roomType,
        int maxAttempts,
        Func<List<RoomType>, bool> isValidNeighborCondition)
    {
        for (int attempts = 0; attempts < maxAttempts; attempts++)
        {
            var parent = placedRooms[URandom.Range(0, placedRooms.Count)];
            var candidate = GenerateRoom(parent);

            if (!IsInBounds(candidate.x, candidate.y, width, height))
                continue;

            if (map[candidate.x, candidate.y] != RoomType.Empty)
                continue;

            List<RoomType> neighbors = GetNeighbors(map, candidate.x, candidate.y);
            if (!isValidNeighborCondition(neighbors))
                continue;

            map[candidate.x, candidate.y] = roomType;
            placedRooms.Add(candidate);
            return true;
        }

        return false;
    }

    static (int x, int y) GenerateRoom((int x, int y) parentRoom)
    {
        (int dx, int dy) dir = RandomDirection();
        return (parentRoom.x + dir.dx, parentRoom.y + dir.dy);
    }

    static (int dx, int dy) RandomDirection()
    {
        int value = URandom.Range(0, 4);
        return value switch
        {
            0 => (0, 1),   // up
            1 => (0, -1),  // down
            2 => (1, 0),   // right
            3 => (-1, 0),  // left
            _ => (0, 0)
        };
    }

    static bool IsInBounds(int x, int y, int width, int height)
        => x >= 0 && y >= 0 && x < width && y < height;

    static List<RoomType> GetNeighbors(RoomType[,] map, int x, int y)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        List<RoomType> neighbors = new(4);

        (int dx, int dy)[] dirs = { (0, 1), (0, -1), (1, 0), (-1, 0) };
        foreach (var (dx, dy) in dirs)
        {
            int nx = x + dx, ny = y + dy;
            if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                neighbors.Add(map[nx, ny]);
            else
                neighbors.Add(RoomType.Empty);
        }
        return neighbors;
    }

    void SpawnLevel(RoomType[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject roomToSpawn = map[x, y] switch
                {
                    RoomType.Start => StartRoom,
                    RoomType.Normal => NormalRoom,
                    RoomType.Merchant => MerchantRoom,
                    RoomType.Hidden => HiddenRoom,
                    RoomType.Exit => ExitRoom,
                    _ => null
                };

                if (roomToSpawn == null)
                    continue;

                Vector3 spawnPos = new Vector3(x * 1f, y * 1f, 0);
                Instantiate(roomToSpawn, spawnPos, Quaternion.identity, transform);
            }
        }
    }

    public void ClearPreviousLevel()
    {
        if (transform == null) return;

        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }
}
