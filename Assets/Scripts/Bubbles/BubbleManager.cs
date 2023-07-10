using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    private bool works = false;

    [SerializeField] private GameObject bubbleContainer;
    [SerializeField] private BubbleGenerator bubbleGenerator;

    [SerializeField] private List<GameObject> bubbleList;

    [SerializeField] private bool disableOnStart = true;

    [SerializeField] private KeyCode debugGenerateBubble = KeyCode.G;

    // Start is called before the first frame update
    void Start()
    {
        bubbleList = new List<GameObject>();

        // Comprobar que todos los sistemas funcionan
        if (!DetectContainerAndSaveBubbles()) return;
        if (!DetectBubbleGenerator()) return;

        // Regla del inspector para desactivar todas las burbujas o no
        if (disableOnStart) DisableOnStart();
    }

    private bool DetectBubbleGenerator()
    {
        if (bubbleContainer != null)
            return works = true;
        else
            Debug.LogError("Missing to assign the bubble container from the inspector " + gameObject.name);

        return works = false;
    }

    private void DisableOnStart()
    {
        foreach (GameObject bubble in bubbleList)
            bubble.SetActive(false);
    }

    private bool DetectContainerAndSaveBubbles()
    {
        if (bubbleGenerator != null)
        {
            foreach (Transform child in bubbleContainer.transform)
                bubbleList.Add(child.gameObject);
            return works = true;
        }
        else
            Debug.LogError("Missing to assign the bubble generator from the inspector " + gameObject.name);

        return works = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!works) return;

        if (Input.GetKeyDown(debugGenerateBubble))
        {
            bubbleGenerator.SpawnBubble(GetBubble());
        }
    }

    public GameObject GetBubble()
    {
        GameObject desiredBubble;

        if (ProvideBubble(out desiredBubble))
        {
            if (desiredBubble?.GetComponent<BubbleBehaviour>())
            {
                desiredBubble?.GetComponent<BubbleBehaviour>().ResetStates();
            }
            return desiredBubble;
        }
        else
            return null;
    }

    //Vamos pasando la bola por las diferentes opciones y determinando su generación
    private bool ProvideBubble(out GameObject desiredBubble)
    {
        // Obliga a definir y declarar la burbuja deseada
        desiredBubble = null;

        if (IsThereAnyBubbleAvailable(out desiredBubble))
            return true;

        if (RecalculateList(out desiredBubble))
            return true;

        return false;
    }

    //private GameObject ProvideBubble()
    //{
    //    GameObject desiredBubble;

    //    if (IsThereAnyBubbleAvailable(out desiredBubble))
    //    {
    //        return desiredBubble;
    //    }

    //    if (RecalculateList(out desiredBubble))
    //    {
    //        return desiredBubble;
    //    }

    //    return null;
    //}

    public void RefreshBubbleConnections(GameObject bubbleSended)
    {
        // Borramos todas las conexiones de la bola eliminada
        if (bubbleSended?.GetComponent<BubbleBehaviour>())
        {
            BubbleBehaviour bubbleBehaviour = bubbleSended.GetComponent<BubbleBehaviour>();
            bubbleBehaviour.GetConnections().Clear();
        }



        foreach (GameObject bubble in bubbleList)
        {
            // Limpiamos las conexiones de las burbujas que tenían un contacto con la burbuja eliminada
            if (bubble.activeSelf)
            {
                if (bubble?.GetComponent<BubbleBehaviour>())
                {
                    BubbleBehaviour bubbleBehaviour = bubble.GetComponent<BubbleBehaviour>();

                    List<GameObject> bubbleConnections = bubbleBehaviour.GetConnections();

                    for (int i = 0; i < bubbleConnections.Count; i++)
                    {
                        if (bubbleConnections[i] == bubbleSended)
                        {
                            // bubbleBehaviour.CanIFreeMyself();
                            bubbleConnections.RemoveAt(i);
                        }
                    }
                }
            }
        }

        bool solidSurface = false;
        bool bubblesAreConnected = false;

        // Comprobamos si las burbujas tienen alguna conexión extra antes de pedirles que ejecuten una revisión
        foreach (GameObject bubble in bubbleList)
        {
            if (bubble?.GetComponent<BubbleBehaviour>())
            {
                BubbleBehaviour bubbleBehaviour = bubble.GetComponent<BubbleBehaviour>();

                List<GameObject> bubbleConnections = bubbleBehaviour.GetConnections();

                for (int i = 0; i < bubbleConnections.Count; i++)
                {
                    if (bubbleConnections[i].tag == "Bubble")
                    {
                        bubblesAreConnected = true;
                    }
                }
            }
        }

        if (bubblesAreConnected)
        {
            foreach (GameObject bubble in bubbleList)
            {
                if (bubble?.GetComponent<BubbleBehaviour>())
                {
                    BubbleBehaviour bubbleBehaviour = bubble.GetComponent<BubbleBehaviour>();

                    List<GameObject> bubbleConnections = bubbleBehaviour.GetConnections();

                    for (int i = 0; i < bubbleConnections.Count; i++)
                    {
                        if (bubbleConnections[i].tag != "Bubble")
                        {
                            solidSurface = true;
                        }
                    }
                }
            }
        }

        // Si entre ellas están interconectadas y no existe otra superficie que las mantenga, se caen.
        if (solidSurface == false && bubblesAreConnected)
        {
            foreach (GameObject bubble in bubbleList)
            {
                if (bubble?.GetComponent<BubbleBehaviour>())
                {
                    BubbleBehaviour bubbleBehaviour = bubble.GetComponent<BubbleBehaviour>();

                    bubbleBehaviour.checkingSystem = false;

                    bubbleBehaviour.Release();

                    bubbleBehaviour.checkingSystem = true;
                }
            }
        }

        // Versión de la V0 o GlueBehavior
        // foreach (GameObject bubble in bubbleList)
        // {
        //    if (bubble != bubbleSended)
        //    {
        //        // 
        //        bubble.GetComponent<GlueBehavior>().RemoveItemList(bubbleSended);
        //        bubble.GetComponent<GlueBehavior>().CheckConnections();
        //    }
        // }
    }

    private bool IsThereAnyBubbleAvailable(out GameObject desiredBubble)
    {
        // Obliga a definir y declarar la burbuja deseada
        desiredBubble = null;

        // Buscamos si hay una burbuja que no se haya generado
        foreach (GameObject bubble in bubbleList)
        {
            // En caso de encontrar una bola desactivada en la lista la mostramos
            if (!bubble.gameObject.activeSelf)
            {
                bubble.SetActive(true); // Debemos activar la bola, para que los scripts externos no den el problema en la referencia
                desiredBubble = bubble;
                return true;
            }
        }

        return false;
    }

    private bool RecalculateList(out GameObject desiredBubble)
    {
        bool isAllActivated = true;
        desiredBubble = null;

        foreach (GameObject bubble in bubbleList)
        {
            if (bubble.activeSelf == false)
                isAllActivated = false;
        }

        if (isAllActivated == false) return false;

        GameObject firstBubble = bubbleList[0]; // Guardo el primer elemento en una variable temporal

        bubbleList.RemoveAt(0); // Remuevo el primer elemento de la lista
        bubbleList.Add(firstBubble);

        desiredBubble = firstBubble;

        return true;
    }

    public void HideBubble(GameObject bubbleSended)
    {
        bool isAllActivated = true;

        foreach (GameObject bubble in bubbleList)
        {
            if (bubble == bubbleSended)
                bubble.SetActive(false);
        }

        bubbleList = bubbleList.OrderByDescending(bubble => bubble.activeSelf).ToList();

        RefreshBubbleConnections(bubbleSended);
    }
}
