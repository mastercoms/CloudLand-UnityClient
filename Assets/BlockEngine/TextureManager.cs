using UnityEngine;
using System.Collections;

public class TextureManager
{
    public const string TEXTURE_PATH_PREFIX = "Textures";
    public const int ANIMATION_FRAMES = 16;

    private ArrayList[] textures = new ArrayList[ANIMATION_FRAMES];
    private ArrayList[] normals = new ArrayList[ANIMATION_FRAMES];

    public Texture2D[] packedTextures;
    public Texture2D[] packedNormals;
    public Material[] packedMaterials;
    public Rect[][] texturePositions;

    public TextureManager()
    {
        for(int i = 0; i < ANIMATION_FRAMES; i++)
        {
            textures[i] = new ArrayList();
            normals[i] = new ArrayList();
        }
    }

    public int RegisterTexture(string tex)
    {
        return RegisterTexture(tex, false);
    }

    public int RegisterTexture(string tex, bool animated)
    {
        int index = -1;
        if (!animated)
        {
            for(int i = 0; i < ANIMATION_FRAMES; i++)
            {
                index = textures[i].Add(Resources.Load(TEXTURE_PATH_PREFIX + "/ColorMap/" + tex));
                index = normals[i].Add(Resources.Load(TEXTURE_PATH_PREFIX + "/NormalMap/" + tex));
            }
        } else
        {
            for (int i = 0; i < ANIMATION_FRAMES; i++)
            {
                index = textures[i].Add(Resources.Load(TEXTURE_PATH_PREFIX + "/ColorMap/" + tex + "/" + i));
                index = normals[i].Add(Resources.Load(TEXTURE_PATH_PREFIX + "/NormalMap/" + tex + "/" + i));
            }
        }
        return index;
    }

    public void PackTextures()
    {
        Material baseMaterial = (Material) Resources.Load("Materials/TextureBaseMaterial");
        packedTextures = new Texture2D[ANIMATION_FRAMES];
        packedNormals = new Texture2D[ANIMATION_FRAMES];
        packedMaterials = new Material[ANIMATION_FRAMES];
        texturePositions = new Rect[ANIMATION_FRAMES][];
        for (int i = 0; i < ANIMATION_FRAMES; i++)
        {
            packedTextures[i] = new Texture2D(0, 0);
            packedNormals[i] = new Texture2D(0, 0);
            packedMaterials[i] = new Material(baseMaterial);
            texturePositions[i] = packedTextures[i].PackTextures((Texture2D[])textures[i].ToArray(typeof(Texture2D)), 0);
            texturePositions[i] = packedNormals[i].PackTextures((Texture2D[])normals[i].ToArray(typeof(Texture2D)), 0);
            packedMaterials[i].mainTexture = packedTextures[i];
            packedMaterials[i].SetTexture("_BumpMap", packedNormals[i]);
        }
    }
}
