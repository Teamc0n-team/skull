using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Voxel
{
    public int nId;

    public bool IsSolid
    {
        get
        {
            return nId != 0;
        }
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Container : MonoBehaviour
{
    //public NoiseBuffer NoiseBuffer;
    private MeshData meshData = new MeshData();

    public Vector3 pos;

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    Container()
    {
    }

    public void Init(Material mat, Vector3 Pos)
    {
        ConfigureComponents();
        meshData = new MeshData();
        meshData.Init();
        meshRenderer.sharedMaterial = mat;
        this.pos = Pos;
    }

    public void Dispose()
    {
        meshData.ClearData();
        meshData.indices = null;
        meshData.verts = null;
        meshData.Color = null;
    }


    public Voxel this[Vector3 index]
    {
        get
        {
            if (WorldManager.Instance.modifiedVoxels.ContainsKey(pos))
            {
                if (WorldManager.Instance.modifiedVoxels[pos].ContainsKey(index))
                {
                    return WorldManager.Instance.modifiedVoxels[pos][index];
                }
                else return new Voxel() { nId = 0 };
            }
            else return new Voxel() { nId = 0 };
        }

        set
        {
            if (!WorldManager.Instance.modifiedVoxels.ContainsKey(pos))
                WorldManager.Instance.modifiedVoxels.TryAdd(pos, new Dictionary<Vector3, Voxel>());
            if (!WorldManager.Instance.modifiedVoxels[pos].ContainsKey(index))
                WorldManager.Instance.modifiedVoxels[pos].Add(index, value);
            else
                WorldManager.Instance.modifiedVoxels[pos][index] = value;
        }
    }


    public void ClearData()
    {
        meshFilter.sharedMesh = null;
        meshCollider.sharedMesh = null;
        meshData.ClearData();
    }

    void ConfigureComponents()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    bool checkVoxelIsSolid(Vector3 point)
    {
        if (point.y < 0 || (point.x > WorldManager.WorldSettings.containerSize + 2) || (point.z > WorldManager.WorldSettings.containerSize + 2))
            return true;
        else
            return this[point].IsSolid;
    }

    public void UploadMesh(MeshBuffer meshBuffer)
    {

        if (meshRenderer == null)
            ConfigureComponents();

        //Get the count of vertices/tris from the shader
        int[] faceCount = new int[2] { 0, 0 };
        meshBuffer.countBuffer.GetData(faceCount);

        //Get all of the meshData from the buffers to local arrays
        meshBuffer.vertexBuffer.GetData(meshData.verts, 0, 0, faceCount[0]);
        meshBuffer.indexBuffer.GetData(meshData.indices, 0, 0, faceCount[0]);
        meshBuffer.colorBuffer.GetData(meshData.Color, 0, 0, faceCount[0]);

        //Assign the mesh
        meshData.mesh = new Mesh();
        meshData.mesh.SetVertices(meshData.verts, 0, faceCount[0]);
        meshData.mesh.SetIndices(meshData.indices, 0, faceCount[0], MeshTopology.Triangles, 0);
        meshData.mesh.SetColors(meshData.Color, 0, faceCount[0]);

        meshData.mesh.RecalculateNormals();
        meshData.mesh.RecalculateBounds();
        meshData.mesh.Optimize();
        meshData.mesh.UploadMeshData(true);

        meshFilter.sharedMesh = meshData.mesh;
        meshCollider.sharedMesh = meshData.mesh;

        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

    }

    public struct MeshData
    {
        public int[] indices;
        public Vector3[] verts;
        public Color[] Color;
        public Mesh mesh;

        public int arraySize;

        public void Init()
        {
            int maxTris = WorldManager.WorldSettings.containerSize * WorldManager.WorldSettings.maxHeight * WorldManager.WorldSettings.containerSize / 4;
            arraySize = maxTris * 3;
            mesh = new Mesh();

            indices = new int[arraySize];
            verts = new Vector3[arraySize];
            Color = new Color[arraySize];
        }

        public void ClearData()
        {
            //Completely clear the mesh reference to help prevent memory problems
            mesh.Clear();
            Destroy(mesh);
            mesh = null;
        }

    }
    static readonly Vector3[] voxelVertices = new Vector3[8]
{
            new Vector3(0,0,0),//0
            new Vector3(1,0,0),//1
            new Vector3(0,1,0),//2
            new Vector3(1,1,0),//3

            new Vector3(0,0,1),//4
            new Vector3(1,0,1),//5
            new Vector3(0,1,1),//6
            new Vector3(1,1,1),//7
};

    static readonly int[,] voxelVertexIndex = new int[6, 4]
    {
            {0,1,2,3},
            {4,5,6,7},
            {4,0,6,2},
            {5,1,7,3},
            {0,1,4,5},
            {2,3,6,7},
    };

    static readonly Vector3[] voxelFaceChecks = new Vector3[6]
{
            new Vector3(0,0,-1),//back
            new Vector3(0,0,1),//front
            new Vector3(-1,0,0),//left
            new Vector3(1,0,0),//right
            new Vector3(0,-1,0),//bottom
            new Vector3(0,1,0)//top
};


    static readonly Vector2[] voxelUVs = new Vector2[4]
    {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,0),
            new Vector2(1,1)
    };

    static readonly int[,] voxelTris = new int[6, 6]
    {
            {0,2,3,0,3,1},
            {0,1,2,1,3,2},
            {0,2,3,0,3,1},
            {0,1,2,1,3,2},
            {0,1,2,1,3,2},
            {0,2,3,0,3,1},
    };
}
