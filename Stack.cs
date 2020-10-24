using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class Stack
    {
		private static Stack instance = null;
		private int top;
		private int capacity;
		private Struct[] array;

		private Stack()
		{
			capacity = 1;
			array = new Struct[capacity];
			top = -1;
		}

		public static Stack Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Stack();
				}
				return instance;
			}
		}

		public void push(Struct data)
		{
			if (isFull())
			{
				expandArray(); // if array is full then increase its capacity
			}
			array[++top] = data; // insert the data
		}

		public Struct pop()
		{
			if (isEmpty())
			{
				Console.WriteLine("Stack is empty");
				return null;
			}
			else
			{
				reduceSize(); // function to check if size can be reduced
				return array[top--];
			}
		}

		public bool isFull()
		{
			if (capacity == top + 1)
				return true;
			else
				return false;
		}

		public bool isEmpty()
		{
			if (top == -1)
				return true;
			else
				return false;
		}

		private void expandArray()
		{
			int curr_size = top + 1;
			Struct[] new_array = new Struct[curr_size * 2];
			for (int i = 0; i < curr_size; i++)
			{
				new_array[i] = array[i];
			}
			array = new_array; // refer to the new array
			capacity = new_array.Length;
		}

		private void reduceSize()
		{
			int curr_length = top + 1;
			if (curr_length < capacity / 2)
			{
				Struct[] new_array = new Struct[capacity / 2];
				Array.Copy(array, 0, new_array, 0, new_array.Length);
				array = new_array;
				capacity = new_array.Length;
			}
		}

		public int getLength()
		{
			return top;
		}

		public Struct getPosition(int position)
		{
			if (position > top)
				return null;
			else
			{
				return array[position];
			}
		}

		public void setPosition(int position, Struct data)
		{
			if (position >= 0 && position <= top)
            {
				array[position] = data;
			}
		}

		public void display()
		{
			for (int i = 0; i <= top; i++)
			{
				Console.WriteLine(array[i] + "=>");
			}
			Console.WriteLine();
			Console.WriteLine("ARRAY SIZE:" + array.Length);
		}

		public void cleanStack()
		{
			top = -1;
		}
    }
}
