
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class terrainManager : MonoBehaviour
{
    //Other variables
    float changePerHit = 10f; //0.01f
    public int sandID = 1;
    float MAX_SLOPE_ANGLE = 30f;
    float[] neighborh;
    //Terrain data
    public Terrain terrain;
    TerrainData td;
    float unit;
    protected int alphaMapWidth;
    protected int alphaMapHeight;
    protected int alphaLayerCount;
    private int xResolution;
    private int zResolution;
    //Original
    private float[,] heightMap;
    private float[,,] alphaMap;
    //Backup data
    private float[,,] originalAlphaMap;
    private float[,] originalHeightMap;

    void Start()
    {
        //Set frame rate
        Application.targetFrameRate = 60;
        //
        neighborh = new float[3];
        td = terrain.terrainData;
        MAX_SLOPE_ANGLE = (MAX_SLOPE_ANGLE / 180f) * Mathf.PI;
        unit = td.size.x / (td.heightmapResolution - 1);
        //load in map size data
        alphaMapWidth = terrain.terrainData.alphamapWidth;
        alphaMapHeight = terrain.terrainData.alphamapHeight;
        alphaLayerCount = terrain.terrainData.alphamapLayers;
        //
        heightMap = terrain.terrainData.GetHeights(0, 0, td.heightmapResolution, td.heightmapResolution);
        alphaMap = terrain.terrainData.GetAlphamaps(0, 0, alphaMapWidth, alphaMapHeight);
        //save original data to restore later
        originalHeightMap = terrain.terrainData.GetHeights(0, 0, td.heightmapResolution, td.heightmapResolution);
        originalAlphaMap = terrain.terrainData.GetAlphamaps(0, 0, alphaMapWidth, alphaMapHeight);
    }
    //Purpose: To restore the map
    void OnApplicationQuit()
    {
        terrain.terrainData.SetHeights(0, 0, originalHeightMap);
        terrain.terrainData.SetAlphamaps(0, 0, originalAlphaMap);
    }
    void resetMap()
    {
        terrain.terrainData.SetHeights(0, 0, originalHeightMap);
        terrain.terrainData.SetAlphamaps(0, 0, originalAlphaMap);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit1;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit1))
            {
                if (hit1.point != null)
                {
                    hit(hit1.point);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit1;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit1))
            {
                if (hit1.point != null)
                    Debug.Log(findAngle(hit1.point));
            }
        }
    }
    void updateBestNeighbor(int x, int y, float height)
    {
        // set the neighbor to an impossibly bad option.  
        //Assumes at least 1 neighbor exists
        neighborh[0] = 0;
        neighborh[1] = 0;
        neighborh[2] = -1000;
        int r = 8;
        // current point (i,j) in circle around point (x,y)
        int i, j;
        // number of points distributed horizontally and
        // vertically across circle
        int nx, ny;
        // neighbor at which maximum offset height occurs
        ny = Mathf.FloorToInt(r);
        // process all points vertically across circle radius
        for (j = y - ny; j <= y + ny; j++)
        {
            nx = Mathf.FloorToInt(Mathf.Sqrt(r * r - (j - y) * (j - y)));
            // process all points horizontally across circle radius
            for (i = x - nx; i <= x + nx; i++)
            {
                // check to ensure that point (i,j) is not at the
                // center of the circle and  not beyond the edge of
                // the terrain and is in the circle
                bool t1 = (j < 0 || j > td.heightmapResolution - 1 || i < 0 || i > td.heightmapResolution - 1); //outside map
                bool t2 = ((j - y) * (j - y) + (i - x) * (i - x) > (r * r));  //outside circle
                bool t3 = (j == y && i == x);  //Ignoire center of circle
                if (t3 || t1 || t2)
                {
                    continue;
                }
                // determine the height based on the distance between // it and the center of the dropping coordinates
                float nc = Mathf.Tan(MAX_SLOPE_ANGLE) * Mathf.Sqrt((i - x) * (i - x) + (j - y) * (j - y)) * unit;
                // if this height is lower than the determined height,
                // then update the selection of the neighbor

                float tdheight = heightMap[j, i] * td.heightmapScale.y;

                if (tdheight < height - nc && height - nc - tdheight > neighborh[2])
                {
                    neighborh[0] = i;
                    neighborh[1] = j;
                    neighborh[2] = height - nc - tdheight;
                }
            }
        }
    }
    void addSand(int x, int y, float height)
    {
        float c = Mathf.Tan(MAX_SLOPE_ANGLE) * Mathf.Sqrt((neighborh[0] - x) * (neighborh[0] - x) + (neighborh[1] - y) * (neighborh[1] - y)) * unit;
        // positive offset?
        if (neighborh[2] > 0)
        {
            // determine additional height to add
            float extrah = height - Mathf.Max(td.GetHeight((int)neighborh[0], (int)neighborh[1]) + c, td.GetHeight(x, y));
            // record change in height in heightmap
            float[,] height2 = new float[1, 1];
            if ((y >= 0) && (y <= td.heightmapResolution - 1) && (x >= 0) && (x <= td.heightmapResolution - 1))
            {
                height2[0, 0] = (height - extrah) / td.heightmapScale.y;
                heightMap[y, x] = height2[0, 0];
                updateAlphaMap(x, y);
            }
            else
            {
                height2[0, 0] = height / td.heightmapScale.y;
                heightMap[y, x] = height2[0, 0];
                updateAlphaMap(x, y);
            }
            terrain.terrainData.SetHeights(x, y, height2); //For whatever reason X and Y really are reversed here.
            // increase height of pile
            CreatePile((int)neighborh[0], (int)neighborh[1], td.GetHeight((int)neighborh[0], (int)neighborh[1]) + extrah);
        }
    }
    void CreatePile(int x, int y, float height)
    {
        updateBestNeighbor(x, y, height);
        addSand(x, y, height);
    }
    public void hit(Vector3 pos)
    {
        // compute standard local positions and offsets
        Vector3 terrainLocalPos = pos - terrain.transform.position;
        float nx = Mathf.InverseLerp(0, td.size.x, terrainLocalPos.x) * (td.heightmapResolution - 1);
        float ny = Mathf.InverseLerp(0, td.size.z, terrainLocalPos.z) * (td.heightmapResolution - 1);
        float fx = Mathf.Floor(nx);
        float fy = Mathf.Floor(ny);
        float wx = nx - fx;
        float wy = ny - fy;
        // deform each neighboring grid point in the heightmap
        for (int j = 0; j <= 1; j++)
        {
            for (int i = 0; i <= 1; i++)
            {
                // compute increased height resulting from impact
                float addHeight = (float)((1 - i) + (2 * i - 1) * wx) * ((1 - j) + (2 * j - 1) * wy) * changePerHit;
                //Debug.Log(addHeight + "add");
                float oldHeight = heightMap[(int)fy + j, (int)fx + i] * td.heightmapScale.y;
                // add increased height to current point
                float[,] newHeightData = new float[1, 1] { { oldHeight + addHeight } };
                CreatePile((int)fx + i, (int)fy + j, newHeightData[0, 0]);
            }
        }
    }
    void test()
    {
        for (int i = 0; i < td.heightmapResolution; i++)
            for (int j = 0; j < td.heightmapResolution; j++)
                heightMap[i, j] = 0;
        terrain.terrainData.SetHeights(0, 0, heightMap);
    }
    float findAngle(Vector3 point)
    {
        float xAdjusted = ((point.x / terrain.terrainData.size.x));
        float zAdjusted = ((point.z / terrain.terrainData.size.z));
        float actualAngle = terrain.terrainData.GetSteepness(xAdjusted, zAdjusted);
        return actualAngle;
    }
    void updateAlphaMap(int y, int x)
    {
        if (alphaMap[y, x, sandID] == 1)
        {
            return;
        }
        float[,,] element = new float[1, 1, 2]; // create a temp 1x1x2 array
        alphaMap[y, x, 0] = element[0, 0, 0] = 0; // set the element and
        alphaMap[y, x, 1] = element[0, 0, 1] = 1; // update splatmapData
        terrain.terrainData.SetAlphamaps(y, x, element);
        //alphaMap[x, y, 1] = 1;
        //alphaMap[x, y, 0] = 0;
       // td.SetAlphamaps(x, y, alphaMap);
    }
    //void updateAlphaMap(int x, int y)
    //{
    //        // process points (i,j) around current point (x,y)
    //    for (int j = -1; j <= 0; j++)
    //    {
    //        for (int i = -1; i <= 0; i++)
    //        {
    //            for (int k = 0; k < alphaLayerCount; k++)
    //            {
    //                // is dirt falling outside of the map?
    //                bool t1 = (y + j < 0 || x + i < 0);
    //                bool t2 = x + j >= alphaMapHeight - 1 || x + i >= alphaMapWidth - 1;
    //                if (t1 || t2)
    //                {
    //                    continue;
    //                }
    //                // change texture layer number to the maximum,
    //                // if it represents the texture of dirt;
    //                // otherwise, change that layer to the minimum

    //                if (k == sandID)
    //                {
    //                    alphaMap[y + j, x + i, k] = 1;
    //                }
    //                else
    //                {
    //                    alphaMap[y + j, x + i, k] = 0;
    //                }
    //            }
    //        }

    //    }

    //}
}

