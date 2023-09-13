//////////////////////////////////////////////
//Assignment/Lab/Project: Minesweeper Project
//Name: Zebulun Baukhagen
//Section: 2023SP.SGD.213.2172
//Instructor: Brian Sowers
//Date: 01/25/2023
/////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Minesweeper : MonoBehaviour
{
    [SerializeField] List<GameObject> panels = new();
    [SerializeField] List<Button> minefieldButtons = new(25);
    [SerializeField] TextMeshProUGUI emptyCellsNumber;
    [SerializeField] TextMeshProUGUI endMessage;

    // representation of our game board as an array that is one column and row larger than the actual playing board
    // values 0-8 indicate number of neighboring mines to a cell
    // the value 9 indicates a mine
    List<int> _mineIndexList = new();

    int[] _minefieldArray = new int[] 
    {
     0, 0, 0, 0, 0, 0, 0, 
     0, 0, 0, 0, 0, 0, 0, 
     0, 0, 0, 0, 0, 0, 0, 
     0, 0, 0, 0, 0, 0, 0, 
     0, 0, 0, 0, 0, 0, 0, 
     0, 0, 0, 0, 0, 0, 0,
     0, 0, 0, 0, 0, 0, 0,
    };

    

    // Start is called before the first frame update
    void Start()
    {
        // on start, populate the field with mines and then assign values indicating neighboring mines
        // to the rest of the indexes
        PopulateMinefieldWithMines(_minefieldArray, 5, out _mineIndexList);
        AssignNumberOfNeighboringMineValues(_minefieldArray, _mineIndexList);
    }

    #region GameLogic

    void PopulateMinefieldWithMines(int[] arrayToPopulate, int numberOfMines, out List<int> mineIndexList)
    {
        // get list of unique legal integers and insert mines at those locations
        // return the mineIndexList for referencing later
        mineIndexList = GenerateRandomUniqueMineIndexList(numberOfMines);
        foreach (int mineIndex in mineIndexList)
        {
            arrayToPopulate[mineIndex] = 9;
        }
    }

    void AssignNumberOfNeighboringMineValues(int[] arrayWithMines, List<int> listOfMineIndexes)
    {
        // take minefield, check all indexes for number of neighbors with value 9 and update with value
        // if the index is not already a mine

        // because our parent grid is one square larger on all sides, these nested for loops
        // keep the checking and assigning within bounds
        for(int x = 8; x <= 12; x++)
            for(int y = x; y <= 40; y += 7)
        {
                if (arrayWithMines[y] == 9)
                {
                    continue;
                }
                else
                {
                    List<int> listOfNeighbors = ProvideListOfNeighboringCells(y);
                    arrayWithMines[y] = DetermineNumberOfNeighboringMinesForCell(listOfMineIndexes, listOfNeighbors);
                }
            }
    }

    int DetermineNumberOfNeighboringMinesForCell(List<int> listOfMineIndexes, List<int> listOfCellNeighbors)
    {
        // take the list of mine indexes and the list of neighbors and compare the two, incrementing when a mine is found
        // return the total number of neighboring mines
        int neighboringMines = 0;
        foreach (int cell in listOfCellNeighbors)
        {
            if (listOfMineIndexes.Contains(cell))
            {
                neighboringMines++;
            }
        }
        return neighboringMines;
    }

    List<int> ProvideListOfNeighboringCells(int indexToAssign)
    {
        // take the given index from _minefieldArray and return a list of the cells that neighbor it
        List<int> neighboringCells = new();
        List<int> possibleNeighbors = new() { -1, -8, -7, -6, +1, +8, +7, +6 };
        for (int i = 0; i <= 7; i++)
        {
            neighboringCells.Add(indexToAssign + possibleNeighbors[i]);
        }
        return neighboringCells;
    }

    List<int> GenerateRandomUniqueMineIndexList(int lengthOfDesiredList)
    {
        // return an integer list with unique values that are within the list of legal numbers
        int[] legalNumbers = new int[] 
        { 
        8,  9,  10, 11, 12,
        15, 16, 17, 18, 19,
        22, 23, 24, 25, 26, 
        29, 30, 31, 32, 33,
        36, 37, 38, 39, 40
        };

        List<int> uniqueIntegerList = new();

        for (int i = 0; i < lengthOfDesiredList; i++)
        {
            int possibleValue = UnityEngine.Random.Range(legalNumbers[0], legalNumbers[24]);
            while (uniqueIntegerList.Contains(possibleValue) || !legalNumbers.Contains(possibleValue))
                // if the number is already in the unique list, or is not in the legal numbers list, generate a new one
                // until it is unique and legal
            {
                possibleValue = UnityEngine.Random.Range(legalNumbers[0], legalNumbers[24]);

            }
            uniqueIntegerList.Add(possibleValue);
        }

        return uniqueIntegerList;
    }

    #endregion

    #region ButtonFunctions

    public void OnClickMinefieldButton(string buttonIndexString)
    {
        // when a minefield button is pressed, the given string is split into two parts,
        // the _minefieldArray index and the minefieldButtons index
        // then the child text component is assigned to mineLabel
        // then check if the index contains a mine
        // if not, update the label with the neighboring mines value and check for a win
        // finally make the button clicked non-interactable
        int[] buttonIndexes = buttonIndexString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
        TextMeshProUGUI mineLabel = minefieldButtons[buttonIndexes[1]].GetComponentInChildren<TextMeshProUGUI>();

        if (_minefieldArray[buttonIndexes[0]] == 9)
        {
            mineLabel.text = "X";
            endMessage.text = "You died";
            panels[2].SetActive(true);
        }
        else
        {
            mineLabel.text = _minefieldArray[buttonIndexes[0]].ToString();
            int emptyCells = int.Parse(emptyCellsNumber.text);
            emptyCells--;
            emptyCellsNumber.text = emptyCells.ToString();
            if (emptyCells == 0)
            {
                endMessage.text = "You won!";
                panels[2].SetActive(true);
            }
        }
        minefieldButtons[buttonIndexes[1]].interactable = false;
    }

    public void OnClickStartButton()
    {
        panels[0].SetActive(false);
        panels[1].SetActive(true);
    }

    public void OnClickQuitButton()
    {
        Application.Quit();
    }

    public void OnClickRestartButton()
    {
        SceneManager.LoadScene("MinesweeperBaukhagen");
    }

    #endregion
}