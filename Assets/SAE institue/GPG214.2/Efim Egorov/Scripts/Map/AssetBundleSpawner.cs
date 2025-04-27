using System.Collections;
using System.IO;
using UnityEngine;

public class AssetBundleSpawner : MonoBehaviour
{
    public Texture2D texture;
    private string folderPath = "AssetBundles";
    private string baseBundleName = "bundleforproject_";
    public float spacing = 8f;
    public float raycastHeight = 100f;
    public int pixelSize = 5;

    private AssetBundle bundle;
    private string currentVersion = "v1"; 
    void Start()
    {
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            StartCoroutine(LoadBundleAndSpawn());
        }
    }

    IEnumerator LoadBundleAndSpawn()
    {
        string versionFilePath = Path.Combine(Application.streamingAssetsPath, folderPath, "version.txt");

        if (File.Exists(versionFilePath))
        {
            currentVersion = File.ReadAllText(versionFilePath).Trim();
            Debug.Log("Found local version: " + currentVersion);
        }
        else
        {
            Debug.LogWarning("No version.txt found. Defaulting to v1.");
        }

        string bundleFileName = baseBundleName + currentVersion;
        string bundlePath = Path.Combine(Application.streamingAssetsPath, folderPath, bundleFileName);

        if (!File.Exists(bundlePath))
        {
            Debug.LogError("Asset bundle is not found at path: " + bundlePath);
            yield break;
        }

        bundle = AssetBundle.LoadFromFile(bundlePath);

        if (bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle.");
            yield break;
        }

        SpawnObjectsFromTexture();
    }

    void SpawnObjectsFromTexture()
    {
        int objectCount = 0;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (x % pixelSize == 0 && y % pixelSize == 0)
                {
                    Color pixelColor = texture.GetPixel(x, y);

                    string prefabName = GetPrefabNameByColor(pixelColor);

                    if (!string.IsNullOrEmpty(prefabName))
                    {
                        GameObject prefab = bundle.LoadAsset<GameObject>(prefabName);

                        if (prefab != null)
                        {
                            Vector3 spawnPosition = new Vector3(x + spacing, raycastHeight, y + spacing);

                            if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit, raycastHeight * 2))
                            {
                                spawnPosition.y = hit.point.y + (prefab.transform.localScale.y / 2);
                            }

                            Instantiate(prefab, spawnPosition, Quaternion.identity);
                            objectCount++;
                        }
                        else
                        {
                            Debug.LogWarning($"Prefab {prefabName} not found inside bundle.");
                        }
                    }
                }
            }
        }

        Debug.Log($"Spawned {objectCount} objects from AssetBundle!");
        bundle.Unload(false); 
    }

    string GetPrefabNameByColor(Color color)
    {
        if (IsSimilar(color, Color.blue))
            return "rock";    
        else if (IsSimilar(color, Color.red))
            return "Enemy";   
        else if (IsSimilar(color, Color.green))
            return "Tree";    

        return null;
    }

    bool IsSimilar(Color a, Color b, float tolerance = 0.5f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}
