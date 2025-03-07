using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenseManager : MonoBehaviour {

    [Header("Nome do arquivo")]
    [SerializeField] private string fileName;

    public static DataPersistenseManager instace { get; private set; }

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    void Awake() {
        if (instace != null) {
            Destroy(this);
            Debug.LogError("Mais de um DataPersistenseManager foi encontrado na cena");
            DontDestroyOnLoad(this);
        }
        instace = this;
    }

    void Start() {
        this.dataPersistenceObjects = this.FindAllDataPersistenceObjects();
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        LoadGame();
    }

    public void NewGame() {
        this.gameData = new GameData();
    }

    public void LoadGame() {
        this.gameData = dataHandler.Load();


        if (this.gameData == null) {
            this.gameData = new GameData();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) {
            dataPersistenceObj.LoadData(this.gameData);
        }
    }

    public void SaveGame() {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) {
            dataPersistenceObj.SaveData(ref this.gameData);
        }

        dataHandler.Save(this.gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() {

        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    void OnApplicationQuit() {
        // SaveGame();
    }
}
