using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundSpawner : MonoBehaviour {
    public static GroundSpawner Instance;
    public Transform enivromentParent;
    public GroundBase groundPrefab;
    public float groundSize;
    public float startOffset;
    public float endOffset;

    protected Queue<GroundBase> prefabPool;
    protected List<GroundBase> activeGrounds;
    protected int poolSize = 10;
    protected float lastGroundPos = -100;

    void Awake()
    {
        Instance = this;
        activeGrounds = new List<GroundBase>();
        RepositionManager.OnReposition += OnReposition;
        FillPool();
    }

    void Update()
    {
        Vector3 playerPos = PlayerManager.Instance.player.transform.position;

        while(lastGroundPos + groundSize * 0.5f < playerPos.z + endOffset)
        {
            lastGroundPos = Mathf.Max(lastGroundPos, playerPos.z - startOffset);

            GroundBase ground = GetNewGround();
            activeGrounds.Add(ground);
            ground.gameObject.SetActive(true);
            ground.transform.position = new Vector3(0, playerPos.y, lastGroundPos + groundSize * 0.5f);

            lastGroundPos += groundSize;
        }
    }


    void FillPool()
    {
        prefabPool = new Queue<GroundBase>();
        for (int i = 0; i < poolSize; i++)
        {
            GroundBase ground = Instantiate(groundPrefab);
            ground.gameObject.SetActive(false);
            ground.transform.parent = enivromentParent;
            prefabPool.Enqueue(ground);
        }
    }

    GroundBase GetNewGround()
    {
        if(prefabPool.Count <= 0)
        {
            GroundBase ground = Instantiate(groundPrefab);
            ground.gameObject.SetActive(false);
            ground.transform.parent = enivromentParent;
            return ground;
        }
        else
        {
            return prefabPool.Dequeue();
        }
    }

    public void FreeGroundInstance(GroundBase ground)
    {
        ground.gameObject.SetActive(false);
        prefabPool.Enqueue(ground);
        activeGrounds.Remove(ground);
    }

    public void OnReposition(float value)
    {
        lastGroundPos += value;

        if(activeGrounds.Count > 0)
        {
            for (int i = activeGrounds.Count - 1; i >= 0; i--)
            {
                Vector3 pos = activeGrounds[i].transform.position;
                pos.z += value;
                activeGrounds[i].transform.position = pos;
            }
        }
        
    }
}
