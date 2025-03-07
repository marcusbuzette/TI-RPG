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

    public GameData Load () {
        string fullPath = Path.Combine(this.dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath)) {
            try {
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open)) {
                    using(StreamReader reader = new StreamReader(stream)) {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e) {
                Debug.LogError("Error ao carregar o jogo: " + e);
            }
        }
        return loadedData;
    }


    public void Save(GameData data) {
        string fullPath = Path.Combine(this.dataDirPath, dataFileName);

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

}
