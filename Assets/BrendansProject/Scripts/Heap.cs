using UnityEngine;
using System.Collections;
using System;

namespace BrendansProject
{
    /// <summary>
    /// A Heap Class that allows for custom types. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Heap<T> where T : IHeapItem<T>
    {

        T[] items;
        int currentItemCount;

        public Heap(int maxHeapSize)
        {
            items = new T[maxHeapSize]; // Sets the heaps maximum size to be the maximum size of the grid.
        }

        /// <summary>
        /// Add the item to the items array.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item; // Add to end of array
            SortUp(item);
            currentItemCount++;
        }

        /// <summary>
        /// Remove the first item from the heap.
        /// </summary>
        /// <returns></returns>
        public T RemoveFirst()
        {
            T firstItem = items[0];
            currentItemCount--;
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return firstItem;
        }

        /// <summary>
        /// Used to change the priority of an item in the heap.
        /// </summary>
        /// <param name="item"></param>
        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        /// <summary>
        /// Get the current number of items in the heap.
        /// </summary>
        public int Count
        {
            get
            {
                return currentItemCount;
            }
        }

        /// <summary>
        /// Check if the heap contains a specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }


        /// <summary>
        /// Move an item down the list if its priority is lower than its parent
        /// </summary>
        /// <param name="item"></param>
        private void SortDown(T item)
        {
            while (true)
            {
                // Get indicies of the items two children.
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;
                int swapIndex = 0;

                // Check if item has a child on the left
                if (childIndexLeft < currentItemCount)
                {
                    swapIndex = childIndexLeft;

                    // Check if item has a child on the right and priority
                    if (childIndexRight < currentItemCount)
                    {
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    // Check if parent has lower priortiy than highest priority child, then swap if true
                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                    {
                        return; // If parent has highest priority its in correct position
                    }

                }
                else
                {
                    return; // If parent has no children its in the correct position
                }

            }
        }

        // Move item up the list if its priority is higher than the parent.
        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = items[parentIndex];
                // Compare checks the priority (fCost)
                if (item.CompareTo(parentItem) > 0)
                { // If has a lower f cost swap with parent.
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        /// <summary>
        /// Swap two items in the items array and assign the correct heapIndex.
        /// </summary>
        /// <param name="itemA"></param>
        /// <param name="itemB"></param>
        private void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;
            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex
        {
            get;
            set;
        }
    }
}