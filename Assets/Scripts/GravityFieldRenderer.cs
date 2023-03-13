using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public enum RenderType : ushort
{
    Grid2D = 0,
    Grid3D = 1,
    Arrow3D = 2
}

public class GravityFieldRenderer : MonoBehaviour
{
    private PlanetDataManager mPlanetDataManager;

    public Mesh mesh;
    private MeshFilter filter;
    public int PointCount = 16;
    public double PointSize = 5.0;
    public double ScaleParameter = 10000.0f;
    public bool ShouldUpdateGrid = true;

    void Start()
    {
        mesh = new Mesh();
        mesh.name = "GridMesh";
        filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
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
                grid[index] = new Vector3((i * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize, 0, (j * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize);
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

        Transform camTransform = Camera.main.transform;
        float distToCenter = (Camera.main.farClipPlane - Camera.main.nearClipPlane) / 2.0f;
        Vector3 center = camTransform.position + camTransform.forward * distToCenter;
        float extremeBound = 500.0f;
        filter.sharedMesh.bounds = new Bounds(center, Vector3.one * extremeBound);
    }
}
