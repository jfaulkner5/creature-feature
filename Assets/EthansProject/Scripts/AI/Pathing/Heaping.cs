using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EthansProject
{
    /// <summary>
    /// Hack: comment This when you get around to it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Heaping<T> where T : IHeapItem<T>
    {

        T[] items;

        int currentItemCount;

        public Heaping(int maxHeapCount)
        {
            items = new T[maxHeapCount];
        }

        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            ++currentItemCount;
        }


        public T RemoveFirst()
        {
            T firstItem = items[0];
            --currentItemCount;
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return firstItem;
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        public int Count
        {
            get { return currentItemCount; }
        }

        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }

        void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;

                int spawIndex = 0;

                if (childIndexLeft < currentItemCount)
                {
                    spawIndex = childIndexLeft;

                    if (childIndexRight < currentItemCount)
                    {
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                            spawIndex = childIndexRight;
                    }

                    if (item.CompareTo(items[spawIndex]) < 0)
                    {
                        Swap(item, items[spawIndex]);
                    }
                    else return;

                }
                else return;

            }
        }


        void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            while (true)
            {
                T parentItem = items[parentIndex];

                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else break;

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;
            int itemAindex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAindex;
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex
        {
            get;
            set;
        }
    }
}