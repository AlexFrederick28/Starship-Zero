using UnityEngine;
using UnityEngine.Tilemaps;

public class WallTransparency : MonoBehaviour
{
    public Tilemap wallTilemap;
    public Color transparency;
    public Color original;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerBase>() != null)
        {
            wallTilemap.color = transparency;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerBase>() != null)
        {
            wallTilemap.color = original;
        }
    }
}
