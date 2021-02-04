using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Road : MonoBehaviour
{
    public GameObject endTile;
    public GameObject[] tilePrefabs;
    public float[] probabilities;
    public GameObject[] obstacles;
    public GameObject pointerPrefab;
    public Transform player;
    public Transform garage;
    List<Transform> roadTiles = new List<Transform>();
    public float tileSize;
    public int currentTile;
    int startOffset;
    public static Road instance;
    public float firstTileZPos;

    GameObject pointer;
    bool endSpawned;
    public UnityEvent onLevelEndEvents;
    void Awake()
    {
        if (probabilities.Length != tilePrefabs.Length)
            throw new System.Exception("Lengths of " + nameof(probabilities) + " and " + nameof(tilePrefabs) + " are not equal");

        for (int i = 1; i < probabilities.Length; i++)
            probabilities[i] += probabilities[i - 1];

        instance = this;
        for (int i = 0; i < 1; i++)
            SpawnTileAtLastPosition();
        firstTileZPos = roadTiles[0].position.z;
        pointer = Instantiate(pointerPrefab, roadTiles.Last().position, Quaternion.identity);
        //PlayerPrefs.DeleteAll();
        currentTile = PlayerPrefs.GetInt("Total distance", 0);
        startOffset = currentTile;
        currentTile -= 10;
        //pointer.transform.position -= Vector3.forward * tileSize * (currentTile % 10 - 8);
        //Meter.SetValue(currentTile - currentTile % 10);
        for (int i = 0; i < 2; i++)
            GameplayCycle(false, false);
        for (int i = 0; i < 8; i++)
            GameplayCycle(false);
    }

    void GameplayCycle(bool destroyFirstTile = true, bool spawnobstacles = true)
    {
        currentTile++;
        if (!endSpawned)
        {
            if (currentTile % 10 == 0)
            {
                endSpawned = true;
                SpawnTileAtLastPosition(endTile).GetComponentInChildren<Trigger>().events += (g) =>
                {
                    if (g == player.GetComponentInChildren<Collider>().gameObject)
                        OnLevelEnd();
                };
                pointer.transform.position = roadTiles.Last().position;
                Meter.SetValue(currentTile);
            }
            else
            {
                SpawnTileAtLastPosition();
                if (spawnobstacles)
                    SpawnObstaclesAtLastTile();
            }
        }
        if (destroyFirstTile)
            DeleteFirstTile();
        garage.position += Vector3.forward * tileSize;
    }
    void Update()
    {
        if (player.position.z >= (currentTile - startOffset + 1) * tileSize)
            GameplayCycle();
    }
    void SpawnObstaclesAtLastTile()
    {
        var lastTilePos = roadTiles[roadTiles.Count - 1].position + Vector3.up * 4.35f;
        for (int i = 0; i <= Random.Range(2, 10); i++)
            PoolManager.Instantiate(obstacles[Random.Range(0, obstacles.Length)],
                lastTilePos + new Vector3(Random.Range(-tileSize, tileSize), 0, Random.Range(-tileSize, tileSize)) * 0.4f,
                Quaternion.Euler(0, Random.Range(0, 360), 0));
    }
    Transform SpawnTileAtLastPosition(GameObject tile = null)
    {
        Transform t;
        if (roadTiles.Count == 0)
            t = SpawnTileAtPosition(new Vector3(0, -4.35f, -tileSize * 2), tile).transform;
        else
            t = SpawnTileAtPosition(roadTiles.Last().position + Vector3.forward * tileSize, tile).transform;
        roadTiles.Add(t);
        return t;
    }
    void DeleteFirstTile()
    {
        if (roadTiles.Count <= 1)
            return;

        var t = roadTiles[0];
        roadTiles.RemoveAt(0);
        PoolManager.Destroy(t.gameObject);
        firstTileZPos = roadTiles[0].position.z;
    }
    GameObject SpawnTileAtPosition(Vector3 position, GameObject tile)
    {
        if (tile == null)
            tile = PoolManager.Instantiate(tilePrefabs[SmartRandom(probabilities)],
            position,
            Quaternion.identity);
        else
            tile = PoolManager.Instantiate(tile, position, Quaternion.identity);

        if (Random.Range(0, 2) == 0)
            tile.transform.localScale = new Vector3(-tile.transform.localScale.x, tile.transform.localScale.y, tile.transform.localScale.z);

        return tile;
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Total distance", currentTile);
    }
    public static int SmartRandom(float[] normalizedProbabilities)
    {
        var f = Random.Range(0, normalizedProbabilities[normalizedProbabilities.Length - 1]);
        for (int i = 0; i < normalizedProbabilities.Length; i++)
        {
            if (normalizedProbabilities[i] >= f)
                return i;
        }
        return normalizedProbabilities.Length - 1;
    }
    async void OnLevelEnd()
    {
        onLevelEndEvents?.Invoke();
        await Task.Delay(1750);
        PlayerPrefs.SetInt("Total distance", currentTile);
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }
}
