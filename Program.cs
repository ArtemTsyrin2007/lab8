using System;

namespace LabWork
{
    public class Node
    {
        public long Value { get; set; }
        public Node Next { get; set; }
        public Node Prev { get; set; }
        public Node(long value) => Value = value;
    }

    public class DoublyLinkedList
    {
        public Node Head { get; private set; }
        public Node Tail { get; private set; }

        public void AddLast(long value)
        {
            Node newNode = new Node(value);
            if (Head == null) Head = Tail = newNode;
            else
            {
                Tail.Next = newNode;
                newNode.Prev = Tail;
                Tail = newNode;
            }
        }

        public Node FindFirstMultipleOfFive()
        {
            Node current = Head;
            while (current != null)
            {
                if (current.Value % 5 == 0) return current;
                current = current.Next;
            }
            return null;
        }

        public long SumEvenPositions()
        {
            long sum = 0;
            int pos = 1;
            Node current = Head;
            while (current != null)
            {
                if (pos % 2 == 0) sum += current.Value;
                current = current.Next;
                pos++;
            }
            return sum;
        }

        public DoublyLinkedList FilterGreater(long threshold)
        {
            DoublyLinkedList newList = new DoublyLinkedList();
            Node current = Head;
            while (current != null)
            {
                if (current.Value > threshold) newList.AddLast(current.Value);
                current = current.Next;
            }
            return newList;
        }

        public void RemoveGreaterThenAverage()
        {
            if (Head == null) return;
            double sum = 0;
            int count = 0;
            Node current = Head;
            while (current != null)
            {
                sum += current.Value;
                count++;
                current = current.Next;
            }
            double avg = sum / count;

            current = Head;
            while (current != null)
            {
                Node next = current.Next;
                if (current.Value > avg)
                {
                    if (current.Prev != null) current.Prev.Next = current.Next;
                    else Head = current.Next;
                    if (current.Next != null) current.Next.Prev = current.Prev;
                    else Tail = current.Prev;
                }
                current = next;
            }
        }

        public void Print()
        {
            Node current = Head;
            while (current != null)
            {
                Console.Write(current.Value + " ");
                current = current.Next;
            }
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main()
        {
            DoublyLinkedList list = new DoublyLinkedList();

            Console.Write("Введіть кількість елементів списку: ");
            int n = int.Parse(Console.ReadLine());

            for (int i = 0; i < n; i++)
            {
                Console.Write($"Введіть {i + 1}-й елемент: ");
                list.AddLast(long.Parse(Console.ReadLine()));
            }

            Console.WriteLine("\n--- Результати ---");

            var f5 = list.FindFirstMultipleOfFive();
            Console.WriteLine("Перше входження елементу, кратного 5: " + (f5?.Value.ToString() ?? "не знайдено"));

            Console.WriteLine("Сума елементів на парних позиціях: " + list.SumEvenPositions());

            Console.Write("\nВведіть значення для фільтрації: ");
            long threshold = long.Parse(Console.ReadLine());
            Console.Write("Новий список зі значень, більших за задане: ");
            list.FilterGreater(threshold).Print();

            list.RemoveGreaterThenAverage();
            Console.Write("Список після видалення елементів, більших за середнє: ");
            list.Print();
        }
    }
}
