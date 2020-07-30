using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MC.Other
{
    public class DropOutStack<T>
    {
        public int Count = 0;

        private T[] items;
        private int top = 0;
        private int capacity;
        
        public DropOutStack(int Capacity)
        {
            capacity = Capacity;
            items = new T[capacity];
        }

        public void Push(T item)
        {
            items[top] = item;
            top = (top + 1) % items.Length;
            if (Count < capacity)
            {
                Count++;
            }
        }
        
        public T Pop()
        {            
            if(Count > 0)
            {
                top = (items.Length + top - 1) % items.Length;
                Count--;
                return items[top];                
            }
            else
            {
                return default(T);
            }
        }

        public void Clear()
        {
            Count = 0;
            top = 0;
        }
    }
}
