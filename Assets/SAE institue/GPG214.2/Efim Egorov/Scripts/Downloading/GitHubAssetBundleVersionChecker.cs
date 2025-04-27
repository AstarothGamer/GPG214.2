using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class GitHubAssetBundleVersionChecker : MonoBehaviour
{
    public string githubVersionUrl = "https://raw.githubusercontent.com/AstarothGamer/BundleAssets/main/version.txt";
    public string githubBaseBundleUrl = "https://raw.githubusercontent.com/AstarothGamer/BundleAssets/main/";
    public string saveFolder = "AssetBundles";
    public string baseBundleFileName = "bundleforproject_"; 
    public string versionFileName = "version.txt";

    private string localVersion;
    private string remoteVersion;
    private string fullBundleUrl;
    private string fullBundleFileName;

    void Start()
    {
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(CheckVersionAndLoad());
        }
    }

    IEnumerator CheckVersionAndLoad()
    {
        string versionFilePath = Path.Combine(Application.streamingAssetsPath, saveFolder, versionFileName);

        if (File.Exists(versionFilePath))
        {
            localVersion = File.ReadAllText(versionFilePath).Trim();
            Debug.Log($"Local version: {localVersion}");
        }
        else
        {
            localVersion = "none";
            Debug.LogWarning("No local version file found.");
        }

        using (UnityWebRequest uwr = UnityWebRequest.Get(githubVersionUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download version file: " + uwr.error);
                yield break;
            }
            else
            {
                remoteVersion = uwr.downloadHandler.text.Trim();
                Debug.Log($"Remote version: {remoteVersion}");
            }
        }

        fullBundleFileName = baseBundleFileName + remoteVersion;
        fullBundleUrl = githubBaseBundleUrl + fullBundleFileName;

        if (localVersion != remoteVersion)
        {
            Debug.Log("Version mismatch. Downloading new AssetBundle...");

            yield return StartCoroutine(DownloadNewBundle());

            string saveVersionPath = Path.Combine(Application.streamingAssetsPath, saveFolder, versionFileName);
            File.WriteAllText(saveVersionPath, remoteVersion);
        }
        else
        {
            Debug.Log("Local bundle is up to date.");
        }

        LoadBundle();
    }

    IEnumerator DownloadNewBundle()
    {
        string bundlePath = Path.Combine(Application.streamingAssetsPath, saveFolder, fullBundleFileName);

        if (!Directory.Exists(Path.Combine(Application.streamingAssetsPath, saveFolder)))
        {
            Directory.CreateDirectory(Path.Combine(Application.streamingAssetsPath, saveFolder));
        }

        using (UnityWebRequest uwr = UnityWebRequest.Get(fullBundleUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download AssetBundle: " + uwr.error);
                yield break;
            }
            else
            {
                File.WriteAllBytes(bundlePath, uwr.downloadHandler.data);
                Debug.Log($"AssetBundle downloaded and saved at {bundlePath}");
            }
        }
    }


//just used to check if it works
    void LoadBundle()
    {
        string bundlePath = Path.Combine(Application.streamingAssetsPath, saveFolder, fullBundleFileName);

        if (File.Exists(bundlePath))
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle != null)
            {
                Debug.Log("AssetBundle successfully loaded!");

                GameObject prefab = bundle.LoadAsset<GameObject>("Tree");

                if (prefab != null)
                {
                    Instantiate(prefab, Vector3.zero, Quaternion.identity);
                }
                else
                {
                    Debug.LogError("Prefab not found inside AssetBundle.");
                }

                bundle.Unload(false);
            }
            else
            {
                Debug.LogError("Failed to load AssetBundle from file.");
            }
        }
        else
        {
            Debug.LogError("AssetBundle file not found.");
        }
    }
}
