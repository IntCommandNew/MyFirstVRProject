using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DartBoard : MonoBehaviour
{
    [SerializeField]
    private Vector3 faceNormalLocal = Vector3.right;

    [SerializeField]
    private Ring[] rings =
    {
        new Ring(0.035f, 10),
        new Ring(0.122f, 9),
        new Ring(0.232f, 8),
        new Ring(0.331f, 7),
        new Ring(0.451f, 6),
        new Ring(0.553f, 5),
        new Ring(0.660f, 4),
        new Ring(0.772f, 3),
        new Ring(0.884f, 2),
        new Ring(0.960f, 1),
    };

    [SerializeField]
    private UnityEvent<int> onDartHit;
    public UnityEvent<int> OnDartHit => onDartHit;

    private float LocalRadius
    {
        get
        {
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
                return 1f;

            Vector3 extents = meshFilter.sharedMesh.bounds.extents;
            Vector3 normal = faceNormalLocal.normalized;
            float radius = 0f;

            for (int axis = 0; axis < 3; axis++)
                if (Mathf.Abs(normal[axis]) < 0.5f)
                    radius = Mathf.Max(radius, extents[axis]);

            return radius;
        }
    }

    public int GetScore(Vector3 worldPoint, out float distanceFromCenter)
    {
        Vector3 local = transform.InverseTransformPoint(worldPoint);

        Vector3 onFace = Vector3.ProjectOnPlane(
            local,
            faceNormalLocal.normalized
        );

        float normalized = onFace.magnitude / LocalRadius;

        distanceFromCenter = (transform.TransformPoint(onFace) - transform.position).magnitude;

        if (Vector3.Dot(local, faceNormalLocal) < 0f)
            return 0;

        foreach (Ring ring in rings)
            if (normalized <= ring.outerRadius)
                return ring.points;

        return 0;
    }

    public void RegisterHit(DartFlight dart, Vector3 worldPoint)
    {
        int score = GetScore(worldPoint, out float distance);

        PlayHitEffects();

        onDartHit?.Invoke(score);
    }

    private void PlayHitEffects()
    {
        ParticleSystem[] effects =
            GetComponentsInChildren<ParticleSystem>(true);

        if (effects.Length == 0)
            return;

        List<Transform> effectObjects = new List<Transform>();

        foreach (ParticleSystem effect in effects)
        {
            Transform child = effect.transform;

            while (child.parent != transform && child.parent != null)
                child = child.parent;

            if (child.parent == transform && !effectObjects.Contains(child))
                effectObjects.Add(child);
        }

        if (effectObjects.Count == 0)
            return;

        Transform selectedEffect =
            effectObjects[Random.Range(0, effectObjects.Count)];

        ParticleSystem[] selectedSystems =
            selectedEffect.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particleSystem in selectedSystems)
        {
            particleSystem.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            particleSystem.Play(true);
        }
    }
}

[System.Serializable]
public struct Ring
{
    [Range(0f, 1f)]
    public float outerRadius;
    public int points;

    public Ring(float outerRadius, int points)
    {
        this.outerRadius = outerRadius;
        this.points = points;
    }
}