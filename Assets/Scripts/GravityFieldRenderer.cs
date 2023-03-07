using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class GravityFieldRenderer : MonoBehaviour
{
    private PlanetDataManager mPlanetDataManager;

    public Mesh mesh;
    public int PointCount = 16;
    public Vector2 PointSize = new Vector2(5,5);
    public double ScaleParameter = 10000.0f;
    public bool ShouldUpdateGrid = true;

    void Start()
    {
        mesh = new Mesh();
        mesh.name = "GridMesh";
        GetComponent<MeshFilter>().mesh = mesh;
        mPlanetDataManager = FindObjectOfType<PlanetDataManager>();
    }

    void LateUpdate()
    {
        if (!ShouldUpdateGrid) return;
        Vector3[] grid = new Vector3[PointCount*PointCount];
        int[] indexes = new int[(PointCount - 1) * (PointCount - 1) * 8];
        int counter = 0;
        for (int i = 0; i < PointCount; i++)
        {
            for (int j = 0; j < PointCount; j++)
            {
                int index = i * PointCount + j;
                grid[index] = new Vector3((i * 1.0f / (PointCount - 1) - 0.5f) * PointSize.x, 0, (j * 1.0f / (PointCount - 1) - 0.5f) * PointSize.y);
                double force = mPlanetDataManager.GetAccelForceAtPoint(mPlanetDataManager.GetFocusLerped() + new DVec3(grid[index]));
                grid[index].y = (float)(-force * ScaleParameter);
                if (i != 0 && j != 0)
                {
                    indexes[counter    ] = i * PointCount + j;
                    indexes[counter + 1] = (i-1) * PointCount + j;
                    indexes[counter + 2] = (i-1) * PointCount + j;
                    indexes[counter + 3] = (i-1) * PointCount + j - 1;
                    indexes[counter + 4] = (i-1) * PointCount + j - 1;
                    indexes[counter + 5] = i * PointCount + j - 1;
                    indexes[counter + 6] = i * PointCount + j - 1;
                    indexes[counter + 7] = i * PointCount + j;
                    counter += 8;
                }
            }
        }
        mesh.SetIndices(null, MeshTopology.Lines, 0);
        mesh.SetVertices(grid);
        mesh.SetIndices(indexes, MeshTopology.Lines, 0);
    }
}
