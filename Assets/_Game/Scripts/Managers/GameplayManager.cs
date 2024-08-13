using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> goBackgrounds;
    [SerializeField] private Canvas canvasParent;
    [SerializeField] private GameObject goUiCanvasPrefab;
    [SerializeField] private GameObject goBlackjackManagerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        goBackgrounds[Random.Range(0, goBackgrounds.Count)].SetActive(true);
        Instantiate(goUiCanvasPrefab, canvasParent.transform); 
        Instantiate(goBlackjackManagerPrefab); 
    }
}
