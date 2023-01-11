using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenyakitAnggur
{
    class FKNN
    {
        public struct InfoTrain
        {
            public string id;
            public string jenisPenyakit;
            public List<double> arrFitur;
            public double arrED;
            public int kelas;
        }

        public List<InfoTrain> EuclideanDistance(List<InfoTrain> arrDataTrain, List<double> arrDataTest)
        {
            List<InfoTrain> allDataArr = new List<InfoTrain>();
            List<InfoTrain> arrDataK = new List<InfoTrain>();
            List<double> tmpED = new List<double>();

            for (int i = 0; i < arrDataTrain.Count; i++)
            {
                InfoTrain infoTrain;
                infoTrain.id = arrDataTrain[i].id;
                infoTrain.jenisPenyakit = arrDataTrain[i].jenisPenyakit;
                infoTrain.arrFitur = arrDataTrain[i].arrFitur;
                infoTrain.arrED = subsTrainTest(infoTrain.arrFitur.ToArray(), arrDataTest.ToArray());
                infoTrain.kelas = 0;
                allDataArr.Add(infoTrain);
            }

            var newList = allDataArr.OrderBy(x => x.arrED).ToList();

            return allDataArr;
        }


        public double subsTrainTest(double[] dataTrain, double[] dataTest)
        {
            double tmp;
            double pangkat = 0.0;
            double hasil = 0.0;
            for (int i = 0; i < dataTrain.Length; i++)
            {
                tmp = 0;
                tmp = dataTrain[i] - dataTest[i];

                pangkat += Math.Pow(tmp, 2);
            }
            hasil = Math.Sqrt(pangkat);

            return hasil;
        }
    }
}
