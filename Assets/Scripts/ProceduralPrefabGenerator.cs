using UnityEngine;

public class ProceduralPrefabGenerator : MonoBehaviour
{
    [Header("Configuración de Grid")]
    public Grid grid; // Arrastra tu componente Grid aquí
    public Transform container; // Un objeto vacío para que los niveles no desordenen la jerarquía

    [System.Serializable]
    public struct PrefabConfig {
        public GameObject prefab;
        public Vector3 rotationOffset; // Aquí pondrás el 272, 90, 0 o lo que necesites
    }

    [Header("Prefabs de Construcción")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject obstaclePrefab;

    [Header("Dimensiones")]
    public int width = 10;
    public int height = 10;
    [Range(0, 100)] public int obstaclePercent = 15;

    void Start()
    {
        // Esto hace que se genere apenas le das a Play
        Generate();

        // Opcional: Si quieres que cada vez sea un nivel distinto de verdad
        // Random.InitState((int)System.DateTime.Now.Ticks);
    }

    public void Generate()
    {
        // Limpiar nivel anterior
        foreach (Transform child in container) {
            Destroy(child.gameObject);
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++) // Usamos Z para el plano horizontal 3D
            {
                Vector3Int cellPos = new Vector3Int(x, z, 0);
                Vector3 worldPos = grid.CellToWorld(cellPos);
		//Vector3 worldPos = grid.CellToWorld(cellPos) + new Vector3(0.5f, 0, 0.5f);
		Debug.Log("Position");
		Debug.Log(worldPos);

                // 1. Colocar Suelo siempre
                SpawnPrefab(floorPrefab, worldPos);

                // 2. Colocar Paredes en los bordes
                if (x == 0 || x == width - 1 || z == 0 || z == height - 1)
                {
                    SpawnPrefab(wallPrefab, worldPos);
                }
                // 3. Colocar Obstáculos aleatorios
                else if (Random.Range(0, 100) < obstaclePercent)
                {
                    // Evitar el punto de inicio del robot (1,0,1)
                    if (!(x == 1 && z == 1))
                    {
                        SpawnPrefab(obstaclePrefab, worldPos);
                    }
                }
            }
        }
    }

    void SpawnPrefab(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return;
        GameObject go = Instantiate(prefab, position, Quaternion.identity);
        go.transform.parent = container;
    }
}
