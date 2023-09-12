using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class with detection memory AI methods
/// </summary>
public class AiMemory {
    public float Age {
        get {
            return Time.time - LastSeen;
        }
    }
    public GameObject GameObject;
    public Vector3 Position;
    public Vector3 Direction;
    public float Distance;
    public float Angle;
    public float LastSeen;
    public float Score;
}

public class AiSensoryMemory
{
    public List<AiMemory> Memories = new List<AiMemory>();
    GameObject[] characters;

    public AiSensoryMemory(int maxPlayers) {
        characters = new GameObject[maxPlayers];
    }

    public void UpdateSenses(AiSensor sensor) {
        int targets = sensor.Filter(characters, "Character");
        for (int i = 0; i < targets; ++i) {
            GameObject target = characters[i];
            RefreshMemory(sensor.gameObject, target);
        }
    }

    public void RefreshMemory(GameObject agent, GameObject target) {
        AiMemory memory = FetchMemory(target);
        memory.GameObject = target;
        memory.Position = target.transform.position;
        memory.Direction = target.transform.position - agent.transform.position;
        memory.Distance = memory.Direction.magnitude;
        memory.Angle = Vector3.Angle(agent.transform.forward, memory.Direction);
        memory.LastSeen = Time.time;
    }

    public AiMemory FetchMemory(GameObject gameObject) {
        AiMemory memory = Memories.Find(x => x.GameObject == gameObject);
        if (memory == null) {
            memory = new AiMemory();
            Memories.Add(memory);
        }
        return memory;
    }

    public void ForgetMemories(float olderThan) {
        Memories.RemoveAll(m => m.Age > olderThan);
        Memories.RemoveAll(m => !m.GameObject);
        Memories.RemoveAll(m => m.GameObject.GetComponent<Health>().IsDead());
    }
}
