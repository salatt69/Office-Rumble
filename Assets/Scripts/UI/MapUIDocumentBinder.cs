using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MapUIDocumentBinder : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] float mapScale = 10f;
    [SerializeField] Vector2 mapOffset = Vector2.zero;
    [SerializeField] float roomCheckPadding = 0.75f;

    UIDocument document;
    VisualElement mapContainer;
    VisualElement markersContainer;
    VisualElement playerMarker;

    ModularLevelGeneration levelGenerator;
    Transform playerTransform;
    Vector2 startCellWorldPos;
    Vector2 roomSize;
    float roomScale;

    readonly Dictionary<Vector2Int, VisualElement> roomMarkers = new();
    readonly HashSet<Vector2Int> discoveredRooms = new();
    readonly HashSet<Vector2Int> visibleRooms = new();
    readonly HashSet<Vector2Int> permanentlyVisibleRooms = new();

    readonly Vector2Int[] NeighborOffsets = new[]
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    public void Bind(ModularLevelGeneration levelGen, Transform player)
    {
        levelGenerator = levelGen;
        playerTransform = player;

        if (levelGenerator != null)
        {
            roomSize = levelGenerator.RoomSize;
            roomScale = levelGenerator.RoomScale;
            CalculateMapTransform();
        }
    }

    void Start()
    {
        if (levelGenerator == null)
            levelGenerator = FindAnyObjectByType<ModularLevelGeneration>();

        if (playerTransform == null)
        {
            var player = FindAnyObjectByType<PlayerController>();
            if (player != null)
                playerTransform = player.transform;
        }

        if (levelGenerator != null && playerTransform != null)
        {
            roomSize = levelGenerator.RoomSize;
            roomScale = levelGenerator.RoomScale;
            CalculateMapTransform();
            BuildMap();

            var startCell = levelGenerator.StartCell;
            discoveredRooms.Add(startCell);
            RevealRoom(startCell);
            UpdateVisibleRooms(startCell);
        }
    }

    void OnEnable()
    {
        if (document == null)
            document = GetComponent<UIDocument>();

        mapContainer = document.rootVisualElement.Q<VisualElement>("MapContainer");
        markersContainer = document.rootVisualElement.Q<VisualElement>("MarkersContainer");
        playerMarker = document.rootVisualElement.Q<VisualElement>("PlayerMarker");
    }

    void Update()
    {
        UpdatePlayerPosition();
    }

    void CalculateMapTransform()
    {
        if (levelGenerator == null) return;

        float scaledRoomSizeX = roomSize.x * roomScale;
        float scaledRoomSizeY = roomSize.y * roomScale;

        float mapWidth = 180f;
        float mapHeight = 180f;

        if (levelGenerator.TryGetBoundsRelativeToStart(out Vector2Int minOffset, out Vector2Int maxOffset))
        {
            int width = maxOffset.x - minOffset.x + 1;
            int height = maxOffset.y - minOffset.y + 1;

            float worldWidth = width * scaledRoomSizeX;
            float worldHeight = height * scaledRoomSizeY;

            mapScale = Mathf.Min(mapWidth / worldWidth, mapHeight / worldHeight);
        }

        float centerX = mapWidth * 0.5f;
        float centerY = mapHeight * 0.5f;

        mapOffset = new Vector2(centerX, centerY);
    }

    void BuildMap()
    {
        if (markersContainer == null || levelGenerator == null) return;

        markersContainer.Clear();
        roomMarkers.Clear();

        var cells = levelGenerator.Cells;
        var startCell = levelGenerator.StartCell;

        startCellWorldPos = new Vector2(startCell.x * roomSize.x * roomScale, startCell.y * roomSize.y * roomScale);

        foreach (var kv in cells)
        {
            Vector2Int cellPos = kv.Key;
            ModularLevelGeneration.CellData cell = kv.Value;

            float worldX = cellPos.x * roomSize.x * roomScale;
            float worldY = cellPos.y * roomSize.y * roomScale;

            CreateRoomMarker(cellPos, new Vector2(worldX, worldY), cell.type);
        }
    }

    void CreateRoomMarker(Vector2Int cellPos, Vector2 worldPos, LevelGeneration.Rooms roomType)
    {
        var marker = new VisualElement();
        marker.AddToClassList("map-marker");

        string typeClass = roomType switch
        {
            LevelGeneration.Rooms.Start => "map-marker-start",
            LevelGeneration.Rooms.Normal => "map-marker-normal",
            LevelGeneration.Rooms.Merchant => "map-marker-merchant",
            LevelGeneration.Rooms.Hidden => "map-marker-hidden",
            LevelGeneration.Rooms.Exit => "map-marker-exit",
            _ => "map-marker-normal"
        };

        marker.AddToClassList(typeClass);
        marker.AddToClassList("map-marker-unvisited");

        float mapX = (worldPos.x - startCellWorldPos.x) * mapScale + mapOffset.x;
        float mapY = -(worldPos.y - startCellWorldPos.y) * mapScale + mapOffset.y;

        marker.style.left = mapX - 7f;
        marker.style.top = mapY - 7f;

        markersContainer.Add(marker);
        roomMarkers[cellPos] = marker;
    }

    void UpdatePlayerPosition()
    {
        if (playerTransform == null || playerMarker == null || startCellWorldPos == Vector2.zero) return;

        float mapX = (playerTransform.position.x - startCellWorldPos.x) * mapScale + mapOffset.x;
        float mapY = -(playerTransform.position.y - startCellWorldPos.y) * mapScale + mapOffset.y;

        playerMarker.style.left = mapX - 4f;
        playerMarker.style.top = mapY - 4f;

        CheckRoomVisibility();
    }

    void CheckRoomVisibility()
    {
        if (levelGenerator == null) return;

        var cells = levelGenerator.Cells;

        float roomHalfWidth = roomSize.x * roomScale * 0.5f;
        float roomHalfHeight = roomSize.y * roomScale * 0.5f;

        Vector2Int? currentRoomPos = null;

        foreach (var kv in cells)
        {
            Vector2Int cellPos = kv.Key;

            float worldX = cellPos.x * roomSize.x * roomScale;
            float worldY = cellPos.y * roomSize.y * roomScale;

            bool isInRoom = playerTransform.position.x >= worldX - roomHalfWidth * roomCheckPadding &&
                            playerTransform.position.x <= worldX + roomHalfWidth * roomCheckPadding &&
                            playerTransform.position.y >= worldY - roomHalfHeight * roomCheckPadding &&
                            playerTransform.position.y <= worldY + roomHalfHeight * roomCheckPadding;

            if (isInRoom)
            {
                currentRoomPos = cellPos;

                if (!discoveredRooms.Contains(cellPos))
                {
                    discoveredRooms.Add(cellPos);
                    RevealRoom(cellPos);
                }
            }
        }

        if (currentRoomPos.HasValue)
        {
            UpdateVisibleRooms(currentRoomPos.Value);
        }
    }

    void RevealRoom(Vector2Int cellPos)
    {
        if (roomMarkers.TryGetValue(cellPos, out var marker))
        {
            marker.RemoveFromClassList("map-marker-unvisited");
            marker.AddToClassList("map-marker-visited");
        }

        foreach (var offset in NeighborOffsets)
        {
            Vector2Int neighborPos = cellPos + offset;
            if (IsConnected(cellPos, neighborPos))
            {
                permanentlyVisibleRooms.Add(neighborPos);
                UpdateMarkerVisibility(neighborPos);
            }
        }
    }

    void UpdateVisibleRooms(Vector2Int currentRoomPos)
    {
        visibleRooms.Clear();
        visibleRooms.Add(currentRoomPos);

        foreach (var offset in NeighborOffsets)
        {
            Vector2Int neighborPos = currentRoomPos + offset;
            if (IsConnected(currentRoomPos, neighborPos))
            {
                visibleRooms.Add(neighborPos);
            }
        }

        foreach (var kv in roomMarkers)
        {
            Vector2Int cellPos = kv.Key;
            VisualElement marker = kv.Value;

            bool shouldBeVisible = discoveredRooms.Contains(cellPos) || 
                                   permanentlyVisibleRooms.Contains(cellPos) || 
                                   visibleRooms.Contains(cellPos);

            if (shouldBeVisible)
                marker.style.display = DisplayStyle.Flex;
            else
                marker.style.display = DisplayStyle.None;
        }
    }

    void UpdateMarkerVisibility(Vector2Int cellPos)
    {
        if (!roomMarkers.TryGetValue(cellPos, out var marker)) return;

        if (discoveredRooms.Contains(cellPos))
        {
            marker.RemoveFromClassList("map-marker-unvisited");
            marker.AddToClassList("map-marker-visited");
        }

        bool shouldBeVisible = discoveredRooms.Contains(cellPos) || 
                               permanentlyVisibleRooms.Contains(cellPos) || 
                               visibleRooms.Contains(cellPos);
        marker.style.display = shouldBeVisible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    bool IsConnected(Vector2Int from, Vector2Int to)
    {
        if (!levelGenerator.Cells.TryGetValue(from, out var fromCell)) return false;
        if (!levelGenerator.Cells.ContainsKey(to)) return false;

        Vector2Int direction = to - from;
        DoorMask requiredDoor = direction switch
        {
            { x: 1 } => DoorMask.Right,
            { x: -1 } => DoorMask.Left,
            { y: 1 } => DoorMask.Up,
            { y: -1 } => DoorMask.Down,
            _ => DoorMask.None
        };

        return (fromCell.doors & requiredDoor) != 0;
    }
}
