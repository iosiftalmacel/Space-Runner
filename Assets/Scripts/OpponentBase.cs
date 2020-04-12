using UnityEngine;
using System.Collections;

public abstract class OpponentBase : MonoBehaviour {
    public float speed;
    public ParticleSystem explosion;

    protected bool seen;
    protected Vector3 startPosition;
    protected float startWorldPos;
    protected float timeAlive;

    protected bool destroyed;
    protected float destroyedTime;

    public void OnBecameVisible()
    {
        seen = true;
    }

    public void OnBecameInvisible()
    {
        if (seen)
        {
            SpawnManager.Instance.FreeOpponent(this);
        }
    }
  
    public void Setup()
    {
        destroyedTime = -1;
        startPosition = transform.position;
        seen = false;
        destroyed = false;
        explosion.transform.parent = transform;
        explosion.transform.localPosition = new Vector3(0, explosion.transform.localPosition.y, 0);
        explosion.gameObject.SetActive(false);
    }

    void Update()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;

        if (destroyedTime != -1 && Time.time > destroyedTime && !destroyed)
        {
            destroyed = true;
            explosion.transform.parent = transform.parent;
            explosion.gameObject.SetActive(true);
        }
        else if (destroyed && Time.time - destroyedTime > 0.1f)
        {
            seen = false;
            SpawnManager.Instance.FreeOpponent(this);
        }
    }


    public virtual void OnDamageTaken(float time)
    {
        if(destroyedTime == -1)
            destroyedTime = time;
    }

    public bool WillDie()
    {
        return destroyedTime != -1;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            OnDamageTaken(Time.time);
        }
    }
}
