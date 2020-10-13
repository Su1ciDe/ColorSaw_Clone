using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPooler instance;

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Color color)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning(tag + " doesn't exist in a pool");
            return null;
        }

        GameObject obj = poolDictionary[tag].Dequeue();
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        // play everytime they spawn
        ParticleSystem objParticles = obj.GetComponent<ParticleSystem>();
        objParticles.GetComponent<ParticleSystemRenderer>().material.color = color;
        if (objParticles != null)
            objParticles.Play();

        poolDictionary[tag].Enqueue(obj);

        return obj;
    }
}