using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Threading;

namespace AutoDoDuongKinh
{
   public static class clsDocFile
    {
        /// <summary>
        /// Load file Setting
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<clsSetting1> LoadCsvFile(string filePath)
        {
            
            List<clsSetting1> listObj = new List<clsSetting1>();
            
            var reader = new StreamReader(File.OpenRead(filePath));
         
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
              
                string[] str = line.Split(',');
                if (str[0]== "ItemName")
                {

                }
                else
                {
                    clsSetting1 cls = new clsSetting1();
                    cls.ItemName = str[0];
                    cls.PARA_1 = str[1];
                    cls.PARA_2 = str[2];
                    cls.PARA_3 = str[3];
                    cls.PARA_4 = str[4];
                    cls.PARA_5 = str[5];
                    cls.PARA_6 = str[6];
                    cls.PARA_7 = str[7];
                    cls.PARA_8 = str[8];
                    cls.PARA_9 = str[9];
                    cls.PARA_10 = str[10];
                    cls.PARA_11 = str[11];
                    cls.PARA_12 = str[12];
                    cls.PARA_13 = str[13];
                    cls.PARA_14 = str[14];
                    cls.PARA_15 = str[15];
                    cls.PARA_16 = str[16];
                    cls.PARA_17 = str[17];
                    cls.PARA_18 = str[18];
                    cls.PARA_19 = str[19];
                    cls.PARA_20 = str[20];
                    cls.PARA_21 = str[21];
                    cls.PARA_22 = str[22];
                    cls.PARA_23 = str[23];
                    cls.PARA_24 = str[24];
                    cls.PARA_25 = str[25];
                    cls.PARA_26 = str[26];
                    cls.PARA_27 = str[27];
                    cls.PARA_28 = str[28];
                    cls.PARA_29 = str[29];
                    cls.PARA_30 = str[30];
                    cls.PARA_31 = str[31];
                    cls.PARA_32 = str[32];
                    cls.PARA_33 = str[33];
                    
                    listObj.Add(cls);
                }
                //    searchList.Add(line);
            }
            return listObj;
        }
        /// <summary>
        /// Load file chứa các biến cần bù dữ liệu
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<clsBuGiaTri> LoadCsvFile_BuGiaTri(string filePath)
        {

            List<clsBuGiaTri> listObj = new List<clsBuGiaTri>();

            var reader = new StreamReader(File.OpenRead(filePath));

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                string[] str = line.Split(',');
                if (str[0] == "ItemName")
                {

                }
                else
                {
                    clsBuGiaTri cls = new clsBuGiaTri();
                    cls.ItemName = str[0];
                    cls.X0 = int.Parse(str[1].Trim());
                    cls.X2 = int.Parse(str[2].Trim());
                    cls.X3 = int.Parse(str[3].Trim());
                    cls.X4 = int.Parse(str[4].Trim());
                    cls.X5 = int.Parse(str[5].Trim());
                    cls.X6 = int.Parse(str[6].Trim());
                    cls.X7 = int.Parse(str[7].Trim());
                    cls.X1 = int.Parse(str[8].Trim());
                    cls.y0_DoanL1 = int.Parse(str[9].Trim());
                    cls.y0_DoanL2 = int.Parse(str[10].Trim());                
                    cls.y0_DoanL3 = int.Parse(str[11].Trim());
                    cls.y0_DoanL4 = int.Parse(str[12].Trim());
                    cls.y0_DoanL5 = int.Parse(str[13].Trim());
                    cls.y0_DoanL6 = int.Parse(str[14]);
                    cls.y0_DoanL7 = int.Parse(str[15]);
                    cls.PARA_16 = int.Parse(str[16]);
                    cls.dk1 = int.Parse(str[17]);
                    cls.dk2 = int.Parse(str[18]);
                    cls.dk3 = int.Parse(str[19]);
                    cls.dk4 = int.Parse(str[20]);
                    cls.dk5 = int.Parse(str[21]);
                    cls.PARA_22 = str[22];
                    cls.PARA_23 = str[23];
                    cls.PARA_24 = str[24];
                    cls.PARA_25 = str[25];
                    cls.PARA_26 = str[26];
                    cls.D_L5 = int.Parse(str[27]);
                    cls.D_L4 = int.Parse(str[28]);
                    cls.D_L3 = int.Parse(str[29]);
                    cls.D_L2 = int.Parse(str[30]);
                    cls.D_After = int.Parse(str[31]);                  
                    cls.D_L1 = int.Parse(str[32]);

                    listObj.Add(cls);
                }
                //    searchList.Add(line);
            }
            return listObj;
        }
        /// <summary>
        /// Đọc dữ liệu từ file ItemSetting.csv.
        /// Trả về DataTable chứa danh sách ItemSetting1
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns> dt</returns>
        public static DataTable LoadTableCsvFile(string filePath)
        {
            DataTable dt = new DataTable();
           
            var reader = new StreamReader(File.OpenRead(filePath));

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                string[] arrayLine = line.Split(',');
                if (arrayLine[0] == "ItemName")
                {
                    for (int j = 0; j < arrayLine.Length; j++)
                    {
                        dt.Columns.Add(arrayLine[j]);
                    }

                }
                else
                {
                    DataRow dr = dt.NewRow();
                    for (int k = 0; k < arrayLine.Length; k++)
                    {
                        dr[k] = arrayLine[k].ToString();
                    }
                    dt.Rows.Add(dr);
                }
            }

            reader.Dispose();
            reader.Close();
            return dt;
        }

        /// <summary>
        /// Trả về DataTable chứa danh sách Du Lieu Data.csv, 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DataTable LoadDataCSV(string filePath)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("col1");
            dt.Columns.Add("col2");
            dt.Columns.Add("col3");

            var reader = new StreamReader(File.OpenRead(filePath));
            
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                string[] arrayLine = line.Split(',');
                
                    DataRow dr = dt.NewRow();
                    
                    dr[0]= arrayLine[0].ToString();
                dr[1] = arrayLine[1].ToString();
                dr[2] = arrayLine[2].ToString();
                dt.Rows.Add(dr);
                
            }
            reader.Close();
            return dt;
        }
        /// <summary>
        /// Trả về DataTable chứa danh sách Du Lieu Data2.csv
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DataTable LoadData2CSV(string filePath)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("col1");
            dt.Columns.Add("col2");
            dt.Columns.Add("col3");

            var reader = new StreamReader(File.OpenRead(filePath));

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                string[] arrayLine = line.Split(',');

                DataRow dr = dt.NewRow();

                dr[0] = arrayLine[0].ToString();
                dr[1] = arrayLine[1].ToString();
                dr[2] = arrayLine[2].ToString();
                dt.Rows.Add(dr);

            }
            reader.Dispose();
            reader.Close();
            return dt;
        }
        /// <summary>
        /// Ghi dữ liệu vào file ItemSetting.csv khi click Set Lot;
        /// Gắn dữ liệu vào mảng số nguyên m_iPara;
        /// PLC Sẽ đọc dữ liệu từ file ItemSetting.csv để biết đường kính Core, độ dài core tiêu chuẩn để điều khiển roler
        /// </summary>
        /// <param name="itemName"></param>

        public static int[] GetItemConfiguration(string itemName, string quantity)
        {
            int[] m_iPara = new int[34];
            try
            {

                List<clsSetting1> listCls = clsDocFile.LoadCsvFile("ItemSetting_Final.csv");
                clsSetting1 cls = listCls.Find(x => x.ItemName == itemName);
                string path = @"C:\FTPUpload\ItemSetting.csv";
                List<String> lines = new List<String>();
                lines.Add("EM06000," + quantity + ",1" + "\r\n");
                lines.Add("EM06001," + cls.PARA_1 + ",1" + "\r\n");
                lines.Add("EM06002," + cls.PARA_2 + ",1" + "\r\n");
                lines.Add("EM06003," + cls.PARA_3 + ",1" + "\r\n");
                lines.Add("EM06004," + cls.PARA_4 + ",1" + "\r\n");
                lines.Add("EM06005," + cls.PARA_5 + ",1" + "\r\n");
                lines.Add("EM06006," + cls.PARA_6 + ",1" + "\r\n");
                lines.Add("EM06007," + cls.PARA_7 + ",1" + "\r\n");
                lines.Add("EM06008," + cls.PARA_8 + ",1" + "\r\n");
                lines.Add("EM06009," + cls.PARA_11 + ",1" + "\r\n");  //Độ dài tiêu chuẩn Max đoạn 10 mm
                lines.Add("EM06010," + cls.PARA_12 + ",1" + "\r\n");  //Độ dài tiêu chuẩn Min đoạn 10 mm
                lines.Add("EM06011," + cls.PARA_9 + ",1" + "\r\n");   //Độ dài tiêu chuẩn Max đoạn vát 20 mm
                lines.Add("EM06012," + cls.PARA_10 + ",1" + "\r\n");  //Độ dài tiêu chuẩn Max đoạn vát 20 mm
                lines.Add("EM06013," + cls.PARA_13 + ",1" + "\r\n");
                lines.Add("EM06014," + cls.PARA_14 + ",1" + "\r\n");
                lines.Add("EM06015," + cls.PARA_15 + ",1" + "\r\n");
                lines.Add("EM06016," + cls.PARA_16 + ",1" + "\r\n");
                lines.Add("EM06017," + cls.PARA_17 + ",1" + "\r\n");
                lines.Add("EM06018," + cls.PARA_18 + ",1" + "\r\n");
                lines.Add("EM06019," + cls.PARA_19 + ",1" + "\r\n");
                lines.Add("EM06020," + cls.PARA_20 + ",1" + "\r\n");
                lines.Add("EM06021," + cls.PARA_21 + ",1" + "\r\n");
                lines.Add("EM06022," + cls.PARA_22 + ",1" + "\r\n");
                lines.Add("EM06023," + cls.PARA_23 + ",1" + "\r\n");
                lines.Add("EM06024," + cls.PARA_24 + ",1" + "\r\n");
                lines.Add("EM06025," + cls.PARA_25 + ",1" + "\r\n");
                lines.Add("EM06026," + cls.PARA_26 + ",1" + "\r\n");
                lines.Add("EM06027," + cls.PARA_27 + ",1" + "\r\n");
                lines.Add("EM06028," + cls.PARA_28 + ",1" + "\r\n");
                lines.Add("EM06029," + cls.PARA_29 + ",1" + "\r\n");
                lines.Add("EM06030," + cls.PARA_30 + ",1" + "\r\n");
                lines.Add("EM06031," + cls.PARA_31 + ",1" + "\r\n");
                lines.Add("EM06032," + cls.PARA_32 + ",1" + "\r\n");
                lines.Add("EM06033," + cls.PARA_33 + ",1" + "\r\n");
                m_iPara[1] = int.Parse(cls.PARA_1);
                m_iPara[2] = int.Parse(cls.PARA_2);
                m_iPara[3] = int.Parse(cls.PARA_3);
                m_iPara[4] = int.Parse(cls.PARA_4);
                m_iPara[5] = int.Parse(cls.PARA_5);
                m_iPara[6] = int.Parse(cls.PARA_6);
                m_iPara[7] = int.Parse(cls.PARA_7);
                m_iPara[8] = int.Parse(cls.PARA_8);
                m_iPara[9] = int.Parse(cls.PARA_9);
                m_iPara[10] = int.Parse(cls.PARA_10);
                m_iPara[11] = int.Parse(cls.PARA_11);
                m_iPara[12] = int.Parse(cls.PARA_12);
                m_iPara[13] = int.Parse(cls.PARA_13);
                m_iPara[14] = int.Parse(cls.PARA_14);
                m_iPara[15] = int.Parse(cls.PARA_15);
                m_iPara[16] = int.Parse(cls.PARA_16);
                m_iPara[17] = int.Parse(cls.PARA_17);
                m_iPara[18] = int.Parse(cls.PARA_18);
                m_iPara[19] = int.Parse(cls.PARA_19);
                m_iPara[20] = int.Parse(cls.PARA_20);
                m_iPara[21] = int.Parse(cls.PARA_21);
                m_iPara[22] = int.Parse(cls.PARA_22);
                m_iPara[23] = int.Parse(cls.PARA_23);
                m_iPara[24] = int.Parse(cls.PARA_24);
                m_iPara[25] = int.Parse(cls.PARA_25);
                m_iPara[26] = int.Parse(cls.PARA_26);
                m_iPara[27] = int.Parse(cls.PARA_27);
                m_iPara[28] = int.Parse(cls.PARA_28);
                m_iPara[29] = int.Parse(cls.PARA_29);
                m_iPara[30] = int.Parse(cls.PARA_30);
                m_iPara[31] = int.Parse(cls.PARA_31);
                m_iPara[32] = int.Parse(cls.PARA_32);
                m_iPara[33] = int.Parse(cls.PARA_33);
         

                File.WriteAllText(path, "");
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    foreach (string line in lines)
                        writer.Write(line);
                }
                return m_iPara;
            }
            catch (System.Exception e)
            {
                
               
                System.Windows.Forms.MessageBox.Show(e.ToString());
               
                return m_iPara;
               
            }

        }
        public static void writeCSVwithDeletefile(string contents,string path)
        {
            
            if (File.Exists(path))
            {
                File.Delete(path);
               
            }
            using (StreamWriter fsResult = File.CreateText(path))
            {
                fsResult.Write(contents);
                fsResult.Dispose();
                fsResult.Close();
            }
        }
        public static string writeCSVNotDelete(string contents, string path)
        {
            string result = "";
            int count = 0;
            try
            {
                if (File.Exists(path) == true)
                {
                    File.WriteAllText(path, "");
                  
                }

                using (StreamWriter fsResult = File.CreateText(path))
                {
                    fsResult.Write(contents);
                }
                result = "Writed file" + path;
            }
            catch (Exception ex)
            {
                result = "ERROR: " + ex.ToString();
                    
               // MessageBox.Show(ex.ToString());
            }

            return result;   
        }
        
    }
}
