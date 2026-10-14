using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winText;

    [SerializeField] private Spawner spawner;

    void Start()
    {
        if(spawner)
        spawner.AllWavesCompleted+=FihishGameByWin;

        if(winText)
        winText.gameObject.SetActive(false);
    }

    private void FihishGameByWin()
    {
        if(spawner)
        spawner.HideAllUI();

        if(winText)
        winText.gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        if(spawner)
        spawner.AllWavesCompleted-=FihishGameByWin;
    }

}
