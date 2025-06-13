using UnityEngine;

public class TerrainTreesToPrefabs : MonoBehaviour
{
    public Terrain terrain;
    public GameObject[] treePrefabs; // Prefabs na mesma ordem que estão no terrain

    void Start()
    {
        if (!terrain) terrain = Terrain.activeTerrain;

        ConvertAllTrees();
        RemoveAllTerrainTrees();
    }

    void ConvertAllTrees()
    {
        TreeInstance[] instances = terrain.terrainData.treeInstances;
        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        for (int i = 0; i < instances.Length; i++)
        {
            TreeInstance tree = instances[i];
            Vector3 worldPos = Vector3.Scale(tree.position, data.size) + terrainPos;

            int prototypeIndex = tree.prototypeIndex;
            if (prototypeIndex >= 0 && prototypeIndex < treePrefabs.Length)
            {
                GameObject prefab = treePrefabs[prototypeIndex];
                if (prefab)
                {
                    GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity);
                    instance.transform.localScale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale) * prefab.transform.localScale.x;
                    instance.tag = "Obstruction";
                }
            }
        }

        Debug.Log("Todas as árvores convertidas em prefabs.");
    }

    void RemoveAllTerrainTrees()
    {
        terrain.terrainData.treeInstances = new TreeInstance[0];
        terrain.Flush(); // Atualiza o terreno para refletir a remoção
        Debug.Log("Todas as árvores removidas do terreno.");
    }
}
