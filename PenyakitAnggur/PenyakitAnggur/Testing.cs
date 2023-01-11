using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PenyakitAnggur
{
    public partial class Testing : Form
    {
        private GLCM glcm = new GLCM();
        private FKNN fKNN = new FKNN();
        private List<double> tabelGlcm;
        OpenFileDialog fileInputOFD = new OpenFileDialog();
        FileInfo[] fileImages;
        string[] testingFiles;
        List<double[]> listVector;
        List<bool> arrAddDecision = new List<bool>();
        List<InfoHasil> listAllTestingData;
        List<HSVEkstrak> listVectorWarna;
        List<InfoTrain> arrHasil;
        List<string> listFileName = new List<string>();
        List<bool> listHasil = new List<bool>();

        public struct InfoTrain
        {
            public string id;
            public string jenisPenyakit;
            public List<double> arrFitur;
            public double euclideanDistance;
            public int kelas;
        }

        public struct InfoHasil
        {
            public double nilaiAnggota;
            public int kelas;
            public string jenisPenyakit;
            public bool benar;
        }

        private struct HSVEkstrak
        {
            public double MeanH;
            public double MeanS;
            public double MeanV;
            public double DeviationH;
            public double DeviationS;
            public double DeviationV;
            public double SkewnessH;
            public double SkewnessS;
            public double SkewnessV;
        }


        public Testing()
        {
            InitializeComponent();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = Application.StartupPath;
            folderBrowserDialog1.ShowDialog();

            DirectoryInfo d = new DirectoryInfo(folderBrowserDialog1.SelectedPath);//Assuming Test is your Folder
            fileImages = d.GetFiles("*.jpg"); //Getting Text files
            testingFiles = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.jpg");

            tbDirektoriUji.Text = folderBrowserDialog1.SelectedPath;
            tbJumlahFileUji.Text = fileImages.Length.ToString();

            if (fileImages.Length > 0)
                btnEkstrak.Enabled = true;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void btnEkstrak_Click(object sender, EventArgs e)
        {
            //if (jlhDataTxt.Text != null)
            //    btnBatal.Enabled = true;

            backgroundWorker.RunWorkerAsync();
            btnInput.Enabled = false;
            btnIdentifikasi.Enabled = false;
        }

        private double[] EkstrakAllData(Bitmap img)
        {
            double[] vektorTexture = new double[12];
            int[,] arrGrayImg = new int[img.Width, img.Height];

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Color c = img.GetPixel(x, y);

                    int gClr = c.R;

                    arrGrayImg[x, y] = gClr;
                }
            }

            int[,] derajat0 = glcm.derajat0(arrGrayImg);
            int[,] derajat45 = glcm.derajat45(arrGrayImg);
            int[,] derajat90 = glcm.derajat90(arrGrayImg);
            int[,] derajat135 = glcm.derajat135(arrGrayImg);

            int[,] simetris0 = glcm.simetris(derajat0);
            int[,] simetris45 = glcm.simetris(derajat45);
            int[,] simetris90 = glcm.simetris(derajat90);
            int[,] simetris135 = glcm.simetris(derajat135);

            double[,] normalD0 = glcm.normalisasi(simetris0);
            double[,] normalD45 = glcm.normalisasi(simetris45);
            double[,] normalD90 = glcm.normalisasi(simetris90);
            double[,] normalD135 = glcm.normalisasi(simetris135);

            vektorTexture[0] = glcm.hitungEnergi(normalD0);
            vektorTexture[1] = glcm.hitungEnergi(normalD45);
            vektorTexture[2] = glcm.hitungEnergi(normalD90);
            vektorTexture[3] = glcm.hitungEnergi(normalD135);
            vektorTexture[4] = glcm.hitungHomogen(normalD0);
            vektorTexture[5] = glcm.hitungHomogen(normalD45);
            vektorTexture[6] = glcm.hitungHomogen(normalD90);
            vektorTexture[7] = glcm.hitungHomogen(normalD135);
            vektorTexture[8] = glcm.hitungKontras(normalD0);
            vektorTexture[9] = glcm.hitungKontras(normalD45);
            vektorTexture[10] = glcm.hitungKontras(normalD90);
            vektorTexture[11] = glcm.hitungKontras(normalD135);

            return vektorTexture;
        }

        private HSVEkstrak RGBtoHSV(Bitmap img)
        {
            int wImg = img.Width;
            int hImg = img.Height;
            Bitmap newImage = new Bitmap(wImg, hImg);
            double sumH = 0.0, sumS = 0.0, sumV = 0.0;

            for (int i = 0; i < wImg; i++)
            {
                for (int j = 0; j < hImg; j++)
                {
                    Color c = img.GetPixel(i, j);

                    double red = (double)(c.R / 255.0);
                    double green = (double)(c.G / 255.0);
                    double blue = (double)(c.B / 255.0);

                    double cmax = Math.Max(red, Math.Max(green, blue));
                    double cmin = Math.Min(red, Math.Min(green, blue));
                    double diff = cmax - cmin;
                    double h = -1.0, s = -1.0;

                    if (cmax == cmin)
                        h = 0;
                    else if (cmax == red)
                        h = (double)(60.0 * (double)((green - blue) / diff) + 360.0) % 360.0;
                    else if (cmax == green)
                        h = (double)(60.0 * (double)((blue - red) / diff) + 120.0) % 360.0;
                    else if (cmax == blue)
                        h = (double)(60.0 * (double)((red - green) / diff) + 240.0) % 360.0;

                    if (cmax == 0.0)
                        s = 0.0;
                    else
                        s = (double)(diff / cmax) * 100.0;

                    double v = cmax * 100.0;

                    if (h > 255.0)
                        h = 255.0;
                    else if (h < 0.0)
                        h = 0.0;
                    else if (s > 255.0)
                        s = 255.0;
                    else if (s < 0.0)
                        s = 0.0;
                    else if (v > 255.0)
                        v = 255.0;
                    else if (v < 0.0)
                        v = 0.0;

                    sumH += h;
                    sumS += s;
                    sumV += v;
                }
            }

            HSVEkstrak hSVEkstrak;
            hSVEkstrak.MeanH = (double)(1.0 / (wImg * hImg)) * sumH;
            hSVEkstrak.MeanS = (double)(1.0 / (wImg * hImg)) * sumS;
            hSVEkstrak.MeanV = (double)(1.0 / (wImg * hImg)) * sumV;
            hSVEkstrak.DeviationH = Math.Sqrt((double)(1.0 / (wImg * hImg)) * Math.Pow((sumH - hSVEkstrak.MeanH), 2));
            hSVEkstrak.DeviationS = Math.Sqrt((double)(1.0 / (wImg * hImg)) * Math.Pow((sumS - hSVEkstrak.MeanS), 2));
            hSVEkstrak.DeviationV = Math.Sqrt((double)(1.0 / (wImg * hImg)) * Math.Pow((sumV - hSVEkstrak.MeanV), 2));
            hSVEkstrak.SkewnessH = Math.Pow((double)(1.0 / (wImg * hImg)) * Math.Pow(sumH - hSVEkstrak.MeanH, 3), (double)(1.0 / 3.0));
            hSVEkstrak.SkewnessS = Math.Pow((double)(1.0 / (wImg * hImg)) * Math.Pow(sumS - hSVEkstrak.MeanS, 3), (double)(1.0 / 3.0));
            hSVEkstrak.SkewnessV = Math.Pow((double)(1.0 / (wImg * hImg)) * Math.Pow(sumV - hSVEkstrak.MeanV, 3), (double)(1.0 / 3.0));


            return hSVEkstrak;
        }


        private Bitmap RGBtoGray(Bitmap dataRGB)
        {
            int[,] grayArr;
            Bitmap Temp = new Bitmap(dataRGB);
            grayArr = new int[Temp.Width, Temp.Height];
            double rCvt = 0.2989, gCvt = 0.5870, bCvt = 0.1140;
            Bitmap grayBmp = new Bitmap(dataRGB.Width, dataRGB.Height);

            for (int x = 0; x < Temp.Width; x++)
            {
                for (int y = 0; y < Temp.Height; y++)
                {
                    Color c = Temp.GetPixel(x, y);

                    double rClr = c.R;
                    double gClr = c.G;
                    double bClr = c.B;

                    double gray = (rCvt * rClr) + (gCvt * gClr) + (bCvt * bClr);

                    grayBmp.SetPixel(x, y, Color.FromArgb((int)Math.Round(gray), (int)Math.Round(gray), (int)Math.Round(gray)));
                }
            }

            return grayBmp;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (tbK.Text.Length > -1)
                btnIdentifikasi.Enabled = true;
            else if (tbK.TextLength < 0)
                btnIdentifikasi.Enabled = false;
        }

        private void btnIdentifikasi_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            bwIdentifikasi.RunWorkerAsync();
            btnInput.Enabled = false;
            btnEkstrak.Enabled = false;
        }

        private InfoHasil getPenyakit(double[] arrKelas)
        {
            InfoHasil infoHasil = new InfoHasil();
            int hasil = getMaxAnggota(arrKelas);

            if(hasil == 0)
            {
                infoHasil.nilaiAnggota = arrKelas[0];
                infoHasil.kelas = 1;
                infoHasil.jenisPenyakit = "Sehat";
            }
            else if (hasil == 1)
            {
                infoHasil.nilaiAnggota = arrKelas[1];
                infoHasil.kelas = 2;
                infoHasil.jenisPenyakit = "Black Rot";
            }
            else if (hasil == 2)
            {
                infoHasil.nilaiAnggota = arrKelas[2];
                infoHasil.kelas = 3;
                infoHasil.jenisPenyakit = "Black Measles";
            }
            else if (hasil == 3)
            {
                infoHasil.nilaiAnggota = arrKelas[3];
                infoHasil.kelas = 4;
                infoHasil.jenisPenyakit = "Leaf Blight";
            }

            return infoHasil;
        }

        private int getMaxAnggota(double[] arrKelas)
        {
            double maxAnggota = 0.0;
            int kelas = 0;

            for (int i = 0; i < 4; i++)
            {
                if (arrKelas[i] > maxAnggota)
                    maxAnggota = arrKelas[i];
            }

            for(int i = 0; i < arrKelas.Length; i++)
                if (arrKelas[i] == maxAnggota)
                    kelas = i;

            return kelas;
        }

        public double FuzzyKelas1(int KNeighbor, List<InfoTrain> arrDataTestingK)
        {
            List<InfoTrain> arrNew = new List<InfoTrain>();
            double nilaiAnggota = 0.0;
            InfoTrain infoTrain;

            for (int i = 0; i < KNeighbor; i++)
            {
                if (arrDataTestingK[i].id.Substring(0, 2) == "HL")
                {
                    infoTrain.id = arrDataTestingK[i].id;
                    infoTrain.jenisPenyakit = arrDataTestingK[i].jenisPenyakit;
                    infoTrain.arrFitur = arrDataTestingK[i].arrFitur;
                    infoTrain.euclideanDistance = arrDataTestingK[i].euclideanDistance;
                    infoTrain.kelas = 1;
                }
                else
                {
                    infoTrain.id = arrDataTestingK[i].id;
                    infoTrain.jenisPenyakit = arrDataTestingK[i].jenisPenyakit;
                    infoTrain.arrFitur = arrDataTestingK[i].arrFitur;
                    infoTrain.euclideanDistance = arrDataTestingK[i].euclideanDistance;
                    infoTrain.kelas = 0;
                }
                arrNew.Add(infoTrain);
            }

            nilaiAnggota = hitungKelas(arrNew);

            return nilaiAnggota;
        }

        public double FuzzyKelas2(int KNeighbor, List<InfoTrain> arrDataTestingK)
        {
            List<InfoTrain> arrNew = new List<InfoTrain>();
            double nilaiAnggota = 0.0;
            InfoTrain infoTrain;

            for (int i = 0; i < KNeighbor; i++)
            {
                if (arrDataTestingK[i].id.Substring(0, 2) == "BR")
                {
                    infoTrain.id = arrDataTestingK[i].id;
                    infoTrain.jenisPenyakit = arrDataTestingK[i].jenisPenyakit;
                    infoTrain.arrFitur = arrDataTestingK[i].arrFitur;
                    infoTrain.euclideanDistance = arrDataTestingK[i].euclideanDistance;
                    infoTrain.kelas = 1;
                }
                else
                {
                    infoTrain.id = arrDataTestingK[i].id;
                    infoTrain.jenisPenyakit = arrDataTestingK[i].jenisPenyakit;
                    infoTrain.arrFitur = arrDataTestingK[i].arrFitur;
                    infoTrain.euclideanDistance = arrDataTestingK[i].euclideanDistance;
                    infoTrain.kelas = 0;
                }
                arrNew.Add(infoTrain);
            }

            nilaiAnggota = hitungKelas(arrNew);

            return nilaiAnggota;
        }

        public double FuzzyKelas3(int KNeighbor, List<InfoTrain> arrDataTestingK)
        {
            List<InfoTrain> arrNew = new List<InfoTrain>();
            double nilaiAnggota = 0.0;
            InfoTrain infoTrain;

            for (int i = 0; i < KNeighbor; i++)
            {
                if (arrDataTestingK[i].id.Substring(0, 2) == "BM")
                {
                    infoTrain.id = arrDataTestingK[i].id;
                    infoTrain.jenisPenyakit = arrDataTestingK[i].jenisPenyakit;
                    infoTrain.arrFitur = arrDataTestingK[i].arrFitur;
                    infoTrain.euclideanDistance = arrDataTestingK[i].euclideanDistance;
                    infoTrain.kelas = 1;
                }
                else
                {
                    infoTrain.id = arrDataTestingK[i].id;
                    infoTrain.jenisPenyakit = arrDataTestingK[i].jenisPenyakit;
                    infoTrain.arrFitur = arrDataTestingK[i].arrFitur;
                    infoTrain.euclideanDistance = arrDataTestingK[i].euclideanDistance;
                    infoTrain.kelas = 0;
                }
                arrNew.Add(infoTrain);
            }

            nilaiAnggota = hitungKelas(arrNew);

            return nilaiAnggota;
        }

        public double FuzzyKelas4(int KNeighbor, List<InfoTrain> arrDataTestingK)
        {
            List<InfoTrain> arrNew = new List<InfoTrain>();
            double nilaiAnggota = 0.0;
            InfoTrain infoTrain;

            for (int i = 0; i < KNeighbor; i++)
            {
                if (arrDataTestingK[i].id.Substring(0, 2) == "LB")
                {
                    infoTrain.id = arrDataTestingK[i].id;
                    infoTrain.jenisPenyakit = arrDataTestingK[i].jenisPenyakit;
                    infoTrain.arrFitur = arrDataTestingK[i].arrFitur;
                    infoTrain.euclideanDistance = arrDataTestingK[i].euclideanDistance;
                    infoTrain.kelas = 1;
                }
                else
                {
                    infoTrain.id = arrDataTestingK[i].id;
                    infoTrain.jenisPenyakit = arrDataTestingK[i].jenisPenyakit;
                    infoTrain.arrFitur = arrDataTestingK[i].arrFitur;
                    infoTrain.euclideanDistance = arrDataTestingK[i].euclideanDistance;
                    infoTrain.kelas = 0;
                }
                arrNew.Add(infoTrain);
            }

            nilaiAnggota = hitungKelas(arrNew);

            return nilaiAnggota;
        }

        private double hitungKelas(List<InfoTrain> arrKnn)
        {
            double uX = 0.0;
            double tmpAtas = 0.0;
            double tmpBawah = 0.0;

            for (int i = 0; i < arrKnn.Count; i++)
            {
                tmpAtas += (arrKnn[i].kelas * Math.Pow(arrKnn[i].euclideanDistance, (-2.0 / 1.0)));
                tmpBawah += Math.Pow(arrKnn[i].euclideanDistance, (-2.0 / 1.0));
            }

            uX = tmpAtas / tmpBawah;

            return uX;
        }

        private void showHasil(List<InfoHasil> listHasil)
        {
            ListViewItem lv;
            string[] arrData = new string[4];
            
            for(int i = 0; i < listHasil.Count; i++)
            {
                int t = i + 1;
                arrData[0] = t.ToString();
                arrData[1] = listHasil[i].nilaiAnggota.ToString();
                arrData[2] = listHasil[i].jenisPenyakit;
                arrData[3] = listHasil[i].benar.ToString();
                lv = new ListViewItem(arrData);
                listView2.Items.Add(lv);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void listView2_ItemActivate(object sender, EventArgs e)
        {
            try
            {
                int itemSelect = listView2.SelectedIndices[0];
                Bitmap showTesting = new Bitmap(testingFiles[itemSelect]);
                richTextBox1.Text = testingFiles[itemSelect].ToString();
                pbShowCitra.Image = showTesting;
                pbShowCitra.Refresh();
            }
            catch
            {

            }
            
        }

        private void confusionMat(List<InfoHasil> listHasilKNN)
        {
            int brTrue = 0;
            int brFalse = 0;
            int bmTrue = 0;
            int bmFalse = 0;
            int lbTrue = 0;
            int lbFalse = 0;
            int shTrue = 0;
            int shFalse = 0;
            List<InfoHasil> newListHasil = new List<InfoHasil>();
            InfoHasil infoHasil;

            for (int i = 0; i < listHasilKNN.Count; i++)
            {
                if (listFileName[i] == "HL")
                {
                    if (listHasilKNN[i].jenisPenyakit == "Sehat")
                    {
                        shTrue++;
                        infoHasil.nilaiAnggota = listHasilKNN[i].nilaiAnggota;
                        infoHasil.jenisPenyakit = listHasilKNN[i].jenisPenyakit;
                        infoHasil.kelas = listHasilKNN[i].kelas;
                        infoHasil.benar = true;
                        newListHasil.Add(infoHasil);
                    }
                    else
                    {
                        //listHasil.Add(false);
                        shFalse++;
                        infoHasil.nilaiAnggota = listHasilKNN[i].nilaiAnggota;
                        infoHasil.jenisPenyakit = listHasilKNN[i].jenisPenyakit;
                        infoHasil.kelas = listHasilKNN[i].kelas;
                        infoHasil.benar = false;
                        newListHasil.Add(infoHasil);
                    }

                }
                if (listFileName[i] == "BR")
                {
                    if (listHasilKNN[i].jenisPenyakit == "Black Rot")
                    {
                        //listHasil.Add(true);
                        brTrue++;
                        infoHasil.nilaiAnggota = listHasilKNN[i].nilaiAnggota;
                        infoHasil.jenisPenyakit = listHasilKNN[i].jenisPenyakit;
                        infoHasil.kelas = listHasilKNN[i].kelas;
                        infoHasil.benar = true;
                        newListHasil.Add(infoHasil);
                    }
                    else
                    {
                        //listHasil.Add(false);
                        brFalse++;
                        infoHasil.nilaiAnggota = listHasilKNN[i].nilaiAnggota;
                        infoHasil.jenisPenyakit = listHasilKNN[i].jenisPenyakit;
                        infoHasil.kelas = listHasilKNN[i].kelas;
                        infoHasil.benar = false;
                        newListHasil.Add(infoHasil);
                    }
                }
                if (listFileName[i] == "BM")
                {
                    if (listHasilKNN[i].jenisPenyakit == "Black Measles")
                    {
                        //listHasil.Add(true);
                        bmTrue++;
                        infoHasil.nilaiAnggota = listHasilKNN[i].nilaiAnggota;
                        infoHasil.jenisPenyakit = listHasilKNN[i].jenisPenyakit;
                        infoHasil.kelas = listHasilKNN[i].kelas;
                        infoHasil.benar = true;
                        newListHasil.Add(infoHasil);
                    }
                    else
                    {
                        //listHasil.Add(false);
                        bmFalse++;
                        infoHasil.nilaiAnggota = listHasilKNN[i].nilaiAnggota;
                        infoHasil.jenisPenyakit = listHasilKNN[i].jenisPenyakit;
                        infoHasil.kelas = listHasilKNN[i].kelas;
                        infoHasil.benar = false;
                        newListHasil.Add(infoHasil);
                    }
                }
                if (listFileName[i] == "LB")
                {
                    if (listHasilKNN[i].jenisPenyakit == "Leaf Blight")
                    {
                        //listHasil.Add(true);
                        lbTrue++;
                        infoHasil.nilaiAnggota = listHasilKNN[i].nilaiAnggota;
                        infoHasil.jenisPenyakit = listHasilKNN[i].jenisPenyakit;
                        infoHasil.kelas = listHasilKNN[i].kelas;
                        infoHasil.benar = true;
                        newListHasil.Add(infoHasil);
                    }
                    else
                    {
                        //listHasil.Add(false);
                        lbFalse++;
                        infoHasil.nilaiAnggota = listHasilKNN[i].nilaiAnggota;
                        infoHasil.jenisPenyakit = listHasilKNN[i].jenisPenyakit;
                        infoHasil.kelas = listHasilKNN[i].kelas;
                        infoHasil.benar = false;
                        newListHasil.Add(infoHasil);
                    }
                }
            }

            showHasil(newListHasil);

            tbTP1.Text = brTrue.ToString();
            tbFP1.Text = brFalse.ToString();
            tbTP2.Text = bmTrue.ToString();
            tbFP2.Text = bmFalse.ToString();
            tbTP3.Text = lbTrue.ToString();
            tbFP3.Text = lbFalse.ToString();
            tbTP4.Text = shTrue.ToString();
            tbFP4.Text = shFalse.ToString();

            double akurasi1 = 0.0, akurasi2 = 0.0, akurasi3 = 0.0, akurasi4 = 0.0;
            if (brTrue != 0 || brFalse != 0)
            {
                akurasi1 = (double)(brTrue + 0.0) / (double)(brTrue + 0.0 + brFalse + 0.0);
                akurasi1 = akurasi1 * 100;
            }
            if (lbTrue != 0 || lbFalse != 0)
            {
                akurasi3 = (double)(lbTrue + 0.0) / (double)(lbTrue + 0.0 + lbFalse + 0.0);
                akurasi3 = akurasi3 * 100;
            }
            if (bmTrue != 0 || bmFalse != 0)
            {
                akurasi2 = (double)(bmTrue + 0.0) / (double)(bmTrue + 0.0 + bmFalse + 0.0);
                akurasi2 = akurasi2 * 100;
            }
            if (shTrue != 0 || shFalse != 0)
            {
                akurasi4 = (double)(shTrue + 0.0) / (double)(shTrue + 0.0 + shFalse + 0.0);
                akurasi4 = akurasi4 * 100;
            }
            //tb1.Text = akurasi1.ToString();
            //tb2.Text = akurasi2.ToString();
            //tb3.Text = akurasi3.ToString();
            //tb4.Text = akurasi4.ToString();
            double[] tmpArr = new double[4];
            tmpArr[0] = akurasi1;
            tmpArr[1] = akurasi2;
            tmpArr[2] = akurasi3;
            tmpArr[3] = akurasi4;

            int tmp = 0;
            double ak = 0;
            for (int i = 0; i < tmpArr.Length; i++)
                if (tmpArr[i] > 0)
                    tmp++;

            for (int i = 0; i < tmpArr.Length; i++)
                ak += tmpArr[i];

            tbAkurasi.Text = (ak / tmp).ToString();

            //tbTotalErr.Text = ((err1 + err2 + err3 + err4) / 4.0).ToString();
            //tbAkurasi.Text = (ak / tmp).ToString();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Bitmap[] dataTrain = new Bitmap[testingFiles.Length];
                HSVEkstrak dataTrainHsv;
                Bitmap[] dataTrainGray = new Bitmap[testingFiles.Length];
                listVectorWarna = new List<HSVEkstrak>();
                listVector = new List<double[]>();
                double[] tmpHsv = new double[9];
                int indexProcess = 1;
                listFileName.Clear();

                if (!backgroundWorker.CancellationPending)
                {
                    for (int i = 0; i < fileImages.Length; i++)
                    {
                        if (backgroundWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            MessageBox.Show("Proses dibatalkan");
                            return;
                        }
                        backgroundWorker.ReportProgress(indexProcess++ * 100 / (dataTrain.Length - 1), string.Format("Process data {0}", i));
                        listFileName.Add(fileImages[i].Name.Substring(0, 2));
                        dataTrain[i] = new Bitmap(testingFiles[i]);
                        dataTrainGray[i] = new Bitmap(RGBtoGray(dataTrain[i]));
                        dataTrainHsv = RGBtoHSV(dataTrain[i]);
                        tmpHsv[0] = dataTrainHsv.MeanH;
                        tmpHsv[1] = dataTrainHsv.MeanS;
                        tmpHsv[2] = dataTrainHsv.MeanV;
                        tmpHsv[3] = dataTrainHsv.DeviationH;
                        tmpHsv[4] = dataTrainHsv.DeviationS;
                        tmpHsv[5] = dataTrainHsv.DeviationV;
                        tmpHsv[6] = dataTrainHsv.SkewnessH;
                        tmpHsv[7] = dataTrainHsv.SkewnessS;
                        tmpHsv[8] = dataTrainHsv.SkewnessV;
                        double[] tmpGlcm = EkstrakAllData(dataTrainGray[i]);

                        double[] tmpCopy = new double[tmpHsv.Length + tmpGlcm.Length];
                        Array.Copy(tmpGlcm, tmpCopy, tmpGlcm.Length);
                        Array.Copy(tmpHsv, 0, tmpCopy, tmpGlcm.Length, tmpHsv.Length);
                        listVector.Add(tmpCopy);
                    }
                }
            }
            catch (Exception ex)
            {
                backgroundWorker.CancelAsync();
                MessageBox.Show(ex.Message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            lblProgress.Text = string.Format("Memproses... {0} %", e.ProgressPercentage);
            progressBar1.Update();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Proses Ekstraksi Selesai", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnIdentifikasi.Enabled = true;
        }

        private void bwIdentifikasi_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                List<InfoTrain> allTrainedData = new List<InfoTrain>();
                arrHasil = new List<InfoTrain>();
                listAllTestingData = new List<InfoHasil>();
                List<bool> arrAddDecision = new List<bool>();
                InfoTrain[] sortedTesting;
                InfoTrain infoTrain;
                
                int KNeighbor = int.Parse(tbK.Text);
                List<InfoTrain> listTestingK = new List<InfoTrain>();

                if (!backgroundWorker.CancellationPending)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(tbK.Text, "[^0-9]"))
                    {
                        MessageBox.Show("Input hanya boleh Angka", "Peringatan");
                        return;
                    }
                    if (tbK.Text.Length > 0 && int.Parse(tbK.Text) != 0 && listVector != null)
                    {
                        try
                        {
                            string[] dataTrain = System.IO.File.ReadAllLines("./VectorTraining/Training.txt");
                            string[] dataTrainHSV = System.IO.File.ReadAllLines("./VectorTraining/TrainingHSV.txt");
                            string[] idDataTrain = System.IO.File.ReadAllLines("./VectorTraining/ID_Training.txt");
                            int indexProcess = 1;
                            //=========

                            for (int i = 0; i < listVector.Count(); i++)
                            {
                                Bitmap tmpTesting = new Bitmap(testingFiles[i]);
                                if (bwIdentifikasi.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    MessageBox.Show("Proses dibatalkan");
                                    return;
                                }
                                bwIdentifikasi.ReportProgress(indexProcess++ * 100 / (listVector.Count - 1), string.Format("Process data {0}", i));
                                pbShowCitra.Image = tmpTesting;
                                allTrainedData.Clear();
                                for (int j = 0; j < dataTrain.Length; j++)
                                {
                                    double[] arrDataFitur;
                                    var tmpData = dataTrain[j].Split('|').Select(Double.Parse).ToList();
                                    var tmpDataHSV = dataTrainHSV[j].Split('|').Select(Double.Parse).ToList();
                                    double[] arrTmpGLCM = tmpData.ToArray();
                                    double[] arrTmpHSV = tmpDataHSV.ToArray();
                                    double[] tmpCopy = new double[arrTmpGLCM.Length + arrTmpHSV.Length];
                                    Array.Copy(arrTmpGLCM, tmpCopy, arrTmpGLCM.Length);
                                    Array.Copy(arrTmpHSV, 0, tmpCopy, arrTmpGLCM.Length, arrTmpHSV.Length);
                                    arrDataFitur = tmpCopy.ToArray();

                                    if (idDataTrain[j] == "HL")
                                    {
                                        infoTrain.jenisPenyakit = "Healthy";
                                        infoTrain.id = idDataTrain[j] + "_" + (j + 1);
                                        infoTrain.arrFitur = tmpData;
                                        infoTrain.euclideanDistance = fKNN.subsTrainTest(arrDataFitur, listVector[i]);
                                        infoTrain.kelas = 0;

                                        allTrainedData.Add(infoTrain);
                                    }
                                    else if (idDataTrain[j] == "BR")
                                    {
                                        infoTrain.jenisPenyakit = "Black Rot";
                                        infoTrain.id = idDataTrain[j] + "_" + (j + 1);
                                        infoTrain.arrFitur = tmpData;
                                        infoTrain.euclideanDistance = fKNN.subsTrainTest(arrDataFitur, listVector[i]);
                                        infoTrain.kelas = 0;

                                        allTrainedData.Add(infoTrain);
                                    }

                                    else if (idDataTrain[j] == "BM")
                                    {
                                        infoTrain.jenisPenyakit = "Black Measles";
                                        infoTrain.id = idDataTrain[j] + "_" + (j + 1);
                                        infoTrain.arrFitur = tmpData;
                                        infoTrain.euclideanDistance = fKNN.subsTrainTest(arrDataFitur, listVector[i]);
                                        infoTrain.kelas = 0;

                                        allTrainedData.Add(infoTrain);
                                    }

                                    else if (idDataTrain[j] == "LB")
                                    {
                                        infoTrain.jenisPenyakit = "Leaf Blight";
                                        infoTrain.id = idDataTrain[j] + "_" + (j + 1);
                                        infoTrain.arrFitur = tmpData;
                                        infoTrain.euclideanDistance = fKNN.subsTrainTest(arrDataFitur, listVector[i]);
                                        infoTrain.kelas = 0;

                                        allTrainedData.Add(infoTrain);
                                    }
                                }

                                var newList = allTrainedData.OrderBy(x => x.euclideanDistance).ToList();
                                sortedTesting = newList.ToArray();
                                listTestingK.Clear();
                                for (int j = 0; j < KNeighbor; j++)
                                {
                                    listTestingK.Add(sortedTesting[j]);
                                }
                                double kelas1 = FuzzyKelas1(KNeighbor, listTestingK);
                                double kelas2 = FuzzyKelas2(KNeighbor, listTestingK);
                                double kelas3 = FuzzyKelas3(KNeighbor, listTestingK);
                                double kelas4 = FuzzyKelas4(KNeighbor, listTestingK);

                                double[] arrKelas = new double[4];
                                arrKelas[0] = kelas1;
                                arrKelas[1] = kelas2;
                                arrKelas[2] = kelas3;
                                arrKelas[3] = kelas4;

                                listAllTestingData.Add(getPenyakit(arrKelas));
                            }
                            //showHasil(listAllTestingData);

                        }
                        catch
                        {
                            MessageBox.Show("Direktori Data tidak lengkap. Mohon Train Ulang Data", "Peringatan");
                        }
                    }
                    else if (listVector == null)
                    {
                        MessageBox.Show("Masukkan Data Pengujian", "Peringatan");
                    }
                    else
                        MessageBox.Show("Nilai K Harus > 0", "Peringatan");
                }
            }
            catch (Exception ex)
            {
                backgroundWorker.CancelAsync();
                MessageBox.Show(ex.Message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bwIdentifikasi_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            lblProgress.Text = string.Format("Memproses... {0} %", e.ProgressPercentage);
            progressBar1.Update();
            pbShowCitra.Refresh();
        }

        private void bwIdentifikasi_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Proses Identifikasi Selesai", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            confusionMat(listAllTestingData);
        }
    }
}
