using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenseManager : MonoBehaviour {

    [Header("Nome base do arquivo")]
    [SerializeField] private string baseFileName = "save";

    public static DataPersistenseManager instance { get; private set; }

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenseManager instace { get; private set; }

    private string currentSaveSlot = "slot1"; // slot padrão

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            Debug.LogError("Mais de um DataPersistenseManager foi encontrado na cena");
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        SetSlot(currentSaveSlot); // Carrega o slot inicial
    }

    public void SetSlot(string slotName) {
        currentSaveSlot = slotName;
        string fileName = $"{baseFileName}_{slotName}.json";
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        LoadGame(); // Carrega automaticamente o slot ao trocar
    }

    public void NewGame() {
        gameData = new GameData();
    }

    public void LoadGame() {
        gameData = dataHandler.Load();

        if (gameData == null) {
            Debug.Log("Nenhum save encontrado, criando novo...");
            NewGame();
        }

        foreach (IDataPersistence obj in dataPersistenceObjects) {
            obj.LoadData(gameData);
        }
    }

    public void SaveGame() {
        foreach (IDataPersistence obj in dataPersistenceObjects) {
            obj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() {
        return FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
    }

    void OnApplicationQuit() {
        SaveGame();
    }
}
