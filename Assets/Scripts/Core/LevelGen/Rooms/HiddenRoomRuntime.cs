using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenRoomRuntime : RoomRuntime
{
    [SerializeField, Range(0f, 1f)] float itemInsteadOfEnemyChance = 0.7f;
    [SerializeField] Grid grid;
    [SerializeField] GameObject mask;
    [SerializeField] float fadeDuration = 0.5f;

    private bool hasTriggered;
    private Tilemap baseTilemap;
    private Tilemap wallsTilemap;
    private Tilemap fakeWallsTilemap;

    protected override void Awake()
    {
        base.Awake();

        if (grid == null) return;

        for (int i = 0; i < grid.transform.childCount; i++)
        {
            var child = grid.transform.GetChild(i);
            var tilemap = child.GetComponent<Tilemap>();

            if (child.name.Contains("Tilemap_Base"))
                baseTilemap = tilemap;
            else if (child.name.Contains("Tilemap_Walls"))
                wallsTilemap = tilemap;
            else if (child.name.Contains("Tilemap_FakeWalls"))
                fakeWallsTilemap = tilemap;
        }

        if (baseTilemap != null)
        {
            var c = baseTilemap.color;
            baseTilemap.color = new Color(c.r, c.g, c.b, 0f);
        }
        if (wallsTilemap != null)
        {
            var c = wallsTilemap.color;
            wallsTilemap.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    protected override void SetupRoom()
    {
        if (spawnPoints == null) return;

        if (Random.value <= itemInsteadOfEnemyChance)
            SpawnItem(RandomItem(), spawnPoints.GetRandomItemPoint());
        else
            SpawnEnemy(RandomEnemy(), spawnPoints.GetRandomEnemyPoint());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (other.GetComponent<HurtboxGroup>() && other.GetComponentInParent<PlayerController>())
        {
            hasTriggered = true;
            if (mask != null) mask.SetActive(false);
            StartCoroutine(FadeTilemaps());
        }
    }

    private IEnumerator FadeTilemaps()
    {
        Color fakeWallsColor = fakeWallsTilemap?.color ?? Color.white;
        Color baseColor = baseTilemap?.color ?? Color.white;
        Color wallsColor = wallsTilemap?.color ?? Color.white;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            if (fakeWallsTilemap != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, t);
                fakeWallsTilemap.color = new Color(fakeWallsColor.r, fakeWallsColor.g, fakeWallsColor.b, alpha);
            }
            if (baseTilemap != null)
            {
                float alpha = Mathf.Lerp(0f, 1f, t);
                baseTilemap.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            }
            if (wallsTilemap != null)
            {
                float alpha = Mathf.Lerp(0f, 1f, t);
                wallsTilemap.color = new Color(wallsColor.r, wallsColor.g, wallsColor.b, alpha);
            }

            yield return null;
        }

        if (fakeWallsTilemap != null)
            fakeWallsTilemap.color = new Color(fakeWallsColor.r, fakeWallsColor.g, fakeWallsColor.b, 0f);
        if (baseTilemap != null)
            baseTilemap.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
        if (wallsTilemap != null)
            wallsTilemap.color = new Color(wallsColor.r, wallsColor.g, wallsColor.b, 1f);
    }
}