using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rejectfunction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
/*
 *     Vector3 findStopLocation(Vector3 point)
    {
        if (underMaxDeltaHeight(point))
        {
            return point;
        }
        
        //if (isUnderMax(new Vector3(x, 0f, z)))
        //{
        //    return new Vector3(x, 0f, z);
        //}
        //If it isn't at the max angle add to it
        //if (lessThanAngle(new Vector3(x, 0f, z)) && isUnderMax(new Vector3(x, 0f, z)))
        //if(true)
        //{
        //    return new Vector3(x, 0f, z);
        //}
        //else if (willDecreaseAngle(new Vector3(x, 0f, z)))
        //{
        //    return new Vector3(x, 0f, z);
        //}

        //It is too high.  The sand particle falls down hill.  Find the smallest point around it
        System.Random r = new System.Random();
        return findStopLocation(new Vector3((float)(r.Next(-1, 2) * heightMapScale.x + point.x), 0f,(float)(r.Next(-1, 2) * heightMapScale.z + point.z)));
       // return new Vector3((float)(r.Next(-1, 2)*heightMapScale.x + x),0f, (float)(r.Next(-1, 2) * heightMapScale.z + z));
        //return findStopLocation((float)(r.Next(-1, 2) + x), (float)(r.Next(-1, 2) + z));
        
    }
    bool isUnderMax(Vector3 point)
    {
        //bool underMax = true;
        //Check if point is under the angle for every point around it
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (!lessThanAngle(point, new Vector3(point.x + i, point.y, point.z + j)))
                {
                    Debug.Log(hitMap[(int)point.x, (int)point.z]);
                    Debug.Log(hitMap[(int)point.x + 1, (int)point.z + 1]);
                    return false;
                }
            }
        }
        return true;
    }
    bool lessThanAngle(Vector3 center, Vector3 edge)
    {
        int delta = hitMap[(int)center.x, (int)center.z] - hitMap[(int)edge.x, (int)edge.z];
        if (delta <= 1)
        {
            
            return true;
        }
        return false;
        //Debug.Log((float)delta / (Mathf.Tan((float)coneAngle * Mathf.PI / 180)));
        //return (delta/(Mathf.Tan(coneAngle * Mathf.PI / 180)) < 1);
    }
    bool willDecreaseAngle(Vector3 point)
    {
        float actualAngle = findAngle(point);
        raiseTerrainHeight(point,1f);
        float newAngle = findAngle(point);
        raiseTerrainHeight(point,-1f);
        return (newAngle < actualAngle);

    }
   bool underMaxDeltaHeight2(Vector3 point)
    {
        //float x = adjust(point.x);
        //float z = adjust(point.z);

        for (int x = (int)adjust(point.x) - (int)heightMapScale.x; x < x + 3 * (int)heightMapScale.x; x += (int)heightMapScale.x)
        {
            for (int z = adjust(point.z) - (int)heightMapScale.z; z < z + 3 * (int)heightMapScale.z; z += (int)heightMapScale.z)
            {
                if (heightMap[(int)point.x, (int)point.z]  > heightMap[x,z] + changePerHit + changePerHit)
                {
                    return false;
                }
            }
        }
        return true;
    }
 bool isLowerThanNeighbors(Vector3 point)
    {
        float xAdjusted = adjust(point.x);
           // ((point.x / terrain.terrainData.size.x));
        float zAdjusted = adjust(point.z);
          //  ((point.z / terrain.terrainData.size.z));


        float middle = terrain.terrainData.GetHeight((int)xAdjusted, (int)zAdjusted);
        for (int x = (int)(xAdjusted - heightMapScale.x); x < (int)((xAdjusted + 1) * (int)heightMapScale.x); xAdjusted += (int)heightMapScale.x)
        {
            for (int z = (int)(zAdjusted - heightMapScale.z); z < (int)((zAdjusted + 1) * (int)heightMapScale.z); zAdjusted += (int)heightMapScale.z)
            {
                if (middle < terrain.terrainData.GetHeight((int)xAdjusted, (int)zAdjusted))
                    return true;
            }
        }
        return false;
        float smallest = heightMap[(int)xAdjusted, (int)zAdjusted];




        for (xAdjusted = xAdjusted - (int)heightMapScale.x; xAdjusted < xAdjusted + 3 * (int)heightMapScale.x; xAdjusted += (int)heightMapScale.x)
        {
            for (zAdjusted = zAdjusted - (int)heightMapScale.z; zAdjusted < zAdjusted + 3 * (int)heightMapScale.z; zAdjusted += (int)heightMapScale.z)
            {
                if (smallest > heightMap[(int)xAdjusted, (int)zAdjusted])
                {
                    return false;
                }
            }
        }
        return true;
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

*/