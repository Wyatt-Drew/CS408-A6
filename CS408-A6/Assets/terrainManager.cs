using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class terrainManager : MonoBehaviour
{
    //Other variables
    float changePerHit = 0.01f; //0.01f
    public int sandID = 1;
    public float effectSize = 1f;
    float coneAngle = 10f;
    //Terrain data
    private float terrainSize;
    public Terrain terrain;
    protected int alphaMapWidth;
    protected int alphaMapHeight;
    protected int alphaLayerCount;
    private int xResolution;
    private int zResolution;
    Vector3 heightMapScale;
    //Original
    private float[,] heightMap;
    //Backup data
    private float[,,] originalAlphaMap;
    private float[,] originalHeightMap;

    // Start is called before the first frame update
    void Start()
    {
        terrainSize = terrain.terrainData.size.x;
        //load in map size data
        xResolution = terrain.terrainData.heightmapResolution;
        zResolution = terrain.terrainData.heightmapResolution;
        heightMapScale = terrain.terrainData.heightmapScale;

        alphaMapWidth = terrain.terrainData.alphamapWidth;
        alphaMapHeight = terrain.terrainData.alphamapHeight;
        alphaLayerCount = terrain.terrainData.alphamapLayers;
        //
        heightMap = terrain.terrainData.GetHeights(0, 0, xResolution, zResolution);
        //save original data to restore later
        originalHeightMap = terrain.terrainData.GetHeights(0, 0, xResolution, zResolution);
        originalAlphaMap = terrain.terrainData.GetAlphamaps(0, 0, alphaMapWidth, alphaMapHeight);
    }
    //Purpose: To restore the map
    void OnApplicationQuit()
    {
        terrain.terrainData.SetHeights(0, 0, originalHeightMap);
        terrain.terrainData.SetAlphamaps(0, 0, originalAlphaMap);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {

                raiseTerrainHeight(hit.point);
                // area middle point x and z, area size, texture ID from terrain textures
                //updateTerrainAlpha(hit.point, effectSize, sandID);
            }
        }
    }
    private void raiseTerrainHeight(Vector3 point)
    {
        //Calculate where it stops rolling (follows gravity when over the set degrees STARTING angle)
        Vector3 temp = findStopLocation(point.x, point.z);

        //Map that point to the proper location
        int x = (int)((temp.x / terrain.terrainData.size.x) * xResolution);
        int z = (int)((temp.z / terrain.terrainData.size.z) * zResolution);
        //Add particle to said location
        float y = heightMap[x, z] + changePerHit;
        float[,] height = new float[1, 1];
        height[0, 0] = Mathf.Clamp(y, 0, 1);     //A 2D array of 1 point
        heightMap[x, z] = Mathf.Clamp(y, 0, 1);  //allows you to add more each time.
        terrain.terrainData.SetHeights(x, z, height);
        return;
        //Smooth out result
        for (x= x- (int)heightMapScale.x; x < x+3 * (int)heightMapScale.x; x+= (int)heightMapScale.x)
        {
            for (z= z- (int)heightMapScale.z; z < z+3* (int)heightMapScale.z; z+= (int)heightMapScale.z)
            {
                y = heightMap[x, z] + changePerHit;
                height[0, 0] = Mathf.Clamp(y, 0, 1);
                heightMap[x, z] = Mathf.Clamp(y, 0, 1);
                terrain.terrainData.SetHeights(x, z, height);
            }
        }
        //smoothAll();
        return;
    }
    void smoothAll()
    {
        Debug.Log(zResolution);
        for (int i = 0; i < xResolution; i++)
        {
            for (int j = 0; j < zResolution; j++)
            {
                heightMap[i, j] = 0;
                //terrain.terrainData.SetHeights(i, j, heightMap);
            }
        }
    }
    void smoothPoint()
    {

    }
    Vector3 findStopLocation(float x, float z)
    {
        //If it isn't at the max angle add to it
        if (lessThanAngle(new Vector3(x, 0f, z)))
        {
            return new Vector3(x, 0f, z);
        }
        Debug.Log("next point");
        //It is too high.  The sand particle falls down hill.  Find the smallest point around it
        float smallest = findAngle(new Vector3(x, 0f, z));
        int smallestX = 1;
        int smallestZ = 1;
        //float adjustedIncrement = (1 / terrain.terrainData.size.x) * xResolution;
        System.Random r = new System.Random();
        return findStopLocation((float)(r.Next(-1, 2) * heightMapScale.x + x), (float)(r.Next(-1, 2) * heightMapScale.z + z));
        return new Vector3((float)(r.Next(-1, 2)*heightMapScale.x + x),0f, (float)(r.Next(-1, 2) * heightMapScale.z + z));
        return findStopLocation((float)(r.Next(-1, 2) + x), (float)(r.Next(-1, 2) + z));
        //Debug.Log(adjustedIncrement);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {

                if (smallest > findAngle(new Vector3(x+(i-1), 0f, z+(j-1))))
                {
                    smallest = findAngle(new Vector3(x + (i - 1), 0f, z + (j - 1)));
                    smallestX = i;
                    smallestZ = j;
                    Debug.Log("youre in");
                }
            }
        }
        //return new Vector3((float)(x + smallestX - 1), 0f, (float)(z + smallestZ - 1));
        return findStopLocation((float)(smallestX - 1 + x), (float)(smallestZ - 1 + z));
    }
    //bool lessThanAngle(float centerHeight, float edgeHeight)
    float findAngle(Vector3 point)
    {
        float xAdjusted = ((point.x / terrain.terrainData.size.x));
        float zAdjusted = ((point.z / terrain.terrainData.size.z));
        float actualAngle = terrain.terrainData.GetSteepness(xAdjusted, zAdjusted);
        return actualAngle;
    }
    bool lessThanAngle(Vector3 point)
    {
        float xAdjusted = ((point.x / terrain.terrainData.size.x));
        float zAdjusted = ((point.z / terrain.terrainData.size.z));
        float actualAngle = terrain.terrainData.GetSteepness(xAdjusted, zAdjusted);
        Debug.Log("angle" + actualAngle);
        return (actualAngle < coneAngle);
    }
    int adjust(float point)
    {
        return (int)((point / terrainSize) * xResolution);
    }
}
