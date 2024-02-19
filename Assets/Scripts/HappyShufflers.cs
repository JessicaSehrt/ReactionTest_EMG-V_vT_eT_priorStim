using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* 
 * VHSS Valis Heuristic Shuffle Service
 */

public class HappyShufflers : MonoBehaviour
{
    public static T[] Shuffle<T>(T[] array, int seed)
    {

         Random.InitState(seed);

        int n = array.Length;
        for (int i = 0; i < n; i++)
        {

            int r = i + (int)(UnityEngine.Random.value * (n - i));
            T t = array[r];
            array[r] = array[i];
            array[i] = t;
        }
        return array;
    }

    public static T[] GetLatinSquare<T>(T[] array, int participant)
    {
        if (array.Length <= 1) return array;
        // Init Square
        int[,] latinSquare = new int[array.Length, array.Length];

        latinSquare[0, 0] = 1;
        latinSquare[0, 1] = 2;

        // Fill 1st row
        for (int i = 2, j = 3, k = 0; i < array.Length; i++)
        {
            if (i % 2 == 1)
                latinSquare[0, i] = j++;
            else
                latinSquare[0, i] = array.Length - (k++);
        }

        // Fill first column
        for (int i = 1; i <= array.Length; i++)
        {
            latinSquare[i - 1, 0] = i;
        }

        // The rest of the square
        for (int row = 1; row < array.Length; row++)
        {
            for (int col = 1; col < array.Length; col++)
            {
                latinSquare[row, col] = (latinSquare[row - 1, col] + 1) % array.Length;

                if (latinSquare[row, col] == 0)
                    latinSquare[row, col] = array.Length;
            }
        }

        int squareItem = (((participant - 1) % array.Length));
        // Debug.Log("participant no. " + (squareItem + 1));

        // Return only the Participants' Latin Square Item 
        T[] newArray = new T[array.Length];

        for (int col = 0; col < array.Length; col++)
        {
            newArray[col] = array[latinSquare[squareItem, col] - 1];
        }
        return newArray;
    }

    public static T[] GetAllPermutations<T>(T[] array, int participant)
    {
        List<List<T>> results = GeneratePermutations<T>(array.ToList());
        T[] newArray = new T[array.Length];
        int row = (participant + 1) % (results.Count);
        for (int i = 0; i < results[row].Count; i++)
        {
            newArray[i] = results[row][i];
        }
        return newArray;
    }

    private static List<List<T>> GeneratePermutations<T>(List<T> items)
    {
        T[] current_permutation = new T[items.Count];
        bool[] in_selection = new bool[items.Count];
        List<List<T>> results = new List<List<T>>();
        PermuteItems<T>(items, in_selection, current_permutation, results, 0);
        return results;
    }

    private static void PermuteItems<T>(List<T> items, bool[] in_selection, T[] current_permutation, List<List<T>> results, int next_position)
    {
        if (next_position == items.Count)
        {
            results.Add(current_permutation.ToList());
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (!in_selection[i])
                {
                    in_selection[i] = true;
                    current_permutation[next_position] = items[i];
                    PermuteItems<T>(items, in_selection, current_permutation, results, next_position + 1);
                    in_selection[i] = false;
                }
            }
        }
    }

    //Heuristic Non-Consecutive Duplicate (NCD) Shuffler
    public static T[] NCDShuffle<T>(T[] array, int seed)
    {
        if (array == null || array.Length <= 1) return null;
        int MAX_RETRIES = 100; //it's heuristic
        bool found;
        int retries = 1;
        do
        {
            array = Shuffle(array, seed);
            found = true;
            for (int i = 0; i < array.Length - 1; i++)
            {
                T cur = array[i];
                T next = array[i + 1];
                if (EqualityComparer<T>.Default.Equals(cur, next)) 
                {
                    //choose between front and back with some probability based on the size of sublists
                    int r = (int)(Random.value * array.Length);
                    if (i < r)
                    {
                        if (!swapFront(i + 1, next, array, true))
                        {
                            found = false;
                            break;
                        }
                    }
                    else
                    {
                        if (!swapBack(i + 1, next, array, true))
                        {
                            found = false;
                            break;
                        }
                    }
                }
            }
            retries++;
        } while (retries <= MAX_RETRIES && !found);
        return array;
    }

    private static bool swapFront<T>(int index, T t, T[] array, bool first)
    {
        if (index == array.Length - 1) return first ? swapBack(index, t, array, false) : false;
        int n = array.Length - index - 1;
        int r = (int)(Random.value * n) + index + 1;
        int counter = 0;
        while (counter < n)
        {
            T t2 = array[r];
            if (!EqualityComparer<T>.Default.Equals(t, t2))
            {
                array = swap(array, index, r);
                //swaps++;
                return true;
            }
            r++;
            if (r == array.Length) r = index + 1;
            counter++;
        }

        //can't move it front, try back
        return first ? swapBack(index, t, array, false) : false;
    }

    private static T[] swap<T>(T[] array, int index, int r)
    {
        T tmp = array[r];
        array[r] = array[index];
        array[index] = tmp;
        return array;
    }

    //try to swap it with an element in a random "previous" position
    private static bool swapBack<T>(int index, T t, T[] array, bool first)
    {
        if (index <= 1) return first ? swapFront(index, t, array, false) : false;
        int n = index - 1;
        int r = (int)(Random.value * n);
        int counter = 0;
        while (counter < n)
        {
            T t2 = array[r];
            if (!EqualityComparer<T>.Default.Equals(t, t2) && !hasEqualNeighbours(r, t, array))
            {
                array = swap(array, index, r);
                //swaps++;
                return true;
            }
            r++;
            if (r == index) r = 0;
            counter++;
        }
        return first ? swapFront(index, t, array, false) : false;
    }

    //check if an element t can fit in position i
    public static bool hasEqualNeighbours<T>(int i, T t, T[] array)
    {
        if (array.Length == 1)
            return false;
        else if (i == 0)
        {
            if (EqualityComparer<T>.Default.Equals(t, array[i + 1]))
                return true;
            return false;
        }
        else
        {
            if (EqualityComparer<T>.Default.Equals(t, array[i - 1]) || EqualityComparer<T>.Default.Equals(t, array[i + 1]))
                return true;
            return false;
        }
    }

    //check if shuffled with no consecutive duplicates
    public static bool isShuffledOK<T>(T[] array)
    {
        for (int i = 1; i < array.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(array[i], array[i - 1]))
                return false;
        }
        return true;
    }
    //count consecutive duplicates, the smaller the better; We need ZERO
    public static int getFitness<T>(T[] array)
    {
        int sum = 0;
        for (int i = 1; i < array.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(array[i], array[i - 1]))
                sum++;
        }
        return sum;
    }
    public static bool Compare<T>(T x, T y) where T : class
    {
        return x == y;
    }
}
