using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    static PoolManager instance;
    public int initialCount;
    public GameObject[] prefabs;
    Dictionary<GameObject, List<GameObject>> prefabInstances = new Dictionary<GameObject, List<GameObject>>();
    Dictionary<GameObject, IEnumerable<IPool>> prefabComponents = new Dictionary<GameObject, IEnumerable<IPool>>();
    Dictionary<GameObject, Vector3> startScales = new Dictionary<GameObject, Vector3>();
    void Awake()
    {
        if (initialCount < 1)
            initialCount = 1;
        instance = this;
        foreach (var i in prefabs)
            for (int j = 0; j < initialCount; j++)
                AddInstance(i);
    }
    void AddInstance(GameObject prefab)
    {
        var g = GameObject.Instantiate(prefab);
        List<GameObject> list;
        if (prefabInstances.TryGetValue(prefab, out list))
            list.Add(g);
        else
        {
            list = new List<GameObject>();
            list.Add(g);
            prefabInstances.Add(prefab, list);
            startScales.Add(prefab, prefab.transform.localScale);
        }
        prefabComponents.Add(g, g.GetComponentsInChildren<IPool>());
        g.SetActive(false);
    }

    public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (instance.prefabInstances.TryGetValue(prefab, out var list))
        {
            foreach (var i in list)
                if (!i.activeSelf)
                {
                    i.transform.position = position;
                    i.transform.rotation = rotation;
                    if (instance.startScales.TryGetValue(prefab, out var v))
                        i.transform.localScale = v;
                    i.SetActive(true);
                    if (instance.prefabComponents.TryGetValue(i, out var components))
                        foreach (var c in components)
                            c.OnTake();
                    return i;
                }
            instance.AddInstance(prefab);
            return Instantiate(prefab, position, rotation);
        }
        Debug.Log(prefab.name + " does not registred in PoolManager");
        return null;
    }
    public static void Destroy(GameObject destroyableObject)
    {
        destroyableObject.SetActive(false);
        if (instance.prefabComponents.TryGetValue(destroyableObject, out var components))
            foreach (var c in components)
                c.OnPush();
    }
}

public interface IPool
{
    public void OnTake();
    public void OnPush();
}
