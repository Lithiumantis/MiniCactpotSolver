using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CactpotSolver : MonoBehaviour
{

    private Dictionary<int, int> payoutsChart = new Dictionary<int, int>();
    public int[] payouts;
    public InputField[] inputFields;
    private int[,] slots = new int[3,3];

    // Start is called before the first frame update
    void Start()
    {
        //fill in initial payout data
        int payoutProgress = 0;
        for(int i = 6; i <= 24; i++)
        {
            if(payoutProgress < payouts.Length)
            {
                payoutsChart.Add(i, payouts[payoutProgress]);
                payoutProgress++;
            }
            else
            {
                Debug.Log("Error: Insufficient payout data");
                break;
            }
        }

        //starting input values
        for(int i = 0; i <= 2; i++)
        {
            for (int j = 0; j <= 2; j++)
            {
                slots[i, j] = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalculateResults()
    {
        GetInput();
        List<int> availableNumbers = FindMissingNumbers();

        int[] testArray = new int[3];
        float testAverage;
        //horizontals
        for(int i = 0; i < 3; i++)
        {
            testArray[0] = slots[i, 0];
            testArray[1] = slots[i, 1];
            testArray[2] = slots[i, 2];

            testAverage = CalculateAverage(testArray, availableNumbers);
            Debug.Log("Average result for row " + i + " with set numbers " + slots[i, 0] + " " + slots[i, 1] + " " + slots[i, 2] + ": " + testAverage);
        }

        //verticals
        for (int i = 0; i < 3; i++)
        {
            testArray[0] = slots[0, i];
            testArray[1] = slots[1, i];
            testArray[2] = slots[2, i];

            testAverage = CalculateAverage(testArray, availableNumbers);
            Debug.Log("Average result for column " + i + " with set numbers " + slots[0, i] +" " + slots[1, i] + " " + slots[2, i] + ": " + testAverage);
        }

        //diagonals
        testArray[0] = slots[0, 0];
        testArray[1] = slots[1, 1];
        testArray[2] = slots[2, 2];
        testAverage = CalculateAverage(testArray, availableNumbers);
        Debug.Log("Average result for SW/NE diagonal: " + testAverage);

        testArray[0] = slots[0, 2];
        testArray[1] = slots[1, 1];
        testArray[2] = slots[2, 0];
        testAverage = CalculateAverage(testArray, availableNumbers);
        Debug.Log("Average result for SE/NW diagonal: " + testAverage);

    }

    //get input from text fields
    private void GetInput()
    {
        int inputStep = 0;
        for (int i = 0; i <=2 ; i++)
        {
            for (int j = 0; j <= 2; j++)
            {
                if(inputFields[inputStep] != null)
                {
                    slots[i, j] = int.Parse(inputFields[inputStep].text);
                    Debug.Log("Slot " + i + ", " + j + " has value " + slots[i, j]);
                }
                else
                {
                    Debug.Log("Error: Not enough input fields registered");
                    break;
                }
                inputStep++;

            }
        }
    }

    //find what numbers player has NOT entered
    private List<int> FindMissingNumbers()
    {
        List<int> availableNumbers = new List<int>();
        for(int i = 1; i <= 9; i++)
        {
            availableNumbers.Add(i);
        }

        //iterate over inputs, remove ones that player has chosen from possible hidden values
        for(int checkedNumber = 1; checkedNumber <= 9; checkedNumber++)
        {
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    if(slots[i,j] == checkedNumber)
                    {
                        //Debug.Log("Found number " + checkedNumber + " in input, removing from MisingNumbers");
                        availableNumbers.Remove(checkedNumber);
                    }
                }
            }
        }

        return availableNumbers;
    }

    //finds the average return of all possible results in a row
    private float CalculateAverage(int[] set, List<int> availableNumbers)
    {
        float averagePayout = 0;

        //if this is a complete set already, just return the known value
        #region check complete set
        bool completeSet = true;
        foreach(int number in set)
        {
            if (number == 0)
                completeSet = false;
        }
        if (completeSet)
        {
            int completeTotal = 0;
            foreach (int number in set)
            {
                completeTotal += number;
            }
            Debug.Log("Found complete set: " + set[0] + " " + set[1] + " " + set[2]);
            return payoutsChart[completeTotal];
        }
        #endregion

        List<int[]> possibleSets = new List<int[]>();

        #region generate sets
        //test all numbers that could go in the first empty slot
        for (int i = 0; i < set.Length; i++)
        {       
            if(set[i] == 0)
            {
                //Debug.Log("Empty slot found at position " + i + " of base array");
                foreach(int availableNumber in availableNumbers)
                {
                    //create a copy of the array for each available number in that empty slot
                    //Debug.Log("Testing available number " + availableNumber + "in position " + i + " of base array");
                    int[] tempArray = new int[set.Length];
                    Array.Copy(set,tempArray,set.Length);
                    tempArray[i] = availableNumber;

                    //if this copy array is a complete set, add it to the list
                    bool complete1 = true;
                    foreach(int number in tempArray)
                    {
                        if (number == 0)
                            complete1 = false;
                    }
                    if (complete1)
                    {
                        possibleSets.Add(tempArray);
                    }
                        

                    //for each of the possible first empty slots, try all remaining numbers in the second empty slot
                    for (int j = 0; j < tempArray.Length; j++)
                    {
                        if(tempArray[j] == 0)
                        {
                            //Debug.Log("Empty slot found at position " + j + " of secondary array");
                            foreach (int availableNumber2 in availableNumbers)
                            {
                                //create a copy of the array for each available number in that empty slot, excluding the first available                               
                                if (availableNumber2 != availableNumber)
                                {
                                    //Debug.Log("Testing available number " + availableNumber2 + "in position " + j + " of secondary array");
                                    int[] tempArray2 = new int[set.Length];
                                    Array.Copy(tempArray, tempArray2, set.Length);
                                    tempArray2[j] = availableNumber2;

                                    //if this copy array is a complete set, add it to the list
                                    bool complete2 = true;
                                    foreach (int number in tempArray2)
                                    {
                                        if (number == 0)
                                            complete2 = false;
                                    }
                                    if (complete2)
                                        possibleSets.Add(tempArray2);

                                    //for each of the possible second empty slots, try all remaining numbers in the third empty slot
                                    for (int k = 0; k < tempArray2.Length; k++)
                                    {
                                        if (tempArray2[k] == 0)
                                        {
                                            //Debug.Log("Empty slot found at position " + k + " of tertiary array");
                                            foreach (int availableNumber3 in availableNumbers)
                                            {
                                                //create a copy of the array for each available number in that empty slot, excluding the first and second available
                                                if (availableNumber3 != availableNumber2 && availableNumber3 != availableNumber)
                                                {
                                                    
                                                    //Debug.Log("Testing available number " + availableNumber3 + "in position " + k + " of secondary array");
                                                    int[] tempArray3 = new int[set.Length];
                                                    Array.Copy(tempArray2, tempArray3, set.Length);
                                                    tempArray3[k] = availableNumber3;

                                                    possibleSets.Add(tempArray3);
                                                    //Debug.Log("Adding possible set: " + tempArray3);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        //find the average payout of all generated sets 
        int numberIterations = 0;

        foreach(int[] possibleSet in possibleSets)
        {
            //Debug.Log("Possible set: " + possibleSet[0] + " " + possibleSet[1] + " " + possibleSet[2]);
            //get the total input of this set
            int total = 0;
            foreach (int number in possibleSet)
            {
                total += number;
            }

            int thisPayout = payoutsChart[total];

            averagePayout += thisPayout;
            numberIterations++;
        }

        averagePayout = averagePayout / numberIterations;

        return averagePayout;
    }
}
