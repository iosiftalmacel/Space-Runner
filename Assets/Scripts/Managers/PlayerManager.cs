using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
    public static PlayerManager Instance;
    public static readonly int[] lanesPosX = { -4, -2, 0, 2, 4 };

    public enum PlayerState
    {
        Idle,
        Enter,
        Running,
    }

    public float CurrentSpeed { get { return OverrideSpeed != 0 ? OverrideSpeed : speed; } }
    public float OverrideSpeed { get; set; }
    public float Progress { get; set; }
    public PlayerState State { get; set; }

    public PlayerBase player;

    public float minSpeed;
    public float maxSpeed;
    public float acceleration;
    public float laneChangeDuration;

    protected float speed;
    protected float laneChangeTime;
    protected int prevLaneIndex = 2;
    protected int currentLaneIndex = 2;
    protected bool inTransition;

    void Awake()
    {
        Instance = this;
        player.OnPlayerHit += OnPlayerHit;
        RepositionManager.OnReposition += RepositionPlayer;
    }

    void Start()
    {
        CameraManager.Instance.FollowObject(player.transform, new Vector3(1.75f, 7.5f, 0));

        InputsManager.Instance.OnSwipeLeft += OnSwipeLeft;
        InputsManager.Instance.OnSwipeRight += OnSwipeRight;

        Progress = 0;
    }

    void Update()
    {
        if (State == PlayerState.Running)
        {
            speed = Mathf.Clamp(speed + acceleration * Time.deltaTime, minSpeed, maxSpeed);
            Progress += CurrentSpeed * Time.deltaTime;

            player.transform.position += Vector3.forward * CurrentSpeed * Time.deltaTime;

            if (prevLaneIndex != currentLaneIndex)
            {
                float xPos = Mathf.Lerp(lanesPosX[prevLaneIndex], lanesPosX[currentLaneIndex], (Time.time - laneChangeTime) / laneChangeDuration);
                player.transform.position = new Vector3(xPos, player.transform.position.y, player.transform.position.z);

                if (Time.time - laneChangeTime > laneChangeDuration)
                {
                    inTransition = false;
                    prevLaneIndex = currentLaneIndex;
                }
            }
        }
        else if(State == PlayerState.Enter)
        {
            player.anim.Play("enter");
            State = PlayerState.Running;
        }
    }

    void OnPlayerHit(Collider colider)
    {
        if (/*!inTransition &&*/ colider.CompareTag("Enemy"))
        {
            if (player.OnDamageTaken())
            {
                //State = PlayerState.Idle;
                speed = 0;
                GameManager.Instance.OnPlayerDestoyed();
            }
        }
    }

    void OnSwipeLeft()
    {
        if (currentLaneIndex <= 0)
            return;
        if (State == PlayerState.Running)
            player.anim.CrossFade("left", 0.1f, 0, 0);

        currentLaneIndex--;
        inTransition = true;
        laneChangeTime = Time.time;
    }

    void OnSwipeRight()
    {
        if (currentLaneIndex >= 4)
            return;

        if(State == PlayerState.Running)
            player.anim.CrossFade("right", 0.1f, 0, 0);

        currentLaneIndex++;
        inTransition = true;
        laneChangeTime = Time.time;
    }

    public void ClearPlayerPickups()
    {
        for (int i = player.ownedItems.Count - 1; i >= 0; i--)
        {
            ItemBase item = player.ownedItems[i];
            item.ClearEffects();
            player.ownedItems.Remove(item);
            Destroy(item);
        }
    }

    public void RepositionPlayer(float value)
    {
        Vector3 position = player.transform.position;
        position.z += value;

        player.transform.position = position;
    }
}
