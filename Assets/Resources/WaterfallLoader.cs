using System.Collections;
using UnityEngine;

public class WaterfallLoader : MonoBehaviour {
    [SerializeField] private string resourcePath = "cachoeira";
    [SerializeField] private Vector3 spawnPosition = new Vector3(0, 0, 0); // ajuste conforme necessário

    void Start() {
        StartCoroutine(LoadWaterfallAsync());
    }

    private IEnumerator LoadWaterfallAsync() {
        ResourceRequest request = Resources.LoadAsync<GameObject>(resourcePath);
        yield return request;

        if (request.asset != null) {
            GameObject prefab = request.asset as GameObject;
            GameObject instance = Instantiate(prefab, spawnPosition, prefab.transform.rotation);

            Debug.Log("Cachoeira instanciada com sucesso!");
        }
        else {
            Debug.LogError("Erro ao carregar a cachoeira.");
        }
    }
}
