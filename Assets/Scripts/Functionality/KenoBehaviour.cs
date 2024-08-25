using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class KenoBehaviour : MonoBehaviour
{
    [Header("Lists")]
    [SerializeField]
    private List<KenoButton> KenoButtonScripts;
    [SerializeField]
    private List<int> SelectedList;
    [SerializeField]
    private List<int> ResultList;
    [SerializeField]
    private List<Transform> Balls_Transform;
    [SerializeField]
    private List<TMP_Text> Balls_Text;

    [Header("Integers")]
    [SerializeField]
    private int MaxSelection;
    [SerializeField]
    internal int selectionCounter = 0;
    [SerializeField]
    internal int ResultCounter = 0;

    [Header("Vectors")]
    [SerializeField]
    private Vector2 initialPosition;
    [SerializeField]
    private int middlePosition;
    [SerializeField]
    private int finalPosition; 

    [Header("Scripts")]
    [SerializeField]
    private UIManager uiManager;

    [Header("Text")]
    [SerializeField]
    private TMP_Text MainNumber_Text;

    [Header("GameObject")]
    [SerializeField]
    private GameObject DisableScreen_object;
    [SerializeField]
    private List<Transform> Win_Transform;
    [SerializeField]
    private List<Transform> TempWin_Transform;

    void Start()
    {

    }

    private List<int> templist = new List<int>();

    internal void PickRandoms()
    {
        SelectedList.Clear();
        SelectedList.TrimExcess();
        foreach (KenoButton kc in KenoButtonScripts)
        {
            kc.ResetButton();
        }

        templist.Clear();
        templist.TrimExcess();
        templist = GenerateRandomNumbers(selectionCounter, 0, 79);
        selectionCounter = 0;

        for (int i = 0; i < templist.Count; i++)
        {
            KenoButtonScripts[templist[i]].OnKenoSelect();
        }
    }

    internal void CheckTransform(Transform thisObject)
    {
        if (thisObject.childCount <= 1) 
        {
            TempWin_Transform[TempWin_Transform.Count - 1].SetParent(thisObject);
            TempWin_Transform[TempWin_Transform.Count - 1].localPosition = new Vector2(2.6f, 0);
            TempWin_Transform[TempWin_Transform.Count - 1].SetAsFirstSibling();
            TempWin_Transform[TempWin_Transform.Count - 1].gameObject.SetActive(true);
            TempWin_Transform.RemoveAt(TempWin_Transform.Count - 1);
        }
        else
        {
            Transform temp = null;
            foreach (Transform t in TempWin_Transform)
            {
                if (t == thisObject.GetChild(0))
                {
                    temp = t;
                    break;
                }
            }
            TempWin_Transform.Remove(temp);
            thisObject.GetChild(0).gameObject.SetActive(true);
        }
    }

    private static List<int> GenerateRandomNumbers(int count, int minValue, int maxValue)
    {
        List<int> possibleNumbers = new List<int>();
        List<int> chosenNumbers = new List<int>();

        for (int index = minValue; index < maxValue; index++)
            possibleNumbers.Add(index);

        while (chosenNumbers.Count < count)
        {
            int position = Random.Range(0, possibleNumbers.Count);
            chosenNumbers.Add(possibleNumbers[position]);
            possibleNumbers.RemoveAt(position);
        }
        return chosenNumbers;
    }

    internal void AddKeno(int value)
    {
        if (!uiManager.isReset)
        {
            SelectedList.Add(value);
        }

        if(selectionCounter >= 2)
        {
            uiManager.CheckPlayButton(true);
        }
        else
        {
            uiManager.CheckPlayButton(false);
        }
        uiManager.UpdateSelectedText(selectionCounter);
    }

    internal void RemoveKeno(int value)
    {
        SelectedList.Remove(value);
        if (selectionCounter >= 2)
        {
            uiManager.CheckPlayButton(true);
        }
        else
        {
            uiManager.CheckPlayButton(false);
        }
        uiManager.UpdateSelectedText(selectionCounter);
    }

    internal void ShowMaxPopup()
    {
        uiManager.MaxPopupEnable();
    }

    internal void PlayDummyGame()
    {
        TempWin_Transform.Clear();
        TempWin_Transform.TrimExcess();
        TempWin_Transform.AddRange(Win_Transform);
        if (DisableScreen_object) DisableScreen_object.SetActive(true);
        ResultList.Clear();
        ResultList.TrimExcess();
        ResultList = GenerateRandomNumbers(20, 0, 79);
        StartCoroutine(PlayGameRoutine());
    }

    private IEnumerator PlayGameRoutine()
    {
        for(int i = 0; i < Balls_Transform.Count; i++)
        {
            if (MainNumber_Text) MainNumber_Text.text = ResultList[i].ToString();
            if (Balls_Text[i]) Balls_Text[i].text = ResultList[i].ToString();
            if (Balls_Transform[i]) Balls_Transform[i].DOLocalMoveY(middlePosition, 0.2f);
            yield return new WaitForSeconds(0.2f);
            if (Balls_Transform[i]) Balls_Transform[i].DOLocalMoveX((finalPosition - (i * 55)), 0.5f);
            KenoButtonScripts[ResultList[i]].ResultColor();
            yield return new WaitForSeconds(0.1f);
        }
        uiManager.CheckFinalWinning();
        uiManager.EnableReset();
        if (DisableScreen_object) DisableScreen_object.SetActive(false);
    }

    internal void ActivateWinning()
    {
        uiManager.CheckWinnings(ResultCounter);
    }

    internal void ResetButtons()
    {
        for (int i = 0; i < KenoButtonScripts.Count; i++)
        {
            KenoButtonScripts[i].ResetButton();
        }

        ResultCounter = 0;
        selectionCounter = 0;

        for (int i = 0; i < SelectedList.Count; i++)
        {
            KenoButtonScripts[SelectedList[i]].OnKenoSelect();
        }

        foreach(Transform ts in Balls_Transform)
        {
            ts.localPosition = initialPosition;
        }
        if (MainNumber_Text) MainNumber_Text.text = "00";
    }

    internal void ResetWinAnim()
    {
        foreach (Transform t in Win_Transform)
        {
            t.gameObject.SetActive(false);
        }
        TempWin_Transform.Clear();
        TempWin_Transform.TrimExcess();
    }

    internal void CleanPage()
    {
        for (int i = 0; i < KenoButtonScripts.Count; i++)
        {
            KenoButtonScripts[i].ResetButton();
        }

        SelectedList.Clear();
        SelectedList.TrimExcess();
        ResultCounter = 0;
        selectionCounter = 0;
    }
}
