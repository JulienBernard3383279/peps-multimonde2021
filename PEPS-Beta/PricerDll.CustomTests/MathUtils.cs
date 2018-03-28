using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PricerDll.CustomTests
{
    public static unsafe class MathUtils
    {
        #region Utils Soufiane
        public static double[,] ComputeReturns(double[,] data)
        {

            double[,] returns = new double[data.GetLength(0) - 1, data.GetLength(1)];
            for (int j = 0; j < returns.GetLength(1); j++)
            {
                for (int i = 0; i < returns.GetLength(0); i++)
                {
                    //returns[i, j] = (data[j, i + 1] - data[j, i]) / data[j, i];
                    returns[i, j] = Math.Log(data[i+1, j] / data[i, j]);
                }
            }
            return returns;
        }
        public static double[] ComputeMean(double[,] returns)
        {
            double[] mean = new double[returns.GetLength(1)];
            for (int j = 0; j < returns.GetLength(1); j++)
            {
                double temp = 0;
                for (int i = 0; i < returns.GetLength(0); i++)
                {
                    temp += returns[i, j];
                }
                mean[j] = temp / returns.GetLength(0);
            }
            return mean;
        }
        public static double[,] ComputeCovMatrix(double[,] returns)
        {
            double[,] covMat = new double[returns.GetLength(1), returns.GetLength(1)];
            double[] mean = ComputeMean(returns);
            for (int i = 0; i < returns.GetLength(1); i++)
            {
                double temp1 = mean[i];
                for (int j = 0; j <= i; j++)
                {
                    double temp2 = mean[j];
                    double res = 0;
                    for (int k = 0; k < returns.GetLength(0); k++)
                    {
                        res += (returns[k, i] - temp1) * (returns[k, j] - temp2);
                    }
                    covMat[ i, j] = res / returns.GetLength(0);
                    covMat[ j, i] = covMat[ i, j];
                }
            }
            return covMat;
        }
        public static double[] ComputeVolatility(double[,] covMatrix)
        {
            double[] vol = new double[covMatrix.GetLength(1)];
            for (int i = 0; i < covMatrix.GetLength(1); i++)
            {
                vol[i] = Math.Sqrt(252*covMatrix[i, i]);
            }
            return vol;
        }
        public static double[,] ComputeCorrMatrix(double[,] covMatrix)
        {
            double[,] corrMat = new double[covMatrix.GetLength(0), covMatrix.GetLength(1)];
            double[] vol = ComputeVolatility(covMatrix);
            for (int i = 0; i < covMatrix.GetLength(1); i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    corrMat[i, j] = covMatrix[i, j] / (vol[i] * vol[j]);
                    corrMat[j, i] = corrMat[i, j];
                }
            }
            return corrMat;
        }
        #endregion

        #region A,B to A+B,B Correlation matrixes utils
        /*
         * Compute the correlation of A+B with B assuming A and B follow normal laws
         */
        public static double CorrAplusBwithB(double corrAB, double volA, double volB)
        {
            return (corrAB * volA + volB) / Math.Sqrt(volA * volA + volB + volB + corrAB * volA * volB);
        }

        /*
         * Takes a 2x2 correlation matrix for variables A and B and
         * transforms it into the correlation matrix of variables A+B and B
         * Uses an array
         * Arrays are passed by reference in C#
         */
        public static void FromCorrABToCorrAPlusBB(double[] matrix, double volA, double volB)
        {
            matrix[1] = CorrAplusBwithB(matrix[1], volA, volB);
            matrix[2] = CorrAplusBwithB(matrix[2], volA, volB);
        }

        /*
         * Generates a 2x2 correlation matrix for variables A+B and B
         * from the correlation matrix of variables A+B and B
         * Uses an array
         */
        public static double[] GenCorrAPlusBBFromCorrAB(double[] matrix, double volA, double volB)
        {
            double[] toBeReturned;
            toBeReturned = (double[])matrix.Clone();
            FromCorrABToCorrAPlusBB(toBeReturned, volA, volB);
            return toBeReturned;
        }
        public static double[] GenCorrAPlusBBFromCorrAB(double[] matrix, double[] vol)
        {
            double[] toBeReturned;
            toBeReturned = (double[])matrix.Clone();
            FromCorrABToCorrAPlusBB(toBeReturned, vol[0], vol[1]);
            return toBeReturned;
        }
        #endregion
    }
}
