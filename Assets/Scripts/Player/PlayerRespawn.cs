using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    public CameraMovement2 camera;
    private Rigidbody rb;
    private CheckpointManager checkpointManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (checkpointManager == null) checkpointManager = GameManager.instance.GetCheckpointManager();

        if (!checkpointManager.AreThereCheckpoints())
            checkpointManager.CreateFirstCheckpoint(gameObject.transform);
    }

    public IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Respawn();
    }

    public void Respawn()
    {
        Debug.Log("HOLA");
        if (checkpointManager.AreThereCheckpoints())
        {
            Debug.Log("ADIOS");
            rb.isKinematic = true;
            transform.position = checkpointManager.GetLastCheckpointChecked().position;
            rb.isKinematic = false;
            camera.SetRotation();
        }
    }

    void Update()
    {
        if (checkpointManager == null) checkpointManager = GameManager.instance.GetCheckpointManager();
    }
}
