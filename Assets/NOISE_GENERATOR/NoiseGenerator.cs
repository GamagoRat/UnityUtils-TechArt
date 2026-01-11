using System;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class NoiseGenerator : MonoBehaviour
{
    public int resolution=128;
    public bool isBlackAndWhite = false;

    public Texture2D texture; 
    
    [Header("Value Noise")]
    public int gridSize = 64;
    public bool repeat=true;
    
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
            resolution,
            resolution,
            TextureFormat.RGBA32,
            false
        );

        generated_texture.wrapMode = TextureWrapMode.Clamp;
        generated_texture.filterMode = FilterMode.Point;
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
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
            resolution,
            resolution,
            TextureFormat.RGBA32,
            false
        );

        generated_texture.wrapMode = TextureWrapMode.Clamp;
        generated_texture.filterMode = FilterMode.Point;

        Color[] colors = new Color[gridSize * gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if(repeat && i==gridSize-1)
                {
                    colors[i * gridSize + j] = colors[j];         
                    continue;          
                }
                else if(repeat && j == gridSize - 1)
                {
                    colors[i * gridSize + j] = colors[i * gridSize];    
                    continue;
                }
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
                colors[i * gridSize + j] = color;
            }
        }
        
        
        int step = resolution / gridSize;
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                int stepCountX = i / step;
                int stepCountY = j / step;
                Color topLeft = colors[stepCountX*gridSize + stepCountY];
                Color topRight = colors[(stepCountX+1)%gridSize*gridSize + stepCountY];
                Color bottomLeft = colors[stepCountX * gridSize + (stepCountY+1)%gridSize];
                Color bottomRight = colors[(stepCountX+1)%gridSize * gridSize + (stepCountY+1)%gridSize];
                int relativeX = i % step;
                int relativeY = j % step;

                Color color = Color.Lerp(Color.Lerp(topLeft, topRight, (float)relativeX/step),
                    Color.Lerp(bottomLeft, bottomRight, (float)relativeX/step), (float)relativeY/step);
                generated_texture.SetPixel(i, j, color);
            }
        }
        
        generated_texture.Apply();
        return generated_texture;
    }

    Vector3 RandomUnitVector()
    {
        float theta = Random.Range(0, 2*MathF.PI);
        float phi = Random.Range(0, 2*MathF.PI);

        float x = MathF.Sin(phi) * MathF.Cos(theta);
        float y = MathF.Sin(phi) * Mathf.Sin(theta);
        float z = MathF.Cos(theta);
        
        return new Vector3(x, y, z);
    }

    Texture2D GeneratePerlinNoise()
    {
        Texture2D generated_texture = new Texture2D(
            width,
            height,
            TextureFormat.RGBA32,
            false
        );

        generated_texture.wrapMode = TextureWrapMode.Clamp;
        generated_texture.filterMode = FilterMode.Point;
        
        Vector3[] vectors = new Vector3[gridWidth * gridHeight];
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                
                vectors[i * gridWidth + j] = RandomUnitVector();
            }
        }
        
        
        int step = width / gridWidth;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int stepCountX = i / step;
                int stepCountY = j / step;
                Vector3 topLeft = vectors[stepCountX*gridWidth + stepCountY];
                Vector3 topRight = vectors[(stepCountX+1)*gridWidth + stepCountY];
                Vector3 bottomLeft = vectors[stepCountX * gridWidth + stepCountY+1];
                Vector3 bottomRight = vectors[(stepCountX+1) * gridWidth + stepCountY+1];
                int relativeX = i % step;
                int relativeY = j % step;

                //generated_texture.SetPixel(i, j, color);
            }
        }
        
        generated_texture.Apply();
        return generated_texture;
    }
}
