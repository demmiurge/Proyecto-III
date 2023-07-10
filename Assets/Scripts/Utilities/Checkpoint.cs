using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Por defecto, siempre el punto de control siempre estará desactivado
    private bool reached;

    public bool IsReached() => reached;

    public void SetReached(bool statusReached) => reached = statusReached;

    private CheckpointManager checkpointManager;

    [SerializeField] private Transform desiredSpawnPoint;

    public Transform GetDesiredSpawnPoint() => desiredSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        CheckManagersInstance();
    }

    // Update is called once per frame
    void Update()
    {
        CheckManagersInstance();
    }

    private void OnTriggerEnter(Collider collider)
    {
        CheckManagersInstance();

        if (collider.tag == "Player" && reached == false)
        {
            checkpointManager.AddNewCheckpoint(this);
            reached = true;
        }
    }

    void CheckManagersInstance()
    {
        if (checkpointManager == null) checkpointManager = GameManager.instance.GetCheckpointManager();
    }
}
