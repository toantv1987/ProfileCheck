using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using libxl;
using Microsoft.Office;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualBasic.Logging;

namespace AutoDoDuongKinh
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            chartCheck.MouseWheel += chartCheck_MouseWheel;
        }

        #region Variable Common

        //Mat khau setting máy PLC: 3744
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        int bienNhoTime = 0;
        /// <summary>
        /// Timer gan gia tri m_bDirty2 = false;
        /// </summary>
       
        /// <summary>
        /// Khai báo lớp theo dõi file
        /// </summary>
        static FileSystemWatcher m_Watcher;
        static FileSystemWatcher m_Watcher2;

        /// <summary>
        /// Lưu số stt của pcs Để kiểm tra pcs đã được check hay chưa
        /// </summary>
        static int soluong = 0;
        /// <summary>
        /// Lưu số stt của pcs Để kiểm tra pcs đã được check hay chưa
        /// </summary>
        /// 
        static int SoLuongTruoc = 0;
        /// <summary>
        /// biến lưu sự kiện đã thay đổi file hay chưa. Tránh trường hợp lưu nhiều lần
        /// </summary>
        /// 

        bool isStatusLotRunning = false;
        const int MAX_POINT = 4000;
        string strMachineNo;
        /// <summary>
        /// Lưu 33 giá trị trong file ItemSetting tương ứng 33 ô nhớ trong PLC.
        /// m_iPara[0] : Đường kính lớn nhất ở điểm đo D1
        /// m_iPara[9] : Đường kính nhỏ nhất ở điểm đo D1
        /// m_iPara[6] : Đường kính lớn nhất ở điểm đo D4
        /// m_iPara[7] : Đường kính nhỏ nhất ở điểm đo D4
        /// m_iPara[21] : Độ dài lớn nhất ở điểm đo D4
        /// m_iPara[22] : Độ dài nhỏ nhất ở điểm đo D4
        /// </summary>
        int[] m_iPara = new int[40];

        string strFileName = "";
        string strFileName2 = "";
        //Khái báo mảng giá trị
        int[] d_No1 = new int[MAX_POINT];
        int[] d_No2 = new int[600];
        /// <summary>
        /// Mảng 2 chiều. Đùng để lưu giá trị đường kính
        /// </summary>
        int[,] d_All = new int[21, 7];
        /// <summary>
        /// mảng lưu số thứ tự tìm các giá trị trên mảng a(mảng chứa các dữ liệu đường kính đọc từ file Data.csv)
        /// </summary>
        int[] p = new int[15];
        /// <summary>
        /// l[0] Lưu giá trị về độ dài của mỗi điểm đo.
        /// </summary>
        //double l[0] = 0;
        double[] l = new double[8];
        /// <summary>
        /// Mảng lưu vị trí trên mảng a, dựa vào các điểm này để tính độ dài đến các điểm đo. D4,D3,D2,D1
        /// </summary>
        int[] x = new int[8];
        int[] vitri = new int[8];
        int[] p2 = new int[4];
        double l2;
        int x2;
        /// <summary>
        /// Kết quả kiểm tra Core
        /// </summary>
        bool bResult = false;
        // m_iSTT là số lượng core đã được check. Mục đích để đếm số core. Khi m_iSTT >=20 sẽ in ra một file Excel mới
        int m_iSTT = 1;

        string strTemp = "";
        /// <summary>
        /// Trạng thái dừng hoặc chạy phần mềm: = True: đang chạy; =False đang dừng
        /// </summary>
        bool m_bIsStop;
        /// <summary>
        /// Cách làm cũ. Sử dụng để lưu các nhóm hàng gắn SION_1830,
        /// </summary>
        string m_sItem;

        int m_iQuantity;
        string strPrinter;

        int m_iFromPcs;
        int m_iToPcs;
        /// <summary>
        /// Biến điều khiển vị trí sẽ xuất ra màn hình. Giá trị đường kính
        /// </summary>
        int iDisplay;
        int m_iRecheck;

        //Các biến giành cho time delay (Sleep)
        static int DelayData_1 = 3000; //delay khi đọc file Data.csv
        static int DelayData_2 = 3000; //delay trước khi đọc file Data2.csv
        static int DelayBeforeResult1 = 2000; //Delay trước khi ghi Result.csv
        static int DelayBeforeResult2 = 2000; //Delay trước khi ghi Result2.csv
                                              //Các biến bù giá trị độ dài
        clsBuGiaTri clsBu = new clsBuGiaTri();
        int DiameterTrungBinh = 2500;

        /// <summary>
        /// Biến thuộc vị trí ;y0 = Average(a,p[14],3)+x
        /// </summary>

        //# Hết các biến giành cho time delay
        double m_kizuValue;
        double maxDistance;
        bool bKizu;
        bool bUseCheckKizu = false;
        int maxDisPos;
        double[] S = new double[8];

        double[] waveValue = new double[8];
        /// <summary>
        /// Lưu giữ giá trị tiêu chuẩn của Sion
        /// </summary>
        double[] sion_waveValue = new double[8];
        /// <summary>
        /// Lưu giữ giá trị tiêu chuẩn của SionBlue
        /// </summary>
        double[] sionblue_waveValue = new double[10];
        bool bWave;
        /// <summary>
        /// Lưu giữ giá trị đọc được từ file LC.csv
        /// Đếm số lượng pcs đã đọc
        /// </summary>
        int m_iLC = 0;
        //static string =[ItemInfo];
        static string ItemName = "";
        static string LotNo = "";
        static string Quantity = "68";
        //static string =[WorkInfo];
        static string PcsProcess = "0";
        static string Filename = @"C:\AutoMeasuringENDO\_1_20.xls";
        static string IsStop = "0";
        static string KizuValue = "25";
        static string UseCheckKizu = "0";
        static string SION_WaveValue0 = "2500";
        static string SION_WaveValue1 = "3500";
        static string SION_WaveValue2 = "600";
        static string SION_WaveValue3 = "3800";
        static string SION_WaveValue4 = "600";
        static string SION_WaveValue5 = "1000";
        static string SION_WaveValue6 = "800";
        static string SION_WaveValue7 = "1500";
        static string SIONBLUE_WaveValue0 = "2500";
        static string SIONBLUE_WaveValue1 = "4500";
        static string SIONBLUE_WaveValue2 = "600";
        static string SIONBLUE_WaveValue3 = "3000";
        static string SIONBLUE_WaveValue4 = "600";
        static string SIONBLUE_WaveValue5 = "1000";
        static string SIONBLUE_WaveValue6 = "200";
        static string SIONBLUE_WaveValue7 = "1200";
        static string PrinterName = "LP-S520";
        static string MachineNo = "2 (EB1-0010)";


        double duongkinhCore = 0;
        double duongkinhDiemUon = 0;
        /// <summary>
        /// Trục X nằm ngang
        /// </summary>
        int AXIS_X_MAXIMUM = 2600;
        /// <summary>
        /// Trục Y
        /// </summary>
        int AXIS_Y_MAXIMUM = 6000;
        int SERIES1_BODERWIDTH = 1;

       
        #endregion

        #region FUNCTION
        /// <summary>
        /// Đọc file ItemSetting_Final.csv và tách lấy ItemName
        /// </summary>
        public void loadDataItemName()
        {
            DataTable dtCsvSetting = clsDocFile.LoadTableCsvFile("ItemSetting_Final.csv");

            //   bindingSource1.DataSource = countries;
            cboItemName.DataSource = dtCsvSetting;
            cboItemName.DisplayMember = "ItemName";
            cboItemName.ValueMember = "ItemName";
        }

        /// <summary>
        /// Copy file từ thư mục FTP Upload tới thư mục của LotNO
        /// </summary>
        /// <param name="strLotITem"></param>
        public void BackupFileData(string strLotITem)
        {
            string strData = "C:\\FTPUpload\\Data.csv";
            string strData2 = "C:\\FTPUpload\\Data2.csv";
            string strItemSetting = "C:\\FTPUpload\\ItemSetting.csv";
            string strResult = "C:\\FTPUpload\\Result.csv";
            string strResult2 = "C:\\FTPUpload\\Result2.csv";
            string strStatus = "C:\\FTPUpload\\Status.csv";
            string strStatus2 = "C:\\FTPUpload\\Status2.csv";

            if (!Directory.Exists(strLotITem))
            {
                Directory.CreateDirectory(strLotITem);
            }
            string strData_DES = strLotITem + "\\" + "Data.csv";
            if (!File.Exists(strData_DES))
            {
                File.Copy(strData, strData_DES, true);
            }

            string strItemSetting_DES = strLotITem + "\\" + "ItemSetting.csv";
            if (!File.Exists(strItemSetting_DES))
            {
                File.Copy(strItemSetting, strItemSetting_DES, true);
            }
            string strResult_DES = strLotITem + "\\" + "Result.csv";
            if (!File.Exists(strResult_DES))
            {
                File.Copy(strResult, strResult_DES, true);
            }

            string strStatus_DES = strLotITem + "\\" + "Status.csv";
            if (!File.Exists(strStatus_DES))
            {
                File.Copy(strStatus, strStatus_DES, true);
            }

        }

        public void BackupFileData2(string strLotITem)
        {
            string strData = "C:\\FTPUpload\\Data.csv";
            string strData2 = "C:\\FTPUpload\\Data2.csv";
            string strItemSetting = "C:\\FTPUpload\\ItemSetting.csv";
            string strResult = "C:\\FTPUpload\\Result.csv";
            string strResult2 = "C:\\FTPUpload\\Result2.csv";
            string strStatus = "C:\\FTPUpload\\Status.csv";
            string strStatus2 = "C:\\FTPUpload\\Status2.csv";


            if (!Directory.Exists(strLotITem))
            {
                Directory.CreateDirectory(strLotITem);
            }
            string strData_DES = strLotITem + "\\" + "Data.csv";
            if (!File.Exists(strData_DES))
            {
                File.Copy(strData, strData_DES, true);
            }
            string strData2_DES = strLotITem + "\\" + "Data2.csv";
            if (!File.Exists(strData2_DES))
            {
                File.Copy(strData2, strData2_DES, true);
            }
            string strItemSetting_DES = strLotITem + "\\" + "ItemSetting.csv";
            if (!File.Exists(strItemSetting_DES))
            {
                File.Copy(strItemSetting, strItemSetting_DES, true);
            }
            string strResult_DES = strLotITem + "\\" + "Result.csv";
            if (!File.Exists(strResult_DES))
            {
                File.Copy(strResult, strResult_DES, true);
            }
            string strResult2_DES = strLotITem + "\\" + "Result2.csv";
            if (!File.Exists(strResult2_DES))
            {
                File.Copy(strResult2, strResult2_DES, true);
            }
            string strStatus_DES = strLotITem + "\\" + "Status.csv";
            if (!File.Exists(strStatus_DES))
            {
                File.Copy(strStatus, strStatus_DES, true);
            }
            string strStatus2_DES = strLotITem + "\\" + "Status2.csv";
            if (!File.Exists(strStatus2_DES))
            {
                File.Copy(strStatus2, strStatus2_DES, true);
            }

        }
        /// <summary>
        /// Ghi dữ liệu tính toán ra file Result.csv
        /// Nếu kết quả = true ghi 1 vào EM05000,1,1
        /// Nếu kết quả = false ghi 0 vào EM05000,0,1
        /// </summary>
        /// <param name="ketQua"></param>
        public void writeResult(bool ketQua)
        {
            //Ghi kết quả OK
            if (ketQua == true)
            {
                String path = @"C:\FTPUpload\Result.csv";
                List<String> lines = new List<String>();

                if (File.Exists(path))
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        String line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains(","))
                            {
                                String[] split = line.Split(',');

                                if (split[0].Contains("EM05000"))
                                {
                                    split[1] = "1";
                                    line = String.Join(",", split);
                                }
                                if (split[0].Contains("EM05001"))
                                {
                                    split[1] = "1";
                                    line = String.Join(",", split);
                                }
                                if (split[0].Contains("EM05002"))
                                {
                                    split[1] = "0";
                                    line = String.Join(",", split);
                                }
                                if (split[0].Contains("EM05014"))
                                {
                                    split[1] = "49"; //Phần này để test 
                                    line = String.Join(",", split);
                                }
                            }

                            lines.Add(line);
                        }
                    }

                    using (StreamWriter writer = new StreamWriter(path, false))
                    {
                        foreach (String line in lines)
                            writer.WriteLine(line);
                    }
                }
            }
            else
            {
                String path = @"C:\FTPUpload\Result.csv";
                List<String> lines = new List<String>();

                if (File.Exists(path))
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        String line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains(","))
                            {
                                String[] split = line.Split(',');

                                if (split[0].Contains("EM05000"))
                                {
                                    split[1] = "1";
                                    line = String.Join(",", split);
                                }
                                if (split[0].Contains("EM05001"))
                                {
                                    split[1] = "0";
                                    line = String.Join(",", split);
                                }
                                if (split[0].Contains("EM05002"))
                                {
                                    split[1] = "1";
                                    line = String.Join(",", split);
                                }
                                if (split[0].Contains("EM05014"))
                                {
                                    split[1] = "49"; //Phần này để test 
                                    line = String.Join(",", split);
                                }
                            }

                            lines.Add(line);
                        }
                    }

                    using (StreamWriter writer = new StreamWriter(path, false))
                    {
                        foreach (String line in lines)
                            writer.WriteLine(line);
                    }
                }
            }

        }
        /// <summary>
        /// Ghi dữ liệu tính toán ra file Result2.csv
        /// Nếu kết quả = true ghi 1 vào EM05100,1,1
        /// Nếu kết quả = true ghi 0 vào EM05100,0,1
        /// </summary>
        /// <param name="ketQua"></param>
        public void writeResult2(bool ketQua)
        {
            if (ketQua == true)
            {
                String path = @"C:\FTPUpload\Result2.csv";
                List<String> lines = new List<String>();

                if (File.Exists(path))
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        String line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains(","))
                            {
                                String[] split = line.Split(',');

                                if (split[0].Contains("EM05100"))
                                {
                                    split[1] = "1";
                                    line = String.Join(",", split);
                                }
                            }

                            lines.Add(line);
                        }
                    }

                    using (StreamWriter writer = new StreamWriter(path, false))
                    {
                        foreach (String line in lines)
                            writer.WriteLine(line);
                    }
                }
            }
            else
            {
                String path = @"C:\FTPUpload\Result2.csv";
                List<String> lines = new List<String>();

                if (File.Exists(path))
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        String line;

                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Contains(","))
                            {
                                String[] split = line.Split(',');

                                if (split[0].Contains("EM05100"))
                                {
                                    split[1] = "0";
                                    line = String.Join(",", split);
                                }
                            }

                            lines.Add(line);
                        }
                    }

                    using (StreamWriter writer = new StreamWriter(path, false))
                    {
                        foreach (String line in lines)
                            writer.WriteLine(line);
                    }
                }
            }

        }
        /// <summary>
        /// Xóa tất cả các giá trị trên form
        /// </summary>
        public void ClearResult()
        {
            txtD1_1.Invoke((MethodInvoker)(() => txtD1_1.Text = ""));
            txtD1_2.Invoke((MethodInvoker)(() => txtD1_2.Text = ""));
            txtD1_3.Invoke((MethodInvoker)(() => txtD1_3.Text = ""));
            txtD1_4.Invoke((MethodInvoker)(() => txtD1_4.Text = ""));
            txtD2_1.Invoke((MethodInvoker)(() => txtD2_1.Text = ""));
            txtD2_2.Invoke((MethodInvoker)(() => txtD2_2.Text = ""));
            txtD2_3.Invoke((MethodInvoker)(() => txtD2_3.Text = ""));
            txtD2_4.Invoke((MethodInvoker)(() => txtD2_4.Text = ""));

            txtL1_1.Invoke((MethodInvoker)(() => txtL1_1.Text = ""));
            txtL1_2.Invoke((MethodInvoker)(() => txtL1_2.Text = ""));
            txtL1_3.Invoke((MethodInvoker)(() => txtL1_3.Text = ""));
            txtL1_4.Invoke((MethodInvoker)(() => txtL1_4.Text = ""));

            txtL2_1.Invoke((MethodInvoker)(() => txtL2_1.Text = ""));
            txtL2_2.Invoke((MethodInvoker)(() => txtL2_2.Text = ""));
            txtL2_3.Invoke((MethodInvoker)(() => txtL2_3.Text = ""));
            txtL2_4.Invoke((MethodInvoker)(() => txtL2_4.Text = ""));

            txtJudge_1.Invoke((MethodInvoker)(() => txtJudge_1.Text = ""));
            txtJudge_2.Invoke((MethodInvoker)(() => txtJudge_2.Text = ""));
            txtJudge_3.Invoke((MethodInvoker)(() => txtJudge_3.Text = ""));
            txtJudge_4.Invoke((MethodInvoker)(() => txtJudge_4.Text = ""));
        }

        /// <summary>
        /// Tính toán giá giá trị của file Data.csv và đưa ra kết quả
        /// </summary>
        /// <param name="pathData1">Đường dẫn file</param>
        /// <returns></returns>
        public bool tinhGiaTriFileData1(string pathData1)
        {
            bool ketQua = true;
            //thực hiện tính toán
            return ketQua;
        }

        /// <summary>
        /// Tính toán giá giá trị của file Data2.csv và đưa ra kết quả
        /// </summary>
        /// <param name="pathData2">Đường dẫn file</param>
        /// <returns></returns>
        public bool tinhGiaTriFileData2(string pathData2)
        {
            bool ketQua = true;
            //thực hiện tính toán
            return ketQua;
        }
        public int tachChuoi(string str)
        {
            int result = 0;
            try
            {
                string[] sp = str.Split(',');
                result = int.Parse(sp[1].ToString());

            }
            catch (System.Exception)
            {


            }
            return result;
        }
        /// <summary>
        /// Hàm kiểm tra độ dài
        /// Xác định vị trí có Đường kính đúng tiêu chuẩn hay không.
        /// </summary>
        /// <param name="a"> mảng chứa giá trị đọc vào từ file Data.csv</param>
        /// <param name="temp"> temp20 = m_iSTT % 20; là số thứ tự pcs %20. (số lượng pcs đã chạy) </param>
        /// <returns></returns>
        public bool MeasureLength(int[] a, int temp)
        {
            p.Clone();

            bool result = true;
            //Gắn các giá trị của mảng lưu đường kính về 0
            for (int j = 0; j < 7; j++)
                d_All[temp, j] = 0;

            for (int j = 0; j < 15; j++)
                p[j] = 0;
            for (int j = 0; j < 8; j++)
                x[j] = 0;
            //Gắn các giá trị của mảng lưu giá trị độ dài về 0
            for (int k = 0; k < 8; k++)
            {
                l[k] = 0;
            }

            //Tìm đến vị trí có đường kính=0 và gán thứ tự CÒN TÌM ĐƯỢC giá trị 0 vào ô nhớ p[0]
            //Tìm P[0]
            int i = 2999;
            while (a[i] == 0)
            {
                p[0] = i;
                i--;
            }

            if (FindP(a, temp) == false)
                result = false;
            return result;
        }

        #region FUNCTIONs tìm điểm giao cắt để tính toán độ dài Plopy
        /// <summary>
        /// Tính trung bình cộng Đường Kính của TẤT CẢ các điểm bắt đầu đến điểm kết thúc
        /// Từ Begin: Vị trí lớn
        /// End : Vị trí nhỏ
        /// </summary>
        /// <param name="a"></param>
        /// <param name="begin">Vị trí bắt đầu Số lớn</param>
        /// <param name="end">Vị trí kết thúc. Số nhỏ</param>
        /// <returns></returns>
        public double Average(int[] a, int begin, int end)
        {
            if ((begin < 0) || (begin >= MAX_POINT) || (end < 0) || (end >= MAX_POINT) || (begin == end))
                return 0;
            double res, total = 0;
            for (int i = begin; i >= end; i--)
                total = total + a[i];
            res = total / (double)(begin - end + 1);
            return res;
        }
        /// <summary>
        /// Tính trung bình cộng các điểm trên trục X từ vị trí s đến vị trí e => lấy gia điểm trung bình trên mảng a
        /// </summary>
        /// <param name="s">Vị trí bắt đầu</param>
        /// <param name="e">vị trí kết thúc</param>
        /// <returns></returns>
        public double averageX(int s, int e)
        {
            if ((s < 0) || (s >= 3000) || (e < 0) || (e >= 3000) || (s == e))
                return 0;
            double sum = 0;
            for (int i = s; i >= e; i--)
                sum += i;
            double result;
            result = sum / Convert.ToDouble(s - e + 1);
            return result;
        }

        /// <summary>
        /// Sxx là tổng của các bình phương của sự khác biệt giữa mỗi xx và giá trị xx trung bình.
        /// </summary>
        /// <param name="s">Vị trí Max _ Ví dụ với trường hợp đo điểm D4: s= p[2]</param>
        /// <param name="e">Vị trí Min _ Ví dụ với trường hợp đo điểm D4: e= p[3]</param>
        /// <param name="tbX"> Giá trị trung bình cộng vị trí từ p[2] đến p[3] </param>
        /// <returns></returns>
        public double ConsiderSxx(int s, int e, double tbX)
        {
            if ((s < 0) || (s >= 3000) || (e < 0) || (e >= 3000) || (s == e))
                return 0;
            double kq = 0;
            for (int i = s; i >= e; i--)
                kq += ((double)i - tbX) * ((double)i - tbX);
            return kq;
        }

        /// <summary>
        /// Sxy là tổng khác biệt giữa XX phương tiện của nó và sự khác biệt giữa YY và giá trị trung bình của nó.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <param name="tbX"></param>
        /// <param name="tbY"></param>
        /// <returns></returns>
        public double ConsiderSxy(int[] a, int s, int e, double tbX, double tbY)
        {
            if ((s < 0) || (s >= 3000) || (e < 0) || (e >= 3000) || (s == e))
                return 0;
            double kq = 0;
            for (int i = s; i >= e; i--)
                kq += ((double)i - tbX) * (a[i] - tbY);
            return kq;
        }

        /// <summary>
        /// Tính trung bình cộng của TẤT CẢ các điểm bắt đầu đến điểm kết thúc
        /// </summary>
        /// <param name="a"></param>
        /// <param name="begin">Vị trí bắt đầu</param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double Average(List<int> listData, int begin, int end)
        {
            if ((begin < 0) || (begin >= 1700) || (end < 0) || (end >= 1700) || (begin == end))
                return 0;
            double res, total = 0;
            for (int i = begin; i >= end; i--)
                total = total + listData[i];
            res = total / (double)(begin - end + 1);
            return res;
        }


        public double ConsiderSxy(List<int> listData, int s, int e, double tbX, double tbY)
        {
            if ((s < 0) || (s >= 3000) || (e < 0) || (e >= 3000) || (s == e))
                return 0;
            double kq = 0;
            for (int i = s; i >= e; i--)
                kq += ((double)i - tbX) * (listData[i] - tbY);
            return kq;
        }
        #endregion
        /// <summary>
        /// Kiểm tra Vị trí đo có đường kính đúng như bản vẽ hay không
        /// </summary>
        /// <param name="a">Mảng giá trị lấy từ Data.csv</param>
        /// <param name="temp"> = Thứ tự của pcs đang chạy % 20</param>
        /// <returns>Trả về True hoặc False. Là kết quả tính toàn đường kính</returns>
        public bool FindP(int[] a, int temp)
        {
            //for (int i = 0; i < a.Length; i++)
            //{
            //   a[i]=(a[i]/100)*100;
            //}
            //y0; giá trị đường kính trung bình nằm giữa 2 điểm đo.
            double beta = 0, alpha = 0, y0 = 0, tbX = 0, tbY = 0, Sxx = 0, Sxy = 0;
            double beta2 = 0, alpha2 = 0, tbX2 = 0, tbY2 = 0, Sxx2 = 0, Sxy2 = 0;
            double beta3 = 0, alpha3 = 0, tbX3 = 0, tbY3 = 0, Sxx3 = 0, Sxy3 = 0;
            double beta4 = 0, alpha4 = 0, tbX4 = 0, tbY4 = 0, Sxx4 = 0, Sxy4 = 0;
            double tb, k;
            int x1, x2, y1, y2;
            int[] b = new int[7];
            int[] w = new int[7];
            bKizu = true;
            bWave = true;
            maxDistance = 0;
            maxDisPos = 0;
            int REPEAT = 1;
           


            switch (ItemName)
            {
                case "SAMPLE-TEST":
                   
                        //ĐIỂM ĐO D4
                        p[1] = p[0] - (m_iPara[10] - 2) * 5;
                        y0 = Average(a, p[0] - 20, p[1]) + clsBu.y0_DoanL7;
                        for (int i = 0; i <= 1; i++)
                        {
                            if (i == 0)
                            {
                                p[2] = p[0] - (m_iPara[9] * 5 + 10);
                                p[3] = p[2] - (m_iPara[12] - 3) * 5;
                            }
                            else
                            {
                                p[2] = x[0] - 10;
                                p[3] = p[2] - m_iPara[12] * 5;
                            }
                            //Tính trung bình cộng của tất cả VỊ TRÍ trong khoảng p[2] đến p[3]. Test: trung bình cộng chạy từ 2608 đến 2488 
                            //tbX=(p[2]+p[3])/
                            tbX = averageX(p[2], p[3]);
                            //Tính trung bình cộng đường kính của tất cả các điểm từ điểm p[2] đến p[3]
                            tbY = Average(a, p[2], p[3]);
                            //Tính trung bình cộng giá trị trên tập X
                            Sxx = ConsiderSxx(p[2], p[3], tbX);
                            double SxxTTT = ConsiderSxx(p[2], p[3], tbX);
                            //Tính trung bình cộng giá trị đường kính trên tập Y
                            Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                            double SxyTTT = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                            if ((Sxx != 0) && (Sxy != 0))
                                beta = Sxy / Sxx; //Tìm hệ số góc của phương trình đường thẳng đoạn p[2], p[3]
                            else return false;

                            alpha = tbY - beta * tbX; //b=y-a*x
                            x[0] = (int)Math.Round((y0 - alpha) / beta);
                        }

                        //Đường kính D4

                        x[0] = x[0] - clsBu.X0 - 2;
                        //Đoạn L7                  
                        l[0] = (p[0] - x[0]) * 0.2;
                        //Cộng thô bạo dạng hiếp dâm giá trị.
                        //Ở đây chúng tôi không làm thế....nữa
                        // l[0] = l[0] + 1;
                        //ĐƯỜNG KÍNH
                        DiameterTrungBinh = (int)Math.Round(Average(a, p[0] - 1, p[0] - 5));
                        d_All[temp, 0] = (int)Math.Round(Average(a, p[0] - 2, p[0] - 4)) + clsBu.dk1;
                        d_All[temp, 1] = (int)Math.Round(Average(a, x[0] + 10, x[0]) - 4) + clsBu.dk2;
                     
                    break;
              
                case "PR11CA011-0":
                case "11CA011-0-VN":
                case "PR11CA011-0-US":
                case "PR13CA021-0":
                case "13CA021-0-VN":
                case "KPR10CA011-0":
                case "K10CA011-0-VN":
                case "10CA011-0-VN":
                case "PR10CA007-0-VN":
                case "KPR10CA007-0":
                case "K10CA007-0-VN":
                case "PR13CA018-0-VN":
                case "K13CA018-0-VN":
                case "13CA018-0-VN":
                case "KPR13CA018-0":
                case "09CA002-0-VN":
                case "PR15CA001-0":
                case "15CA001-0-VN":
                case "KPR12CA016-0":
                case "K12CA016-0-VN":
                case "12CA016-0-VN":
                case "KPR12CA022-0":
                case "K12CA022-0-VN":
                case "12CA022-0-VN":
                case "KPR12CA002-A":
                case "K12CA002-A-VN":
                case "12CA002-A-VN":
                case "KPR12CA009-0":
                case "K12CA009-0-VN":
                case "12CA009-0-VN":
                case "KPR7CA006-E":
                case "K7CA006-E-VN":
                case "7CA006-E-VN":
                case "KPR4VC006-0":
                case "K4VC006-0-VN":
                case "4VC006-0-VN":
                case "17CA010-0-VN":
                case "KPR10CA005-0":
                case "K10CA005-0-VN":
                case "10CA005-0-VN":
                case "KPR09CA006-0":
                case "K09CA006-0-VN":
                case "09CA006-0-VN":
                case "KPR09VC003-0":
                case "K09VC003-0-VN":
                case "09VC003-0-VN":
                case "KPR1CA001-B":
                case "K1CA001-B-VN":
                case "1CA001-B-VN":
                case "KPR8CA001-B":
                case "K8CA001-B-VN":
                case "8CA001-B-VN":
                case "KPR5CA004-C":
                case "K5CA004-C-VN":
                case "5CA004-C-VN":
                case "KPR0CA002-A":
                case "K0CA002-A-VN":
                case "0CA002-A-VN":
                case "09CA001-0-VN":
                case "KPR7CA016-D":
                case "K7CA016-D-VN":
                case "7CA016-D-VN":
                case "1CA004-A-VN":
                case "10CA014-0-VN":
                case "07CA001-0-VN":
                case "KPR6VC003-B":
                case "K6VC003-B-VN":
                case "6VC003-B-VN":
                case "KPR4CA004-B":
                case "K4CA004-B-VN":
                case "4CA004-B-VN":
                case "KPR6CA007-B":
                case "K6CA007-B-VN":
                case "6CA007-B-VN":
                case "KPR07CA004-B":
                case "K07CA004-B-VN":
                case "07CA004-B-VN":
                case "PR09VC010-0":
                case "09VC010-0-VN":
                case "PR10VC005-0":
                case "10VC005-0-VN":
                case "PR10VC008-0":
                case "10VC008-0-VN":
                case "PR16VC009-0":
                case "16VC009-0-VN":
                case "PR11VC008-0":
                case "11VC008-0-VN":
                case "KPR09VC013-0":
                case "PR13VC016-0":
                case "13VC016-0-VN":
                case "PR12VC002-0":
                case "12VC002-0-VN":
                case "PR4VC013-A":
                case "4VC013-A-VN":
                case "7CA027-F-VN":
                case "K7CA027-F-VN":
                case "KPR7CA027-F":
                case "7CA024-F-VN":
                case "K7CA024-F-VN":
                case "KPR7CA024-F":
                case "K7CA023-F-VN":
                case "7CA023-F-VN":
                case "KPR7CA023-F":
                case "BPR13CA021-AUS":
                case "16VC023-0":
                case "16VC021-0":
                case "16VC024-0":
                case "16VC020-0":
                case "16VC022-0":
                case "18VC019-0":
                case "18VC020-0":
                case "18VC021-0":
                case "18VC022-0":
                case "18VC023-0":
                case "16VC025-0":
                case "KPR11VC012-0":
                case "PR16VC014-0":
                case "PR16VC016-0":
                case "PR16VC017-0":
                case "BPR13CA021-A":
                case "BPR11CA011-AUS":
                case "PR18VC014-0":
                case "PR16VC015-0":
                case "PR19CA055-0":
                case "PR19CA056-0":
                case "PR19CA057-0":
                case "PR19CA059-0":
                case "3CY003-E":
                case "PR16VC018-0":
                case "KPR5CA002-B":
                case "KPR07CA004-B-T":
                case "KPR12CA016-0-T":
                case "KPR4CA004-B-T":
                case "KPR7CA006-E-T":
                case "KPR12CA002-A-T":
                case "KPR12CA009-0-T":
                case "BPR20CA085-0":
                case "BPR20CA085-0US":
                case "BPR20CA086-0":
                case "BPR20CA086-0US":
                case "KPR13CA007-0-T":
                case "KPR12CA022-0-T":
                case "KPR7CA016-D-T":
                case "KPR10CA011-0-T":
                case "KPR13CA018-0-T":
                case "KPR6CA007-B-T":
                case "KPR7CA023-F-T":
                case "KPR7CA027-F-T":
                case "KPR8CA001-B-T":
                case "KPR09CA006-0-T":
                case "BPR20CA086-0-T":
                case "BPR20CA053-0":
                case "KPR13CA007-0":
                case "KPR07CA009-B":
                case "KPR20VC081-0":
                case "KPR20VC078-0":
                case "KPR2CA001-0":
                case "BPR20CA011-001":
                case "KPR14VC032-0":
                case "KPR14VC034-0":
                case "KPR14VC028-0":
                case "5CA005":
                case "6VC004":
                case "4VC007":
                case "4VC009-B-VN":
                case "08VC019-0-VN":
                case "10VC012-0-VN":
                case "11VC006-A":
                case "13VC016-0":
                case "12CA001-0":
                case "10CA014-0":
                case "12CA015-0":
                case "15VC006-0-VN":
                case "08VC020-0":
                case "16VC007-0-VN":
                case "13CA017-0":
                case "12CA008-0":
                case "PR16VC008-0":
                case "PR11VC006-A":
                case "PR12CA015-0":
                case "PR08VC019-0":
                case "PR10VC012-0":
                case "PR15VC008-0":
                case "PR17VC009-0":
                case "PR19CA044-0":
                case "PR07CA001-0":
                case "PR10CA014-0":
                case "PR4VC009-B":
                case "15VC008-0-VN":
                case "PR12CA008-0":
                case "PR12CA001-0":
                case "PR15VC006-0":
                case "10CA006-0":
                case "17VC009-0":
                case "PR21VC026-0":
                case "PR21VC027-0":
                case "PR21VC028-0":
                case "KPR10CA007-0-T":
                case "BPR20CA086-0US-T":
                case "BPR20CA085-0 -T":
                case "BPR20CA053-0-T":
                case "PR11VC008-0-T":
                case "PR09VC010-0-T":
                case "PR4VC013-A-T":
                case "KPR0CA002-A-T":
                case "PR12VC002-0-T":
                case "PR10VC005-0-T":
                case "KPR1CA001-B-T":
                case "KPR09VC003-0-T":
                case "PR16VC008-0-T":
                case "PR13VC016-0-T":
                case "PR10VC008-0-T":
                case "PR16VC009-0-T":
                case "PR10VC012-0-T":
                case "PR15VC008-0-T":
                case "PR17VC009-0-T":
                case "PR15VC006-0-T":
                case "PR21VC026-0-T":
                case "PR21VC027-0-T":
                case "PR21VC028-0-T":
                case "KPR07CA009-B-T":
                case "KPR5CA004-C-T":
                case "KPR5CA002-B-T":
                case "KPR11VC012-0-T":
                case "KPR4VC006-0-T":
                case "KPR6VC003-B-T":
                case "PR11VC006-A-T":
                case "PR08VC019-0-T":
                case "PR4VC009-B-T":
                    p[0] = p[0] - 1;
                    //Điểm Uốn Đầu Tiên và độ dài
                    p[1] = p[0] - (m_iPara[10] - 3) * 5;
                    y0 = Average(a, p[0] - 20, p[1]);
                     

                    for (int i = 0; i <= 1; i++)
                    {
                        if (i == 0)
                        { 
                            p[2] = p[0] - (m_iPara[9] +1) * 5; //21.Mar.2023. Vị trí P2 tìm từ ngoài vị trí Max tiêu chuẩn luôn
                            p[3] = p[2] - (m_iPara[12] - 2) * 5;
                        }
                        else
                        {
                            p[2] = x[0] - 10; //21.Mar.2023. Nắm 1 mm <=> 10 điểm
                            p[3] = p[2] - (m_iPara[12]-1) * 5;
                        }
                        //Tính trung bình cộng của tất cả VỊ TRÍ trong khoảng p[2] đến p[3]. Test: trung bình cộng chạy từ 2608 đến 2488 
                        //tbX=(p[2]+p[3])/
                        tbX = averageX(p[2], p[3]);
                        //Tính trung bình cộng đường kính của tất cả các điểm từ điểm p[2] đến p[3]
                        tbY = Average(a, p[2], p[3]);
                        //Tính trung bình cộng giá trị trên tập X
                        Sxx = ConsiderSxx(p[2], p[3], tbX);
                        double SxxTTT = ConsiderSxx(p[2], p[3], tbX);
                        //Tính trung bình cộng giá trị đường kính trên tập Y
                        Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                        double SxyTTT = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                        if ((Sxx != 0) && (Sxy != 0))
                            beta = Sxy / Sxx; //Tìm hệ số góc của phương trình đường thẳng đoạn p[2], p[3]
                        else return false;

                        alpha = tbY - beta * tbX; //b=y-a*x
                        x[0] = (int)Math.Round((y0 - alpha) / beta);
                    }

                     
                    x[0] = x[0] - clsBu.X0;
                    //Đoạn core ngang L1                  
                    l[0] = (p[0] - x[0]) * 0.2;
                    //Cộng thô bạo dạng hiếp dâm giá trị.
                    //Ở đây chúng tôi không làm thế....nữa
                    // l[0] = l[0] + 1;
                    //ĐƯỜNG KÍNH 
                    DiameterTrungBinh = (int)Math.Round(Average(a, p[0] - 5, p[0] - 13));
                    d_All[temp, 0] = (int)Math.Round(Average(a, p[0] - 3, p[0] - 8)) + clsBu.dk1;

                    // clsBu.dk1 Bù sai số đường kính của thiết bị đo

                    // d_All[temp, 1] = (int)Math.Round(Average(a, x[0] + 10, x[0]+2)) + clsBu.dk2; 
                    //clsBu.dk1; Bù sai số đường kính của thiết bị đo Có thể -30 -> -40

                    // p[0] - 90, p[0] - 100 Thực tế đo trên Line người ta đặt sẵn thước 20 mm và đo bằng micrometer.
                    //  p[0] - 100 (điểm) <=> 20 mm
                    //  clsBu.dk2 Bù sai số đường kính của thiết bị đo. Có thể -30 -> -40
                   // d_All[temp, 1] = (int)Math.Round(Average(a, p[0] - 90, p[0] - 100)) + clsBu.dk2; 
                   //update 03.Jun.2023. By Toàn
                    d_All[temp, 1] = (int)Math.Round(Average(a, x[0] +30, x[0]+10)) + clsBu.dk2; 
                   
                    int test11 = (int)Math.Round(Average(a, p[0] - 90, p[0] - 100));
                    int vtd1 = p[0] - 90;
                    int vtd2 = p[0] - 100;

                    //Tìm độ dài đoạn Vát L2
                    //Duong kinh D4(min)
                    //d_All[temp, 3] = (int)Math.Round(Average(a, p[0] - 50, x[0])) + clsBu.dk4;
                    //l[7] = ((p[0] - x[0]) * 0.2);
                    //l[7] = l[7] + 1; 

                    for (int i = 0; i <= REPEAT; i++)
                    {
                        if (i == 0)
                        { 
                            p[4] = x[0] - (m_iPara[11]+5) * 5;   //Tính từ điểm vượt quá tiêu chuẩn và tăng thêm 3 mm
                            p[5] = p[4] - 60; //21.Mar.23. Dò từ 3 mm <=>60 điểm //Trước đây p[5] = p[4] - (m_iPara[12]+15) * 5; => không có nhiều nghĩa lắm nhỉ
                        }
                        else
                        {
                            p[4] = x[1]; 
                            p[5] = p[5] = p[4] - 60; //21.Mar.23. Dò từ 3 mm <=>60 điểm
                        }
                        tbX2 = averageX(p[4], p[5]);
                        tbY2 = Average(a, p[4], p[5]);
                        Sxx2 = ConsiderSxx(p[4], p[5], tbX2);
                        Sxy2 = ConsiderSxy(a, p[4], p[5], tbX2, tbY2);
                        if ((Sxx2 != 0) && (Sxy2 != 0))
                        {
                            beta2 = Sxy2 / Sxx2;
                        }
                        else
                            return false;

                        alpha2 = tbY2 - beta2 * tbX2;


                        if (beta != beta2)
                            x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta));
                        else
                            return false;
                        
                    }
                    x[1] = x[1] - clsBu.X2;
                    //Duong kinh D3 không dùng
                    //d_All[temp, 2] = (int)Math.Round(Average(a, x[1], x[2])) + clsBu.dk3;
                    
                    l[1] = ((x[0] - x[1]) * 0.2);

                    break;
                default:
                    break;
             }
             

            txtL1_1.Invoke((MethodInvoker)(() => txtL1_1.Text = l[0].ToString()));
            txtL2_1.Invoke((MethodInvoker)(() => txtL2_1.Text = l[1].ToString()));



            return true;
        }
     

        #endregion

        #region EVENT
        /// <summary>
        /// Khi bắt đầu Form Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            CreateFolder();
            cboPrintMode.SelectedIndex = 0;
            string strSection = "";
            string strItem = "";
            int t = 0;
            string strW = "", str = "";

            //Load ra các tiêu chuẩn check của item
            loadDataItemName();
            //Load dữ liệu trong file Data.ini
            loadFileData_dat();
            // Tải cấu hình các biến trong phần mềm
            loadConfigFile();
            resetVariable();

            firstLoadChart(chartCheck);

            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
              
        }

        #region VẼ Đồ thị

        /// <summary>
        /// Lần đầu load đồ thị ra
        /// </summary>
        /// <param name="chartCheck"></param>
        public void firstLoadChart(Chart chartCheck)
        {


            chartCheck.BorderlineColor = System.Drawing.Color.Black;
            chartCheck.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartCheck.ChartAreas["ChartArea1"].AxisX.Interval = 100D;// 100D;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MajorGrid.Interval = 100D;//100D;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineWidth = 2; //1;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineColor = System.Drawing.Color.Black;

            //Trục Ngang
            chartCheck.ChartAreas["ChartArea1"].AxisX.Maximum = int.Parse(numericUpDownWidthChart.Value.ToString());// AXIS_X_MAXIMUM;
            chartCheck.ChartAreas["ChartArea1"].AxisX.Minimum = 0D;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MinorGrid.Enabled = true;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MinorGrid.Interval = 20D;//20D
            chartCheck.ChartAreas["ChartArea1"].AxisX.MinorGrid.LineWidth = 1;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MinorGrid.LineColor = System.Drawing.Color.LightGray;
            //chartCheck.ChartAreas["ChartArea1"].AxisX.LabelStyle.Enabled = false; //Ẩn giá trị hiển thị trên trục X

            chartCheck.ChartAreas["ChartArea1"].AxisX.MajorTickMark.Enabled = true;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MajorTickMark.Interval = 20D;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MajorTickMark.LineColor = System.Drawing.Color.Black;

            chartCheck.ChartAreas["ChartArea1"].AxisX.MajorTickMark.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MajorTickMark.LineWidth = 1;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MajorTickMark.TickMarkStyle = TickMarkStyle.InsideArea;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MinorTickMark.Enabled = true;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MinorTickMark.Interval = 20;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MinorTickMark.LineColor = System.Drawing.Color.Black;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MinorTickMark.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MinorTickMark.LineWidth = 1;
            chartCheck.ChartAreas["ChartArea1"].AxisX.MinorTickMark.TickMarkStyle = TickMarkStyle.InsideArea;


            chartCheck.ChartAreas["ChartArea1"].AxisX.Title = "Length 100 = 20 [mm]";
            chartCheck.ChartAreas["ChartArea1"].AxisX.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            chartCheck.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "{0:0}";

            chartCheck.ChartAreas["ChartArea1"].AxisY.Maximum = int.Parse(numericUpDownHeightChart.Value.ToString());// AXIS_Y_MAXIMUM;
            chartCheck.ChartAreas["ChartArea1"].AxisY.Interval = 1000D;//50D;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = true;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineWidth = 1;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = System.Drawing.Color.LightBlue;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MajorGrid.Interval = 1000D;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MajorTickMark.TickMarkStyle = TickMarkStyle.InsideArea;

            chartCheck.ChartAreas["ChartArea1"].AxisY.Minimum = 0D;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MinorGrid.Enabled = true;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MinorGrid.Interval = 1000D;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MinorGrid.LineWidth = 1;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MinorGrid.LineColor = System.Drawing.Color.LightBlue;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MinorTickMark.Enabled = true;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MinorTickMark.Interval = 1000;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MinorTickMark.LineColor = System.Drawing.Color.Gray;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MinorTickMark.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartCheck.ChartAreas["ChartArea1"].AxisY.MinorTickMark.LineWidth = 1;

            chartCheck.ChartAreas["ChartArea1"].AxisY.MinorTickMark.TickMarkStyle = TickMarkStyle.InsideArea;

            chartCheck.ChartAreas["ChartArea1"].AxisY.Title = "O.D [mm]";
            chartCheck.ChartAreas["ChartArea1"].AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            chartCheck.ChartAreas["ChartArea1"].AxisY.LabelStyle.Format = "{0:0}";
          
            chartCheck.ChartAreas["ChartArea1"].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartCheck.ChartAreas["ChartArea1"].BorderWidth = 2;
            chartCheck.Series["Series1"].ChartType = SeriesChartType.Spline;

            chartCheck.Series["Series1"].Color = System.Drawing.Color.Red;
            chartCheck.Series["Series1"].BorderWidth = SERIES1_BODERWIDTH;
            chartCheck.Series["Series1"].BorderDashStyle = ChartDashStyle.Solid;


            // set the callout line width of 1 pixel 
            chartCheck.Series["Series1"].SmartLabelStyle.CalloutLineWidth = 1;

            chartCheck.Series["Series1"].SmartLabelStyle.Enabled = true;

            chartCheck.Series["Series1"].SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;

            chartCheck.Series["Series1"].SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.Arrow;

            chartCheck.Series["Series1"].SmartLabelStyle.CalloutLineColor = System.Drawing.Color.Transparent;

            chartCheck.Series["Series1"].SmartLabelStyle.CalloutLineWidth = 0;

            chartCheck.Series["Series1"].SmartLabelStyle.CalloutStyle = LabelCalloutStyle.None;

        }
        /// <summary>
        /// Vẽ đồ thị
        /// </summary>
        /// <param name="listData"></param>
        public void UpdateChart(List<int> listData)
        {
            try
            {
                //BEGIN: Phần định đạng của đồ thị
                MethodInvoker action = delegate
                {
                    foreach (var series in chartCheck.Series)
                    {
                        chartCheck.Series[series.Name].Points.Clear();

                    }

                    int totalData = listData.Count();
                    for (int i = 0; i < totalData; i++)
                    {

                        int conchimnon = listData[i];
                        int aa = i + 1;
                      
                        chartCheck.Series["Series1"].Points.AddXY(aa, conchimnon);
                    }

                    //END: Hết biên dạng profile
                };

                chartCheck.Invoke(action);



                ////Vị trí hiển thị thước nằm ngang đo độ dài
                int vitriDuongKinhTrungBinh = 4000;//DiameterTrungBinh + 800;


                //chartCheck.Invoke(action);

                chartCheck.Invoke(new Action(() =>
                {
                   
                    #region Thước đo đoạn L1 từ X[0] - P[0]

                    //Dùng để xác định vị trí sẽ hiển thị thước đo Theo trục Y và giá trị độ dài
                    int vitri_Y_L1 = vitriDuongKinhTrungBinh + 300;
                    int vitri_X_L1 = (x[0] + p[0]) / 2;

                    //BEGIN:  //THước đo độ dài đoạn L 1  (Tử P0-X0) 
                    chartCheck.Series["ThuocDoL1"].Color = System.Drawing.Color.Blue;
                    chartCheck.Series["ThuocDoL1"].BorderWidth = 1;
                    int ma = 0;
                    for (int i = x[0]; i <= p[0]; i++)
                    {

                        chartCheck.Series["ThuocDoL1"].Points.AddXY(i, vitri_Y_L1);
                    }

                    //END: Vẽ độ dài

                    //BEGIN: Hiển thị giá trị độ dài  nẳm ở khoảng giữa X0 và P0
                   
                    chartCheck.Series["GiaTriL1"].Points.AddXY(vitri_X_L1, vitri_Y_L1);
                    chartCheck.Series["GiaTriL1"].Label = l[0].ToString() + " mm";
                    chartCheck.Series["GiaTriL1"].IsValueShownAsLabel = true;
                    chartCheck.Series["GiaTriL1"].Font = new System.Drawing.Font("Microsoft Sans Serif", 16.0f, FontStyle.Regular);
                    chartCheck.Series["GiaTriL1"].BackSecondaryColor = System.Drawing.Color.FromArgb(0, 0, 0);
                    chartCheck.Series["GiaTriL1"].LabelForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
                    //END: Hiển thị giá trị độ dài

                    //BEGIN: Đánh dấu độ dài L1_1 
                    chartCheck.Series["danhDauL1_1"].Color = System.Drawing.Color.Blue;
                    chartCheck.Series["danhDauL1_1"].BorderWidth = 1;
                    chartCheck.Series["danhDauL1_1"].ChartType = SeriesChartType.StepLine;
                    for (int i = vitri_Y_L1 - 100; i <= vitri_Y_L1 + 100; i++)
                    {
                        chartCheck.Series["danhDauL1_1"].Points.AddXY(x[0], i);

                    }

                    //END: Đánh dấu độ dài 1 L1_1

                    //BEGIN: Đánh dấu độ dài L1_2 
                    chartCheck.Series["danhDauL1_2"].Color = System.Drawing.Color.Blue;
                    chartCheck.Series["danhDauL1_2"].BorderWidth = 1;
                    chartCheck.Series["danhDauL1_2"].ChartType = SeriesChartType.StepLine;

                    for (int i = vitri_Y_L1 - 100; i <= vitri_Y_L1 + 100; i++)
                    {
                        chartCheck.Series["danhDauL1_2"].Points.AddXY(p[0], i);
                    }
                    //END: Đánh dấu độ dài L1_ 2
                    #endregion



                    #region Thước đo đoạn L2

                    //BEGIN: Hiển thị giá trị độ dài L1 nẳm ở khoảng giữa X0 và P0
                    int vitriYL2 = vitriDuongKinhTrungBinh + 300;
                    int vitriXL2 = (x[1]+x[0])/2;

                    //int conchimnon2 = vitriDuongKinhTrungBinh - 200;
                    chartCheck.Series["GiaTriL2"].Points.AddXY(vitriXL2, vitriYL2);
                    chartCheck.Series["GiaTriL2"].Label = Math.Round(l[1]).ToString() + " mm";
                    chartCheck.Series["GiaTriL2"].IsValueShownAsLabel = true;
                    chartCheck.Series["GiaTriL2"].Font = new System.Drawing.Font("Microsoft Sans Serif", 16.0f, FontStyle.Regular);

                    chartCheck.Series["GiaTriL2"].BackSecondaryColor = System.Drawing.Color.FromArgb(0, 0, 0);
                    chartCheck.Series["GiaTriL2"].LabelForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
                    //END: Hiển thị giá trị độ dài



                    //THước đo độ dài đoạn L2 (Tử X0-X1) 
                    //BEGIN: Vẽ Đường thẳng thể hiện độ dài của profile ThuocDoL2
                    chartCheck.Series["ThuocDoL2"].Color = System.Drawing.Color.Blue;
                    chartCheck.Series["ThuocDoL2"].ChartType = SeriesChartType.Line;
                    chartCheck.Series["ThuocDoL2"].BorderWidth = 1;

                    for (int i = x[1]; i <= x[0]; i++)
                    {
                      
                        chartCheck.Series["ThuocDoL2"].Points.AddXY(i, vitriYL2);
                    }

                    //END: Vẽ độ dài

                    //BEGIN: Đánh dấu của Thước độ dài L2_1 
                    chartCheck.Series["danhDauL2_1"].Color = System.Drawing.Color.Blue;
                    chartCheck.Series["danhDauL2_1"].BorderWidth = 1;
                    chartCheck.Series["danhDauL2_1"].ChartType = SeriesChartType.StepLine;
                    for (int i = vitriYL2 - 100; i <= vitriYL2 + 100; i++)
                    {
                        chartCheck.Series["danhDauL2_1"].Points.AddXY(x[0], i);

                    }

                    //END: Đánh dấu của Thước độ dài L2_1

                    //BEGIN: Đánh dấu độ dài L2_2 
                    chartCheck.Series["danhDauL2_2"].Color = System.Drawing.Color.Blue;
                    chartCheck.Series["danhDauL2_2"].BorderWidth = 1;
                    chartCheck.Series["danhDauL2_2"].ChartType = SeriesChartType.StepLine;

                    for (int i = vitriYL2 - 100; i <= vitriYL2 + 100; i++)
                    {
                        chartCheck.Series["danhDauL2_2"].Points.AddXY(x[1], i);
                    }
                    //END: Đánh dấu của Thước độ dài L2_2

                    //Hiển thị giá trị đường kính
                    if (checkBoxDiameter.Checked==true)
                    {
                        System.Drawing.Font fontDiameterValue = new System.Drawing.Font("Microsoft Sans Serif", 12.0f, FontStyle.Regular);
                        chartCheck.Series["Series1"].Points[x[0] + 5].Label = "Ø " + txtD2_1.Text + "";
                        // chartCheck.Series["Series1"].Points[x[0] + 5].LabelBackColor = System.Drawing.Color.Yellow;
                        chartCheck.Series["Series1"].Points[x[0] + 5].Font = fontDiameterValue;
                        chartCheck.Series["Series1"].Points[p[0] - 5].Label = "Ø " + txtD1_1.Text + "";
                        // chartCheck.Series["Series1"].Points[p[0] - 5].LabelBackColor = System.Drawing.Color.Yellow;
                        chartCheck.Series["Series1"].Points[p[0] - 5].Font = fontDiameterValue;
                    }
                   
                    #endregion

                    //BEGIN: Hiển thị ShowMaker trên tập dữ liệu CSV
                    //if (setting.ShowMaker=="0")
                    //{
                    if (checkBoxMaker.Checked == true)
                    {
                        chartCheck.Series["Series1"].Points[x[0]].MarkerColor = System.Drawing.Color.Black;
                        chartCheck.Series["Series1"].Points[x[0]].MarkerStyle = MarkerStyle.Star4;
                        chartCheck.Series["Series1"].Points[x[0]].MarkerSize = 8;


                        chartCheck.Series["Series1"].Points[p[0]].MarkerColor = System.Drawing.Color.Black;
                        chartCheck.Series["Series1"].Points[p[0]].MarkerStyle = MarkerStyle.Star4;
                        chartCheck.Series["Series1"].Points[p[0]].MarkerSize = 8;

                        chartCheck.Series["Series1"].Points[x[1]].MarkerColor = System.Drawing.Color.Black;
                        chartCheck.Series["Series1"].Points[x[1]].MarkerStyle = MarkerStyle.Star4;
                        chartCheck.Series["Series1"].Points[x[1]].MarkerSize = 8;
                    }
                    // chartCheck.ChartAreas[0].AxisX.LabelStyle.Angle = 45; // this works
                    //chartCheck.Series["Series1"].LabelAngle = 45;
                    //}

                    //BEGIN: Hiển thị ShowMakerValue trên tập dữ liệu CSV

                    //if (setting.ShowMakerValue == "1")
                    //{
                    if (checkBoxMakerValue.Checked == true)
                    {
                        System.Drawing.Font myFont = new System.Drawing.Font(FontFamily.GenericSerif, 10, FontStyle.Regular);
                        chartCheck.Series["Series1"].Points[x[0]].Label = "X0 (" +x[0].ToString() +")";
                        //chartCheck.Series["Series1"].Points[x[0]].LabelBackColor = System.Drawing.Color.Yellow;
                        chartCheck.Series["Series1"].Points[x[0]].Font = myFont;

                        chartCheck.Series["Series1"].Points[p[0]].Label = p[0].ToString();
                        //chartCheck.Series["Series1"].Points[p[0] - 1].LabelBackColor = System.Drawing.Color.Transparent;
                        chartCheck.Series["Series1"].Points[p[0]].Font = myFont; 

                        //chartCheck.Series["Series1"].Points[x[0]].Label = x[0].ToString(); 
                        //chartCheck.Series["Series1"].Points[x[0]].Font = myFont;

                        chartCheck.Series["Series1"].Points[x[1]].Label = x[1].ToString();
                        chartCheck.Series["Series1"].Points[x[1]].Font = myFont;
                    }
                    if (checkBoxTraceability.Checked == true)
                    {
                        chartCheck.Series["Series1"].Points[x[0]].MarkerColor = System.Drawing.Color.Black;
                        chartCheck.Series["Series1"].Points[x[0]].MarkerStyle = MarkerStyle.Star5;
                        chartCheck.Series["Series1"].Points[x[0]].MarkerSize = 8;

                        chartCheck.Series["Series1"].Points[x[1]].MarkerColor = System.Drawing.Color.Black;
                        chartCheck.Series["Series1"].Points[x[1]].MarkerStyle = MarkerStyle.Star5;
                        chartCheck.Series["Series1"].Points[x[1]].MarkerSize = 8;

                        chartCheck.Series["Series1"].Points[p[0]].MarkerColor = System.Drawing.Color.Black;
                        chartCheck.Series["Series1"].Points[p[0]].MarkerStyle = MarkerStyle.Star5;
                        chartCheck.Series["Series1"].Points[p[0]].MarkerSize = 8;
                          
                        chartCheck.Series["Series1"].Points[p[1]].MarkerColor = System.Drawing.Color.Black;
                        chartCheck.Series["Series1"].Points[p[1]].MarkerStyle = MarkerStyle.Circle;
                        chartCheck.Series["Series1"].Points[p[1]].MarkerSize = 8;

                        chartCheck.Series["Series1"].Points[p[2]].MarkerColor = System.Drawing.Color.Black;
                        chartCheck.Series["Series1"].Points[p[2]].MarkerStyle = MarkerStyle.Circle;
                        chartCheck.Series["Series1"].Points[p[2]].MarkerSize = 8;

                        chartCheck.Series["Series1"].Points[p[3]].MarkerColor = System.Drawing.Color.Black;
                        chartCheck.Series["Series1"].Points[p[3]].MarkerStyle = MarkerStyle.Circle;
                        chartCheck.Series["Series1"].Points[p[3]].MarkerSize = 8;

                        chartCheck.Series["Series1"].Points[p[4]].MarkerColor = System.Drawing.Color.Black;
                        chartCheck.Series["Series1"].Points[p[4]].MarkerStyle = MarkerStyle.Circle;
                        chartCheck.Series["Series1"].Points[p[4]].MarkerSize = 8;

                        chartCheck.Series["Series1"].Points[p[5]].MarkerColor = System.Drawing.Color.Black;
                        chartCheck.Series["Series1"].Points[p[5]].MarkerStyle = MarkerStyle.Circle;
                        chartCheck.Series["Series1"].Points[p[5]].MarkerSize = 8;


                        System.Drawing.Font myFont = new System.Drawing.Font(FontFamily.GenericSerif, 9, FontStyle.Regular);
                        chartCheck.Series["Series1"].Points[x[0]].Label = "X0 (" + x[0].ToString() + ")"; 
                        chartCheck.Series["Series1"].Points[x[0]].Font = myFont;
                        chartCheck.Series["Series1"].Points[x[0]].LabelBackColor = System.Drawing.Color.Yellow;

                        chartCheck.Series["Series1"].Points[x[1]].Label = "X1 (" + x[1].ToString() + ")";
                        chartCheck.Series["Series1"].Points[x[1]].Font = myFont;
                        chartCheck.Series["Series1"].Points[x[1]].LabelBackColor = System.Drawing.Color.Yellow;

                        chartCheck.Series["Series1"].Points[p[0]].Label = "P0 (" + p[0].ToString() + ")"; 
                        chartCheck.Series["Series1"].Points[p[0]].Font = myFont;
                         
                        chartCheck.Series["Series1"].Points[p[1]].Label = "P1 (" + p[1].ToString() + ")";
                        chartCheck.Series["Series1"].Points[p[1]].Font = myFont;
                        chartCheck.Series["Series1"].Points[p[1]].Color = System.Drawing.Color.Blue;

                        chartCheck.Series["Series1"].Points[p[2]].Label = "P2 (" + p[2].ToString() + ")";
                        chartCheck.Series["Series1"].Points[p[2]].Font = myFont;

                        chartCheck.Series["Series1"].Points[p[3]].Label = "P3 (" + p[3].ToString() + ")";
                        chartCheck.Series["Series1"].Points[p[3]].Font = myFont;

                        chartCheck.Series["Series1"].Points[p[4]].Label = "P4 (" + p[4].ToString() + ")";
                        chartCheck.Series["Series1"].Points[p[4]].Font = myFont;

                        chartCheck.Series["Series1"].Points[p[5]].Label = "P5 (" + p[5].ToString() + ")";
                        chartCheck.Series["Series1"].Points[p[5]].Font = myFont;

                    }

                    //if (setting.ShowLongRuler == "1")
                    //{
                    if (checkBoxProjectionline.Checked == true)
                    {

                        chartCheck.Series["SeriesDuongGiongXuongP0"].Color = System.Drawing.Color.Blue;
                        chartCheck.Series["SeriesDuongGiongXuongP0"].BorderWidth = 1;
                        chartCheck.Series["SeriesDuongGiongXuongP0"].ChartType = SeriesChartType.StepLine;
                        chartCheck.Series["SeriesDuongGiongXuongP0"].BorderDashStyle = ChartDashStyle.Solid;
                        for (int i = vitri_Y_L1 - 1500; i <= vitri_Y_L1 - 150; i++)
                        {

                            chartCheck.Series["SeriesDuongGiongXuongP0"].Points.AddXY(p[0], i);
                        }


                        chartCheck.Series["SeriesDuongGiongXuongX0"].Color = System.Drawing.Color.Blue;
                        chartCheck.Series["SeriesDuongGiongXuongX0"].BorderWidth = 1;
                        chartCheck.Series["SeriesDuongGiongXuongX0"].ChartType = SeriesChartType.StepLine;
                        chartCheck.Series["SeriesDuongGiongXuongX0"].BorderDashStyle = ChartDashStyle.Solid;

                        for (int i = vitri_Y_L1 - 1500; i <= vitri_Y_L1-150; i++)
                        {
                            chartCheck.Series["SeriesDuongGiongXuongX0"].Points.AddXY(x[0], i);
                        }



                        chartCheck.Series["SeriesDuongGiongXuongX1"].Color = System.Drawing.Color.Blue;
                        chartCheck.Series["SeriesDuongGiongXuongX1"].BorderWidth = 1;
                        chartCheck.Series["SeriesDuongGiongXuongX1"].ChartType = SeriesChartType.StepLine;
                        chartCheck.Series["SeriesDuongGiongXuongX1"].BorderDashStyle = ChartDashStyle.Solid;
                        for (int i = vitri_Y_L1 - 500; i <= vitri_Y_L1 - 150; i++)
                        {

                            chartCheck.Series["SeriesDuongGiongXuongX1"].Points.AddXY(x[1], i);
                        }
                    }
                }));


            }
            catch (System.Exception ex)
            {
               //log.Error("An error happened", ex);
                throw;
            }

        }
        #endregion


        /// <summary>
        /// Tạo ra các folder cần cho Phần mềm: FTPUpload, AutoMeasuringENDO,FileBackupENDO
        /// </summary>
        public void CreateFolder()
        {
            string ftp = "C:\\FTPUpload";
            string autoMeasuring = "C:\\AutoMeasuringENDO";
            string fileBackup = "C:\\FileBackupENDO";
            if (!Directory.Exists(ftp))
            {
                Directory.CreateDirectory(ftp);
            }
            if (!Directory.Exists(autoMeasuring))
            {
                Directory.CreateDirectory(autoMeasuring);
            }
            if (!Directory.Exists(fileBackup))
            {
                Directory.CreateDirectory(fileBackup);
            }

        }
        /// <summary>
        /// Tải cấu hình các biến trong phần mềm
        /// </summary>
        public bool loadConfigFile()
        {
            bool result = true;
            try
            {
                string filePath = "config.ini";
                string fullPath = System.IO.Path.GetFullPath(filePath);
                DelayData_1 = int.Parse(clsReadDataINI.ReadValue("TimerDelay", "DelayData_1", fullPath));
                DelayData_2 = int.Parse(clsReadDataINI.ReadValue("TimerDelay", "DelayData_2", fullPath));
                DelayBeforeResult1 = int.Parse(clsReadDataINI.ReadValue("TimerDelay", "DelayBeforeResult1", fullPath));
                DelayBeforeResult2 = int.Parse(clsReadDataINI.ReadValue("TimerDelay", "DelayBeforeResult2", fullPath));

                result = true;
            }
            catch (System.Exception)
            {
                result = false;

                DelayData_1 = 3000;
                DelayData_2 = 3000;
                DelayBeforeResult1 = 0;
                DelayBeforeResult2 = 3000;
                MessageBox.Show("Có lỗi khi tải file config.ini \r\n Phần mềm sẽ sử dụng file cấu hình mặc định", "THÔNG BÁO");
            }
            return result;
        }
        /// <summary>
        /// Bù Giá trị sai lệch
        /// </summary>
        /// <param name="itemName"></param>
        public bool loadBuGiaTri(string itemName)
        {
            try
            {
                List<clsBuGiaTri> list = clsDocFile.LoadCsvFile_BuGiaTri("SettingValue.csv");
                clsBu = list.Find(x => x.ItemName == ItemName);


                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }


        }

        /// <summary>
        /// lấy dữ liệu từ lần chạy trước đó: LotNo, Quantity, Printer...
        /// </summary>
        public void loadFileData_dat()
        {
            string filePath = "Data.ini";
            string fullPath = System.IO.Path.GetFullPath(filePath);
            ItemName = clsReadDataINI.ReadValue("ItemInfo", "ItemName", fullPath);
            LotNo = clsReadDataINI.ReadValue("ItemInfo", "LotNo", fullPath);
            Quantity = clsReadDataINI.ReadValue("ItemInfo", "Quantity", fullPath);
            PcsProcess = clsReadDataINI.ReadValue("WorkInfo", "PcsProcess", fullPath);
            Filename = clsReadDataINI.ReadValue("WorkInfo", "Filename", fullPath);
            IsStop = clsReadDataINI.ReadValue("WorkInfo", "IsStop", fullPath);
            KizuValue = clsReadDataINI.ReadValue("WorkInfo", "KizuValue", fullPath);
            UseCheckKizu = clsReadDataINI.ReadValue("WorkInfo", "UseCheckKizu", fullPath);
            SION_WaveValue0 = clsReadDataINI.ReadValue("WorkInfo", "SION_WaveValue0", fullPath);
            SION_WaveValue1 = clsReadDataINI.ReadValue("WorkInfo", "SION_WaveValue1", fullPath);
            SION_WaveValue2 = clsReadDataINI.ReadValue("WorkInfo", "SION_WaveValue2", fullPath);
            SION_WaveValue3 = clsReadDataINI.ReadValue("WorkInfo", "SION_WaveValue3", fullPath);
            SION_WaveValue4 = clsReadDataINI.ReadValue("WorkInfo", "SION_WaveValue4", fullPath);
            SION_WaveValue5 = clsReadDataINI.ReadValue("WorkInfo", "SION_WaveValue5", fullPath);
            SION_WaveValue6 = clsReadDataINI.ReadValue("WorkInfo", "SION_WaveValue6", fullPath);
            SION_WaveValue7 = clsReadDataINI.ReadValue("WorkInfo", "SION_WaveValue7", fullPath);
            SIONBLUE_WaveValue0 = clsReadDataINI.ReadValue("WorkInfo", "SIONBLUE_WaveValue0", fullPath);
            SIONBLUE_WaveValue1 = clsReadDataINI.ReadValue("WorkInfo", "SIONBLUE_WaveValue1", fullPath);
            SIONBLUE_WaveValue2 = clsReadDataINI.ReadValue("WorkInfo", "SIONBLUE_WaveValue2", fullPath);
            SIONBLUE_WaveValue3 = clsReadDataINI.ReadValue("WorkInfo", "SIONBLUE_WaveValue3", fullPath);
            SIONBLUE_WaveValue4 = clsReadDataINI.ReadValue("WorkInfo", "SIONBLUE_WaveValue4", fullPath);
            SIONBLUE_WaveValue5 = clsReadDataINI.ReadValue("WorkInfo", "SIONBLUE_WaveValue5", fullPath);
            SIONBLUE_WaveValue6 = clsReadDataINI.ReadValue("WorkInfo", "SIONBLUE_WaveValue6", fullPath);
            SIONBLUE_WaveValue7 = clsReadDataINI.ReadValue("WorkInfo", "SIONBLUE_WaveValue7", fullPath);
            PrinterName = clsReadDataINI.ReadValue("SystemInfo", "PrinterName", fullPath);
            MachineNo = clsReadDataINI.ReadValue("SystemInfo", "MachineNo", fullPath);


            txtLotNo.Text = LotNo;
            txtQuantity.Text = Quantity.ToString().Trim();
            //Load số lượng ở lần check trước
            m_iSTT = int.Parse(PcsProcess);
            txtSTT.Text = PcsProcess.ToString();
            //txtSTT2.Text = PcsProcess.ToString();
            strFileName = Filename;
            sion_waveValue[0] = Convert.ToDouble(SION_WaveValue0);
            sion_waveValue[1] = Convert.ToDouble(SION_WaveValue1);
            sion_waveValue[2] = Convert.ToDouble(SION_WaveValue2);
            sion_waveValue[3] = Convert.ToDouble(SION_WaveValue3);
            sion_waveValue[4] = Convert.ToDouble(SION_WaveValue4);
            sion_waveValue[5] = Convert.ToDouble(SION_WaveValue5);
            sion_waveValue[6] = Convert.ToDouble(SION_WaveValue6);
            sion_waveValue[7] = Convert.ToDouble(SION_WaveValue7);
            sionblue_waveValue[0] = Convert.ToDouble(SIONBLUE_WaveValue0);
            sionblue_waveValue[1] = Convert.ToDouble(SIONBLUE_WaveValue1);
            sionblue_waveValue[2] = Convert.ToDouble(SIONBLUE_WaveValue2);
            sionblue_waveValue[3] = Convert.ToDouble(SIONBLUE_WaveValue3);
            sionblue_waveValue[4] = Convert.ToDouble(SIONBLUE_WaveValue4);
            sionblue_waveValue[5] = Convert.ToDouble(SIONBLUE_WaveValue5);
            sionblue_waveValue[6] = Convert.ToDouble(SIONBLUE_WaveValue6);
            sionblue_waveValue[7] = Convert.ToDouble(SIONBLUE_WaveValue7);
            strPrinter = PrinterName;
            strMachineNo = MachineNo;
            /*
            string strPath_LCfile = "C:\\Users\\Public\\Documents\\LC.csv";
            var reader = new StreamReader(File.OpenRead(strPath_LCfile));
            string temp1 = "0";
            while (!reader.EndOfStream)
            {
                temp1 = reader.ReadLine();
            }
            reader.Close();
            m_iLC = int.Parse(temp1.Trim());
            */
        }
        /// <summary>
        /// Reset các biến về giá trị ban đầu
        /// </summary>
        public void resetVariable()
        {
            for (int i = 0; i < MAX_POINT; i++)
            {
                d_No1[i] = 0;
            }

            for (int i = 0; i < 350; i++)
            {
                d_No2[i] = 0;
            }

            for (int i = 0; i < 21; i++)
                for (int j = 0; j < 7; j++)
                    d_All[i, j] = 0;

            for (int i = 0; i < 15; i++)
                p[i] = 0;

            for (int i = 0; i < 8; i++)
            {

                x[i] = 0;
                S[i] = 0;
                waveValue[i] = 0;
            }
            for (int i = 0; i < 33; i++)
            {
                m_iPara[i] = 0;
            }

            m_iLC = 0;
            m_iRecheck = 0;
        }


        /// <summary>
        /// Sự kiện khi chọn Item Name
        /// Sẽ thực hiện thay đổi ảnh Picture Box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboItemName_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtLotNo.Focus();
            // Thay đổi ảnh trên lot đã chọn
            string strItemName = cboItemName.SelectedValue.ToString();
            try
            {
                pictureBox1.Image = Image.FromFile("Item image/" + strItemName + ".bmp");

            }
            catch (System.Exception)
            {
                pictureBox1.Image = Image.FromFile("Item image/imgStop.bmp");
                //   throw;
            }

        }
        /// <summary>
        /// Thay đổi ảnh trên lot đã chọn
        /// </summary>
        public void SetItemImage(string strItemName)
        {
            switch (strItemName)
            {
                case "10CA007-0-VN":
                    m_sItem = "SION_1830";
                    break;
                case "13CA018-0-VN":
                    m_sItem = "SION_1930";
                    break;
                case "11CA011-0-VN":
                    m_sItem = "SION_BLUE_1830";
                    break;
                case "13CA021-0-VN":
                    m_sItem = "SION_BLUE_1930";
                    break;
                case "SAMPLE":
                    m_sItem = "SAMPLE";
                    break;
                default:
                    m_sItem = strItemName;
                    break;
            }
            try
            {
                pictureBox1.Image = Image.FromFile("Item image/" + strItemName + ".bmp");
            }
            catch (System.Exception)
            {
                pictureBox1.Image = Image.FromFile("Item image/notImage.bmp");
                MessageBox.Show("Lỗi không hiển thị được ảnh", "Image Error");
            }
        }

        /// <summary>
        /// Sự kiện Set lot=> Tạo lot mới trong file Auto measuring
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetLot_Click(object sender, EventArgs e)
        {
            firstLoadChart(chartCheck);
            try
            {
                if (IsNumber(txtQuantity.Text) && (txtLotNo.Text.Length > 1) && (txtQuantity.Text.Length > 0) && string.IsNullOrWhiteSpace(txtLotNo.Text) == false)
                {
                    //Nếu Lot đang chạy dở thì yêu cầu kết thúc Lot. Mới cho chạy và tạo file mới
                    if (m_iSTT != 0)
                    {
                        MessageBox.Show("You need end lot first \n Bạn cần kết thúc lot đang chạy");
                        return;
                    }
                    else
                    {

                        txtSTT.Text = m_iSTT.ToString();
                        //txtSTT2.Text = PcsProcess.ToString();
                        LotNo = txtLotNo.Text.Trim();
                        int indexItemName = int.Parse(cboItemName.SelectedIndex.ToString());
                        ItemName = cboItemName.SelectedValue.ToString();


                        // Tạo File Mẫu cho pcs 1-20 item đầu tiên. Copy file tới thư mục AutoMeasuringENDO
                        string filePath = @"C:\AutoMeasuringENDO\" + LotNo + "_1_20.xls";
                        if (System.IO.File.Exists(filePath))
                        {
                            MessageBox.Show("Đã tồn tại tập tin " + LotNo + "_1_20.xls." + " Có thể tiếp tục dùng chương trình để kiểm tra", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            strFileName = @"C:\AutoMeasuringENDO\" + LotNo + "_1_20.xls";
                            strFileName2 = @"C:\AutoMeasuringENDO\" + LotNo + "_After_cutting_floppy.xls";
                            //Tao file moi va cho phep ghi de
                            File.Copy("Template.xls", strFileName, true); //Trước cắt
                            File.Copy("Template2.xls", strFileName2, true);// Sau cắt
                            ghiThongBao("Created new excel file " + strFileName);
                            ghiThongBao("Created new excel file " + strFileName2);
                        }
                        // strFileName = "C:\\AutoMeasuringENDO\\" + LotNo + ".xls";
                        // Thay đổi ảnh trên lot đã chọn
                        SetItemImage(ItemName);
                        //So luong cua Lot 
                        m_iQuantity = int.Parse(txtQuantity.Text.Trim());
                        //Load các biến bù giá trị
                        loadBuGiaTri(ItemName);

                        // #Hết Tạo file mẫu cho pcs 1-20 đầu tiên.

                        //  btnStart.Text = "STOP";
                        // m_bIsStop = false;
                        ghiThongBao("Begins production. Created thread continue running.... ");
                        //Hiện thị các tiêu chuẩn 

                        //Ghi dữ liệu vào file ItemSetting.csv
                        m_iPara = clsDocFile.GetItemConfiguration(ItemName, txtQuantity.Text);

                        if ((m_sItem == "SION_1830") || (m_sItem == "SION_1930"))
                        {
                            //txtLimit_S1.Text = SION_WaveValue0;
                            //txtLimit_S2.Text = SION_WaveValue1;
                            //txtLimit_S3.Text = SION_WaveValue2;
                            //txtLimit_S4.Text = SION_WaveValue3;
                            //txtLimit_S5.Text = SION_WaveValue4;
                            //txtLimit_S6.Text = SION_WaveValue5;
                            //txtLimit_S7.Text = SION_WaveValue6;

                        }
                        else
                        {
                            if ((m_sItem == "SION_BLUE_1830") || (m_sItem == "SION_BLUE_1930"))
                            {
                                //txtLimit_S1.Text = SIONBLUE_WaveValue0;
                                //txtLimit_S2.Text = SIONBLUE_WaveValue1;
                                //txtLimit_S3.Text = SIONBLUE_WaveValue2;
                                //txtLimit_S4.Text = SIONBLUE_WaveValue3;
                                //txtLimit_S5.Text = SIONBLUE_WaveValue4;
                                //txtLimit_S6.Text = SIONBLUE_WaveValue5;
                                //txtLimit_S7.Text = SIONBLUE_WaveValue6;
                            }
                        }


                        pictureBox2.Visible = true;
                        lblStatusRun.Visible = false;
                        groupBoxControl.Text = "Status: RUNNING";
                        btnStart.BackColor = System.Drawing.Color.Orange;
                        btnStart.Text = "STOP Produced";
                        btnStart.Visible = true;
                        m_bIsStop = true;
                        //Khởi tạo theo dõi file
                        string path = @"C:\FTPUpload";
                        string fileData = "Data.csv";
                        string fileData2 = "Data2.csv";



                        m_Watcher = new FileSystemWatcher();
                        m_Watcher.Filter = fileData;
                        m_Watcher.Path = path;
                        m_Watcher.Changed += new FileSystemEventHandler(OnChangedData);
                        m_Watcher.EnableRaisingEvents = true;


                        m_Watcher2 = new FileSystemWatcher();
                        m_Watcher2.Filter = fileData2;
                        m_Watcher2.Path = path;
                        m_Watcher2.Changed += new FileSystemEventHandler(OnChangedData2);
                        m_Watcher2.EnableRaisingEvents = true;

                        MessageBox.Show("Lot setup. Ready for produced.\n\n Đã cài đặt Lot. Phần mềm đã sẵn sàng sản xuất.", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }



                }
                else
                {
                    MessageBox.Show("Quantity and Lot No is not empty;\nQuanity is number. \n\nSố lượng và LotNo không được để trống \nSố lượng phải là kiểu số.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (System.Exception ex)
            {

                MessageBox.Show(ex.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Dừng theo dõi file
        /// </summary>
        public void DungTheoDoiFile()
        {
            m_Watcher.EnableRaisingEvents = false;
            m_Watcher.Changed -= new FileSystemEventHandler(OnChangedData);
            m_Watcher.Dispose();

            m_Watcher2.EnableRaisingEvents = false;
            m_Watcher2.Changed -= new FileSystemEventHandler(OnChangedData);
            m_Watcher2.Dispose();

        }



        /// <summary>
        /// Sự kiện khi Kết Thúc Lot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLotEnd_Click(object sender, EventArgs e)
        {


            if (m_bIsStop)
            {
                MessageBox.Show("Ấn STOP trước khi Lot End", "Thông báo");
                return;
            }
            else
            {

                DialogResult result = MessageBox.Show("Bạn muốn kết thúc lot?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    // btnStart.Text = "START";
                    ghiThongBao("Lot End. Stopped thread");
                    m_iSTT = 0;
                    txtSTT.Text = "0";
                    //txtSTT2.Text = PcsProcess.ToString();
                    pictureBox1.Image = Image.FromFile("Item image/imgStop.bmp");
                    txtLotNo.Text = "";
                    txtQuantity.Text = "";
                    ClearResult();
                }
                else
                {
                    return;
                }

            }

        }

        #endregion

        /// <summary>
        /// Đọc file Data.csv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        public bool IsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (IsNumber(txtQuantity.Text) && (txtLotNo.Text.Length > 1) && (txtQuantity.Text.Length > 0) && string.IsNullOrWhiteSpace(txtLotNo.Text) == false)
            {
                string str;
                if (m_bIsStop)
                {

                    DialogResult result = MessageBox.Show("Are you sure want to STOP software ? \r\n Xác nhận dừng phần mềm ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        pictureBox2.Visible = false;
                        lblStatusRun.Visible = true;
                        groupBoxControl.Text = "Status: NOT RUN";
                        m_bIsStop = false;
                        btnStart.Text = "START";
                        ghiThongBao("Stopped thread ...");
                        DungTheoDoiFile();
                        btnStart.BackColor = SystemColors.ButtonFace;
                        btnStart.ForeColor = default(System.Drawing.Color);
                        btnStart.UseVisualStyleBackColor = true;
                        m_Watcher.EnableRaisingEvents = false;
                        m_Watcher2.EnableRaisingEvents = false;
                        try
                        {
                            pictureBox1.Image = Image.FromFile("Item image//imgStop.bmp");
                        }
                        catch (System.Exception)
                        {
                            pictureBox1.Image = Image.FromFile("Item image/notImage.bmp");
                            MessageBox.Show("Lỗi không hiển thị được ảnh", "Image Error");
                        }
                    }
                }
                else
                {
                    try
                    {
                        if (IsNumber(txtQuantity.Text))
                        {
                            pictureBox2.Visible = true;
                            lblStatusRun.Visible = false;
                            groupBoxControl.Text = "Status: RUNNING";
                            btnStart.BackColor = System.Drawing.Color.Orange;
                            m_bIsStop = true;
                            btnStart.Text = "STOP Produced";
                            string strItemName = cboItemName.SelectedValue.ToString();
                            SetItemImage(strItemName);

                            m_iPara = clsDocFile.GetItemConfiguration(cboItemName.SelectedValue.ToString(), txtQuantity.Text);
                            ghiThongBao("Begins production. Thread continue running...");

                            //Khởi tạo theo dõi file
                            string path = @"C:\FTPUpload";
                            string fileData = "Data.csv";
                            string fileData2 = "Data2.csv";
                            m_Watcher = new FileSystemWatcher();
                            m_Watcher.Filter = fileData;
                            m_Watcher.Path = path;
                            m_Watcher.Changed += new FileSystemEventHandler(OnChangedData);
                            m_Watcher.EnableRaisingEvents = true;

                            m_Watcher2 = new FileSystemWatcher();
                            m_Watcher2.Filter = fileData2;
                            m_Watcher2.Path = path;
                            m_Watcher2.Changed += new FileSystemEventHandler(OnChangedData2);
                            m_Watcher2.EnableRaisingEvents = true;

                            //phần gắn giá trị để check kizu; Tạm không dùng
                            if ((m_sItem == "10CA007-0-VN") || (m_sItem == "13CA018-0-VN"))
                            {
                                for (int i = 0; i < 8; i++)
                                    waveValue[i] = sion_waveValue[i];
                            }
                            else if ((m_sItem == "11CA011-0-VN") || (m_sItem == "13CA021-0-VN"))
                            {
                                for (int i = 0; i < 8; i++)
                                    waveValue[i] = sionblue_waveValue[i];
                            }

                            //Tiếp theo code cho check kizu.KHÔNG DÙNG ĐẾN =>Bỏ 
                        }
                        else
                        {
                            MessageBox.Show("Số lượng phải là kiểu số", "Warning!");
                        }
                    }
                    catch (System.Exception ex)
                    {

                        MessageBox.Show(ex.ToString(), "Warning!");
                    }


                }
            }
            else
            {
                MessageBox.Show("Please Set Lot first", "Warning!");
            }

        }

        private void btnWriteResult1_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Ghi thông báo trạng thái v tiến trinh đang chạy lên listBox1
        /// </summary>
        /// <param name="thongDiep"></param>
        public void ghiThongBao(string thongDiep)
        {
            string logTime = "";
            logTime = DateTime.Now.ToLongTimeString();
            thongDiep = logTime + ": " + thongDiep;
            listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Insert(0, thongDiep)));

        }
        /// <summary>
        /// Sự kiện theo dõi sự thay đổi file Data.csv
        /// Thực hiện tính toán trên file Data.csv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChangedData(object sender, FileSystemEventArgs e)
        {
            try
            {
                m_Watcher.EnableRaisingEvents = false;

                SaveDisplayResult();
                //SaveResultForAfterCut();


                string s = sender.ToString();
                string s1 = e.ToString();
                string str = "", str1 = "", str2 = "", str3 = "";

                chartCheck.Invoke((MethodInvoker)(() => chartCheck.Visible = false));
                //Đọc file Data.CSV
                // listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Insert(0, "Processing C:\\FTPUpload\\Data.csv")));

                ghiThongBao("Processing C:\\FTPUpload\\Data.csv");
                //Cho delay để plc xong với Data.csv
                Thread.Sleep(DelayData_1);

                string fileData = @"C:\FTPUpload\Data.csv";

                //Đọc tất cả dữ liệu trong file Data.csv vào dataTable
                DataTable dt = clsDocFile.LoadDataCSV(fileData);
                ghiThongBao("Received all data from PLC");
                //Data is re-check or not.
                m_iRecheck = int.Parse(dt.Rows[0][1].ToString());
                //lấy số thứ tự được máy PLC ghi vào file data.csv để biết nó đã xử lý đến core thứ mấy.
                //Mục đích để tạo file CSV tương ứng LOTNO_1_20 rồi tiếp LOTNO_21-40  etc.
                m_iSTT = int.Parse(dt.Rows[1][1].ToString());


                //txtSTT.Text = m_iSTT.ToString();
                txtSTT.Invoke((MethodInvoker)(() => txtSTT.Text = m_iSTT.ToString()));
                //txtSTT2.Invoke((MethodInvoker)(() => txtSTT2.Text = m_iSTT.ToString()));
                //Temp20: = số thứ tự của PCS % 20
                int temp20 = m_iSTT % 20;
                if (temp20 == 0)
                    temp20 = 20;
                //Đưa dữ liệu từ file data.csv vào mảng
                for (int i = 2; i < dt.Rows.Count; i++)
                {
                    d_No1[i - 2] = int.Parse(dt.Rows[i][1].ToString());
                }
                //Kiem tra Core va tra ve ket qua
                bResult = MeasureLength(d_No1, temp20);

                //Lưu lại giá trị gốc trước khi làm tròn giá trị. Để gửi dữ liệu gốc này tới plc
                double l0_goc = l[0];


                //Làm tròn giá trị

                l[0] = Math.Round(l[0]);


                d_All[temp20, 5] = 1; //Kết quả tính toán là OK 

                if (bResult == true)
                {
                    switch (ItemName)
                    {
                        case "SAMPLE-TEST":
                            str3 = "EM05016," + (int)Math.Round(l[0]) + ",1\r\n";

                            //SAMPLE TEST đánh giá là OK
                            break;

                        case "PR11CA011-0": 
                        case "11CA011-0-VN":
                        case "PR11CA011-0-US":
                        case "PR13CA021-0":
                        case "13CA021-0-VN":
                        case "KPR10CA011-0":
                        case "K10CA011-0-VN":
                        case "10CA011-0-VN":
                        case "PR10CA007-0-VN":
                        case "KPR10CA007-0":
                        case "K10CA007-0-VN":
                        case "PR13CA018-0-VN":
                        case "K13CA018-0-VN":
                        case "13CA018-0-VN":
                        case "KPR13CA018-0":
                        case "09CA002-0-VN":
                        case "PR15CA001-0":
                        case "15CA001-0-VN":
                        case "KPR12CA016-0":
                        case "K12CA016-0-VN":
                        case "12CA016-0-VN":
                        case "KPR12CA022-0":
                        case "K12CA022-0-VN":
                        case "12CA022-0-VN":
                        case "KPR12CA002-A":
                        case "K12CA002-A-VN":
                        case "12CA002-A-VN":
                        case "KPR12CA009-0":
                        case "K12CA009-0-VN":
                        case "12CA009-0-VN":
                        case "KPR7CA006-E":
                        case "K7CA006-E-VN":
                        case "7CA006-E-VN":
                        case "KPR4VC006-0":
                        case "K4VC006-0-VN":
                        case "4VC006-0-VN":
                        case "17CA010-0-VN":
                        case "KPR10CA005-0":
                        case "K10CA005-0-VN":
                        case "10CA005-0-VN":
                        case "KPR09CA006-0":
                        case "K09CA006-0-VN":
                        case "09CA006-0-VN":
                        case "KPR09VC003-0":
                        case "K09VC003-0-VN":
                        case "09VC003-0-VN":
                        case "KPR1CA001-B":
                        case "K1CA001-B-VN":
                        case "1CA001-B-VN":
                        case "KPR8CA001-B":
                        case "K8CA001-B-VN":
                        case "8CA001-B-VN":
                        case "KPR5CA004-C":
                        case "K5CA004-C-VN":
                        case "5CA004-C-VN":
                        case "KPR0CA002-A":
                        case "K0CA002-A-VN":
                        case "0CA002-A-VN":
                        case "09CA001-0-VN":
                        case "KPR7CA016-D":
                        case "K7CA016-D-VN":
                        case "7CA016-D-VN":
                        case "1CA004-A-VN":
                        case "10CA014-0-VN":
                        case "07CA001-0-VN":
                        case "KPR6VC003-B":
                        case "K6VC003-B-VN":
                        case "6VC003-B-VN":
                        case "KPR4CA004-B":
                        case "K4CA004-B-VN":
                        case "4CA004-B-VN":
                        case "KPR6CA007-B":
                        case "K6CA007-B-VN":
                        case "6CA007-B-VN":
                        case "KPR07CA004-B":
                        case "K07CA004-B-VN":
                        case "07CA004-B-VN":
                        case "PR09VC010-0":
                        case "09VC010-0-VN":
                        case "PR10VC005-0":
                        case "10VC005-0-VN":
                        case "PR10VC008-0":
                        case "10VC008-0-VN":
                        case "PR16VC009-0":
                        case "16VC009-0-VN":
                        case "PR11VC008-0":
                        case "11VC008-0-VN":
                        case "KPR09VC013-0":
                        case "PR13VC016-0":
                        case "13VC016-0-VN":
                        case "PR12VC002-0":
                        case "12VC002-0-VN":
                        case "PR4VC013-A":
                        case "4VC013-A-VN":
                        case "7CA027-F-VN":
                        case "K7CA027-F-VN":
                        case "KPR7CA027-F":
                        case "7CA024-F-VN":
                        case "K7CA024-F-VN":
                        case "KPR7CA024-F":
                        case "K7CA023-F-VN":
                        case "7CA023-F-VN":
                        case "KPR7CA023-F":
                        case "BPR13CA021-AUS":
                        case "16VC023-0":
                        case "16VC021-0":
                        case "16VC024-0":
                        case "16VC020-0":
                        case "16VC022-0":
                        case "18VC019-0":
                        case "18VC020-0":
                        case "18VC021-0":
                        case "18VC022-0":
                        case "18VC023-0":
                        case "16VC025-0":
                        case "KPR11VC012-0":
                        case "PR16VC014-0":
                        case "PR16VC016-0":
                        case "PR16VC017-0":
                        case "BPR13CA021-A":
                        case "BPR11CA011-AUS":
                        case "PR18VC014-0":
                        case "PR16VC015-0":
                        case "PR19CA055-0":
                        case "PR19CA056-0":
                        case "PR19CA057-0":
                        case "PR19CA059-0":
                        case "3CY003-E":
                        case "PR16VC018-0":
                        case "KPR5CA002-B":
                        case "KPR07CA004-B-T":
                        case "KPR12CA016-0-T":
                        case "KPR4CA004-B-T":
                        case "KPR7CA006-E-T":
                        case "KPR12CA002-A-T":
                        case "KPR12CA009-0-T":
                        case "BPR20CA085-0":
                        case "BPR20CA085-0US":
                        case "BPR20CA086-0":
                        case "BPR20CA086-0US":
                        case "KPR13CA007-0-T":
                        case "KPR12CA022-0-T":
                        case "KPR7CA016-D-T":
                        case "KPR10CA011-0-T":
                        case "KPR13CA018-0-T":
                        case "KPR6CA007-B-T":
                        case "KPR7CA023-F-T":
                        case "KPR7CA027-F-T":
                        case "KPR8CA001-B-T":
                        case "KPR09CA006-0-T":
                        case "BPR20CA086-0-T":
                        case "BPR20CA053-0":
                        case "KPR13CA007-0":
                        case "KPR07CA009-B":
                        case "KPR20VC081-0":
                        case "KPR20VC078-0":
                        case "KPR2CA001-0":
                        case "BPR20CA011-001":
                        case "KPR14VC032-0":
                        case "KPR14VC034-0":
                        case "KPR14VC028-0":
                        case "5CA005":
                        case "6VC004":
                        case "4VC007":
                        case "4VC009-B-VN":
                        case "08VC019-0-VN":
                        case "10VC012-0-VN":
                        case "11VC006-A":
                        case "13VC016-0":
                        case "12CA001-0":
                        case "10CA014-0":
                        case "12CA015-0":
                        case "15VC006-0-VN":
                        case "08VC020-0":
                        case "16VC007-0-VN":
                        case "13CA017-0":
                        case "12CA008-0":
                        case "PR16VC008-0":
                        case "PR11VC006-A":
                        case "PR12CA015-0":
                        case "PR08VC019-0":
                        case "PR10VC012-0":
                        case "PR15VC008-0":
                        case "PR17VC009-0":
                        case "PR19CA044-0":
                        case "PR07CA001-0":
                        case "PR10CA014-0":
                        case "PR4VC009-B":
                        case "15VC008-0-VN":
                        case "PR12CA008-0":
                        case "PR12CA001-0":
                        case "PR15VC006-0":
                        case "10CA006-0":
                        case "17VC009-0":
                        case "PR21VC026-0":
                        case "PR21VC027-0":
                        case "PR21VC028-0":
                        case "KPR10CA007-0-T":
                        case "BPR20CA086-0US-T":
                        case "BPR20CA085-0 -T":
                        case "BPR20CA053-0-T":
                        case "PR11VC008-0-T":
                        case "PR09VC010-0-T":
                        case "PR4VC013-A-T":
                        case "KPR0CA002-A-T":
                        case "PR12VC002-0-T":
                        case "PR10VC005-0-T":
                        case "KPR1CA001-B-T":
                        case "KPR09VC003-0-T":
                        case "PR16VC008-0-T":
                        case "PR13VC016-0-T":
                        case "PR10VC008-0-T":
                        case "PR16VC009-0-T":
                        case "PR10VC012-0-T":
                        case "PR15VC008-0-T":
                        case "PR17VC009-0-T":
                        case "PR15VC006-0-T":
                        case "PR21VC026-0-T":
                        case "PR21VC027-0-T":
                        case "PR21VC028-0-T":
                        case "KPR07CA009-B-T":
                        case "KPR5CA004-C-T":
                        case "KPR5CA002-B-T":
                        case "KPR11VC012-0-T":
                        case "KPR4VC006-0-T":
                        case "KPR6VC003-B-T":
                        case "PR11VC006-A-T":
                        case "PR08VC019-0-T":
                        case "PR4VC009-B-T":
                            str3 = "EM05016," + (int)Math.Round(l[0]) + ",1\r\n";

                            ///Đánh giá độ dài
                            if ((l[0] > m_iPara[9]) || (l[0] < m_iPara[10]))
                            {
                                d_All[temp20, 5] = 0;
                                d_All[temp20, 6] = 7;
                               
                                txtL1_1.Invoke((Action)(() => txtL1_1.ForeColor =System.Drawing.Color.Red));
                            }
                            else
                            {
                                txtL1_1.Invoke((Action)(() => txtL1_1.ForeColor = System.Drawing.Color.Blue));
                            }

                            if ((l[1] > m_iPara[11]) || (l[1] < m_iPara[12]))
                            {
                                d_All[temp20, 5] = 0;
                                d_All[temp20, 6] = 7;
                                txtL2_1.Invoke((Action)(() => txtL2_1.ForeColor = System.Drawing.Color.Red));
                            }
                            else
                            {
                                txtL2_1.Invoke((Action)(() => txtL2_1.ForeColor = System.Drawing.Color.Blue));
                            }

                           // Đánh giá đường kính 1
                            int dk = d_All[temp20, 0] / 10;
                            if (d_All[temp20, 0] / 10 > m_iPara[3] || d_All[temp20, 0] / 10 < m_iPara[4])
                            {
                                d_All[temp20, 5] = 0;  //Kết quả tính toán là NG
                                d_All[temp20, 6] = 4;
                                txtD1_1.Invoke((Action)(() => txtD1_1.ForeColor = System.Drawing.Color.Red));
                            }
                            else
                            {
                                txtD1_1.Invoke((Action)(() => txtD1_1.ForeColor = System.Drawing.Color.Blue));
                            }
                            //Đánh giá đường kính 2
                            int dk2 = d_All[temp20, 1] / 10;
                            if (d_All[temp20, 1] / 10 > m_iPara[1] || d_All[temp20, 1] / 10 < m_iPara[2])
                            {
                                //d_All[temp20, 5] = 0;  
                                d_All[temp20, 6] = 5;
                                txtD2_1.Invoke((Action)(() => txtD2_1.ForeColor = System.Drawing.Color.Red));
                            }
                            else
                            {
                                txtD2_1.Invoke((Action)(() => txtD2_1.ForeColor = System.Drawing.Color.Blue));
                            }
                            break;
                        default:
                            break;
                    }
                    



                    ///Write the results
                    str2 = str2 + "EM05008," + Math.Round(l[1]) + ",1\r\n";
                    str2 = str2 + "EM05009," + Math.Round(l[0]) + ",1\r\n";
                    str2 = str2 + "EM05010," + "0,1\r\n";
                    str2 = str2 + "EM05011," + "0,1\r\n";
                    str2 = str2 + "EM05012," + "0,1\r\n";
                    str2 = str2 + "EM05013," + "0,1\r\n";
                    str2 = str2 + "EM05014," + "0,1\r\n";
                    str2 = str2 + "EM05015," + "0,1\r\n";

                    //str2 = str2 + "EM05011," + Math.Round(l[3]) + ",1\r\n";
                    //str2 = str2 + "EM05012," + Math.Round(l[4]) + ",1" + Environment.NewLine;
                    //str2 = str2 + "EM05013," + Math.Round(l[5]) + ",1\r\n";
                    //str2 = str2 + "EM05014," + Math.Round(l[6]) + ",1\r\n";
                    //str2 = str2 + "EM05015," + Math.Round(l[7]) + ",1\r\n";  //Sử dụng tăng lên

                    //str2 = str2 + "EM05009," + "0,1\r\n";
                    //str2 = str2 + "EM05010," + "0,1\r\n";
                    //str2 = str2 + "EM05011," + "0,1\r\n";
                    //str2 = str2 + "EM05012," + "0,1" + Environment.NewLine;
                    //str2 = str2 + "EM05013," + "0,1\r\n";
                    //str2 = str2 + "EM05014," + "0,1\r\n";
                    //str2 = str2 + "EM05015," + "0,1\r\n";  //Sử dụng tăng lên

                    //double varTemp = 0;
                    //varTemp = Convert.ToDouble(d_All[temp20, 0] / 10);


                    str1 = str1 + "EM05003," + d_All[temp20, 1] / 10 + ",1\r\n";
                    str1 = str1 + "EM05004," + d_All[temp20, 0] / 10 + ",1\r\n";
                    str1 = str1 + "EM05005," + "0,1\r\n";
                    str1 = str1 + "EM05006," + d_All[temp20, 3] / 10 + ",1\r\n";
                    str1 = str1 + "EM05007," + d_All[temp20, 4] / 10 + ",1\r\n";

                    //str1 = str1 + "EM05005," + d_All[temp20, 2] / 10 + ",1\r\n";
                    //str1 = str1 + "EM05006," + d_All[temp20, 3] / 10 + ",1\r\n";
                    //str1 = str1 + "EM05007," + d_All[temp20, 4] / 10 + ",1\r\n";

                    if (d_All[temp20, 5] != 0)
                    {
                        str = str + "EM05000,1,1\r\n";
                        str = str + "EM05001,1,1\r\n";
                        str = str + "EM05002,0,1\r\n";
                        ghiThongBao("Write to Result.csv OK");
                        ghiThongBao("Sent result to PLC: OK");
                        m_Watcher2.EnableRaisingEvents = true;
                    }
                    else
                    {
                        str = str + "EM05000,1,1\r\n";
                        str = str + "EM05001,0,1\r\n";
                        str = str + "EM05002,1,1\r\n";
                        ghiThongBao("Sented result to PLC: NG");

                        //Nếu data1 lỗi thì không phân tích file 2 nữa
                        m_Watcher2.EnableRaisingEvents = false;
                    }

                }
                else // File lỗi không phân tích được thì ghi kết quả NG
                {

                    ///Write the results
                    str2 = "EM05008,0,1\r\nEM05009,0,1\r\nEM05010,0,1\r\nEM05011,0,1\r\nEM05012,0,1\r\nEM05013,0,1\r\nEM05014,0,1\r\nEM05015,0,1\r\n";
                    str1 = "EM05003,0,1\r\nEM05004,0,1\r\nEM05005,0,1\r\nEM05006,0,1\r\nEM05007,0,1\r\n";
                    d_All[temp20, 5] = 0; //Nếu gắn d_All[temp20, 5]=0 nếu kết quả tính toán là NG
                    ghiThongBao("Can not analyse data!!!Re-check please! - Core không nằm trong tiêu chuẩn)");

                    //Báo lỗi nên hiển thị thêm thông tin lỗi lên màn hình
                    baoLoi();
                    //Nếu data1 lỗi thì không phân tích file 2 nữa
                    m_Watcher2.EnableRaisingEvents = false;
                }
                // Xuất dữ liệu đoạn cuối để cắt ///
                string str4 = "EM05017," + l[0] * 10 + ",1\r\n";
                switch (ItemName)
                {
                    case "SAMPLE-TEST":
                        str4 = "EM05017," + l0_goc * 10 + ",1\r\n";
                        break;
                    case "SION 1.8M":
                        str4 = "EM05017," + l0_goc * 10 + ",1\r\n";
                        break;
                    default:
                        break;
                }

                //Ghi dữ liệu ra file Result.csv
                //1.Xóa file
                //2.Tạo file mới và ghi dữ liệu là dữ liệu OK hoặc NG
                //3.Chờ 3giây. Xóa file
                //4.Tạo file mới và ghi dữ liệu mới(Là dữ liệu xóa về = 0 
                str = str + str1 + str2 + str3 + str4;

                string pathResultFile = "C:\\FTPUpload\\Result.csv";
                //if (File.Exists(pathResultFile))
                //{
                File.Delete(pathResultFile);
                //}
                using (StreamWriter fsResult = File.CreateText(pathResultFile))
                {
                    fsResult.Write(str);
                    fsResult.Close();
                }

                Thread.Sleep(DelayBeforeResult1);  // Cho ngủ chương trình để đợi PLC đọc kết quả này. Sau đấy gán lại Về NG(NG để không cho sang bước tiếp theo

                str = "EM05000,0,1\r\nEM05001,0,1\r\nEM05002,0,1\r\n";
                str = str + str1 + str2 + str3 + str4;
                if (File.Exists(pathResultFile))
                {
                    File.Delete(pathResultFile);

                }
                using (StreamWriter fs = File.CreateText(pathResultFile))
                {
                    fs.Write(str);
                    fs.Close();
                }
                //Copy file to Folder Backup
                string strLotITem = "C:\\FileBackupENDO\\" + ItemName + "_" + LotNo + "\\" + m_iSTT.ToString();
                //Copy nhung file Raw đến thư mục khác
                BackupFileData(strLotITem);
                //Write to excel file
                WriteToExcel(temp20);

                WriteToExcelWithChart(temp20);

                //Hiển thị dữ liệu hiển thị lên giao diện
                DisplayResultBeforeCut(temp20);


                if (m_iSTT == m_iQuantity)
                {
                    MessageBox.Show("Finished Lot. Please Lot End!");
                    m_bIsStop = true;
                    btnStart.Text = "START";
                    ghiThongBao("Stopped thread....");
                }

                m_iLC++;

                //Hiển thị chart

                chartCheck.Invoke((Action)(() => chartCheck.Visible = true));
              
                int c = d_No1.Count();
                List<int> listData = new List<int>();
                for (int i = 0; i < c; i++)
                {
                    listData.Add(d_No1[i]);
                }
                UpdateChart(listData);
            }
            catch (System.Exception ex)
            {
                ghiThongBao(ex.Message);
                ghiThongBao(ex.ToString());
                m_Watcher.EnableRaisingEvents = false;

                //Thread thread = new Thread(baoLoi);
                //thread.Start();

            }
            finally
            {
                m_Watcher.EnableRaisingEvents = true;
            }
        }
        /// <summary>
        /// Sự kiện theo dõi sự thay đổi file Data2.csv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChangedData2(object sender, FileSystemEventArgs e)
        {
            try
            {
                m_Watcher2.EnableRaisingEvents = false;

                string str = "", str1 = "", str2 = "", strW = "";
                string pathResult2 = "C:\\FTPUpload\\Result2.csv";
                string pathData2 = "C:\\FTPUpload\\Data2.csv";
                //Đợi cho PLC ghi dữ liệu vào file Data2 xong thì bắt đầu đọc file
                Thread.Sleep(DelayData_2);

                //Kiểm tra file Data2.csv có tồn tại hay không
                if (File.Exists(pathData2) == false)
                {
                    ghiThongBao("Can not open file Data2.CSV in folder C:\\FTPUload");
                    return;
                }
                ghiThongBao("Start processing file Data2.csv");

                int temp20 = m_iSTT % 20;
                if (temp20 == 0)
                    temp20 = 20;

                //Đọc tất cả dữ liệu trong file Data2.csv vào dataTable
                DataTable dt2 = clsDocFile.LoadData2CSV(pathData2);


                //Check moi
                //Đưa dữ liệu vào mảng
                //Trước sửa i=2
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    d_No2[i] = int.Parse(dt2.Rows[i][1].ToString());
                }

                // Hàm kiểm tra Độ dài sau cut
                // bool bResult = MeasureLengthAfterCut(d_No2,ItemName);
                bool bResult = true;
                // m_iPara[29] : độ dài nhỏ nhất Plopy cần cắt
                // m_iPara[30] : độ dài lớn nhất Plopy cần cắt
                if (bResult == true)
                {
                    //Compare data
                    //   if (( Math.Round(l2) > m_iPara[29]) || ( Math.Round(l2) < m_iPara[30]))
                    if ((Math.Round(l2) > m_iPara[29]) || (Math.Round(l2) < m_iPara[30]))
                    {
                        d_All[temp20, 5] = 0; //Kết quả tính toán là NG 
                        d_All[temp20, 6] = 10;
                    }
                    str2 = string.Format("EM05103,{0},1\r\n", Math.Round(l2)); //làm tròn lên
                    str2 = str2 + string.Format("EM05104,{0},1\r\n", l2 * 10);
                }
                else
                {
                    str2 = "EM05103,0,1\r\n";
                    str2 = str2 + "EM05104,0,1\r\n";
                    d_All[temp20, 5] = 0; //Kết quả tính toán là NG 
                    ghiThongBao("EM05103: After Cut: Không phân tích được core này !!! Can not analyse data!!!Re-check please!");

                }
                //Ghi file Excel
                if (d_All[temp20, 5] > 0) //Kết quả tính toán là OK 
                {
                    str = "EM05100,1,1\r\nEM05101,1,1\r\nEM05102,0,1\r\n";
                    ghiThongBao("Gửi kết quả OK đến PLC ! Sent result to PLC: OK");
                }
                else
                {
                    str = "EM05100,1,1\r\nEM05101,0,1\r\nEM05102,1,1\r\n";
                    ghiThongBao("Gửi kết quả NG tới PLC ! Sent result to PLC: NG");
                }

                //Bat dau ghi du lieu ra file Result2.CSV
                str = str + str2;

                if (!File.Exists(pathResult2))
                {
                    ghiThongBao("Can not find Result2.csv");
                }
                //Ghi kết quả ra file Result2
                strTemp = clsDocFile.writeCSVNotDelete(str, pathResult2);
                ghiThongBao(strTemp);
                ghiThongBao("Wait plc read result2.csv");
                //Ngủ chương trình x giây.
                Thread.Sleep(DelayBeforeResult2);


                str = "EM05100,0,1\r\nEM05101,0,1\r\nEM05102,0,1\r\n";
                str = str + str2;
                //2000

                //Mở và ghi lại file Result2.csv với các giá trị set về 0
                strTemp = clsDocFile.writeCSVNotDelete(str, pathResult2);
                ghiThongBao(strTemp);
                //# Hết ghi du lieu ra file Result2.CSV

                Thread.Sleep(DelayBeforeResult2);
                //Copy file
                string strLotITem = "C:\\FileBackupENDO\\" + ItemName + "_" + LotNo + "\\" + m_iSTT.ToString();
                BackupFileData2(strLotITem);
                //Ghi dữ liệu ra file Excel sau khi cut fllopy
                WriteToExcel2(m_iSTT);


                //Hiển thị dữ liệu lên màn hình

                string strLengthPloppy = Math.Round(l2).ToString(); //làm tròn lên            

                //#Hết phần hiển thị dữ liệu lên màn hình
                if (m_iRecheck == 0)
                {
                    m_iSTT = m_iSTT + 1;
                }

            }
            catch (System.Exception ex)
            {

                ghiThongBao(ex.ToString());
                m_Watcher2.EnableRaisingEvents = false;
            }
            finally
            {
                m_Watcher2.EnableRaisingEvents = false;
            }
        }

        /// <summary>
        /// Ghi File Excel sau khi đọc fiel Data.csv (trước khi cut)
        /// </summary>
        /// <param name="temp">số thứ tự pcs % 20. Ví dụ số thứ tự =5 => temp=5%20 =5</param>
        public void WriteToExcel(int temp)
        {
            try
            {
                string str, str1;
                //m_editLotNo.GetWindowText(str);
                //   str = cboItemName.SelectedValue.ToString();

                int t;
                if (m_iSTT % 20 != 0)
                    t = m_iSTT / 20;
                else
                    t = m_iSTT / 20 - 1;

                str1 = "_" + (t * 20 + 1).ToString() + "_" + ((t + 1) * 20).ToString();
                strFileName = "C:\\AutoMeasuringENDO\\" + LotNo + str1 + ".xls";
                //Kiểm tra nếu chưa có file này thì tạo mới. Không cho lỗi
                if (!File.Exists(strFileName))
                {
                    File.Copy("Template.xls", strFileName, true);
                    ghiThongBao("Created new file " + strFileName);
                }
                else
                {

                }
                Book book = new BinBook();
                book.setKey("Luot Nguyen Thi", "windows-2d2f25020bc0ef0263bc6d63a0n8h9od");
                book.load(strFileName);
                //Sheet sheet = book.addSheet("Main1");
                Sheet sheet = book.getSheet(1);
                //Ghi giá trị đường kính. Ghi Excel từ hàng 2 cột 6 đến hàng 2 cột 11
                for (int i = 0; i < 5; i++)
                {
                    sheet.writeNum(temp, i + 6, d_All[temp, i] / 10);
                }
                ///kết quả của việc do: OK hoặc NG
                if (d_All[temp, 5] > 0)
                {
                    sheet.writeStr(temp, 11, "OK");
                    sheet.writeStr(temp, 12, "");
                }
                else
                {
                    sheet.writeStr(temp, 11, "NG");
                    sheet.writeNum(temp, 12, d_All[temp, 6]);
                }
                //Ghi giá trị độ dài từ. Ghi Excel từ hàng 2 cột 16 đến hàng 2 cột 24
                for (int i = 0; i <= 7; i++)
                {
                     int convertInt = (int)Math.Round(l[i]);
                    str1 = convertInt.ToString();
                    
                    sheet.writeStr(temp, i + 16, str1);
                }

                if (temp >= 1 && temp <= 10)
                {
                    for (int i = 1; i <= MAX_POINT; i++)
                    {
                        sheet.writeNum(i, 0, d_No1[i - 1]);
                    }

                }

                else if (temp >= 10 && temp <= 20)
                {
                    for (int i = 1; i <= MAX_POINT; i++)
                    {
                        sheet.writeNum(i, 1, d_No1[i - 1]);
                    }
                }

                for (int i = 1; i <= 20; i++)
                {
                    sheet.writeNum(i, 15, t * 20 + i);
                }

                sheet.writeStr(1, 14, strMachineNo); //Write Machine Name: B (EB1-0008)

                // GetDlgItemTextW(IDC_COMBO1, str1);
                sheet.writeStr(2, 14, ItemName); //ItemName

                sheet.writeStr(3, 14, txtLotNo.Text); //Write LotNO

                str1 = (t * 20 + 1) + "-" + ((t + 1) * 20);
                sheet.writeStr(4, 14, str1);

                str1 = m_iQuantity.ToString();
                sheet.writeStr(5, 14, str1);

                book.save(strFileName);

                //Xuat du lieu ra bieu do
                // lblMessenge.Invoke((Action)(() => lblMessenge.Text = "------------"));
            }
            catch (System.Exception ex)
            {
                ghiThongBao(ex.ToString());

                lblMessenge.Invoke((Action)(() => lblMessenge.Text = "Error write Excel / Lỗi không ghi được dữ liệu vào file Excel"));
            }
        }

        /// <summary>
        /// Ghi File Excel với Chart
        /// </summary>
        /// <param name="temp">số thứ tự pcs % 20. Ví dụ số thứ tự =5 => temp=5%20 =5</param>
        public void WriteToExcelWithChart(int temp)
        {
            try
            {
                string str, str1;
                //m_editLotNo.GetWindowText(str);
                //   str = cboItemName.SelectedValue.ToString();

                int t;
                if (m_iSTT % 20 != 0)
                    t = m_iSTT / 20;
                else
                    t = m_iSTT / 20 - 1;

                str1 = "_" + (t * 20 + 1).ToString() + "_" + ((t + 1) * 20).ToString();
                strFileName = "C:\\AutoMeasuringENDO\\Chart.xls";

                foreach (var process in Process.GetProcessesByName("excel")) //whatever you need to close 
                {
                    if (process.MainWindowTitle.Contains("Chart.xls"))
                    {
                        process.Kill();
                        break;
                    }
                }

                if (!File.Exists(strFileName))
                {
                    File.Copy("TemplateChart222222.xls", strFileName, true);
                    ghiThongBao("Created new file " + strFileName);
                }
                else
                {
                    File.Delete(strFileName);
                    File.Copy("TemplateChart222222.xls", strFileName, true);
                    ghiThongBao("Created new file " + strFileName);
                }
                Book book = new BinBook();
                book.setKey("Luot Nguyen Thi", "windows-2d2f25020bc0ef0263bc6d63a0n8h9od");
                book.load(strFileName);

                Sheet sheet = book.getSheet(1);


                //Ghi giá trị đường kính. Ghi Excel từ hàng 2 cột 6 đến hàng 2 cột 11
                sheet.writeStr(2, 16, "ĐƯỜNG KÍNH");
                for (int i = 0; i < 5; i++)
                {
                    sheet.writeNum(i + 3, 16, d_All[temp, i] / 10);
                }

                //Ghi giá trị độ dài từ. Ghi Excel từ hàng 36 cột 13  
                sheet.writeStr(2, 10, "ĐỘ DÀI");
                for (int i = 0; i <= 7; i++)
                {
                    int convertInt = (int)Math.Round(l[i]); 
                    str1 = convertInt.ToString();
                    sheet.writeStr(i + 3, 10, str1);
                }

                //Ghi Vij trí xác định độ dài  
                sheet.writeStr(14, 10, "Vị trí ĐỘ DÀI");
                for (int i = 0; i < vitri.Length; i++)
                {
                    //int convertInt = (int)Math.Round(l[i]);

                    //str1 = convertInt.ToString();
                    sheet.writeStr(i + 15, 10, vitri[i].ToString());
                }

                //In ra excel giá trị đo từ PLC
                for (int i = 1; i <= MAX_POINT; i++)
                {
                    //Ghi ra tất cả đường kính
                    sheet.writeNum(i + 2, 1, d_No1[i - 1]);

                    //ghi ra vị trí đường kính xác định đoạn. Ở trên file Excel sẽ dựa vào các giá trị này mà Vlookup ra đường kính.
                    //Mục đích để đánh dấu trên đồ thị
                    if (i == vitri[0] - 1 || i == vitri[1] || i == vitri[2] || i == vitri[3] || i == vitri[4] || i == vitri[5] || i == vitri[6] || i == vitri[7])
                    {
                        sheet.writeNum(i + 2, 3, 1);
                    }
                    else
                    {
                        sheet.writeNum(i + 2, 3, 0);
                    }
                }
                sheet.writeStr(2, 17, "INFOR");
                sheet.writeStr(3, 17, strMachineNo); //Write Machine Name: B (EB1-0008)

                // GetDlgItemTextW(IDC_COMBO1, str1);
                sheet.writeStr(4, 17, ItemName); //ItemName

                sheet.writeStr(5, 17, txtLotNo.Text); //Write LotNO

                str1 = (t * 20 + 1) + "-" + ((t + 1) * 20);
                sheet.writeStr(6, 17, str1);

                str1 = m_iQuantity.ToString();
                sheet.writeStr(7, 17, str1);

                book.save(strFileName);

                //Xuat du lieu ra bieu do
                //   lblMessenge.Invoke((Action)(() => lblMessenge.Text = "------------"));
            }
            catch (System.Exception ex)
            {
                ghiThongBao(ex.ToString());

                lblMessenge.Invoke((Action)(() => lblMessenge.Text = "Error write Excel / Lỗi không ghi được dữ liệu vào file Excel"));
            }
        }
        /// <summary>
        /// Xuat du lieu ra bieu do
        /// </summary>
        /// <param name="d_No1">Table chua du lieu</param>

        /// <summary>
        /// Ghi File Excel sau khi đọc file Data2.csv (sau khi cut)
        /// </summary>
        /// <param name="temp"></param>
        public void WriteToExcel2(int temp)
        {
            try
            {
                string str, str1;
                strFileName2 = "C:\\AutoMeasuringENDO\\" + LotNo + "_After_cutting_floppy.xls";
                //Nếu không có file thì tạo mới
                if (!File.Exists(strFileName))
                {
                    File.Copy("Template2.xls", strFileName, true);
                    ghiThongBao("Created new file " + strFileName);
                }
                Book book = new BinBook();
                book.setKey("Luot Nguyen Thi", "windows-2d2f25020bc0ef0263bc6d63a0n8h9od");
                book.load(strFileName2);
                Sheet sheet = book.getSheet(1);

                if (((Math.Round(l2) == m_iPara[29]) && (l2 > m_iPara[29])) || ((Math.Round(l2) == m_iPara[30]) && (l2 < m_iPara[30])))
                    sheet.writeNum(temp, 6, Math.Round(l2));
                //else if (((l[0] == m_iPara[29] || l[0] == m_iPara[30])))
                //    sheet.writeNum(temp, 6, Math.Round(l[0]));
                else
                    sheet.writeNum(temp, 6, l2);
                //Ghi Tên máy, Tên Item, Lot No//
                sheet.writeStr(1, 14, strMachineNo);
                sheet.writeStr(2, 14, ItemName);
                sheet.writeStr(3, 14, LotNo);
                //Ghi Số lượng của Lot đó//
                sheet.writeStr(5, 14, m_iQuantity.ToString());
                // sheet.writeNum(5, 14, m_iQuantity);
                temp = temp % 20;
                //Ghi dữ liệu biểu đồ sau cut fllopy
                if (temp >= 1 && temp <= 10)
                {
                    for (int i = 0; i < 350; i++)
                    {
                        sheet.writeNum(i + 1, 0, d_No2[i]);
                    }
                }
                else if (temp >= 10 && temp <= 20)
                {
                    for (int i = 0; i < 350; i++)
                    {
                        sheet.writeNum(i + 1, 1, d_No2[i]);
                    }
                }
                book.save(strFileName2);
            }
            catch (System.Exception ex)
            {
                ghiThongBao(ex.ToString());
                MessageBox.Show("Lỗi không ghi được dữ liệu sau cut floppy", "ERROR Write floppy file");
            }


        }

        public string returnUrl(int temp)
        {
            string str1 = "";
            string strFileName = "";
            int t;
            if (m_iSTT % 20 != 0)
                t = m_iSTT / 20;
            else
                t = m_iSTT / 20 - 1;

            str1 = "_" + (t * 20 + 1).ToString() + "_" + ((t + 1) * 20).ToString();
            strFileName = "C:\\AutoMeasuringENDO\\" + LotNo + str1 + ".xls";
            return strFileName;

        }


        /// <summary>
        /// Lưu kết quả hiển thị cũ
        /// </summary>
        public void SaveDisplayResult()
        {
            txtD1_4.Invoke((MethodInvoker)(() => txtD1_4.Text = txtD1_3.Text));
            txtD2_4.Invoke((MethodInvoker)(() => txtD2_4.Text = txtD2_3.Text));
            txtL1_4.Invoke((MethodInvoker)(() => txtL1_4.Text = txtL1_3.Text));
            txtL2_4.Invoke((MethodInvoker)(() => txtL2_4.Text = txtL2_3.Text));

            txtD1_3.Invoke((MethodInvoker)(() => txtD1_3.Text = txtD1_2.Text));
            txtD2_3.Invoke((MethodInvoker)(() => txtD2_3.Text = txtD2_2.Text));
            txtL1_3.Invoke((MethodInvoker)(() => txtL1_3.Text = txtL1_2.Text));
            txtL2_3.Invoke((MethodInvoker)(() => txtL2_3.Text = txtL2_2.Text));


            txtD1_2.Invoke((MethodInvoker)(() => txtD1_2.Text = txtD1_1.Text));
            txtD2_2.Invoke((MethodInvoker)(() => txtD2_2.Text = txtD2_1.Text));
            txtL1_2.Invoke((MethodInvoker)(() => txtL1_2.Text = txtL1_1.Text));
            txtL2_2.Invoke((MethodInvoker)(() => txtL2_2.Text = txtL2_1.Text));

            txtD1_1.Invoke((MethodInvoker)(() => txtD1_1.Text = ""));
            txtD2_1.Invoke((MethodInvoker)(() => txtD2_1.Text = "")); 
            txtL1_1.Invoke((MethodInvoker)(() => txtL1_1.Text = ""));
            txtL2_1.Invoke((MethodInvoker)(() => txtL2_1.Text = ""));
           

            txtJudge_4.Invoke((MethodInvoker)(() => txtJudge_4.Text = txtJudge_3.Text));
            txtJudge_3.Invoke((MethodInvoker)(() => txtJudge_3.Text = txtJudge_2.Text));
            txtJudge_2.Invoke((MethodInvoker)(() => txtJudge_2.Text = txtJudge_1.Text));
            txtJudge_1.Invoke((MethodInvoker)(() => txtJudge_1.Text = ""));

        }
        /// <summary>
        /// Hiển thị kết quả lên các ô text Trước khi cut
        /// </summary>
        /// <param name="temp"></param>
        public void DisplayResultBeforeCut(int temp)
        {
            //Làm tròn

            //d_All[temp20, 0] / 10
            //Luôn hiển thị ở dòng đầu tiên

            double varTemp = 0;
            varTemp = Convert.ToDouble(d_All[temp, 0] / 10);
            varTemp = varTemp / 1000;
            txtD1_1.Invoke((MethodInvoker)(() => txtD1_1.Text = varTemp.ToString("0.###")));

            varTemp = Convert.ToDouble(d_All[temp, 1] / 10);
            varTemp = varTemp / 1000;
            txtD2_1.Invoke((MethodInvoker)(() => txtD2_1.Text = varTemp.ToString("0.###")));

            txtL1_1.Invoke((MethodInvoker)(() => txtL1_1.Text = Math.Round(l[0]).ToString()));
            txtL2_1.Invoke((MethodInvoker)(() => txtL2_1.Text = Math.Round(l[1]).ToString()));



            int vl = d_All[temp, 5];
            if (d_All[temp, 5] <= 0)
            {

                txtJudge_1.Invoke((MethodInvoker)(() => txtJudge_1.Text = "NG"));
                //txtJudge_1.Invoke((MethodInvoker)(() => txtJudge_1.Text = d_All[temp, 6].ToString()));
            }
            else
            {

                txtJudge_1.Invoke((MethodInvoker)(() => txtJudge_1.Text = "OK"));
            }
        } 


        double sumxy(int[] a, int s, int e)
        {
            double kq = 0;
            for (int i = s; i >= e; i--)
                kq += i * a[i];
            return kq;
        }


        double sumx(int s, int e)
        {
            double kq = 0;
            for (int i = s; i >= e; i++)
                kq += i;
            return kq;
        }


        double sumy(int[] a, int s, int e)
        {
            double kq = 0;
            for (int i = s; i >= e; i++)
                kq += a[i];
            return kq;
        }

        double sumx2(int s, int e)
        {
            double kq = 0;
            for (int i = s; i >= e; i++)
                kq += i * i;
            return kq;
        }



        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 a = new AboutBox1();
            a.ShowDialog();
        }

        private void btnPrintMainData_Click(object sender, EventArgs e)
        {
            try
            {
                string printMode = cboPrintMode.Text.ToString();
                switch (printMode)
                {
                    case "BY Order":
                        // TODO: Add your control notification handler code here
                        m_iFromPcs = int.Parse(txtFromPCS.Text);
                        m_iToPcs = int.Parse(txtToPCS.Text);

                        if ((m_iFromPcs > m_iToPcs) || (m_iToPcs > int.Parse(txtSTT.Text)))
                        {
                            ghiThongBao("Print mode ERROR! May be number From PCS or To PCS incorrect \r\n Số thứ tự pcs nhập vào không hợp lệ");
                            return;
                        }
                        else
                        {
                            int m_iStartSheet;
                            if (m_iFromPcs % 20 == 0)
                                m_iStartSheet = m_iFromPcs / 20;
                            else m_iStartSheet = m_iFromPcs / 20 + 1;
                            int m_iEndSheet;
                            if (m_iToPcs % 20 == 0)
                                m_iEndSheet = m_iToPcs / 20;
                            else m_iEndSheet = m_iToPcs / 20 + 1;

                            string str = txtLotNo.Text.Trim(), str1, str2;
                            for (int i = m_iStartSheet; i <= m_iEndSheet; i++)
                            {


                                str1 = "_" + ((i - 1) * 20 + 1).ToString() + "_" + (i * 20).ToString();

                                str2 = "C:\\AutoMeasuringENDO\\" + str + str1 + ".xls";
                                if (!File.Exists(str2))
                                {
                                    ghiThongBao("Không tìm thấy file bạn yêu cầu \r\n Find not found !");
                                }
                                else
                                {
                                    PrintMyExcelFile(str2);
                                    ghiThongBao("Printed checking file!");
                                }


                            }
                        }

                        break;
                    case "ALL LOT":
                        // TODO: Add your control notification handler code here
                        if (string.IsNullOrEmpty(txtToPCS.Text)==true)
                        {
                            txtToPCS.Text = "0";
                        }
                        m_iFromPcs = 1;
                        
                        m_iToPcs = int.Parse(txtToPCS.Text);

                        if (m_iToPcs == 0)
                        {
                            ghiThongBao("Chưa nhập số vị trí số pcs cần in");
                            MessageBox.Show("Bạn cần nhập số PCS Cần IN (To PCS phải lớn hơn 1)");
                            return;
                        }
                        if ((m_iFromPcs > m_iToPcs) || (m_iToPcs > m_iQuantity))
                        {
                            MessageBox.Show("Máy đang dừng, không có dữ liệu");
                            ghiThongBao("Print mode ERROR!");
                            return;
                        }
                        else
                        {
                            int m_iStartSheet;
                            if (m_iFromPcs % 20 == 0)
                                m_iStartSheet = m_iFromPcs / 20;
                            else m_iStartSheet = m_iFromPcs / 20 + 1;
                            int m_iEndSheet;
                            if (m_iToPcs % 20 == 0)
                                m_iEndSheet = m_iToPcs / 20;
                            else m_iEndSheet = m_iToPcs / 20 + 1;

                            string str = txtLotNo.Text.Trim(), str1, str2;
                            for (int i = m_iStartSheet; i <= m_iEndSheet; i++)
                            {


                                str1 = "_" + ((i - 1) * 20 + 1).ToString() + "_" + (i * 20).ToString();

                                str2 = "C:\\AutoMeasuringENDO\\" + str + str1 + ".xls";
                                if (!File.Exists(str2))
                                {
                                    ghiThongBao("Không tìm thấy file bạn yêu cầu \r\n Find not found !");
                                    MessageBox.Show("Không tìm thấy file bạn yêu cầu \r\n Find not found !");
                                }
                                else
                                {
                                    PrintMyExcelFile(str2);
                                    ghiThongBao("Printed checking file!");
                                    MessageBox.Show("Printed checking file! Đã in xong, mời kiểm tra máy in","Thông báo");
                                }


                            }
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (System.Exception ex)
            {

                MessageBox.Show("Có lỗi khi in, vui lòng kiểm tra lại", "Có lỗi khi in");
                
            }


        }
        /// <summary>
        /// Hàm in dữ liệu
        /// </summary>
        /// <param name="fileExcel"></param>
        void PrintMyExcelFile(string fileExcel)
        {
            Excel.Application excelApp = new Excel.Application();

            // Open the Workbook:
            Excel.Workbook wb = excelApp.Workbooks.Open(fileExcel,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            // Get the first worksheet.
            // (Excel uses base 1 indexing, not base 0.)
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];

            // Print out 1 copy to the default printer:
            ws.PrintOut(
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            // Cleanup:
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Marshal.FinalReleaseComObject(ws);

            wb.Close(false, Type.Missing, Type.Missing);
            Marshal.FinalReleaseComObject(wb);

            excelApp.Quit();
            Marshal.FinalReleaseComObject(excelApp);
        }
        private void btnPrintAfterCutting_Click(object sender, EventArgs e)
        {
            try
            {
                strFileName2 = "C:\\AutoMeasuringENDO\\" + LotNo + "_After_cutting_floppy" + ".xls";
                if (!File.Exists(strFileName2))
                {
                    MessageBox.Show("Không tìm thấy file bạn yêu cầu \r\n Find not found !");
                }
                else
                {
                    PrintMyExcelFile(strFileName2);
                    ghiThongBao("Printed data after cutting!");
                }

            }
            catch (System.Exception)
            {


            }

        }
        /// <summary>
        /// Hàm thông báo lỗi không hoạt động
        /// </summary>
        public void baoLoi()
        {
            bienNhoTime = 0;
            timer.Interval = 300;

            timer.Enabled = true;
            timer.Start();
            timer.Tick += new EventHandler(timer_Tick);
        }
        /// <summary>
        /// Sự kiện thông báo lỗi bằng màu và âm thanh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            bienNhoTime = bienNhoTime + 1;
            if (bienNhoTime >= 150)
            {
                timer.Enabled = false;
                timer.Stop();
                this.BackColor = DefaultBackColor;
            }
            else
            {

                //this.BackColor.Invoke((Action)(() => this.BackColor = System.Drawing.Color.Red));
                if (bienNhoTime % 5 == 0)
                {
                    this.BackColor = System.Drawing.Color.Red;

                }
                else
                {
                    this.BackColor = System.Drawing.Color.Orange;

                }
                if (bienNhoTime % 10 == 0)
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer("errorMsg.wav");
                    player.Play();
                }


            }
        }

        



        private void cboPrintMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string printMode = cboPrintMode.Text.ToString();
            //switch (printMode)
            //{
            //    case "BY Order":
            //        groupBoxFromPCSToPCS.Visible = true;
            //        txtFromPCS.Text = 1.ToString();
            //        txtToPCS.Text = txtSTT.Text;
            //        break;
            //    case "ALL LOT":
            //        groupBoxFromPCSToPCS.Visible = false;
            //        txtFromPCS.Text = 1.ToString();
            //        txtToPCS.Text = txtSTT.Text;
            //        break;
            //    default:
            //        groupBoxFromPCSToPCS.Visible = false;
            //        break;
            //}
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmProperty f = new FrmProperty();
            f.ShowDialog();
        }

        private void refreshConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loadConfigFile() == true && loadBuGiaTri(ItemName) == true)
            {
                MessageBox.Show("Đã cập nhật thay đổi cấu hình !!! \r\nConfig updated !!!", "THÔNG BÁO");
            }
            else
            {
                MessageBox.Show("Không cập nhật được cấu hình mới !!! \r\n Can not update !!!", "THÔNG BÁO");
            }
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inforGroupITEM f = new inforGroupITEM();
            f.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void openFileResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = "C:\\AutoMeasuringENDO\\"; // vd: "D:"
            prc.Start();
        }

        private void fTPUploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = "C:\\FTPUpload\\"; // vd: "D:"
            prc.Start();
        }

        private void openFileBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = "C:\\FileBackupENDO\\"; // vd: "D:"
            prc.Start();
        }

        private void refeshConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loadConfigFile() == true && loadBuGiaTri(ItemName) == true)
            {
                MessageBox.Show("Đã cập nhật thay đổi cấu hình !!! \r\nConfig updated !!!", "THÔNG BÁO");
            }
            else
            {
                MessageBox.Show("Không cập nhật được cấu hình mới !!! \r\n Can not update !!!", "THÔNG BÁO");
            }

            firstLoadChart(chartCheck);
           
            loadBuGiaTri(ItemName);
        }

        private void fTPUploadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = "C:\\FTPUpload\\"; // vd: "D:"
            prc.Start();
        }

        private void btnOpenExcel_Click(object sender, EventArgs e)
        {
            FileInfo fi = new FileInfo("C:\\AutoMeasuringENDO\\Chart.xls");
            if (fi.Exists)
            {
                System.Diagnostics.Process.Start(@"C:\\AutoMeasuringENDO\\Chart.xls");
            }
            else
            {
                //file doesn't exist
            }
        }

        /// <summary>
        /// Để test 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            int c = d_No1.Count();
            List<int> listData = new List<int>();
            for (int i = 0; i < c; i++)
            {
                listData.Add(d_No1[i]);
            }
            UpdateChart(listData);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
          
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = "C:\\AutoMeasuringENDO\\"; // vd: "D:"
            prc.Start();

            
        }

        private void folderSoftwaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // string b = System.IO.Path.GetDirectoryName(
            //System.Reflection.Assembly.GetExecutingAssembly().Location);
            // string a = Application.StartupPath;

            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = Application.StartupPath;
            prc.Start();
      
            
        }

        private void chartCheck_MouseWheel(object sender, MouseEventArgs e)
        {

            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            try
            {
                if (e.Delta < 0) // Scrolled down.
                {
                    xAxis.ScaleView.ZoomReset();
                    yAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    var yMin = yAxis.ScaleView.ViewMinimum;
                    var yMax = yAxis.ScaleView.ViewMaximum;

                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }

        private void numericUpDownWidthChart_ValueChanged(object sender, EventArgs e)
        {
            chartCheck.ChartAreas["ChartArea1"].AxisX.Maximum = int.Parse(numericUpDownWidthChart.Value.ToString());
        }

        private void numericUpDownHeightChart_ValueChanged(object sender, EventArgs e)
        {
            chartCheck.ChartAreas["ChartArea1"].AxisY.Maximum = int.Parse(numericUpDownHeightChart.Value.ToString());
        }
    }
}
