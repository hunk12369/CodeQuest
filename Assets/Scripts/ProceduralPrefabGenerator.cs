using UnityEngine;

public class ProceduralPrefabGenerator : MonoBehaviour
{
    [Header("Configuración de Grid")]
    public Grid grid; // Arrastra tu componente Grid aquí
    public Transform container; // Un objeto vacío para que los niveles no desordenen la jerarquía

    [System.Serializable]
    public struct PrefabConfig {
	public string nombre; // Solo para organizar en el Inspector
	public GameObject prefab;
	public Vector3 positionOffset; // <-- NUEVO: Ajuste fino de posición
	public Vector3 rotationOffset; // Aquí pondrás el 272, 90, 0 o lo que necesites
	public Vector3 scale;          // Aquí pones la escala que desees (ej. 1, 1, 1)
    }

    [Header("Configuración de Prefabs")]
    public PrefabConfig floor;
    public PrefabConfig wall;
    public PrefabConfig obstacle;

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
	// Limpieza de objetos anteriores
        if (container == null) return;
	/*
        foreach (Transform child in container) {
            Destroy(child.gameObject);
        }
	*/

	for (int i = container.childCount - 1; i >= 0; i--) {
            if (Application.isPlaying) Destroy(container.GetChild(i).gameObject);
            else DestroyImmediate(container.GetChild(i).gameObject);
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
		// Lógica de decisión
                PrefabConfig currentConfig = floor;
                // 1. Colocar Suelo siempre
                SpawnPrefab(currentConfig, worldPos);

                // 2. Colocar Paredes en los bordes
                if (x == 0 || x == width - 1 || z == 0 || z == height - 1)
                {
			currentConfig = wall;
			SpawnPrefab(currentConfig, worldPos);
                }
                // 3. Colocar Obstáculos aleatorios
                else if (Random.Range(0, 100) < obstaclePercent)
                {
                    // Evitar el punto de inicio del robot (1,0,1)
                    if (!(x == 1 && z == 1))
                    {
			currentConfig = obstacle;
			SpawnPrefab(currentConfig, worldPos);
                    }
                }
            }
        }
    }

    void SpawnPrefab(PrefabConfig config, Vector3 basePosition)
    {
        if (config.prefab == null) return;
	// 1. Aplicamos el Position Offset sumándolo a la posición base de la celda
        // OJO: Como tu grid está rotada, quizás debas probar si el offset es local o global
        Vector3 finalPos = basePosition + config.positionOffset;

	// Aplicamos la rotación específica que definas en el Inspector
        Quaternion rotation = Quaternion.Euler(config.rotationOffset);
        GameObject go = Instantiate(config.prefab, finalPos, rotation);
	// 2. Aplicar la escala guardada
        go.transform.localScale = config.scale;

        go.transform.parent = container;
    }
}
