using UnityEditor;
using UnityEngine;
using UnityEditor.Tilemaps;

// Vinculamos este Editor a la nueva clase CustomGameObjectBrush
[CustomEditor(typeof(CustomGameObjectBrush))]
public class CustomGameObjectBrushEditor : GridBrushEditorBase
{
    // SerializedProperty para acceder a las variables del pincel
    private SerializedProperty m_Prefab;
    private SerializedProperty m_RandomRotation;


    // --- NUEVAS PROPIEDADES SERIALIZADAS ---
    private SerializedProperty m_Offset;
    private SerializedProperty m_Scale;
    private SerializedProperty m_Orientation; 

    protected void OnEnable()
    {
        // Inicializar las propiedades serializadas
        m_Prefab = serializedObject.FindProperty("m_Prefab");
        m_RandomRotation = serializedObject.FindProperty("m_RandomRotation");
	//
	// --- Inicializar las nuevas propiedades ---
        m_Offset = serializedObject.FindProperty("m_Offset");
        m_Scale = serializedObject.FindProperty("m_Scale");
        m_Orientation = serializedObject.FindProperty("m_Orientation");
    }

    public override void OnPaintInspectorGUI()
    {
        // Asegurarse de que el objeto serializado esté actualizado
        serializedObject.Update(); 

        EditorGUILayout.Space();

        // Dibuja el campo para el Prefab
        EditorGUILayout.PropertyField(m_Prefab, new GUIContent("Prefab a Pintar", "El GameObject/Prefab que será instanciado."));
        
        // Dibuja el toggle para la rotación aleatoria
        EditorGUILayout.PropertyField(m_RandomRotation, new GUIContent("Rotación Aleatoria", "Si está activo, rotará el Prefab aleatoriamente en el eje Y."));


	// --- NUEVA SECCI�N DE TRANSFORMACIONES ---
        EditorGUILayout.LabelField("Transformaciones del Objeto", EditorStyles.boldLabel);

        // 1. Offset (Vector3)
        EditorGUILayout.PropertyField(m_Offset, new GUIContent("Offset (Posici�n)", "Ajuste de posici�n relativo a la celda."));
        
        // 2. Scale (Vector3)
        EditorGUILayout.PropertyField(m_Scale, new GUIContent("Scale (Escala)", "Escala del objeto instanciado."));
        
        // 3. Orientation (Vector3)
        EditorGUILayout.PropertyField(m_Orientation, new GUIContent("Orientation (Rotaci�n Base)", "Rotaci�n inicial en grados (Euler)."));

        // ------------------------------------------
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Pinta Prefabs instanciados en la cuadrícula.", MessageType.Info);

        // Aplicar los cambios al asset del pincel
        serializedObject.ApplyModifiedProperties(); 
    }
}
