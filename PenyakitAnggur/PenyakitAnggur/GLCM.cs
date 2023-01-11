using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenyakitAnggur
{
    class GLCM
    {
        public int[,] derajat0(int[,] matrixGray)
        {
            //int maxValue = getMaxFrom2DArray(matrixGray);
            int[,] derajat0Arr = new int[matrixGray.GetLength(0), matrixGray.GetLength(1)];

            for(int x = 0; x < matrixGray.GetLength(1); x++)
            {
                for(int y = 0; y < matrixGray.GetLength(0); y++)
                {
                    if (y == matrixGray.GetLength(0) - 1)
                        continue;
                    else
                    {
                        int kernelW = matrixGray[x, y + 1];//3
                        int kernelH = matrixGray[x, y];//2

                        derajat0Arr[kernelW, kernelH] += 1;
                    }
                }
            }

            return derajat0Arr;
        }

        public int[,] derajat45(int[,] matrixGray)
        {
            //int maxValue = getMaxFrom2DArray(matrixGray);
            int[,] derajat45Arr = new int[matrixGray.GetLength(0), matrixGray.GetLength(1)];

            for (int x = 1; x < matrixGray.GetLength(1); x++)//H
            {
                for (int y = 0; y < matrixGray.GetLength(0); y++)//W
                {
                    if (y == matrixGray.GetLength(0) - 1)
                        continue;
                    else
                    {
                        int kernelW = matrixGray[x - 1, y + 1];
                        int kernelH = matrixGray[x, y];

                        derajat45Arr[kernelW, kernelH] += 1;
                    }
                }
            }

            return derajat45Arr;
        }

        public int[,] derajat90(int[,] matrixGray)
        {
            //int maxValue = getMaxFrom2DArray(matrixGray);
            int[,] derajat90Arr = new int[matrixGray.GetLength(0), matrixGray.GetLength(1)];

            for (int x = 1; x < matrixGray.GetLength(1); x++)
            {
                for (int y = 0; y < matrixGray.GetLength(0); y++)
                {
                    int kernelW = matrixGray[x - 1, y];
                    int kernelH = matrixGray[x, y];

                    derajat90Arr[kernelW, kernelH] += 1;
                }
            }

            return derajat90Arr;
        }

        public int[,] derajat135(int[,] matrixGray)
        {
            //int maxValue = getMaxFrom2DArray(matrixGray);
            int[,] derajat135Arr = new int[matrixGray.GetLength(0), matrixGray.GetLength(1)];

            for (int x = 1; x < matrixGray.GetLength(1); x++)
            {
                for (int y = 1; y < matrixGray.GetLength(0); y++)
                {
                    int kernelW = matrixGray[x - 1, y - 1];//1
                    int kernelH = matrixGray[x, y];//3

                    derajat135Arr[kernelW, kernelH] += 1;
                }
            }

            return derajat135Arr;
        }

        public int[,] simetris(int[,] arrDerajatN)
        {
            int[,] simetrisArr = new int[arrDerajatN.GetLength(0), arrDerajatN.GetLength(1)];
            int[,] transposeArr = new int[arrDerajatN.GetLength(0), arrDerajatN.GetLength(1)];

            //Transpose Matrix
            for (int x = 0; x < arrDerajatN.GetLength(0); x++)
            {
                for (int y = 0; y < arrDerajatN.GetLength(1); y++)
                {
                    transposeArr[x, y] = arrDerajatN[y, x];
                }
            }

            //Transpose + derajatN
            for (int x = 0; x < arrDerajatN.GetLength(0); x++)
            {
                for (int y = 0; y < arrDerajatN.GetLength(1); y++)
                {
                    simetrisArr[x, y] = arrDerajatN[x, y] + transposeArr[x, y];
                }
            }

            return simetrisArr;
        }

        public double[,] normalisasi(int[,] arrDerajatN)
        {
            double[,] normalisasiArr = new double[arrDerajatN.GetLength(0), arrDerajatN.GetLength(1)];
            int nilaiProbabilitas = hitungProbabilitas(arrDerajatN);

            for(int x = 0; x < arrDerajatN.GetLength(0); x++)
            {
                for (int y = 0; y < arrDerajatN.GetLength(1); y++)
                {
                    if (arrDerajatN[x, y] > 0)
                        normalisasiArr[x, y] = (double) arrDerajatN[x, y] / (double) nilaiProbabilitas;
                }
            }
            
            return normalisasiArr;
        }

        private int hitungProbabilitas(int[,] arr2d)
        {
            int nilaiProbabilitas = 0;

            for (int x = 0; x < arr2d.GetLength(0); x++)
                for (int y = 0; y < arr2d.GetLength(1); y++)
                    nilaiProbabilitas += arr2d[x, y];

            return nilaiProbabilitas;
        }

        public double hitungKontras(double[,] arrNormalisasi)
        {
            double nilaiKontras = 0.0;

            for(int x = 0; x < arrNormalisasi.GetLength(0); x++)
                for(int y = 0; y < arrNormalisasi.GetLength(1); y++)
                {
                    arrNormalisasi[x, y] = Math.Pow(Math.Abs((x - y)), 2) * arrNormalisasi[x, y];
                    nilaiKontras += arrNormalisasi[x, y];
                }
                        

            return nilaiKontras;
        }

        public double hitungKorelasi(double[,] arrNormalisasi)
        {
            double nilaiKorelasi = 0.0;
            double ox = 0.0, oy = 0.0;
            double[,] tmpUx = new double[arrNormalisasi.GetLength(0), arrNormalisasi.GetLength(1)];
            double[,] tmpUy = new double[arrNormalisasi.GetLength(0), arrNormalisasi.GetLength(1)];

            for (int x = 0; x < arrNormalisasi.GetLength(0); x++)
                for (int y = 0; y < arrNormalisasi.GetLength(1); y++)
                        tmpUx[x, y] += x * arrNormalisasi[x, y];

            for (int x = 0; x < arrNormalisasi.GetLength(0); x++)
                for (int y = 0; y < arrNormalisasi.GetLength(1); y++)
                        tmpUy[x, y] += y * arrNormalisasi[x, y];

            for (int x = 0; x < arrNormalisasi.GetLength(0); x++)
                for (int y = 0; y < arrNormalisasi.GetLength(1); y++)
                    ox += Math.Pow((x - tmpUx[x, y]), 2) * arrNormalisasi[x, y];

            for (int x = 0; x < arrNormalisasi.GetLength(0); x++)
                for (int y = 0; y < arrNormalisasi.GetLength(1); y++)
                {
                    oy += Math.Pow((y - tmpUy[x, y]), 2) * arrNormalisasi[x, y];
                    nilaiKorelasi += (tmpUx[x, y] * tmpUy[x, y] * arrNormalisasi[x, y]) / (Math.Sqrt(ox) * Math.Sqrt(oy));
                }
                   
             return nilaiKorelasi;
        }

        public double hitungEnergi(double[,] arrNormalisasi)
        {
            double nilaiEnergi = 0.0;

            for (int x = 0; x < arrNormalisasi.GetLength(0); x++)
                for (int y = 0; y < arrNormalisasi.GetLength(1); y++)
                    nilaiEnergi += Math.Pow(arrNormalisasi[x, y], 2);

            return nilaiEnergi;
        }

        public double hitungHomogen(double[,] arrNormalisasi)
        {
            double nilaiHomogen = 0.0;

            for (int x = 0; x < arrNormalisasi.GetLength(0); x++)
                for (int y = 0; y < arrNormalisasi.GetLength(1); y++)
                    nilaiHomogen += arrNormalisasi[x, y] / (1 + Math.Abs(x - y));

            return nilaiHomogen;
        }

        private int getMaxFrom2DArray(int[,] array2d)
        {
            int tmpVal = 0;
            for(int i = 0; i < array2d.GetLength(0); i++)
            {
                for(int j = 0; j < array2d.GetLength(0); j++)
                {
                    if (array2d[i, j] > tmpVal)
                        tmpVal = array2d[i, j];
                }
            }

            return tmpVal;
        }

    }
}
