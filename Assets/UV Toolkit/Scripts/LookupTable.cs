using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SurfaceMeshToolkit
{
    public class LookupTable<T>
    {
        public Dictionary<int, List<T>> Table;

        public LookupTable()
        {
            Table = new Dictionary<int, List<T>>();
        }

        public void AddElement(int tableID, T element)
        {
            if (!Table.ContainsKey(tableID))
                Table.Add(tableID, new List<T>());
            Table[tableID].Add(element);
        }

        public bool HasElement(int tableID, T element)
        {
            if (!Table.ContainsKey(tableID)) return false;
            return Table[tableID].Contains(element);
        }

        public void Clear()
        {
            Table.Clear();
        }
    }
}