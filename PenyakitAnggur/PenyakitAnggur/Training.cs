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
    public partial class Training : Form
    {
        FileInfo[] fileImages;
        Bitmap[] bmpImages, inputFile;
        List<double[]> listVectorTexture;
        List<HSVEkstrak> listVectorWarna;
        string[] files;
        int[,] gray;
        private GLCM glcm = new GLCM();
        private double[,] tabelGlcm = new double[4, 4];

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

        public Training()
        {
            InitializeComponent();
            readExistTrain();
        }

        private void readExistTrain()
        {
            try
            {
                string[] dataTrain = System.IO.File.ReadAllLines("./VectorTraining/Training.txt");
                string[] dataTrainHSV = System.IO.File.ReadAllLines("./VectorTraining/TrainingHSV.txt");
                string[] idDataTrain = System.IO.File.ReadAllLines("./VectorTraining/ID_Training.txt");
                ListViewItem lv;
                string[] arrData = new string[3];
                int numHL = 0, numBR = 0, numLB = 0, numBM = 0;

                for (int i = 0; i < dataTrain.Length; i++)
                {
                    if(idDataTrain[i] == "HL")
                    {
                        numHL++;
                        arrData[0] = "HL_" + numHL.ToString("00");
                        arrData[1] = "Healthy";
                        arrData[2] = dataTrain[i] + "," + dataTrainHSV[i];
                    }
                    if (idDataTrain[i] == "BM")
                    {
                        numBM++;
                        arrData[0] = "BM_" + numBM.ToString("00");
                        arrData[1] = "Black Measles";
                        arrData[2] = dataTrain[i] + "," + dataTrainHSV[i];
                    }
                    if (idDataTrain[i] == "BR")
                    {
                        numBR++;
                        arrData[0] = "BR_" + numBR.ToString("00");
                        arrData[1] = "Black Rot";
                        arrData[2] = dataTrain[i] + "," + dataTrainHSV[i];
                    }
                    if (idDataTrain[i] == "LB")
                    {
                        numLB++;
                        arrData[0] = "LB_" + numLB.ToString("00");
                        arrData[1] = "Leaf Blight";
                        arrData[2] = dataTrain[i] + "," + dataTrainHSV[i];
                    }

                    lv = new ListViewItem(arrData);
                    listView1.Items.Add(lv);
                }

                rtbInfo.Text = "Data sudah pernah di-Latih. Berisi " + dataTrain.Length.ToString() + " Data Fitur";
            }
            catch(Exception e)
            {
                rtbInfo.Text = e.Message.ToString();
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            Bitmap[] dataTrain = new Bitmap[files.Length];
            string lineVectorID = "";
            string lineVectorTexture = "";
            string lineVectorWarna = "";

            ListViewItem lv;
            string[] arrData = new string[3];
            //=====================0 Derajat

            if (!Directory.Exists(@"DataTraining\") || !Directory.Exists(@"VectorTraining\"))
            {
                Directory.CreateDirectory(@"DataTraining");
                Directory.CreateDirectory(@"VectorTraining");
                int num = 1;
                for (int i = 0; i < fileImages.Length; i++)
                {
                    dataTrain[i] = new Bitmap(files[i]);
                    string namaFile = fileImages[i].Name;

                    string pathFile = string.Format("./DataTraining/{0}_{1}.bmp", namaFile.Substring(0, 2), num.ToString("00"));
                    dataTrain[i].Save(@pathFile);
                    string tmpVec = string.Join("|", listVectorTexture[i]);
                    lineVectorTexture += tmpVec;
                    lineVectorTexture += Environment.NewLine;

                    string vectorColorMoment = listVectorWarna[i].MeanH + "|" + listVectorWarna[i].MeanS + "|" + listVectorWarna[i].MeanV + "|" +
                        listVectorWarna[i].DeviationH + "|" + listVectorWarna[i].DeviationS + "|" + listVectorWarna[i].DeviationV + "|" +
                        listVectorWarna[i].SkewnessH + "|" + listVectorWarna[i].SkewnessS + "|" + listVectorWarna[i].SkewnessV;
                    lineVectorWarna += vectorColorMoment;
                    lineVectorWarna += Environment.NewLine;

                    if (namaFile.Substring(0,2) == "HL")
                    {
                        arrData[0] = "HL_" + num.ToString("00");
                        arrData[1] = "Healthy";
                        arrData[2] = tmpVec;
                        lineVectorID += "HL";
                        lineVectorID += Environment.NewLine;
                    }
                    if (namaFile.Substring(0, 2) == "BR")
                    {
                        arrData[0] = "BR_" + num.ToString("00");
                        arrData[1] = "Black Rot";
                        arrData[2] = tmpVec;
                        lineVectorID += "BR";
                        lineVectorID += Environment.NewLine;
                    }
                    if (namaFile.Substring(0, 2) == "BM")
                    {
                        arrData[0] = "BM_" + num.ToString("00");
                        arrData[1] = "Black Measles";
                        arrData[2] = tmpVec;
                        lineVectorID += "BM";
                        lineVectorID += Environment.NewLine;
                    }
                    if (namaFile.Substring(0, 2) == "LB")
                    {
                        arrData[0] = "LB_" + num.ToString("00");
                        arrData[1] = "Leaf Blight";
                        arrData[2] = tmpVec;
                        lineVectorID += "LB";
                        lineVectorID += Environment.NewLine;
                    }
                    lv = new ListViewItem(arrData);
                    listView1.Items.Add(lv);

                    num++;
                }
                using (StreamWriter writetext = new StreamWriter("./VectorTraining/ID_Training.txt"))
                {
                    writetext.Write(lineVectorID);
                }
                using (StreamWriter writetext = new StreamWriter("./VectorTraining/TrainingHSV.txt"))
                {
                    writetext.Write(lineVectorWarna);
                }
                using (StreamWriter writetext = new StreamWriter("./VectorTraining/Training.txt"))
                {
                    writetext.Write(lineVectorTexture);
                }
                MessageBox.Show("Data Telah di-Latih", "Info");
            }
            else
            {
                Array.ForEach(Directory.GetFiles(@"DataTraining"), File.Delete);
                Array.ForEach(Directory.GetFiles(@"VectorTraining"), File.Delete);
                int num = 1;
                for (int i = 0; i < fileImages.Length; i++)
                {
                    dataTrain[i] = new Bitmap(files[i]);
                    string namaFile = fileImages[i].Name;

                    string pathFile = string.Format("./DataTraining/{0}_{1}.bmp", namaFile.Substring(0, 2), num.ToString("00")); ;
                    dataTrain[i].Save(@pathFile);
                    string tmpVec = string.Join("|", listVectorTexture[i]);
                    lineVectorTexture += tmpVec;
                    lineVectorTexture += Environment.NewLine;

                    string vectorColorMoment = listVectorWarna[i].MeanH + "|" + listVectorWarna[i].MeanS + "|" + listVectorWarna[i].MeanV + "|" +
                        listVectorWarna[i].DeviationH + "|" + listVectorWarna[i].DeviationS + "|" + listVectorWarna[i].DeviationV + "|" +
                        listVectorWarna[i].SkewnessH + "|" + listVectorWarna[i].SkewnessS + "|" + listVectorWarna[i].SkewnessV;
                    lineVectorWarna += vectorColorMoment;
                    lineVectorWarna += Environment.NewLine;

                    if (namaFile.Substring(0, 2) == "HL")
                    {
                        arrData[0] = "HL_" + num.ToString("00");
                        arrData[1] = "Healthy";
                        arrData[2] = tmpVec;
                        lineVectorID += "HL";
                        lineVectorID += Environment.NewLine;
                    }
                    if (namaFile.Substring(0, 2) == "BR")
                    {
                        arrData[0] = "BR_" + num.ToString("00");
                        arrData[1] = "Black Rot";
                        arrData[2] = tmpVec;
                        lineVectorID += "BR";
                        lineVectorID += Environment.NewLine;
                    }
                    if (namaFile.Substring(0, 2) == "BM")
                    {
                        arrData[0] = "BM_" + num.ToString("00");
                        arrData[1] = "Black Measles";
                        arrData[2] = tmpVec;
                        lineVectorID += "BM";
                        lineVectorID += Environment.NewLine;
                    }
                    if (namaFile.Substring(0, 2) == "LB")
                    {
                        arrData[0] = "LB_" + num.ToString("00");
                        arrData[1] = "Leaf Blight";
                        arrData[2] = tmpVec;
                        lineVectorID += "LB";
                        lineVectorID += Environment.NewLine;
                    }
                    lv = new ListViewItem(arrData);
                    listView1.Items.Add(lv);

                    num++;
                }
                using (StreamWriter writetext = new StreamWriter("./VectorTraining/ID_Training.txt"))
                {
                    writetext.Write(lineVectorID);
                }
                using (StreamWriter writetext = new StreamWriter("./VectorTraining/TrainingHSV.txt"))
                {
                    writetext.Write(lineVectorWarna);
                }
                using (StreamWriter writetext = new StreamWriter("./VectorTraining/Training.txt"))
                {
                    writetext.Write(lineVectorTexture);
                }
                MessageBox.Show("Data Telah di-Latih", "Info");
            }

        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = Application.StartupPath;
            folderBrowserDialog1.ShowDialog();

            DirectoryInfo d = new DirectoryInfo(folderBrowserDialog1.SelectedPath);//Assuming Test is your Folder
            fileImages = d.GetFiles("*.jpg"); //Getting Text files
            files = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.jpg");
            PictureBox[] pbShow;

            Bitmap[] MLeak = new Bitmap[fileImages.Length];
            pbShow = new PictureBox[fileImages.Length];

            direktoriTxt.Text = folderBrowserDialog1.SelectedPath;
            jlhDataTxt.Text = fileImages.Length.ToString();

            if(fileImages.Length > 0)
                btnEkstraksi.Enabled = true;
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

                    //Normalisasi
                    double red = (double)(c.R / 255.0);
                    double green = (double)(c.G / 255.0);
                    double blue = (double)(c.B / 255.0);

                    double cmax = Math.Max(red, Math.Max(green, blue));
                    double cmin = Math.Min(red, Math.Min(green, blue));
                    double diff = cmax - cmin;
                    double h = -1.00, s = -1.0;

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

                    //Color newColor = Color.FromArgb((int)Math.Round(h), (int)Math.Round(s), (int)Math.Round(v));
                    //newImage.SetPixel(i, j, newColor);

                    //double rata2 = (double)((h + s + v) / 3.00);
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

        private Bitmap MedianFilter(int[,] grayArr)
        {
            
            int width = grayArr.GetLength(0);
            int height = grayArr.GetLength(1);
            Bitmap edited = new Bitmap(width, height);
            List<int> termsList = new List<int>();

            for (int x = 0; x <= width - 3; x++)
            {
                for (int y = 0; y <= height - 3; y++)
                {
                    for (int i = x; i <= x + 2; i++)
                    {
                        for (int j = y; j <= y + 2; j++)
                        {
                            termsList.Add(grayArr[i, j]);
                        }
                    }
                    int[] terms = termsList.ToArray();
                    termsList.Clear();
                    Array.Sort<int>(terms);
                    Array.Reverse(terms);
                    int color = terms[4];

                    edited.SetPixel(x + 1, y + 1, Color.FromArgb(color, color, color));
                }
            }
            
            return edited;
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

                    //grayArr[x, y] = (int)Math.Round(gray);
                    grayBmp.SetPixel(x, y, Color.FromArgb((int)Math.Round(gray), (int)Math.Round(gray), (int)Math.Round(gray)));
                }
            }

            return grayBmp;
        }

        private void btnEkstraksi_Click(object sender, EventArgs e)
        {
            if (jlhDataTxt.Text != null)
                btnBatal.Enabled = true;

            backgroundWorker.RunWorkerAsync();
            btnFolder.Enabled = false;
            btnSimpan.Enabled = false;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Bitmap[] dataTrain = new Bitmap[files.Length];
                HSVEkstrak dataTrainHsv;
                Bitmap[] dataTrainGray = new Bitmap[files.Length];
                listVectorTexture = new List<double[]>();
                listVectorWarna = new List<HSVEkstrak>();
                int indexProcess = 1;

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
                        dataTrain[i] = new Bitmap(files[i]);
                        dataTrainHsv = RGBtoHSV(dataTrain[i]);
                        dataTrainGray[i] = RGBtoGray(dataTrain[i]);
                        //namaFile[i] = fileImages[i].Name;
                        listVectorTexture.Add(EkstrakAllData(dataTrainGray[i]));
                        listVectorWarna.Add(dataTrainHsv);
                    }
                }

                MessageBox.Show("Ekstraksi Selesai");
                
            }
            catch(Exception ex)
            {
                backgroundWorker.CancelAsync();
                MessageBox.Show(ex.Message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            lblPercent.Text = string.Format("Memproses... {0} %", e.ProgressPercentage);
            progressBar1.Update();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnSimpan.Enabled = true;
            btnBatal.Enabled = false;
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();
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

                    int gray = c.R;

                    arrGrayImg[x, y] = gray;
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

    }
}
