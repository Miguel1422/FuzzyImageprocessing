using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyLogic.ImageProcessing
{
    public static class Extensions
    {
        public static T[,] SubMatrix<T>(this T[,] matrix, int i1, int j1, int i2, int j2)
        {
            T[,] newMatrix = new T[i2 - i1 + 1, j2 - j1 + 1];

            for (int i = i1; i <= i2; i++)
            {
                for (int j = j1; j <= j2; j++)
                {
                    newMatrix[i - i1, j - j1] = matrix[i, j];
                }
            }

            return newMatrix;
        }
        public static IEnumerable<T> AsIEnumerable<T>(this T[,] matrix)
        {
            List<T> list = new List<T>();
            foreach(T t in matrix)
            {
                list.Add(t);
            }
            return list;
        }
        
        // Return the standard deviation of an array of Doubles.
        //
        // If the second argument is True, evaluate as a sample.
        // If the second argument is False, evaluate as a population.
        public static double StdDev(this IEnumerable<double> values, bool as_sample = true)
        {
            // Get the mean.
            double mean = values.Sum() / values.Count();

            // Get the sum of the squares of the differences
            // between the values and the mean.
            var squares_query =
                from double value in values
                select (value - mean) * (value - mean);
            double sum_of_squares = squares_query.Sum();

            if (as_sample)
            {
                return Math.Sqrt(sum_of_squares / (values.Count() - 1));
            }
            else
            {
                return Math.Sqrt(sum_of_squares / values.Count());
            }
        }
    }
}
