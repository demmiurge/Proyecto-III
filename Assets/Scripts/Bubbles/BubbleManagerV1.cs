using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class BubbleManagerV1 : MonoBehaviour
{
    private bool works;

    [SerializeField] private GameObject bubbleContainer;

    [SerializeField] private bool disableOnStart = true;

    private List<GameObject> bubbleList;

    public bool AreAllTheBubblesActive()
    {
        foreach (GameObject bubble in bubbleList)
            if (bubble.activeSelf == false) 
                return false;

        return true;
    }

    void Awake()
    {
        bubbleList = new List<GameObject>();

        foreach (Transform child in bubbleContainer.transform)
            bubbleList.Add(child.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.SetBubbleManager(this);

        if (disableOnStart)
        {
            foreach (GameObject bubble in bubbleList)
                bubble.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetBubble()
    {
        return ProvideBubble(out GameObject desiredBubble) ? desiredBubble : null;
    }

    //Vamos pasando la bola por las diferentes opciones y determinando su generación
    private bool ProvideBubble(out GameObject desiredBubble)
    {
        if (IsThereAnyBubbleAvailable(out desiredBubble))
            return true;

        //// Simular la solicitud de una burbuja ya existente en caso de que existan tres burbujas activas
        //if (GetTheOldest(out desiredBubble))
        //    return true;

        if (RecalculateList(out desiredBubble))
            return true;

        return false;
    }

    private bool IsThereAnyBubbleAvailable(out GameObject desiredBubble)
    {
        // Obliga a definir y declarar la burbuja deseada
        desiredBubble = null;

        // Buscamos si hay una burbuja que no se haya generado
        foreach (GameObject bubble in bubbleList)
        {
            // En caso de encontrar una burbuja desactivada en la lista la mostramos
            if (!bubble.gameObject.activeSelf)
            {
                bubble.SetActive(true); // Debemos activar la burbuja, para que los scripts externos no den el problema en la referencia
                desiredBubble = bubble;
                return true;
            }
        }

        return false;
    }

    private bool GetTheOldest(out GameObject desiredBubble)
    {
        GameObject firstBubble = bubbleList[0]; // Guardo el primer elemento en una variable temporal

        bubbleList.RemoveAt(0); // Remuevo el primer elemento de la lista
        bubbleList.Add(firstBubble);

        desiredBubble = firstBubble;
        GetModifiedBubble(desiredBubble);

        return true;
    }

    // La oculto, reinicio sus estados y te la devuelvo
    private void GetModifiedBubble(GameObject bubble)
    {
        BubbleBehaviourV1 bubbleBehaviourV1 = bubble.GetComponent<BubbleBehaviourV1>();
        bubbleBehaviourV1.NotifyNeighboringBubbles();
    }

    private bool RecalculateList(out GameObject desiredBubble)
    {
        GameObject firstBubble = bubbleList[0]; // Guardo el primer elemento en una variable temporal

        bubbleList.RemoveAt(0); // Remuevo el primer elemento de la lista
        bubbleList.Add(firstBubble);

        desiredBubble = firstBubble;
        ForcedReboot(desiredBubble);

        return true;
    }

    // Forzamos la desactivación de la burbuja y su activación
    private void ForcedReboot(GameObject bubble)
    {
        bubble.SetActive(false);

        BubbleBehaviourV1 bubbleBehaviourV1 = bubble.GetComponent<BubbleBehaviourV1>();
        bubbleBehaviourV1.NotifyNeighboringBubbles();

        bubble.SetActive(true);
    }

    public void HideBubble(GameObject bubbleSended)
    {
        bool isAllActivated = true;

        foreach (GameObject bubble in bubbleList)
        {
            if (bubble == bubbleSended)
            {
                BubbleBehaviourV1 bubbleBehaviour = bubble.GetComponent<BubbleBehaviourV1>();
                bubbleBehaviour.DestroyBubble();
            }
        }

        bubbleList = bubbleList.OrderByDescending(bubble => bubble.activeSelf).ToList();

        BubbleBehaviourV1 bubbleBehaviourV1 = bubbleSended.GetComponent<BubbleBehaviourV1>();
        bubbleBehaviourV1.NotifyNeighboringBubbles();
    }

    public void HideAllBubbles()
    {
        foreach (GameObject bubble in bubbleList)
            if (bubble.activeSelf)
                bubble.SetActive(false);
    }

    public void HideTheOldestBubble()
    {
        GameObject olderBubble = bubbleList[0];
        BubbleBehaviourV1 bubbleBehaviourV1 = olderBubble.GetComponent<BubbleBehaviourV1>();

        bubbleBehaviourV1.GetRidOfYourChildren();
        olderBubble.SetActive(false);

        bubbleList.RemoveAt(0);
        bubbleList.Add(olderBubble);

        bubbleBehaviourV1.NotifyNeighboringBubbles();
    }
}
