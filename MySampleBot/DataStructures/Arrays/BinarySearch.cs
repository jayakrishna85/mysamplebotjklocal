using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures.Arrays
{
    class BinarySearch
    {
        public static void Process()
        {
            int[] data = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            //int searchedItemIndex = BinarySearchElement(data, data.Length, 1);
            int searchedItemIndex = BinarySearchElementRecursive(data, data.Length, 1);
            
            Console.Write(searchedItemIndex);
            Console.ReadLine();
        }

        public static int BinarySearchElement(int[] data, int n, int searchKey)
        {
            if (n == 0)
                return -1;

            int low = 0;
            int high = n - 1;

            while (low <= high)
            {
                int mid = (low + high) / 2;

                if (data[mid] == searchKey)
                {
                    return mid;
                }
                if (searchKey > data[mid])
                {
                    low = mid + 1;
                }
                else if (searchKey < data[mid])
                {
                    high = mid - 1;
                }
            }

            return -1;
        }

        public static int BinarySearchElementRecursive(int[] data, int n, int searchKey)
        {
            if (n == 0)
                return -1;

            return BinarySearchElementRecursive(data, 0, n - 1, searchKey);
        }

        public static int BinarySearchElementRecursive(int[] data, int low, int high, int searchKey)
        {
            if (low > high)
                return -1;

            int mid = (low + high) / 2;

            if (data[mid] == searchKey)
                return mid;
            else if (data[mid] > searchKey)
                return BinarySearchElementRecursive(data, low, mid - 1, searchKey);
            else
                return BinarySearchElementRecursive(data, mid + 1, high, searchKey);
        }
    }
}
