using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class with targeting-related AI methods
/// </summary>
[ExecuteInEditMode]
public class AiTargetingSystem : MonoBehaviour
{
    public bool debug;
    public float memorySpan = 3.0f;
    public float distanceWeight = 1.0f;
    public float angleWeight = 1.0f;
    public float ageWeight = 1.0f;

    public bool HasTarget {
        get {
            return _bestMemory != null;
        }
    }

    public GameObject Target {
        get {
            return _bestMemory.GameObject;
        }
    }

    public Vector3 TargetPosition {
        get {
            return _bestMemory.GameObject.transform.position;
        }
    }

    public bool TargetInSight {
        get {
            return _bestMemory.Age < 0.5f; //seconds
        }
    }

    public float TargetDistance {
        get {
            return _bestMemory.Distance;
        }
    }

    AiSensoryMemory _memory = new AiSensoryMemory(10);
    AiSensor _sensor;
    AiMemory _bestMemory;
    
    void Start()
    {
        _sensor = GetComponent<AiSensor>();
    }
    
    void Update()
    {
        _memory.UpdateSenses(_sensor);
        _memory.ForgetMemories(memorySpan);

        EvaluateScores();
    }

    void EvaluateScores() {
        _bestMemory = null;

        foreach (var aiMemory in _memory.Memories) {
            aiMemory.Score = CalculateScore(aiMemory);
            if (_bestMemory == null ||
                aiMemory.Score > _bestMemory.Score) {
                _bestMemory = aiMemory;
            }
        }
    }

    float Normalize(float value, float maxValue) {
        return 1.0f - (value / maxValue);
    }

    float CalculateScore(AiMemory aiMemory) {
        float distanceScore = Normalize(aiMemory.Distance, _sensor.distance) * distanceWeight;
        float angleScore = Normalize(aiMemory.Angle, _sensor.angle) * angleWeight;
        float ageScore = Normalize(aiMemory.Age, memorySpan) * ageWeight;
        return distanceScore + angleScore + ageScore;
    }

    private void OnDrawGizmos() {
        if (debug) {
            float maxScore = float.MinValue;
            foreach (var aiMemory in _memory.Memories) {
                maxScore = Mathf.Max(maxScore, aiMemory.Score);
            }

            foreach (var aiMemory in _memory.Memories) {
                Color color = Color.red;
                if (aiMemory == _bestMemory) {
                    color = Color.yellow;
                }
                color.a = aiMemory.Score / maxScore;
                Gizmos.color = color;
                Gizmos.DrawSphere(aiMemory.Position, 0.2f);
            }
        }
    }
}
