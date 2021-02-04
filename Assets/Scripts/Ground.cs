using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public Material[] groundMaterials;
    Material selectedGroundMaterial;
    public float maxHeight;
    public float verticalOffset;
    public GameObject[] obstacles;
    public Transform player;
    List<Transform> groundTiles = new List<Transform>();
    Dictionary<Transform, IEnumerable<GameObject>> obstaclesOnParticularTile = new Dictionary<Transform, IEnumerable<GameObject>>();
    public float tileSize;
    int currentTile;
    public static Ground instance;
    Vector3[] edge;
    Mesh groundColoredMesh;

    public Material vertexLitMaterial;
    public Material waterMaterial;
    public bool isWater;
    void Start()
    {
        selectedGroundMaterial = groundMaterials[Random.Range(0, groundMaterials.Length)];
#if !UNITY_EDITOR
        RenderSettings.skybox = selectedGroundMaterial;
#endif
        edge = new Vector3[11];
        instance = this;
        currentTile = -3;
        for (int i = 0; i < 3; i++)
        {
            SpawnTileAtLastPosition();
            currentTile++;
            SpawnObstaclesAtLastTile();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position.z >= (currentTile + 1) * tileSize)
        {
            currentTile++;
            SpawnTileAtLastPosition();
            SpawnObstaclesAtLastTile();
            DeleteFirstTile();           
        }
    }
    void SpawnObstaclesAtLastTile()
    {
        if (isWater)
            return;

        Vector3 dir = Vector3.zero;
        var lastTilePos = groundTiles[groundTiles.Count - 1].position;
        var list = new List<GameObject>();
        for (int i = 0; i <= Random.Range(50, 500); i++)
        {
            dir.x = Random.Range(-1.0f + 1.75f * Road.instance.tileSize / tileSize, 1.0f - 1.75f * Road.instance.tileSize / tileSize);
            dir.x += Mathf.Sign(dir.x) * 1.75f * Road.instance.tileSize / tileSize;
            dir.z = Random.Range(-1.0f, 1.0f);
            var g = PoolManager.Instantiate(obstacles[Random.Range(0, obstacles.Length)],
                lastTilePos + dir * tileSize * 0.5f,
                Quaternion.Euler(Random.Range(0, 30) - 90, Random.Range(0, 360), 0));
            g.transform.localScale *= Random.Range(4, 12);
            list.Add(g);
        }
        obstaclesOnParticularTile.Add(groundTiles[groundTiles.Count - 1], list);
    }
    void SpawnTileAtLastPosition()
    {
        if (groundTiles.Count == 0)
            groundTiles.Add(SpawnTileAtPosition(new Vector3(0, verticalOffset, 0)).transform);
        else
            groundTiles.Add(SpawnTileAtPosition(groundTiles.Last().position + Vector3.forward * tileSize).transform);
    }
    void DeleteFirstTile()
    {
        if (groundTiles.Count <= 1)
            return;

        var t = groundTiles[0];
        groundTiles.RemoveAt(0);
        if (obstaclesOnParticularTile.TryGetValue(t, out var list))
        {
            foreach (var i in list)
                PoolManager.Destroy(i);
            obstaclesOnParticularTile.Remove(t);
        }    
        Destroy(t.gameObject);
    }
    GameObject SpawnTileAtPosition(Vector3 position)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Destroy(g.GetComponent<Collider>());
        g.transform.localScale *= tileSize / 10;
        g.transform.position = position;        
        if (isWater)
        {
            var meshRenderer = g.GetComponent<MeshRenderer>();
            meshRenderer.material = waterMaterial;
            meshRenderer.material.SetFloat("_Offset", -currentTile);
        }
        else
        {
            var meshFilter = g.GetComponent<MeshFilter>();
            if (groundColoredMesh == null)
                groundColoredMesh = RandomColor.GenerateColoredMesh(meshFilter.sharedMesh, selectedGroundMaterial.color);
            meshFilter.mesh = groundColoredMesh;
                       
            var vertices = meshFilter.mesh.vertices;

            for (int i = 0; i < 11; i++)
                vertices[i + 110] += edge[i];

            for (int i = 0; i < vertices.Length - 11; i++)
                if (i < 11)
                {
                    edge[i] = Vector3.up * Random.Range(0, maxHeight);
                    vertices[i] += edge[i];
                }
                else
                    vertices[i] += Vector3.up * Random.Range(0, maxHeight);

            meshFilter.mesh.vertices = vertices;
            g.GetComponent<MeshRenderer>().material = vertexLitMaterial;
        }
        return g;
    }
}
