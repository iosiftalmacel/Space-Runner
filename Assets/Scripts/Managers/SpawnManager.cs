using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    public static SpawnManager Instance;
    public static readonly int[] spawnCountProbIndex = { 10, 50, 80, 92, 100 };
    public static readonly int[] opponentPrefabProbIndex = { 90, 100 };
    public static readonly int initialOpponentPoolSize = 10;
    public static readonly int initialPickupPoolSize = 2;

    public float ExtraWindow
    {
        get
        {
            if (OverrideExtraWindow != 0)
                return OverrideExtraWindow;
            else
                return periodIncreaseCoef * PlayerManager.Instance.CurrentSpeed;
        }
    }
    public float OverrideExtraWindow { get; set; }

    public Transform spawnsParent;
    public OpponentBase[] opponentPrefabs;
    public ItemPickup[] pickupPrefabs;

    public float spawnPeriod;
    public float periodIncreaseCoef;
    public float spawnOffset;

    protected bool canSpawn;
    protected float lastSpawnTime;
    protected List<int> freeSpawnPositions;
    protected List<Queue<OpponentBase>> opponentPools;
    protected List<Queue<ItemPickup>> pickupPools;
    protected List<OpponentBase> activeOpponents;
    protected List<ItemPickup> activePickups;
    protected Vector3 spawnVector;

    #region Unity Callbacks
    void Awake()
    {
        Instance = this;
        freeSpawnPositions = new List<int>();
        activeOpponents = new List<OpponentBase>();
        activePickups = new List<ItemPickup>();
        ResetFreeSpawnPositions();
        FillOpponentPools();
        FillPickupPools();
        RepositionManager.OnReposition += RepositionSpawns;
    }

    void Start()
    {
        spawnVector = PlayerManager.Instance.player.transform.forward * spawnOffset;
    }

    void Update()
    {
        if(canSpawn && (Time.time - lastSpawnTime) > spawnPeriod / PlayerManager.Instance.CurrentSpeed + ExtraWindow)
        {
            SpawnNewOpponents();
            SpawnPickup();
            ResetFreeSpawnPositions();
            lastSpawnTime = Time.time;
        }
    }
    #endregion

    #region Private Functions
    #region Opponents
    void SpawnNewOpponents()
    {
        int spawnCount = GetRandomSpawnCount();
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnRandomOpponent(GetRandomFreeSpawnPosition());
        }
    }

    void SpawnRandomOpponent(Vector3 pos)
    {
        OpponentBase opponent = GetRandomOpponent();
        opponent.gameObject.SetActive(true);
        opponent.transform.position = pos;
        opponent.Setup();
        activeOpponents.Add(opponent);
    }

    int GetRandomSpawnCount()
    {
        int random = Random.Range(0, 101);
        int spawnCount = 0;

        for (int i = 0; i < spawnCountProbIndex.Length; i++)
        {
            if (random < spawnCountProbIndex[i])
            {
                spawnCount = i;
                break;
            }
        }
        return spawnCount;
    }

    OpponentBase GetRandomOpponent()
    {
        int random = Random.Range(0, 101);
        int spawnOpponentType = 0;

        for (int i = 0; i < opponentPrefabProbIndex.Length; i++)
        {
            if (random < opponentPrefabProbIndex[i])
            {
                spawnOpponentType = i;
                break;
            }
        }

        if (opponentPools[spawnOpponentType].Count > 0)
            return opponentPools[spawnOpponentType].Dequeue();
        else
        {
            OpponentBase opponent = Instantiate(opponentPrefabs[spawnOpponentType]);
            opponent.transform.parent = spawnsParent;
            return opponent;
        }
    }

    void FillOpponentPools()
    {
        opponentPools = new List<Queue<OpponentBase>>();

        for (int i = 0; i < opponentPrefabs.Length; i++)
        {
            opponentPools.Add(new Queue<OpponentBase>());

            for (int j = 0; j < initialOpponentPoolSize; j++)
            {
                OpponentBase newOpp = Instantiate(opponentPrefabs[i]);
                newOpp.gameObject.SetActive(false);
                newOpp.transform.parent = spawnsParent;
                opponentPools[i].Enqueue(newOpp);
            }
        }
    }
    #endregion

    #region Pickup
    void SpawnPickup()
    {
        ItemPickup randomPickupPrefab = GetRandompPickupPrefab();
        int random = Random.Range(0, 100);

        if (randomPickupPrefab.itemData.spawnProbability * (1.0f - (float)freeSpawnPositions.Count / 5) > random)
        {
            Vector3 pos = GetRandomFreeSpawnPosition();
            ItemPickup newPickup = GetPickupOfType(randomPickupPrefab);
            newPickup.gameObject.SetActive(true);
            newPickup.transform.position = pos;
            activePickups.Add(newPickup);
        }
    }

    ItemPickup GetRandompPickupPrefab()
    {
        int random = Random.Range(0, pickupPrefabs.Length);

        return pickupPrefabs[random];
    }

    ItemPickup GetPickupOfType(ItemPickup pickup)
    {
        int pickupType = 0;
        for (int i = 0; i < pickupPrefabs.Length; i++)
        {
            if (pickupPrefabs[i].GetType() == pickup.GetType())
            {
                pickupType = i;
                break;
            }
        }

        if (pickupPools[pickupType].Count > 0)
            return pickupPools[pickupType].Dequeue();
        else
        {
            ItemPickup item = Instantiate(pickupPrefabs[pickupType]);
            item.transform.parent = spawnsParent;
            return item;
        }
    }

    void FillPickupPools()
    {
        pickupPools = new List<Queue<ItemPickup>>();

        for (int i = 0; i < pickupPrefabs.Length; i++)
        {
            pickupPools.Add(new Queue<ItemPickup>());

            for (int j = 0; j < initialPickupPoolSize; j++)
            {
                ItemPickup newPick = Instantiate(pickupPrefabs[i]);
                newPick.gameObject.SetActive(false);
                newPick.transform.parent = spawnsParent;
                pickupPools[i].Enqueue(newPick);
            }
        }
    }
    #endregion

    Vector3 GetRandomFreeSpawnPosition()
    {
        Vector3 playerPos = PlayerManager.Instance.player.transform.position;
        int random = Random.Range(0, freeSpawnPositions.Count);
        int value = freeSpawnPositions[random];
        freeSpawnPositions.RemoveAt(random);
        return new Vector3(value, playerPos.y, playerPos.z) + spawnVector;
    }

    void ResetFreeSpawnPositions()
    {
        freeSpawnPositions.Clear();
        for (int i = 0; i < PlayerManager.lanesPosX.Length; i++)
        {
            freeSpawnPositions.Add(PlayerManager.lanesPosX[i]);
        }
    }
    #endregion

    #region Public Functions
    public void FreeOpponent(OpponentBase opponent)
    {
        int opponentType = 0;
        for (int i = 0; i < opponentPrefabs.Length; i++)
        {
            if (opponentPrefabs[i].GetType() == opponent.GetType())
            {
                opponentType = i;
                break;
            }
        }

        opponent.gameObject.SetActive(false);
        opponentPools[opponentType].Enqueue(opponent);
        activeOpponents.Remove(opponent);
    }

    public void FreePickup(ItemPickup pickup)
    {
        int pickupType = 0;
        for (int i = 0; i < pickupPrefabs.Length; i++)
        {
            if (pickupPrefabs[i].GetType() == pickup.GetType())
            {
                pickupType = i;
                break;
            }
        }
        pickup.gameObject.SetActive(false);
        pickupPools[pickupType].Enqueue(pickup);
        activePickups.Remove(pickup);
    }

    public void DestroyAllOpponentsWithRandomDelay(float min, float max)
    {
        foreach (var item in activeOpponents)
        {
            item.OnDamageTaken(Time.time + Random.Range(min, max));
        }
    }

    public void ClearNotDyingOpponent()
    {
        if (activeOpponents.Count > 0)
        {
            for (int i = activeOpponents.Count - 1; i >= 0; i--)
            {
                if(!activeOpponents[i].WillDie())
                    FreeOpponent(activeOpponents[i]);
            }
        }
    }

    public void Toggle(bool value)
    {
        canSpawn = value;
        lastSpawnTime = Time.time;
    }

    public void ClearSpawns()
    {
        if (activeOpponents.Count > 0)
        {
            for (int i = activeOpponents.Count - 1; i >= 0; i--)
            {
                FreeOpponent(activeOpponents[i]);
            }
        }

        if (activePickups.Count > 0)
        {
            for (int i = activePickups.Count - 1; i >= 0; i--)
            {
                FreePickup(activePickups[i]);//
            }
        }
    }

    public void RepositionSpawns(float value)
    {
        if (activeOpponents.Count > 0)
        {
            for (int i = activeOpponents.Count - 1; i >= 0; i--)
            {
                Vector3 position = activeOpponents[i].transform.position;
                position.z += value;

                activeOpponents[i].transform.position = position;
            }
        }

        if (activePickups.Count > 0)
        {
            for (int i = activePickups.Count - 1; i >= 0; i--)
            {
                Vector3 position = activePickups[i].transform.position;
                position.z += value;

                activePickups[i].transform.position = position;
            }
        }
    }

    #endregion
}
