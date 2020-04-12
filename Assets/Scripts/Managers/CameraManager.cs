using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
    protected class CameraShake
    {
        public float strength;
        public float frequency;
        public float duration;

        public float startTime;
        public float period;
        public bool fadeOut;

        public Vector3 startPos;
        public Vector3 currentPos;
        public Vector3 nextPos;
    }

    public static CameraManager Instance;
    public Transform player;
    protected CameraShake cameraShake;
    protected Transform followedObject;
    protected Vector3 followedObjOffset;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(followedObject != null)
        {
            transform.position = new Vector3(0, followedObject.position.y, followedObject.position.z) + followedObjOffset;
        }

        transform.position += GetShakeOffset();
    }

    public Vector3 GetShakeOffset()
    {
        if (cameraShake == null)
            return Vector3.zero;

        if(cameraShake.startTime + cameraShake.duration < Time.time)
        {
            cameraShake = null;
            return Vector3.zero;
        }

        if(cameraShake.period >= cameraShake.frequency)
        {
            cameraShake.period = 0;
            cameraShake.startPos = cameraShake.currentPos;
            cameraShake.nextPos = Random.insideUnitSphere * cameraShake.strength;
        }
        else
        {
            float fadeOutCoef = (cameraShake.fadeOut ? cameraShake.duration / (Time.time - cameraShake.startTime) : 1);
            cameraShake.period += Time.deltaTime;
            cameraShake.currentPos = Vector3.Lerp(cameraShake.startPos, cameraShake.nextPos, cameraShake.period / cameraShake.frequency * fadeOutCoef);
        }

        return cameraShake.currentPos;
    }

    public void FollowObject(Transform obj, Vector3 offset)
    {
        followedObject = obj;
        followedObjOffset = offset;
    }

    public void Shake(float strength, float frequency, float duration, bool fadeOut = false)
    {
        cameraShake = new CameraShake()
        {
            strength = strength,
            frequency = frequency,
            duration = duration,
            fadeOut = fadeOut,
            startTime = Time.time,
            period = frequency
        };
    }

}
