using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class FileDataHandler : MonoBehaviour {
    private string dataDirPath = "";

    private string dataFileName = "";

    public FileDataHandler(string dataPath, string fileName) {
        this.dataDirPath = dataPath;
        this.dataFileName = fileName;
    }

    public GameData Load (string profileId) {
        string fullPath = Path.Combine(this.dataDirPath,profileId ,dataFileName);
        Debug.Log(fullPath);
        GameData loadedData = null;
        Debug.Log(File.Exists(fullPath));
        if (File.Exists(fullPath)) {
            try {
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open)) {
                    using(StreamReader reader = new StreamReader(stream)) {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                Debug.Log(dataToLoad);

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e) {
                Debug.LogError("Error ao carregar o jogo: " + e);
            }
        }
        return loadedData;
    }


    public void Save(GameData data, string profileId) {
        string fullPath = Path.Combine(this.dataDirPath,profileId ,dataFileName);

        try {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using(FileStream stream = new FileStream(fullPath, FileMode.Create)) {
                using(StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(dataToStore);
                }
            }
        } 
        catch (Exception e) {
            Debug.LogError("Erro ao salvar jogo: " + e);
        }
    }

    public Dictionary<string,GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;
            string fullPath = Path.Combine(dataDirPath,profileId,dataFileName);
            if(!File.Exists(fullPath))
            {
                Debug.LogWarning("Save não tem dados:"+profileId);
                continue;
            }

            GameData profileData = Load(profileId);

            if(profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }

            else 
            {
                Debug.LogError("Load deu errado:"+profileId);
            }
        }

        return profileDictionary;
    }

}
