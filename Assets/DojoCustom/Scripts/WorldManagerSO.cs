using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldManagerSO", menuName = "Dojo/World Manager SO")]
public class WorldManagerData : ScriptableObject
{
    public string ToriiURL;
    public string RPCURL;
    public string WorldAddress;
}
