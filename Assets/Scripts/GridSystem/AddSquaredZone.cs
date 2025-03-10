using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddSquaredZoneGrid", menuName = "GridZone")]
public class AddSquaredZone: ScriptableObject {
    public int zoneNumber;
    public int startX;
    public int startZ;
    public int endX;
    public int endZ;
    public List<ZoneSpawnPoint> spawnPoints;
}

[System.Serializable]
public struct ZoneSpawnPoint {
    public int x;
    public int z;
}
