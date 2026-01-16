using System;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class NoiseGenerator : MonoBehaviour
{
    public int resolution=128;
    public bool isBlackAndWhite = false;

    public Texture2D texture; 


    
    [Header("Value Noise")]
    public int cellSize = 16;
    public bool repeat=true;
    public AnimationCurve easingFunction;


    

    [ContextMenu("Generate")]
    void showGeneratedTexture()
    {
        texture = GeneratePerlinNoise();
        //texture = GenerateValueNoise();
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

        int gridSize = (int)Mathf.Ceil(resolution/cellSize);

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
        
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                int stepCountX = i / cellSize;
                int stepCountY = j / cellSize;
                Color topLeft = colors[stepCountX*gridSize + stepCountY];
                Color topRight = colors[(stepCountX+1)%gridSize*gridSize + stepCountY];
                Color bottomLeft = colors[stepCountX * gridSize + (stepCountY+1)%gridSize];
                Color bottomRight = colors[(stepCountX+1)%gridSize * gridSize + (stepCountY+1)%gridSize];
                int relativeX = i % cellSize;
                int relativeY = j % cellSize;

                Color color = Color.Lerp(Color.Lerp(topLeft, topRight, easingFunction.Evaluate((float)relativeX/cellSize)),
                    Color.Lerp(bottomLeft, bottomRight, easingFunction.Evaluate((float)relativeX/cellSize)), easingFunction.Evaluate((float)relativeY/cellSize));
                generated_texture.SetPixel(i, j, color);
            }
        }
        
        generated_texture.Apply();
        return generated_texture;
    }

    Vector3 RandomUnitVector(bool is2D)
    {

        float theta = Random.Range(0, 2*MathF.PI);
        float phi = is2D ? MathF.PI/2 : Random.Range(0, 2*MathF.PI);

        float x = MathF.Sin(phi) * MathF.Cos(theta);
        float y = MathF.Sin(phi) * Mathf.Sin(theta);
        float z = MathF.Cos(phi);
        
        return new Vector3(x, y, z);
    }

    Texture2D GeneratePerlinNoise()
    {
        Texture2D generated_texture = new Texture2D(
            resolution,
            resolution,
            TextureFormat.RGBA32,
            false
        );

        generated_texture.wrapMode = TextureWrapMode.Clamp;
        generated_texture.filterMode = FilterMode.Point;

        int gridSize = (int)Mathf.Ceil(resolution/cellSize);

        Vector2[] vectors = new Vector2[gridSize * gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if(repeat && i==gridSize-1)
                {
                    vectors[i * gridSize + j] = vectors[j];         
                    continue;          
                }
                else if(repeat && j == gridSize - 1)
                {
                    vectors[i * gridSize + j] = vectors[i * gridSize];    
                    continue;
                }

                Vector2 vector = RandomUnitVector(true);
                vectors[i * gridSize + j] = vector;
            }
        }
        
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                
                float relativeY = j % cellSize;
                float relativeX = i % cellSize;

                int stepCountX = i / cellSize;
                int stepCountY = j / cellSize;

                Vector2 cellPos = new Vector2(i, j);

                Vector2 topLeftCellPos = cellPos - new Vector2(stepCountX , stepCountY)*cellSize ;
                Vector2 topRightCellPos = cellPos - new Vector2(stepCountX + 1  , stepCountY) * cellSize;
                Vector2 bottomLeftCellPos = cellPos - new Vector2(stepCountX   , stepCountY+1) * cellSize;
                Vector2 bottomRightCellPos = cellPos - new Vector2(stepCountX+1   , stepCountY+1) * cellSize;

                float topLeftDot = Vector2.Dot(vectors[stepCountX*gridSize + stepCountY], topLeftCellPos);
                Debug.Log(topLeftDot);
                float topRightDot = Vector2.Dot(vectors[(stepCountX+1)%gridSize*gridSize + stepCountY], topRightCellPos);
                float bottomLeftDot = Vector2.Dot(vectors[stepCountX * gridSize + (stepCountY+1)%gridSize], bottomLeftCellPos);
                float bottomRightDot = Vector2.Dot(vectors[(stepCountX+1)%gridSize * gridSize + (stepCountY+1)%gridSize], bottomRightCellPos);

                float value = Mathf.Lerp(Mathf.Lerp(topLeftDot, topRightDot, easingFunction.Evaluate(relativeX/cellSize)),
                    Mathf.Lerp(bottomLeftDot, bottomRightDot, easingFunction.Evaluate(relativeX/cellSize)), easingFunction.Evaluate(relativeY/cellSize));
                generated_texture.SetPixel(i, j, new Color(value, value, value));
            }
        }
        
        generated_texture.Apply();
        return generated_texture;
    }

    


}
