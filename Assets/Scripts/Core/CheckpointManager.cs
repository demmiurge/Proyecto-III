using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private List<Checkpoint> checkpointReachedList;
    [SerializeField] private GameObject checkpointPrefab;

    // Saneamos la lista, comprobamos que no pueda ser nula
    public void CheckCheckpointList()
    {
        if (checkpointReachedList == null)
            checkpointReachedList = new List<Checkpoint>();
    }

    public void AddNewCheckpoint(Checkpoint checkpoint)
    {
        CheckCheckpointList();
        if (checkpoint.IsReached() == false)
            checkpointReachedList.Add(checkpoint);
    }

    public void CreateFirstCheckpoint(Transform position)
    {
        GameObject gameObjectCheckpoint = Instantiate(checkpointPrefab, position);
        gameObjectCheckpoint.transform.parent = null;
    }

    public bool AreThereCheckpoints()
    {
        CheckCheckpointList();
        return checkpointReachedList.Count > 0;
    }

    public Transform GetLastCheckpointChecked()
    {
        return checkpointReachedList[^1].GetDesiredSpawnPoint();
    } 

    public void ClearSavedList()
    {
        CheckCheckpointList();
        checkpointReachedList.Clear();
    }
}
