using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps; // Sigue siendo necesario para la clase base

// Cambiamos el nombre del pincel en el menú
[CustomGridBrush(false, false, false, "GameObject Brush")]
[CreateAssetMenu(fileName = "NewCustomGameObjectBrush", menuName = "Custom/2D Brushes/Custom Game Object Brush")]

public class CustomGameObjectBrush : GridBrushBase
{
    // Propiedad 1: El Prefab que vamos a pintar
    public GameObject m_Prefab; 

    // Propiedad 2: Bandera para rotación aleatoria
    public bool m_RandomRotation = false;

    // --- NUEVAS PROPIEDADES ---
    public Vector3 m_Offset = Vector3.zero; // Desplazamiento
    public Vector3 m_Scale = Vector3.one;   // Escala
    public Vector3 m_Orientation = Vector3.zero; // Rotación base

    // --- Métodos principales ---

    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        // 1. Verificar si hay un Prefab para pintar
        if (m_Prefab == null)
        {
            Debug.LogError("CustomGameObjectBrush: ¡Asigna un Prefab para pintar!");
            return;
        }

        // 2. Obtener la posición mundial de la celda
        Vector3 worldPosition = grid.CellToWorld(position);

        // 3. Instanciar el Prefab
        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(m_Prefab);
	// 3. Registrar la acción para deshacer (Undo) (Ctrl+Z)
        Undo.RegisterCreatedObjectUndo((Object)go, "Paint Prefab");
        
	// 4. Aplicar transformaciones
        
        // A. Posici�n con Offset: Posici�n de celda + Offset
        go.transform.position = worldPosition + m_Offset;
	Debug.Log(worldPosition + m_Offset);
        go.transform.parent = brushTarget.transform; 

        // B. Escala
        go.transform.localScale = m_Scale;

        // C. Rotaci�n: Orientaci�n base + Rotaci�n Aleatoria
        Quaternion rotationBase = Quaternion.Euler(m_Orientation);
        

        if (m_RandomRotation)
        {
            // Rotación aleatoria en el eje Y
            Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
	    go.transform.rotation = rotationBase * randomRot;
        }
	else
	{
		// Solo aplica la orientación base
            go.transform.rotation = rotationBase;
	}

    }

    public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        // Obtener la posición mundial de la celda
        Vector3 worldPosition = grid.CellToWorld(position);
	//Vector3 offset = new Vector3(-0.5f, 0f, 0.5f);
	worldPosition = worldPosition + m_Offset;
	float searchRadius = grid.cellSize.magnitude * 0.5f;

        // Buscar objetos cercanos a esa posición para borrarlos
        //Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition);
	//
	// 4. Buscar objetos 3D cercanos a esa posición
	// Usamos Physics.OverlapSphere para buscar en 3D.
	Collider[] colliders = Physics.OverlapSphere(worldPosition, searchRadius);
	//Debug.Log($"Buscando en: {worldPosition} con radio: {searchRadius}. Encontrados: {colliders.Length}");
        
	for (int i = brushTarget.transform.childCount - 1; i >= 0; i--)
	{
		Transform child = brushTarget.transform.GetChild(i);
        
		// 5. Comparar la posici�n del hijo con la posici�n objetivo
		if (Vector3.Distance(child.position, worldPosition) < grid.cellSize.magnitude * 0.5f)
		{
			//Debug.Log($"Borrando objeto en: {child.position}");
			Undo.DestroyObjectImmediate(child.gameObject);
		}
	}
	/*
        foreach (Collider collider in colliders)
        {
		Debug.Log(collider);
            if (collider.gameObject.transform.parent == brushTarget.transform)
            {
                // Borrar el objeto y registrar la acción para deshacer
                Undo.DestroyObjectImmediate(collider.gameObject);
            }
        }
	*/
    }

    // El resto de los métodos (BoxFill, FloodFill, etc.) se heredan de GridBrushBase
}
