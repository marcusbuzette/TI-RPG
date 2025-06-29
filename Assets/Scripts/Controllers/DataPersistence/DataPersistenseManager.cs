using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenseManager : MonoBehaviour
{
    [Header("Nome do arquivo de save")]
    [SerializeField] private string saveFileName = "saveData.json";

    public static DataPersistenseManager instance { get; private set; }

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    private string currentProfileId = "slot1"; // Pasta padrão

    public int currentLevel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            Debug.LogError("Mais de um DataPersistenseManager foi encontrado na cena");
            return;
        }
    }

    void Start()
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        SetSlot(currentProfileId); // Carrega o slot inicial
    }

    public void SetSlot(string slotName)
    {
        currentProfileId = slotName;
        dataHandler = new FileDataHandler(Application.persistentDataPath, saveFileName);
        Debug.Log("Save path: " + Application.persistentDataPath);
        Debug.Log("Slot: " + slotName);
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
        GameController.controller.ResetVariables();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load(currentProfileId);

        if (gameData == null)
        {
            Debug.Log("Nenhum save encontrado, criando novo...");
            NewGame();
        }

        dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence obj in dataPersistenceObjects)
        {
            obj.LoadData(gameData);
            Debug.Log("Dados carregados nos objetos.");
        }
    }

    public void SaveGame()
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence obj in dataPersistenceObjects)
        {
            obj.SaveData(ref gameData);
            currentLevel = gameData.currentLevel;
        }

        dataHandler.Save(gameData, currentProfileId);
        Debug.Log("Jogo salvo no slot: " + currentProfileId);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
    }

    void OnApplicationQuit()
    {
        // SaveGame();
    }

    public GameData GetGameData() {return this.gameData;}
}