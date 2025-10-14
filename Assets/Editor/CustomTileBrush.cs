using UnityEngine;
using UnityEditor; // Necesario para [CustomGridBrush]
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps; // NECESARIO

[CustomGridBrush(false, false, false, "Custom Tile Brush")]
[CreateAssetMenu(fileName = "NewCustomBrush", menuName = "Custom/2D Brushes/Custom Tile Brush")]

public class CustomTileBrush : GridBrushBase
{
    public TileBase m_Tile = null;

    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (m_Tile == null) return;
        Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            tilemap.SetTile(position, m_Tile);
        }
    }

    public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            tilemap.SetTile(position, null);
        }
    }
}
