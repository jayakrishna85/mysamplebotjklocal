using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures.Arrays
{
    public class Circular_MinItem
    {
        public static void Process()
        {
            int[] data = new int[] { 7, 8, 1, 2, 3, 4, 5, 6 };

        }

        public static int FindIndexOfMinItem(int[] data, int n)
        {
            if (n == 0)
                return -1;

            int low = 0;
            int high = n - 1;
            int min = -1;
            
            while(low<=high)
            {
                int mid = (low + high) / 2;

                if(data[mid]>data[low])
                {
                    low = mid + 1;
                }
                else if(data[mid]<data[high])
                {
                    
                }

            }


            return -1;
        }
    }
}
