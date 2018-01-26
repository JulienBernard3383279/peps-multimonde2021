using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerDll.CustomTests
{
    public static unsafe class MathUtils
    {
        public static double[,] ComputeReturns(double[,] data)
        {
            double[,] returns = new double[data.GetLength(0)-1, data.GetLength(1)];
            for (int j = 0; j < returns.GetLength(1); j++)
            {
                for (int i = 0; i < returns.GetLength(0); i++)
                {
                    returns[i, j] = (data[ i + 1, j] - data[ i, j]) / data[ i, j];
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
                mean[j] = temp / returns.GetLength(1);
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
                vol[i] = Math.Sqrt(covMatrix[i, i]);
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
    }
}
