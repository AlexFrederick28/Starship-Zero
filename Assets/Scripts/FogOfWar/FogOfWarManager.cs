using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

/// <summary>
/// shelved mechanic for now (not working)
/// </summary>
public class FogOfWarManager : MonoBehaviour
{
    public RenderTexture fogTexture;
    public Material fogMaterial;
    public Material revealMaterial;
    public Tilemap fogTileMap;

    public Texture2D texture;
    public int textureWidth;
    public int textureHeight;
    public int revealRadius;
    public Color fogColour;
    public Color clearFog;
    public Vector3[] meshCorners;

    public float Remap(float src_range_start, float src_range_end, float dst_range_start, float dst_range_end, float value_to_remap)
    {
        return ((dst_range_end - dst_range_start) * (value_to_remap - src_range_start)) / (src_range_end - src_range_start) + dst_range_start;
    }

    private void Start()
    {
        ApplyTexture();
        //GetObjectCorners();
    }

    public void ApplyTexture()
    {
        fogTexture = new RenderTexture(512, 512, 0, RenderTextureFormat.RFloat);
        fogTexture.Create();

        fogMaterial.SetTexture("_FogTex", fogTexture);
    }

    public void GetObjectCorners()
    {
        Bounds meshBounds = GetComponent<MeshFilter>().mesh.bounds;
        meshCorners[0] = meshBounds.min;
        meshCorners[1] = meshBounds.max;
    }

    Vector2 WorldToUV(Vector3 worldPos)
    {
        Vector3 local = fogTileMap.transform.InverseTransformPoint(worldPos);
        Bounds bounds = fogTileMap.localBounds;

        float u = Mathf.InverseLerp(bounds.min.x, bounds.max.x, local.x);
        float v = Mathf.InverseLerp(bounds.min.y, bounds.max.y, local.y);

        return new Vector2(u, v);
    }

    public void RevealFogOfWar()
    {
        Vector2 uv = WorldToUV(Vector3.zero);

        revealMaterial.SetVector("_RevealPos", new Vector4(uv.x, uv.y, 0, 0));
        revealMaterial.SetFloat("_Radius", revealRadius);

        RenderTexture temp = RenderTexture.GetTemporary(fogTexture.width, fogTexture.height);

        // Copy current fog into temp
        Graphics.Blit(fogTexture, temp);

        // IMPORTANT: pass the texture into the shader
        revealMaterial.SetTexture("_MainTex", temp);

        // THIS IS THE LINE YOU WERE ASKING ABOUT
        Graphics.Blit(temp, fogTexture, revealMaterial);

        RenderTexture.ReleaseTemporary(temp);

        //Vector3 p = transform.position;
        //int x = (int)Remap(meshCorners[0].x + transform.position.x, meshCorners[1].x + transform.position.x, 0, textureWidth - 1, p.x);
        //int z = (int)Remap(meshCorners[0].z + transform.position.z, meshCorners[1].z + transform.position.z, 0, textureHeight - 1, p.z);

        //if (x >= 2 && x < textureWidth - 2 && z >= 2 && z < textureHeight - 2)
        //{
        //    for (int oz = -revealRadius; oz <= revealRadius; oz++)
        //    {
        //        for (int ox = -revealRadius; ox <= revealRadius; ox++)
        //        {
        //            Color c = texture.GetPixel(x + ox, z + oz);
        //            texture.SetPixel(x + ox, z + oz, c += clearFog);
        //        }
        //    }
            
        //    texture.Apply();
        //}
    }
}
