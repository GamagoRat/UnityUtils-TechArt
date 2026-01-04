using System.Globalization;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public int height = 128;
    public int width = 128;
    public bool isBlackAndWhite = false;

    public Texture2D texture; 
    
    [Header("Value Noise")]
    public int gridHeight = 64;
    public int gridWidth = 64;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    
    
    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Generate")]
    void showGeneratedTexture()
    {
        texture = GenerateValueNoise();
        //texture = GenerateWhiteNoise();
    }

    Texture2D GenerateWhiteNoise()
    {
        Texture2D generated_texture = new Texture2D(
            width,
            height,
            TextureFormat.RGBA32,
            false
        );

        generated_texture.wrapMode = TextureWrapMode.Clamp;
        generated_texture.filterMode = FilterMode.Point;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Color color;
                if (isBlackAndWhite)
                {
                    float value =  Random.Range(0.0f, 1.0f);
                    color = new Color(value, value, value);
                }
                else
                {
                    color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                }
                generated_texture.SetPixel(i, j, color);
            }
        }
        generated_texture.Apply();
        return generated_texture;
    }
    
    Texture2D GenerateValueNoise()
    {
        Texture2D generated_texture = new Texture2D(
            width,
            height,
            TextureFormat.RGBA32,
            false
        );

        generated_texture.wrapMode = TextureWrapMode.Clamp;
        generated_texture.filterMode = FilterMode.Point;
        
        Color[] colors = new Color[gridWidth * gridHeight];
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                Color color;
                if (isBlackAndWhite)
                {
                    float value =  Random.Range(0.0f, 1.0f);
                    color = new Color(value, value, value);
                }
                else
                {
                    color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                }
                colors[i * gridWidth + j] = color;
            }
        }
        
        
        int step = width / gridWidth;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int stepCountX = i / step;
                int stepCountY = j / step;
                Color topLeft = colors[stepCountX*gridWidth + stepCountY];
                Color topRight = colors[(stepCountX+1)*gridWidth + stepCountY];
                Color bottomLeft = colors[stepCountX * gridWidth + stepCountY+1];
                Color bottomRight = colors[(stepCountX+1) * gridWidth + stepCountY+1];
                int relativeX = i % step;
                int relativeY = j % step;

                Color color = Color.Lerp(Color.Lerp(topLeft, topRight, relativeX),
                    Color.Lerp(bottomLeft, bottomRight, relativeX), relativeY);
                generated_texture.SetPixel(i, j, color);
            }
        }
        
        
        
        generated_texture.Apply();
        return generated_texture;
    }
}
