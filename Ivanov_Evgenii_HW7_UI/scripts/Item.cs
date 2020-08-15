using System;
using UnityEngine;


namespace Game2D
{
    [Serializable]
    public class Item
    {
        public string Name;
        public ItemType Type;
        public GameObject GameItem;
        public bool IsTwoHandled;
        public int Count;

        public Item()
        {

        }

        public Item(string name, ItemType type, GameObject item, bool isTwoHanded, int count)
        {
            Name = name;
            Type = type;
            GameItem = item;
            IsTwoHandled = isTwoHanded;
            Count = count;
        }

        public bool Compare(Item other)
        {
            bool flag = false;

            var name = Name.Split()[0];
            var otherName = other.Name.Split()[0];

            if (this.Type.Equals(other.Type) 
                && (name.Equals(otherName))
                && this.IsTwoHandled == other.IsTwoHandled)
            {
                flag = true;
            }

            return flag;
        }
    }
}
