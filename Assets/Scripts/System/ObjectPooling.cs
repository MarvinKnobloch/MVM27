using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static List<PoolObjectInfo> poolObjectInfos = new List<PoolObjectInfo>();

    private static GameObject rootGameObject;
    public enum ProjectileType
    {
        Default,
        Player,
        Enemy,
        None,
    }

    private void Awake()
    {
        rootGameObject = gameObject;
        poolObjectInfos.Clear();

        int enumLength = Enum.GetNames(typeof(ProjectileType)).Length;

        for (int i = 0; i < enumLength; i++)
        {
            GameObject obj = new GameObject(Enum.GetName(typeof(ProjectileType), i));
            obj.transform.parent = transform;
        }
    }

    public static GameObject SpawnObject(GameObject objToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, ProjectileType projectileType = ProjectileType.Default)
    {
        PoolObjectInfo pool = poolObjectInfos.Find(p => p.objectName == objToSpawn.name);

        if (pool == null)
        {
            pool = new PoolObjectInfo();
            poolObjectInfos.Add(pool);
            pool.objectName = objToSpawn.name;
        }

        GameObject obj = pool.inactiveObjects.FirstOrDefault();
        
        if(obj == null)
        {
            obj = Instantiate(objToSpawn, spawnPosition, spawnRotation);

            obj.GetComponent<IPoolingList>().poolingList = pool;
        }
        else
        {
            obj.transform.position = spawnPosition;
            obj.transform.rotation = spawnRotation;
            obj.SetActive(true);
            pool.inactiveObjects.Remove(obj);
        }

        obj.gameObject.transform.parent = rootGameObject.transform.GetChild((int)projectileType);

        return obj;
    }
    public static void ReturnObjectToPool(GameObject obj, PoolObjectInfo poolObjectInfo)
    {
        poolObjectInfo.inactiveObjects.Add(obj);
        obj.SetActive(false);

        //string searchString = obj.name.Substring(0 , obj.name.Length - 7);

        //PoolObjectInfo pool = poolObjectInfos.Find(p => p.objectName == searchString);

        //if (pool == null) Debug.Log("no pool available");
        //else
        //{
        //    obj.SetActive(false);
        //    pool.inactiveObjects.Add(obj);
        //}
    }

    public class PoolObjectInfo
    {
        public string objectName;
        public List<GameObject> inactiveObjects = new List<GameObject>();
    }
}
