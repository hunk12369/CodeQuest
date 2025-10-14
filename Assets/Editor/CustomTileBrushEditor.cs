using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps; // ¡Necesitas este using!

// Este atributo vincula este script de editor a tu clase CustomTileBrush
[CustomEditor(typeof(CustomTileBrush))]
public class CustomTileBrushEditor : GridBrushEditorBase
{
    // El método correcto a sobreescribir SÍ está en GridBrushEditorBase
    public override void OnPaintInspectorGUI()
    {
        // 1. Obtener una referencia a tu pincel (el asset)
        CustomTileBrush brush = (CustomTileBrush)target;

        // 2. Dibujar campos personalizados
        EditorGUILayout.Space();
        
        // Dibuja el campo "m_Tile" de tu CustomTileBrush
        brush.m_Tile = (TileBase)EditorGUILayout.ObjectField("Tile a Pintar", brush.m_Tile, typeof(TileBase), false);

        // 3. Añadir mensaje de ayuda (opcional)
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Este es mi pincel personalizado!", MessageType.Info);
    }
}
