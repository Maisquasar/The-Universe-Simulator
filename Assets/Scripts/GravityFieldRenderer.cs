using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public enum RenderType : ushort
{
    Grid2D = 0,
    Arrow2D,
    Rotate2D,
    Grid3D,
    Arrow3D,
    Rotate3D
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
    public bool IncludeAllPlanets = false;
    public double delta = 0.0001;
    public RenderType type = RenderType.Grid2D;

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
        if (!ShouldUpdateGrid || PointCount < 2) return;
        Vector3[] grid;
        int[] indexes;
        switch (type)
        {
            case RenderType.Grid2D:
                Create2DGrid(out grid, out indexes);
                break;
            case RenderType.Arrow2D:
                Create2DVec(out grid, out indexes);
                break;
            case RenderType.Rotate2D:
                Create2DRotate(out grid, out indexes);
                break;
            case RenderType.Grid3D:
                Create3DGrid(out grid, out indexes);
                break;
            case RenderType.Arrow3D:
                Create3DVec(out grid, out indexes);
                break;
            default:
                Create3DRotate(out grid, out indexes);
                break;
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

    private void Create2DGrid(out Vector3[] grid, out int[] indexes)
    {
        grid = new Vector3[PointCount * PointCount];
        indexes = new int[(PointCount - 1) * (PointCount - 1) * 4 + (PointCount - 1) * 4];
        int counter = 0;
        for (int i = 0; i < PointCount; ++i)
        {
            for (int j = 0; j < PointCount; ++j)
            {
                int index = i * PointCount + j;
                grid[index] = new Vector3((i * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize, 0, (j * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize);
                double force = mPlanetDataManager.GetAccelForceAtPoint(mPlanetDataManager.GetFocusLerped() + new DVec3(grid[index]), null, IncludeAllPlanets ? 0 : PointSize / 2);
                grid[index].y = (float)(-force * ScaleParameter);
                if (i != PointCount - 1)
                {
                    indexes[counter] = i * PointCount + j;
                    indexes[counter + 1] = (i + 1) * PointCount + j;
                    counter += 2;
                }
                if (j != PointCount - 1)
                {
                    indexes[counter] = i * PointCount + j;
                    indexes[counter + 1] = i * PointCount + j + 1;
                    counter += 2;
                }
            }
        }
    }

    private void Create3DGrid(out Vector3[] grid, out int[] indexes)
    {
        grid = new Vector3[PointCount * PointCount * PointCount];
        indexes = new int[(PointCount - 1) * (PointCount - 1) * (PointCount - 1) * 6 + (PointCount - 1) * (PointCount - 1) * 12 + (PointCount - 1) * 6];
        int counter = 0;
        for (int i = 0; i < PointCount; ++i)
        {
            for (int j = 0; j < PointCount; ++j)
            {
                for (int k = 0; k < PointCount; ++k)
                {
                    int index = (i * PointCount + j) * PointCount + k;
                    grid[index] = new Vector3((i * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize, (j * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize, (k * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize);
                    if (!mPlanetDataManager.IsPosInsideSomething(grid[index]))
                    {
                        DVec3 force = mPlanetDataManager.GetAccelAtPoint(mPlanetDataManager.GetFocusLerped() + new DVec3(grid[index]), null, true, IncludeAllPlanets ? 0 : PointSize / 2) * ScaleParameter;
                        if (force.Length() > PointSize / PointCount) force = force.Normalized() * (PointSize / (PointCount - 1));
                        grid[index] = grid[index] + force.AsVector();
                    }
                    if (i != PointCount - 1)
                    {
                        indexes[counter] = (i * PointCount + j) * PointCount + k;
                        indexes[counter + 1] = ((i + 1) * PointCount + j) * PointCount + k;
                        counter += 2;
                    }
                    if (j != PointCount - 1)
                    {
                        indexes[counter] = (i * PointCount + j) * PointCount + k;
                        indexes[counter + 1] = (i * PointCount + j + 1) * PointCount + k;
                        counter += 2;
                    }
                    if (k != PointCount - 1)
                    {
                        indexes[counter] = (i * PointCount + j) * PointCount + k;
                        indexes[counter + 1] = (i * PointCount + j) * PointCount + k + 1;
                        counter += 2;
                    }
                }
            }
        }
    }

    private void Create2DVec(out Vector3[] grid, out int[] indexes)
    {
        grid = new Vector3[PointCount * PointCount * 2];
        indexes = new int[PointCount * PointCount * 2];
        int counter = 0;
        for (int i = 0; i < PointCount; ++i)
        {
            for (int j = 0; j < PointCount; ++j)
            {
                int index = (i * PointCount + j) * 2;
                grid[index] = new Vector3((i * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize, 0, (j * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize);
                if (!mPlanetDataManager.IsPosInsideSomething(grid[index]))
                {
                    DVec3 force = mPlanetDataManager.GetAccelAtPoint(mPlanetDataManager.GetFocusLerped() + new DVec3(grid[index]), null, true, IncludeAllPlanets ? 0 : PointSize / 2) * ScaleParameter;
                    if (force.Length() > PointSize / PointCount) force = force.Normalized() * (PointSize / (PointCount - 1));
                    grid[index + 1] = grid[index] + force.AsVector();
                }
                else
                {
                    grid[index + 1] = grid[index];
                }
                indexes[counter] = (i * PointCount + j) * 2;
                indexes[counter + 1] = (i * PointCount + j) * 2 + 1;
                counter += 2;
            }
        }
    }

    private void Create3DVec(out Vector3[] grid, out int[] indexes)
    {
        grid = new Vector3[PointCount * PointCount * PointCount * 2];
        indexes = new int[PointCount * PointCount * PointCount * 2];
        int counter = 0;
        for (int i = 0; i < PointCount; ++i)
        {
            for (int j = 0; j < PointCount; ++j)
            {
                for (int k = 0; k < PointCount; ++k)
                {
                    int index = ((i * PointCount + j) * PointCount + k) * 2;
                    grid[index] = new Vector3((i * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize, (j * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize, (k * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize);
                    if (!mPlanetDataManager.IsPosInsideSomething(grid[index]))
                    {
                        DVec3 force = mPlanetDataManager.GetAccelAtPoint(mPlanetDataManager.GetFocusLerped() + new DVec3(grid[index]), null, true, IncludeAllPlanets ? 0 : PointSize / 2) * ScaleParameter;
                        if (force.Length() > PointSize / PointCount) force = force.Normalized() * (PointSize / (PointCount - 1));
                        grid[index + 1] = grid[index] + force.AsVector();
                    }
                    else
                    {
                        grid[index + 1] = grid[index];
                    }
                    indexes[counter] = ((i * PointCount + j) * PointCount + k) * 2;
                    indexes[counter + 1] = ((i * PointCount + j) * PointCount + k) * 2 + 1;
                    counter += 2;
                }
            }
        }
    }

    private void Create2DRotate(out Vector3[] grid, out int[] indexes)
    {
        grid = new Vector3[PointCount * PointCount * 2];
        indexes = new int[PointCount * PointCount * 2];
        int counter = 0;
        for (int i = 0; i < PointCount; ++i)
        {
            for (int j = 0; j < PointCount; ++j)
            {
                int index = (i * PointCount + j) * 2;
                grid[index] = new Vector3((i * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize, 0, (j * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize);
                if (!mPlanetDataManager.IsPosInsideSomething(grid[index]))
                {
                    DVec3 point = mPlanetDataManager.GetFocusLerped() + new DVec3(grid[index]);
                    DVec3 forceA = mPlanetDataManager.GetAccelAtPoint(point, null, true, IncludeAllPlanets ? 0 : PointSize / 2);
                    DVec3 forceB = mPlanetDataManager.GetAccelAtPoint(point + new DVec3(delta), null, true, IncludeAllPlanets ? 0 : PointSize / 2);
                    DVec3 deriv = (forceB - forceA) / delta * ScaleParameter;
                    if (deriv.Length() > PointSize / PointCount) deriv = deriv.Normalized() * (PointSize / (PointCount - 1));
                    grid[index + 1] = grid[index] + deriv.AsVector();
                }
                else
                {
                    grid[index + 1] = grid[index];
                }
                indexes[counter] = (i * PointCount + j) * 2;
                indexes[counter + 1] = (i * PointCount + j) * 2 + 1;
                counter += 2;
            }
        }
    }

    private void Create3DRotate(out Vector3[] grid, out int[] indexes)
    {
        grid = new Vector3[PointCount * PointCount * PointCount * 2];
        indexes = new int[PointCount * PointCount * PointCount * 2];
        int counter = 0;
        for (int i = 0; i < PointCount; ++i)
        {
            for (int j = 0; j < PointCount; ++j)
            {
                for (int k = 0; k < PointCount; ++k)
                {
                    int index = ((i * PointCount + j) * PointCount + k) * 2;
                    grid[index] = new Vector3((i * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize, (j * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize, (k * 1.0f / (PointCount - 1) - 0.5f) * (float)PointSize);
                    if (!mPlanetDataManager.IsPosInsideSomething(grid[index]))
                    {
                        DVec3 point = mPlanetDataManager.GetFocusLerped() + new DVec3(grid[index]);
                        DVec3 forceR = mPlanetDataManager.GetAccelAtPoint(point, null, true, IncludeAllPlanets ? 0 : PointSize / 2);
                        DVec3 deltaX = (mPlanetDataManager.GetAccelAtPoint(point + new DVec3(delta,0,0), null, true, IncludeAllPlanets ? 0 : PointSize / 2) - forceR) / delta;
                        DVec3 deltaY = (mPlanetDataManager.GetAccelAtPoint(point + new DVec3(0,delta,0), null, true, IncludeAllPlanets ? 0 : PointSize / 2) - forceR) / delta;
                        DVec3 deltaZ = (mPlanetDataManager.GetAccelAtPoint(point + new DVec3(0,0,delta), null, true, IncludeAllPlanets ? 0 : PointSize / 2) - forceR) / delta;
                        DVec3 result = new DVec3(
                            deltaY.z - deltaZ.y,
                            deltaZ.x - deltaX.z,
                            deltaX.y - deltaY.x
                            ) * ScaleParameter;
                        if (result.Length() > PointSize / PointCount) result = result.Normalized() * (PointSize / (PointCount - 1));
                        grid[index + 1] = grid[index] + result.AsVector();
                    }
                    else
                    {
                        grid[index + 1] = grid[index];
                    }
                    indexes[counter] = ((i * PointCount + j) * PointCount + k) * 2;
                    indexes[counter + 1] = ((i * PointCount + j) * PointCount + k) * 2 + 1;
                    counter += 2;
                }
            }
        }
    }
}
