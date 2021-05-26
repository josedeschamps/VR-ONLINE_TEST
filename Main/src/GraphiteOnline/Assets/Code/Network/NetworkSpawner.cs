using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefabs;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        spawnedPlayerPrefabs = PhotonNetwork.Instantiate("Prefabs/Network Player",transform.position, transform.rotation);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefabs);
    }

}
