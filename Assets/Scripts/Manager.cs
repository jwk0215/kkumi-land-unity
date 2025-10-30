using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void onUnityLoaded();

    [SerializeField] GameObject[] __prefabs;            // prefab array
    private GameObject __currentInstance;               // 현재 prefab


    private void Start()
    {
        onUnityLoaded();
    }



    public void LoadPrefab(string prefabName)
    {
        foreach (var prefab in __prefabs)
        {
            if (prefab.name == prefabName)
            {
                Instantiate(prefab, Vector3.zero, Quaternion.identity);
                return;
            }
        }

        Debug.LogWarning($"❌ Prefab '{prefabName}' not found in PrefabManager.");
    }
}
