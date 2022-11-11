using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class terrainManager : MonoBehaviour
{
    //Other variables
    float changePerHit = 0.001f; //0.01f
    public int sandID = 1;
    public float effectSize = 1f;
    float coneAngle = 5f;
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
    private int[,] hitMap;

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
        hitMap = new int[xResolution, zResolution];
        for (int i = 0; i < xResolution; i++)
            for (int j = 0; j < zResolution; j++)
                hitMap[i, j] = 0;
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

                //test();
                if (hit.point != null)
                {
                    //Debug.Log("adjusted coords:" + adjust(hit.point.x) + adjust(hit.point.z));
                    raiseTerrainHelper(hit.point);
                }
                    
                // area middle point x and z, area size, texture ID from terrain textures
                //updateTerrainAlpha(hit.point, effectSize, sandID);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.point != null)
                    Debug.Log(findAngle(hit.point));
                
                // area middle point x and z, area size, texture ID from terrain textures
                //updateTerrainAlpha(hit.point, effectSize, sandID);
            }
        }
    }
    void test()
    {
        for (int i = 0; i < xResolution; i++)
            for (int j = 0; j < zResolution; j++)
                heightMap[i, j] = 0;
        terrain.terrainData.SetHeights(0, 0, heightMap);
    }
    private void raiseTerrainHelper(Vector3 point)
    {
        //Calculate where it stops rolling (follows gravity when over the set degrees STARTING angle)
        Vector3 temp = findStopLocation(point);
        raiseTerrainHeight(temp, 1);
        //hitMap[(int)point.x, (int)point.z] += 1; 
        //Smooth(temp);
    }
    
    private void raiseTerrainHeight(Vector3 point, float cardinality)
    {
        //Map that point to the proper location
        int x = adjust(point.x);
        int z = adjust(point.z);
        //Add particle to said location
        float y = heightMap[x, z] + changePerHit*cardinality;
        float[,] height = new float[1, 1];
        height[0, 0] = Mathf.Clamp(y, 0, 1);     //A 2D array of 1 point
        heightMap[x, z] = Mathf.Clamp(y, 0, 1);  //allows you to add more each time.
        terrain.terrainData.SetHeights(x, z, height);
        return;
        for (int i = -2; i < 3; i++)
            for (int j = -2; j < 3; j++)
            {
                y = heightMap[x + i * (int)heightMapScale.x, z + j * (int)heightMapScale.z] + changePerHit/2;
                height[0, 0] = Mathf.Clamp(y, 0, 1);
                heightMap[x + i * (int)heightMapScale.x, z + j * (int)heightMapScale.z] = Mathf.Clamp(y, 0, 1);
                terrain.terrainData.SetHeights(x + i * (int)heightMapScale.x, z + j * (int)heightMapScale.z, height);
            }
                

        //terrain.terrainData.SetHeights(x, z, height);
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
    bool underMaxDeltaHeight(Vector3 point)
    {
        int x = adjust(point.x);
        int z = adjust(point.z);
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (heightMap[x, z] - changePerHit > heightMap[x + i, z + j])
                {
                    return false;
                }
            }
        }
        return true;
    }
    Vector3 findStopLocation(Vector3 point)
    {
        if (underMaxDeltaHeight(point))
        {
            return point;
        }
        
        //It is too high.  The sand particle falls down hill.  Find the smallest point around it
        System.Random r = new System.Random();
        return findStopLocation(new Vector3((float)(r.Next(-1, 2) * heightMapScale.x + point.x), 0f,(float)(r.Next(-1, 2) * heightMapScale.z + point.z)));
       // return new Vector3((float)(r.Next(-1, 2)*heightMapScale.x + x),0f, (float)(r.Next(-1, 2) * heightMapScale.z + z));
        //return findStopLocation((float)(r.Next(-1, 2) + x), (float)(r.Next(-1, 2) + z));
        
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
 
    private void Smooth(Vector3 point)
    {
        int size = 10;
        int startx = (int)point.x - size;
        if (startx < 0)
            startx = 0;
        int startz = (int)point.z - size;
        if (startz < 0)
            startz = 0;
        int endx = (int)point.x + size;
        if (endx > terrain.terrainData.heightmapResolution)
            endx = terrain.terrainData.heightmapResolution;
        int endz = (int)point.z + size;
        if (endz > terrain.terrainData.heightmapResolution)
            endz = terrain.terrainData.heightmapResolution;

        float[,] height = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution,
                              terrain.terrainData.heightmapResolution);

        float k = 0.9f;
        /* Rows, left to right */
        for (int x = startx; x < endx; x++)
            for (int z = startz; z < endz; z++)
                height[x, z] = height[x - 1, z] * (1 - k) + height[x, z] * k;

        /* Rows, right to left*/
        for (int x = endx - 2; x < startx; x--)
            for (int z = startz; z < endz; z++)
                height[x, z] = height[x + 1, z] * (1 - k) + height[x, z] * k;

        /* Columns, bottom to top */
        for (int x = startx; x < endx; x++)
            for (int z = startz+1; z < endz; z++)
                height[x, z] = height[x, z - 1] * (1 - k) + height[x, z] * k;

        /* Columns, top to bottom */
        for (int x = startx; x < endx; x++)
            for (int z = endz; z < startz -1; z--)
                height[x, z] = height[x, z + 1] * (1 - k) + height[x, z] * k;

        terrain.terrainData.SetHeights(0, 0, height);
    }
}
