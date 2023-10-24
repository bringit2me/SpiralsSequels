using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapButton : MonoBehaviour
{

    public int level;



    [Header("ButtonUIRef")]
    public Image buttonImage;

    public Sprite basicEncounterImage;
    [SerializeField] bool basicEncounter;
    public Sprite specialEncounterImage;
    [SerializeField] bool specialEncounter;
    public Sprite draftSpiralImage;
    [SerializeField] bool draftSpiral;
    public Sprite bossSpiralImage;
    [SerializeField] bool bossSpiral;
    void Start()
    {
        int randNum = Random.Range(1, 5);
        if(randNum == 1)
        {
            buttonImage.sprite = basicEncounterImage;
            basicEncounter = true;
        }
        if (randNum == 2)
        {
            buttonImage.sprite = specialEncounterImage;
            specialEncounter = true;
        }
        if (randNum == 3)
        {
            buttonImage.sprite = draftSpiralImage;
            draftSpiral = true;
        }
        if (randNum == 4)
        {
            buttonImage.sprite = bossSpiralImage;
            bossSpiral = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonClicked()
    {
        Debug.Log("Player has interacted with button");
    }
}
