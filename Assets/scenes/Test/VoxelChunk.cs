using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class VoxelChunk : MonoBehaviour
{
    public MeshRenderer renderer;
    public MeshFilter meshFilter;
    public MeshCollider collider;

    public short[,,] BlockIds;
    public int nSize = 12;

    [SerializeField]
    bool GenerateMeshes = false;

    [SerializeField]
    bool GenerateIds = false;

    public float amp = 10.0f;
    public float freq = 10.0f;

    void Start()
    {
        renderer =GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        collider = GetComponent<MeshCollider>();
        GenBlockIds();
        GenerateMeshes = true;
    }

    private void Update()
    {

        if(GenerateIds)
        {
            GenBlockIds();
            GenerateIds = false;

            GenerateMeshes = true;
        }

        if(GenerateMeshes)
        {
            UnOptimizedGenMeshes();
            //GenMeshes();
        }
    }

    void GenMeshes()
    {
        Debug.Log("Generating!");

        Mesh mesh = new Mesh();
        List<Vector3> Verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indices = new List<int>();

        int Inx = 0;
        for (int x = 0; x < nSize; x++)
        {
            for (int y = 0; y < nSize; y++)
            {
                for (int z = 0; z < nSize; z++)
                {
                    Vector3Int offset = new Vector3Int(x, y, z);
                    if(BlockIds[x,y,z] == 0)
                    {
                        continue;
                    }
                    else
                    {
                        if(BlockIds[x, y + 1, z] == 0)
                        GenerateBlockTop(ref Inx, offset, Verts, normals, uvs, indices, new Rect());
                        if(BlockIds[x, y, x + 1] == 0)
                        GenerateBlockRight(ref Inx, offset, Verts, normals, uvs, indices, new Rect());
                        if(( x - 1 ) < 0 || BlockIds[x - 1, y, z] == 0)
                        GenerateBlockLeft(ref Inx, offset, Verts, normals, uvs, indices, new Rect());
                        if(BlockIds[x, y, z + 1] == 0)
                        GenerateBlockForward(ref Inx, offset, Verts, normals, uvs, indices, new Rect());
                        if((z - 1) < 0 || BlockIds[x, y, z - 1] == 0)
                        GenerateBlockBack(ref Inx, offset, Verts, normals, uvs, indices, new Rect());
                        if((y - 1) < 0 || BlockIds[x, y - 1, z] == 0)
                        GenerateBlockBottom(ref Inx, offset, Verts, normals, uvs, indices, new Rect());
                    }
                }
            }
        }
        mesh.SetVertices(Verts);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        mesh.RecalculateTangents();
        meshFilter.mesh = mesh;
        collider.sharedMesh = mesh;

        GenerateMeshes = false;
        Debug.Log("Generated!");
    }

    void GenBlockIds()
    {
        BlockIds = new short[nSize + 1, nSize + 1, nSize + 1];

        for (int x = 0; x < nSize; x++)
        {
            for(int z= 0; z < nSize; z++)
            {
                float Noise = Mathf.PerlinNoise(x / freq, z / freq) * amp;

                int y = (int)Mathf.Floor(Noise);

                if (y > nSize)
                    y = nSize;

                if (y < 0)
                    y = 0;

                BlockIds[x, y, z] = 1;

                Debug.Log($"Noise{Noise}");
            }
        }
    }

    void UnOptimizedGenMeshes()
    {
        Debug.Log("Generating!");

        Mesh mesh = new Mesh();
        List<Vector3> Verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indices = new List<int>();

        int Inx = 0;
        for (int x = 0; x < nSize; x++)
        {
            for (int y = 0; y < nSize; y++)
            {
                for (int z = 0; z < nSize; z++)
                {
                    Vector3Int offset = new Vector3Int(x, y, z);
                    if (BlockIds[x, y, z] == 0)
                    {
                        continue;
                    }
                    else
                    {
                            GenerateBlockTop(ref Inx, offset, Verts, normals, uvs, indices, new Rect());
                            GenerateBlockRight(ref Inx, offset, Verts, normals, uvs, indices, new Rect());
                            GenerateBlockLeft(ref Inx, offset, Verts, normals, uvs, indices, new Rect());
                            GenerateBlockForward(ref Inx, offset, Verts, normals, uvs, indices, new Rect());
                            GenerateBlockBack(ref Inx, offset, Verts, normals, uvs, indices, new Rect());
                            GenerateBlockBottom(ref Inx, offset, Verts, normals, uvs, indices, new Rect());
                    }
                }
            }
        }
        mesh.SetVertices(Verts);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        mesh.RecalculateTangents();
        meshFilter.mesh = mesh;
        collider.sharedMesh = mesh;

        GenerateMeshes = false;
        Debug.Log("Generated!");
    }

    void GenerateBlockTop(ref int Inx, Vector3Int Offset, List<Vector3> verts, List<Vector3> normal, List<Vector2> uvs, List<int> Indices, Rect BlockUvs)
    {
        verts.Add(new Vector3(-0.5f, 0.5f, 0.5f) + Offset);
        verts.Add(new Vector3(0.5f, 0.5f, 0.5f) + Offset);
        verts.Add(new Vector3(0.5f, 0.5f, -0.5f) + Offset);
        verts.Add(new Vector3(-0.5f, 0.5f, -0.5f) + Offset);

        normal.Add(Vector3.up);
        normal.Add(Vector3.up);
        normal.Add(Vector3.up);
        normal.Add(Vector3.up);

        uvs.Add(new Vector2(BlockUvs.xMin, BlockUvs.yMax));
        uvs.Add(new Vector2(BlockUvs.xMax, BlockUvs.yMax));
        uvs.Add(new Vector2(BlockUvs.xMax, BlockUvs.yMin));
        uvs.Add(new Vector2(BlockUvs.xMin, BlockUvs.yMin));

        Indices.Add(Inx + 0);
        Indices.Add(Inx + 1);
        Indices.Add(Inx + 2);
        Indices.Add(Inx + 0);
        Indices.Add(Inx + 2);
        Indices.Add(Inx + 3);

        Inx += 4;
    }

    void GenerateBlockRight(ref int Inx, Vector3Int Offset, List<Vector3> verts, List<Vector3> normal, List<Vector2> uvs, List<int> Indices, Rect BlockUvs)
    {
        verts.Add(new Vector3(0.5f, 0.5f, -0.5f) + Offset);
        verts.Add(new Vector3(0.5f, 0.5f, 0.5f) + Offset);
        verts.Add(new Vector3(0.5f, -0.5f, 0.5f) + Offset);
        verts.Add(new Vector3(0.5f, -0.5f, -0.5f) + Offset);

        normal.Add(Vector3.right);
        normal.Add(Vector3.right);
        normal.Add(Vector3.right);
        normal.Add(Vector3.right);

        uvs.Add(new Vector2(BlockUvs.xMin, BlockUvs.yMax));
        uvs.Add(new Vector2(BlockUvs.xMax, BlockUvs.yMax));
        uvs.Add(new Vector2(BlockUvs.xMax, BlockUvs.yMin));
        uvs.Add(new Vector2(BlockUvs.xMin, BlockUvs.yMin));

        Indices.Add(Inx + 0);
        Indices.Add(Inx + 1);
        Indices.Add(Inx + 2);
        Indices.Add(Inx + 0);
        Indices.Add(Inx + 2);
        Indices.Add(Inx + 3);

        Inx += 4;
    }

    void GenerateBlockLeft(ref int Inx, Vector3Int Offset, List<Vector3> verts, List<Vector3> normal, List<Vector2> uvs, List<int> Indices, Rect BlockUvs)
    {
        verts.Add(new Vector3(-0.5f, 0.5f, 0.5f) + Offset);
        verts.Add(new Vector3(-0.5f, 0.5f, -0.5f) + Offset);
        verts.Add(new Vector3(-0.5f, -0.5f, -0.5f) + Offset);
        verts.Add(new Vector3(-0.5f, -0.5f, 0.5f) + Offset);

        normal.Add(Vector3.left);
        normal.Add(Vector3.left);
        normal.Add(Vector3.left);
        normal.Add(Vector3.left);

        uvs.Add(new Vector2(BlockUvs.xMin, BlockUvs.yMax));
        uvs.Add(new Vector2(BlockUvs.xMax, BlockUvs.yMax));
        uvs.Add(new Vector2(BlockUvs.xMax, BlockUvs.yMin));
        uvs.Add(new Vector2(BlockUvs.xMin, BlockUvs.yMin));

        Indices.Add(Inx + 0);
        Indices.Add(Inx + 1);
        Indices.Add(Inx + 2);
        Indices.Add(Inx + 0);
        Indices.Add(Inx + 2);
        Indices.Add(Inx + 3);

        Inx += 4;
    }

    void GenerateBlockForward(ref int Inx, Vector3Int Offset, List<Vector3> verts, List<Vector3> normal, List<Vector2> uvs, List<int> Indices, Rect BlockUvs)
    {
        verts.Add(new Vector3(0.5f, 0.5f, 0.5f) + Offset);
        verts.Add(new Vector3(-0.5f, 0.5f, 0.5f) + Offset);
        verts.Add(new Vector3(-0.5f, -0.5f, 0.5f) + Offset);
        verts.Add(new Vector3(0.5f, -0.5f, 0.5f) + Offset);

        normal.Add(Vector3.right);
        normal.Add(Vector3.right);
        normal.Add(Vector3.right);
        normal.Add(Vector3.right);

        uvs.Add(new Vector2(BlockUvs.xMin, BlockUvs.yMax));
        uvs.Add(new Vector2(BlockUvs.xMax, BlockUvs.yMax));
        uvs.Add(new Vector2(BlockUvs.xMax, BlockUvs.yMin));
        uvs.Add(new Vector2(BlockUvs.xMin, BlockUvs.yMin));

        Indices.Add(Inx + 0);
        Indices.Add(Inx + 1);
        Indices.Add(Inx + 2);
        Indices.Add(Inx + 0);
        Indices.Add(Inx + 2);
        Indices.Add(Inx + 3);

        Inx += 4;
    }

    void GenerateBlockBack(ref int Inx, Vector3Int Offset, List<Vector3> verts, List<Vector3> normal, List<Vector2> uvs, List<int> Indices, Rect BlockUvs)
    {
        verts.Add(new Vector3(-0.5f, 0.5f, -0.5f) + Offset);
        verts.Add(new Vector3(0.5f, 0.5f, -0.5f) + Offset);
        verts.Add(new Vector3(0.5f, -0.5f, -0.5f) + Offset);
        verts.Add(new Vector3(-0.5f, -0.5f, -0.5f) + Offset);

        normal.Add(Vector3.back);
        normal.Add(Vector3.back);
        normal.Add(Vector3.back);
        normal.Add(Vector3.back);

        uvs.Add(new Vector2(BlockUvs.xMin, BlockUvs.yMax));
        uvs.Add(new Vector2(BlockUvs.xMax, BlockUvs.yMax));
        uvs.Add(new Vector2(BlockUvs.xMax, BlockUvs.yMin));
        uvs.Add(new Vector2(BlockUvs.xMin, BlockUvs.yMin));

        Indices.Add(Inx + 0);
        Indices.Add(Inx + 1);
        Indices.Add(Inx + 2);
        Indices.Add(Inx + 0);
        Indices.Add(Inx + 2);
        Indices.Add(Inx + 3);

        Inx += 4;
    }

    void GenerateBlockBottom(ref int Inx, Vector3Int Offset, List<Vector3> verts, List<Vector3> normal, List<Vector2> uvs, List<int> Indices, Rect BlockUvs)
    {
        verts.Add(new Vector3(-0.5f, -0.5f, -0.5f) + Offset);
        verts.Add(new Vector3(0.5f, -0.5f, -0.5f) + Offset);
        verts.Add(new Vector3(0.5f, -0.5f, 0.5f) + Offset);
        verts.Add(new Vector3(-0.5f, -0.5f, 0.5f) + Offset);

        normal.Add(Vector3.down);
        normal.Add(Vector3.down);
        normal.Add(Vector3.down);
        normal.Add(Vector3.down);

        uvs.Add(new Vector2(BlockUvs.xMin, BlockUvs.yMax));
        uvs.Add(new Vector2(BlockUvs.xMax, BlockUvs.yMax));
        uvs.Add(new Vector2(BlockUvs.xMax, BlockUvs.yMin));
        uvs.Add(new Vector2(BlockUvs.xMin, BlockUvs.yMin));

        Indices.Add(Inx + 0);
        Indices.Add(Inx + 1);
        Indices.Add(Inx + 2);
        Indices.Add(Inx + 0);
        Indices.Add(Inx + 2);
        Indices.Add(Inx + 3);

        Inx += 4;
    }
}
