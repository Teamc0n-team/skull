using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

[System.Serializable]
public struct BlockData
{ 
    public Color color;
    public float Metalic;
    public float Smoothness;
}

[System.Serializable]
public struct VoxelColor32
{
    public float color;
    public float metallic;
    public float smoothness;
}

public class WorldManager : MonoBehaviour
{
    public Material worldMat;
    public BlockData[] BlocksIds;
    public WorldSettings worldSettings;

    public Container container;
    private Vector3 lastUpdatedPosition;
    private Vector3 previouslyCheckedPosition;

    public ConcurrentDictionary<Vector3, Dictionary<Vector3, Voxel>> modifiedVoxels = new ConcurrentDictionary<Vector3, Dictionary<Vector3, Voxel>>();
    public ConcurrentDictionary<Vector3, Container> activeContainers;
    public Queue<Container> containerPool;
    ConcurrentQueue<Vector3> containersNeedCreation = new ConcurrentQueue<Vector3>();
    ConcurrentQueue<Vector3> deactiveContainers = new ConcurrentQueue<Vector3>();

    public int maxChunksToProcessPerFrame = 6;
    int mainThreadID;
    Thread checkActiveChunks;
    bool killThreads = false;
    bool performedFirstPass = false;

    void Start()
    {
        if(inst != null)
        {
            if(inst != this) Destroy(this);
        }
        else
        {
            inst = this;
        }

        WorldSettings = worldSettings;

        int renderSizePlusExcess = WorldSettings.renderDistance + 3;
        int totalContainers = renderSizePlusExcess * renderSizePlusExcess;

        ComputeManager.Instance.Initialize(6*3);

        activeContainers = new ConcurrentDictionary<Vector3, Container>();
        containerPool = new Queue<Container>();

        mainThreadID = Thread.CurrentThread.ManagedThreadId;

        for (int i = 0; i < totalContainers; i++)
        {
            GenerateContainer(Vector3.zero, true);
        }

        checkActiveChunks = new Thread(CheckActiveChunksLoop);
        checkActiveChunks.Priority = System.Threading.ThreadPriority.BelowNormal;
        checkActiveChunks.Start();
    }

    public static WorldManager inst;
    public static WorldManager Instance
    {
        get
        {
            if(inst == null)
                inst = FindObjectOfType<WorldManager>();
            return inst;
        }
    }

    private void Update()
    {
        if (Camera.main.transform.position != lastUpdatedPosition)
        {
            //Update position so our CheckActiveChunksLoop thread has it
            lastUpdatedPosition = positionToChunkCoord(Camera.main.transform.position);
        }

        Vector3 contToMake;

        while (deactiveContainers.Count > 0 && deactiveContainers.TryDequeue(out contToMake))
        {
            deactiveContainer(contToMake);
        }
        for (int x = 0; x < maxChunksToProcessPerFrame; x++)
        {
            if (x < maxChunksToProcessPerFrame && containersNeedCreation.Count > 0 && containersNeedCreation.TryDequeue(out contToMake))
            {
                Container container = GetContainer(contToMake);
                container.pos = contToMake;
                activeContainers.TryAdd(contToMake, container);
                ComputeManager.Instance.GenerateVoxelData(container, contToMake);
                x++;
            }

        }
    }

    void CheckActiveChunksLoop()
    {
        Profiler.BeginThreadProfiling("Chunks", "ChunkChecker");
        int halfRenderSize = WorldSettings.renderDistance / 2;
        int renderDistPlus1 = WorldSettings.renderDistance + 1;
        Vector3 pos = Vector3.zero;

        Bounds chunkBounds = new Bounds();
        chunkBounds.size = new Vector3(renderDistPlus1 * WorldSettings.containerSize, 1, renderDistPlus1 * WorldSettings.containerSize);
        while (true && !killThreads)
        {
            if (previouslyCheckedPosition != lastUpdatedPosition || !performedFirstPass)
            {
                previouslyCheckedPosition = lastUpdatedPosition;

                for (int x = -halfRenderSize; x < halfRenderSize; x++)
                    for (int z = -halfRenderSize; z < halfRenderSize; z++)
                    {
                        pos.x = x * WorldSettings.containerSize + previouslyCheckedPosition.x;
                        pos.z = z * WorldSettings.containerSize + previouslyCheckedPosition.z;

                        if (!activeContainers.ContainsKey(pos))
                        {
                            containersNeedCreation.Enqueue(pos);
                        }
                    }

                chunkBounds.center = previouslyCheckedPosition;

                foreach (var kvp in activeContainers)
                {
                    if (!chunkBounds.Contains(kvp.Key))
                        deactiveContainers.Enqueue(kvp.Key);
                }
            }

            if (!performedFirstPass)
                performedFirstPass = true;

            Thread.Sleep(300);
        }
        Profiler.EndThreadProfiling();
    }

    public Container GetContainer(Vector3 pos)
    {
        if (containerPool.Count > 0)
        {
            return containerPool.Dequeue();
        }
        else
        {
            return GenerateContainer(pos, false);
        }
    }

    Container GenerateContainer(Vector3 position, bool enqueue = true)
    {
        if (Thread.CurrentThread.ManagedThreadId != mainThreadID)
        {
            containersNeedCreation.Enqueue(position);
            return null;
        }
        Container container = new GameObject().AddComponent<Container>();
        container.transform.parent = transform;
        container.pos = position;
        container.Init(worldMat, position);

        if (enqueue)
        {
            container.gameObject.SetActive(false);
            containerPool.Enqueue(container);
        }

        return container;
    }

    public bool deactiveContainer(Vector3 position)
    {
        if (activeContainers.ContainsKey(position))
        {
            if (activeContainers.TryRemove(position, out Container c))
            {
                c.ClearData();
                containerPool.Enqueue(c);
                c.gameObject.SetActive(false);
                return true;
            }
            else
                return false;

        }

        return false;
    }

    public static Vector3 positionToChunkCoord(Vector3 pos)
    {
        pos /= WorldSettings.containerSize;
        pos = math.floor(pos) * WorldSettings.containerSize;
        pos.y = 0;
        return pos;
    }

    private void OnApplicationQuit()
    {
        killThreads = true;
        checkActiveChunks?.Abort();

        foreach (var c in activeContainers.Keys)
        {
            if (activeContainers.TryRemove(c, out var cont))
            {
                cont.Dispose();
            }
        }

        //Try to force cleanup of editor memory
#if UNITY_EDITOR
        EditorUtility.UnloadUnusedAssetsImmediate();
        GC.Collect();
#endif
    }

    public static WorldSettings WorldSettings;
}

[System.Serializable]
public class WorldSettings
{
    public int containerSize = 16;
    public int maxHeight = 128;
    public int renderDistance;

    public int ChunkCount
    {
        get { return (containerSize + 3) * (maxHeight + 1) * (containerSize + 3); }
    }
}