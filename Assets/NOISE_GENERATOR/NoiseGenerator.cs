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

        int gridSize = (int)Mathf.Ceil((float)resolution/cellSize);

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
                int stepCountY = i / cellSize;
                int stepCountX = j / cellSize;
                Color bottomLeft = colors[stepCountY*gridSize + stepCountX];
                Color topLeft = colors[(stepCountY+1)%gridSize*gridSize + stepCountX];
                Color bottomRight = colors[stepCountY * gridSize + (stepCountX+1)%gridSize];
                Color topRight = colors[(stepCountY+1)%gridSize * gridSize + (stepCountX+1)%gridSize];
                int relativeY = i % cellSize;
                int relativeX = j % cellSize;

                Color color = Color.Lerp(Color.Lerp(bottomLeft, bottomRight, easingFunction.Evaluate((float)relativeX/cellSize)),
                    Color.Lerp(topLeft, topRight, easingFunction.Evaluate((float)relativeX/cellSize)),
                     easingFunction.Evaluate((float)relativeY/cellSize));
                generated_texture.SetPixel(j, i, color);
            }
        }
        
        generated_texture.Apply();
        return generated_texture;
    }

    Vector3 RandomUnitVector(bool is2D)
    {

        float theta = Random.Range(0, 2*Mathf.PI);
        float phi = is2D ? Mathf.PI/2f : Random.Range(0, 2*Mathf.PI);

        float x = Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = Mathf.Sin(phi) * Mathf.Sin(theta);
        float z = Mathf.Cos(phi);
        
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

        int gridSize = (int)Mathf.Ceil((float)resolution/cellSize);

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
                
                float relativeX = j % cellSize;
                float relativeY = i % cellSize;

                int stepCountY = i / cellSize;
                int stepCountX = j / cellSize;

                Vector2 normalizedCellPos = new Vector2((float)relativeX/cellSize, (float)relativeY/cellSize);//Not really normalized

                Vector2 bottomLeftCellPos = normalizedCellPos ;
                Vector2 bottomRightCellPos = normalizedCellPos - new Vector2(1  , 0) ;
                Vector2 topLeftCellPos = normalizedCellPos - new Vector2(0   , 1) ;
                Vector2 topRightCellPos = normalizedCellPos - new Vector2(1   , 1);

                float bottomLeftDot = Vector2.Dot(vectors[stepCountY*gridSize + stepCountX], bottomLeftCellPos);
                float topLeftDot = Vector2.Dot(vectors[(stepCountY+1)%gridSize*gridSize + stepCountX], topLeftCellPos);
                float bottomRightDot = Vector2.Dot(vectors[stepCountY * gridSize + (stepCountX+1)%gridSize], bottomRightCellPos);
                float topRightDot = Vector2.Dot(vectors[(stepCountY+1)%gridSize * gridSize + (stepCountX+1)%gridSize], topRightCellPos);

                float value =   Mathf.Lerp(Mathf.Lerp(bottomLeftDot, bottomRightDot, easingFunction.Evaluate(relativeX/cellSize)),
                                Mathf.Lerp(topLeftDot, topRightDot, easingFunction.Evaluate(relativeX/cellSize)),                
                                easingFunction.Evaluate(relativeY/cellSize));
                
                
                value = (value+1)/2f;
                generated_texture.SetPixel(i, j, new Color(value, value, value));
            }
        }
        
        generated_texture.Apply();
        return generated_texture;
    }

    


}
