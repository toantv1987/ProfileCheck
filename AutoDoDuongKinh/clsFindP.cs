using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDoDuongKinh
{
   public class clsFindP
    {
        bool bUseCheckKizu = true;
        /// <summary>
        /// Kiểm tra Vị trí đo có đường kính đúng như bản vẽ hay không
        /// </summary>
        /// <param name="a">Mảng giá trị lấy từ Data.csv</param>
        /// <param name="temp"> = Thứ tự của pcs đang chạy % 20</param>
        /// <returns>Trả về True hoặc False. Là kết quả tính toàn đường kính</returns>
        public bool FindP(string ItemName,int[] a, int temp,int[] p,int[] m_iPara, int[,] d_All,double[] l,int[] x)
        {
            //for (int i = 0; i < a.Length; i++)
            //{
            //   a[i]=(a[i]/100)*100;
            //}
            //y0; giá trị đường kính trung bình nằm giữa 2 điểm đo.
            double beta = 0, alpha = 0, y0 = 0, tbX = 0, tbY = 0, Sxx = 0, Sxy = 0;
            double beta2 = 0, alpha2 = 0, tbX2 = 0, tbY2 = 0, Sxx2 = 0, Sxy2 = 0;
            double tb, k;
            int x1, x2, y1, y2;
            int[] b = new int[7];
            int[] w = new int[7];
            //bKizu = true;
            //bWave = true;
            //maxDistance = 0;
            //maxDisPos = 0;
            int REPEAT = 1;
            //string itemName = cboItemName.SelectedValue.ToString().Trim();

            //********NHÓM 1********

            if (ItemName == "11CA011-0-US")
            {

                //ĐIỂM ĐO D4
                p[1] = p[0] - (m_iPara[22] - 2) * 5;

                y0 = Average(a, p[0] - 60, p[1]) + 5;
                for (int i = 0; i <= 1; i++)
                {
                    if (i == 0)
                    {
                        //p[2] =2878- 54*5
                        //p3[2608]- (30-6)*5
                        p[2] = p[0] - m_iPara[21] * 5;
                        p[3] = p[2] - (m_iPara[20] - 6) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[20] * 5;
                    }
                    //Tính trung bình cộng của tất cả VỊ TRÍ trong khoảng p[2] đến p[3]. Test: trung bình cộng chạy từ 2608 đến 2488 
                    //tbX=(p[2]+p[3])/
                    tbX = averageX(p[2], p[3]);
                    //Tính trung bình cộng đường kính của tất cả các điểm từ điểm p[2] đến p[3]
                    tbY = Average(a, p[2], p[3]);

                    //??? 
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    double SxxTTT = ConsiderSxx(p[2], p[3], tbX);
                    //??? để làm gì
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    double SxyTTT = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx; //Tìm hệ số góc của phương trình đường thẳng đoạn p[2], p[3]
                    else return false;

                    alpha = tbY - beta * tbX; //b=y-a*x
                    double alphaPhong30Giay = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);

                    //x[0] =y0 - alpha) / beta
                }

                //Đường kính D4
                d_All[temp, 3] = (int)Math.Round(Average(a, p[0] - 50, x[0]));
                //Đoạn L7
                l[6] = round(((p[0] - x[0]) * 0.2));
                //ĐIỂM ĐO D3

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[19] + 1) * 5;
                        p[5] = p[4] - (m_iPara[18] - 6) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[18] * 5;
                    }
                    tbX2 = averageX(p[4], p[5]);
                    tbY2 = Average(a, p[4], p[5]);
                    Sxx2 = ConsiderSxx(p[4], p[5], tbX2);
                    Sxy2 = ConsiderSxy(a, p[4], p[5], tbX2, tbY2);
                    if ((Sxx2 != 0) && (Sxy2 != 0))
                        beta2 = Sxy2 / Sxx2;
                    else
                        return false;

                    alpha2 = tbY2 - beta2 * tbX2;

                    if (beta != beta2)
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta));
                    else
                        return false;

                    x[2] = x[1];
                    //d_All[temp][2] = beta*x[1] + alpha;
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, x[1] + 5, x[1]));
                //Đoạn L6            
                l[5] = round((x[0] - x[1]) * 0.2);
                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                //Bắt Đầu ĐIỂM ĐO D2
                p[6] = x[2] - (m_iPara[17]) * 5;
                p[7] = p[6] - (m_iPara[16] - 10) * 5;

                y0 = Average(a, p[6], p[7]);

                x[3] = (int)Math.Round((y0 - alpha2) / beta2 - 10);
                //Đoạn L5
                l[4] = round((x[1] - x[3]) * 0.2);
                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 10) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - m_iPara[14] * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else
                        return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta) - 10;
                }
                //Đoạn L4
                l[3] = round((x[3] - x[4]) * 0.2);
                

                d_All[temp, 1] = (int)Math.Round(Average(a, x[3], x[4]));
                //#Hết ĐIỂM ĐO D2

                //Bắt Đầu ĐIỂM ĐO D1
                p[10] = x[4] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[5] = (int)Math.Round((y0 - alpha) / beta) - 10;
                //Đoạn L3
                l[2] = round((x[4] - x[5]) * 0.2);
                if (bUseCheckKizu)
                {
                    //b[4] = CheckKizu(a, x[4], x[5], beta, alpha);
                    //w[4] = CheckWave(a, x[4], x[5], beta, alpha, 2);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[5] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }
                    else
                    {
                        p[12] = x[6];
                        p[13] = p[12] - m_iPara[10] * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[6] = (int)Math.Round((y0 - alpha) / beta) + 10;
                }
                //Đoạn L2
                l[1] = round((x[5] - x[6]) * 0.2);
                if (bUseCheckKizu)
                {
                    //b[5] = CheckKizu(a, x[5], x[6], 0, y0);
                    //w[5] = CheckWave(a, x[5], x[6], 0, y0, 1);
                }
                //Gắn giá trị đường kính D1 vào mảng 2 chiều d_All
                d_All[temp, 0] = (int)Math.Round(Average(a, x[5], x[6]));
                //#Hết ĐIỂM ĐO D1

                p[14] = x[6] - (m_iPara[9]) * 5;
                //Giá trị hiện tại
                // y0 = Average(a, p[14], 3) + 40; //Giá trị cũ

                y0 = Average(a, p[14], 3);
                x[7] = (int)Math.Round((y0 - alpha) / beta + 10);
                if (bUseCheckKizu)
                {
                    //b[6] = CheckKizu(a, x[6], x[7] + 30, beta, alpha);
                    //w[6] = CheckWave(a, x[6], x[7] + 30, beta, alpha, 0);
                }
                //Đoạn L1
                l[0] = round((x[6] - x[7]) * 0.2);

            }
            if (ItemName == "11CA011-0-VN")
            {

                //ĐIỂM ĐO D4
                p[1] = p[0] - (m_iPara[22] - 2) * 5;

                y0 = Average(a, p[0] - 60, p[1]) + 5;
                for (int i = 0; i <= 1; i++)
                {
                    if (i == 0)
                    {
                        //p[2] =2878- 54*5
                        //p3[2608]- (30-6)*5
                        p[2] = p[0] - m_iPara[21] * 5;
                        p[3] = p[2] - (m_iPara[20] - 6) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[20] * 5;
                    }
                    //Tính trung bình cộng của tất cả VỊ TRÍ trong khoảng p[2] đến p[3]. Test: trung bình cộng chạy từ 2608 đến 2488 
                    //tbX=(p[2]+p[3])/
                    tbX = averageX(p[2], p[3]);
                    //Tính trung bình cộng đường kính của tất cả các điểm từ điểm p[2] đến p[3]
                    tbY = Average(a, p[2], p[3]);

                    //??? 
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    double SxxTTT = ConsiderSxx(p[2], p[3], tbX);
                    //??? để làm gì
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    double SxyTTT = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx; //Tìm hệ số góc của phương trình đường thẳng đoạn p[2], p[3]
                    else return false;

                    alpha = tbY - beta * tbX; //b=y-a*x
                    double alphaPhong30Giay = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);

                    //x[0] =y0 - alpha) / beta
                }

                //Đường kính D4
                d_All[temp, 3] = (int)Math.Round(Average(a, p[0] - 50, x[0]));
                //Đoạn L7
                l[6] = round(((p[0] - x[0]) * 0.2));
                //ĐIỂM ĐO D3

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[19] + 1) * 5;
                        p[5] = p[4] - (m_iPara[18] - 6) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[18] * 5;
                    }
                    tbX2 = averageX(p[4], p[5]);
                    tbY2 = Average(a, p[4], p[5]);
                    Sxx2 = ConsiderSxx(p[4], p[5], tbX2);
                    Sxy2 = ConsiderSxy(a, p[4], p[5], tbX2, tbY2);
                    if ((Sxx2 != 0) && (Sxy2 != 0))
                        beta2 = Sxy2 / Sxx2;
                    else
                        return false;

                    alpha2 = tbY2 - beta2 * tbX2;

                    if (beta != beta2)
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta));
                    else
                        return false;

                    x[2] = x[1];
                    //d_All[temp][2] = beta*x[1] + alpha;
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, x[1] + 5, x[1]));
                //Đoạn L6            
                l[5] = round((x[0] - x[1]) * 0.2);
                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                //Bắt Đầu ĐIỂM ĐO D2
                p[6] = x[2] - (m_iPara[17]) * 5;
                p[7] = p[6] - (m_iPara[16] - 10) * 5;

                y0 = Average(a, p[6], p[7]);

                x[3] = (int)Math.Round((y0 - alpha2) / beta2 - 10);
                //Đoạn L5
                l[4] = round((x[1] - x[3]) * 0.2);
                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 10) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - m_iPara[14] * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else
                        return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta) - 10;
                }
                //Đoạn L4
                l[3] = round((x[3] - x[4]) * 0.2);
                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[3], x[4]));
                //#Hết ĐIỂM ĐO D2

                //Bắt Đầu ĐIỂM ĐO D1
                p[10] = x[4] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[5] = (int)Math.Round((y0 - alpha) / beta) - 10;
                //Đoạn L3
                l[2] = round((x[4] - x[5]) * 0.2);
                if (bUseCheckKizu)
                {
                    //b[4] = CheckKizu(a, x[4], x[5], beta, alpha);
                    //w[4] = CheckWave(a, x[4], x[5], beta, alpha, 2);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[5] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }
                    else
                    {
                        p[12] = x[6];
                        p[13] = p[12] - m_iPara[10] * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[6] = (int)Math.Round((y0 - alpha) / beta) + 10;
                }
                //Đoạn L2
                l[1] = round((x[5] - x[6]) * 0.2);
                if (bUseCheckKizu)
                {
                    //b[5] = CheckKizu(a, x[5], x[6], 0, y0);
                    //w[5] = CheckWave(a, x[5], x[6], 0, y0, 1);
                }
                //Gắn giá trị đường kính D1 vào mảng 2 chiều d_All
                d_All[temp, 0] = (int)Math.Round(Average(a, x[5], x[6]));
                //#Hết ĐIỂM ĐO D1

                p[14] = x[6] - (m_iPara[9]) * 5;
                //Giá trị hiện tại
                // y0 = Average(a, p[14], 3) + 40; //Giá trị cũ

                y0 = Average(a, p[14], 3);
                x[7] = (int)Math.Round((y0 - alpha) / beta + 10);
                if (bUseCheckKizu)
                {
                    //b[6] = CheckKizu(a, x[6], x[7] + 30, beta, alpha);
                    //w[6] = CheckWave(a, x[6], x[7] + 30, beta, alpha, 0);
                }
                //Đoạn L1
                l[0] = round((x[6] - x[7]) * 0.2);

                /*
                l[0] = round((x[6] - x[7]) * 0.2);
                l[1] = round((x[5] - x[6]) * 0.2);
                l[2] = round((x[4] - x[5]) * 0.2);
                l[3] = round((x[3] - x[4]) * 0.2);
                l[4] = round((x[1] - x[3]) * 0.2);
                l[5] = round((x[0] - x[1]) * 0.2);
                l[6] = round(((p[0] - x[0]) * 0.2));
                */
            }
            if (ItemName == "13CA021-0-VN")
            {

                //ĐIỂM ĐO D4
                p[1] = p[0] - (m_iPara[22] - 2) * 5;

                //    p[1] = p[0] - m_iPara[22] * 5;

                // lấy trung bình cộng đường kính từ vị trí a[p[0] - 60] đến a[p[1]]
                // y0 = Average(a, p[0] - 60, p[1]) + 5;  // không biết tại sao  lại + 5 => bỏ mẹ nó đi xem sao?? 
                //y0 = 538.85106382978722
                y0 = Average(a, p[0] - 60, p[1]); //Đến đây đã có thể sử dụng là đường kính D4 được rồi. Với tiêu chuẩn khoảng cách Min
                for (int i = 0; i <= 1; i++)
                {
                    if (i == 0)
                    {
                        //p[2] =2878- 54*5
                        //p3[2608]- (30-6)*5
                        p[2] = p[0] - m_iPara[21] * 5;
                        p[3] = p[2] - (m_iPara[20] - 6) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[20] * 5;
                    }
                    //Tính trung bình cộng của tất cả VỊ TRÍ trong khoảng p[2] đến p[3]. Test: trung bình cộng chạy từ 2608 đến 2488 
                    //tbX=(p[2]+p[3])/
                    tbX = averageX(p[2], p[3]);
                    //Tính trung bình cộng đường kính của tất cả các điểm từ điểm p[2] đến p[3]
                    tbY = Average(a, p[2], p[3]);

                    //??? 
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    double SxxTTT = ConsiderSxx(p[2], p[3], tbX);
                    //??? để làm gì
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    double SxyTTT = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx; //Tìm hệ số góc của phương trình đường thẳng đoạn p[2], p[3]
                    else return false;

                    alpha = tbY - beta * tbX; //b=y-a*x
                    double alphaPhong30Giay = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);

                    //x[0] =y0 - alpha) / beta
                }
                d_All[temp, 3] = (int)Math.Round(Average(a, p[0] - 50, x[0]));
                int vitriTrungBinh = (int)Math.Round(averageX(p[1], p[2]));
                int tito = d_All[temp, 3];
                //Đến đây đã có thể sử dụng là đường kính D4 được rồi. Với tiêu chuẩn khoảng cách Max

                //ĐIỂM ĐO D3

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[19] + 1) * 5;
                        p[5] = p[4] - (m_iPara[18] - 6) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[18] * 5;
                    }
                    tbX2 = averageX(p[4], p[5]);
                    tbY2 = Average(a, p[4], p[5]);
                    Sxx2 = ConsiderSxx(p[4], p[5], tbX2);
                    Sxy2 = ConsiderSxy(a, p[4], p[5], tbX2, tbY2);
                    if ((Sxx2 != 0) && (Sxy2 != 0))
                        beta2 = Sxy2 / Sxx2;
                    else
                        return false;

                    alpha2 = tbY2 - beta2 * tbX2;

                    if (beta != beta2)
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta));
                    else
                        return false;
                    x[2] = x[1];
                    //d_All[temp][2] = beta*x[1] + alpha;
                }
                d_All[temp, 2] = (int)Math.Round(Average(a, x[1] + 5, x[1]));

                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                //Bắt Đầu ĐIỂM ĐO D2
                p[6] = x[2] - (m_iPara[17]) * 5;
                p[7] = p[6] - (m_iPara[16] - 10) * 5;

                y0 = Average(a, p[6], p[7]) - 5;

                x[3] = (int)Math.Round((y0 - alpha2) / beta2);

                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 10) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - m_iPara[14] * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else
                        return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[3], x[4]));
                //#Hết ĐIỂM ĐO D2

                //Bắt Đầu ĐIỂM ĐO D1
                p[10] = x[4] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[5] = (int)Math.Round((y0 - alpha) / beta);

                if (bUseCheckKizu)
                {
                    //b[4] = CheckKizu(a, x[4], x[5], beta, alpha);
                    //w[4] = CheckWave(a, x[4], x[5], beta, alpha, 2);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[5] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[6];
                        p[13] = p[12] - m_iPara[10] * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[6] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[5] = CheckKizu(a, x[5], x[6], 0, y0);
                    //w[5] = CheckWave(a, x[5], x[6], 0, y0, 1);
                }
                //Gắn giá trị đường kính D1 vào mảng 2 chiều d_All
                d_All[temp, 0] = (int)Math.Round(Average(a, x[5], x[6]));
                //#Hết ĐIỂM ĐO D1

                p[14] = x[6] - (m_iPara[9]) * 5;
                //Giá trị hiện tại
                // y0 = Average(a, p[14], 3) + 40; //Giá trị cũ

                y0 = Average(a, p[14], 3) + 40;
                x[7] = (int)Math.Round((y0 - alpha) / beta);
                if (bUseCheckKizu)
                {
                    //b[6] = CheckKizu(a, x[6], x[7] + 30, beta, alpha);
                    //w[6] = CheckWave(a, x[6], x[7] + 30, beta, alpha, 0);
                }

                l[0] = round((x[6] - x[7]) * 0.2);
                l[1] = round((x[5] - x[6]) * 0.2);
                l[2] = round((x[4] - x[5]) * 0.2);
                l[3] = round((x[3] - x[4]) * 0.2);
                l[4] = round((x[1] - x[3]) * 0.2);
                l[5] = round((x[0] - x[1]) * 0.2);
                l[6] = round((p[0] - x[0]) * 0.2) + 1; //Bù kết quả để chính xác với kết quả đo +1
                double toanChimTo = round((p[0] - vitriTrungBinh) * 0.2);
                if (bUseCheckKizu)
                {
                    //for (int i = 0; i <= 6; i++)
                    //{
                    //    bKizu = bKizu * b[i];
                    //    bWave = bWave * w[i];
                    //}
                }
            }
            //********NHÓM 2********
            else if (ItemName == "10CA011-0-VN")
            {
                p[1] = p[0] - (m_iPara[24] - 2) * 5;
                y0 = Average(a, p[0] - 60, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - m_iPara[23] * 5;
                        p[3] = p[2] - (m_iPara[22] - 7) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[22] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[0] = CheckKizu(a, p[0] - 20, x[0], 0, y0);
                    //w[0] = CheckWave(a, p[0] - 20, x[0], 0, y0, 7);
                }
                d_All[temp, 3] = (int)Math.Round(Average(a, p[0] - 50, x[0]));
                l[7] = round((p[0] - x[0]) * 0.2);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[21] + 1) * 5;
                        p[5] = p[4] - (m_iPara[18] - 8) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[18] * 5;
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
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta) - 10);
                    else
                        return false;
                    x[2] = x[1] - 5;
                }
                d_All[temp, 2] = (int)Math.Round(Average(a, x[1], x[2]));
                l[6] = round((x[0] - x[1]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                p[6] = x[2] - (m_iPara[17]) * 5;
                p[7] = p[6] - (m_iPara[16] - 10) * 5;

                y0 = Average(a, p[6], p[7]) - 5;

                x[3] = (int)Math.Round((y0 - alpha2) / beta2);
                l[4] = round((x[2] - x[3]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }


                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 10) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    //Đang báo lỗi không phân tích được ở đây ERROR

                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }



                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }
                l[3] = round((x[3] - x[4]) * 0.2);
                d_All[temp, 1] = (int)Math.Round(Average(a, x[3], x[4]));

                p[10] = x[4] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[2] = round((x[4] - x[5]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[4] = CheckKizu(a, x[4], x[5], beta, alpha);
                    //w[4] = CheckWave(a, x[4], x[5], beta, alpha, 2);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[5] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[6];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[6] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[5] = CheckKizu(a, x[5], x[6], 0, y0);
                    //w[5] = CheckWave(a, x[5], x[6], 0, y0, 1);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[5], x[6]));
                l[1] = round((x[5] - x[6]) * 0.2);
                p[14] = x[6] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3);

                x[7] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[6] - x[7]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[6] = CheckKizu(a, x[6], x[7] + 30, beta, alpha);
                    //w[6] = CheckWave(a, x[6], x[7] + 30, beta, alpha, 0);
                }

                l[0] = round((x[6] - x[7]) * 0.2);
                l[1] = round((x[5] - x[6]) * 0.2);
                l[2] = round((x[4] - x[5]) * 0.2);
                l[3] = round((x[3] - x[4]) * 0.2);
                l[4] = round((x[2] - x[3]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);
                l[6] = round((x[0] - x[1]) * 0.2);
                l[7] = round((p[0] - x[0]) * 0.2);

                if (bUseCheckKizu)
                {
                    //for (int i = 0; i <= 6; i++)
                    //{
                    //    bKizu = bKizu * b[i];
                    //    bWave = bWave * w[i];
                    //}
                }

            }
            else if (ItemName == "10CA007-0-VN")
            {
                p[1] = p[0] - (m_iPara[24] - 2) * 5;
                y0 = Average(a, p[0] - 60, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - m_iPara[23] * 5;
                        p[3] = p[2] - (m_iPara[22] - 7) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[22] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[0] = CheckKizu(a, p[0] - 20, x[0], 0, y0);
                    //w[0] = CheckWave(a, p[0] - 20, x[0], 0, y0, 7);
                }
                d_All[temp, 3] = (int)Math.Round(Average(a, p[0] - 50, x[0]));
                l[7] = round((p[0] - x[0]) * 0.2);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[21] + 1) * 5;
                        p[5] = p[4] - (m_iPara[18] - 8) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[18] * 5;
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
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta) - 10);
                    else
                        return false;
                    x[2] = x[1] - 5;
                }
                d_All[temp, 2] = (int)Math.Round(Average(a, x[1], x[2]));
                l[6] = round((x[0] - x[1]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                p[6] = x[2] - (m_iPara[17]) * 5;
                p[7] = p[6] - (m_iPara[16] - 10) * 5;

                y0 = Average(a, p[6], p[7]) - 5;

                x[3] = (int)Math.Round((y0 - alpha2) / beta2);
                l[4] = round((x[2] - x[3]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }


                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 10) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    //Đang báo lỗi không phân tích được ở đây ERROR

                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }



                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }
                l[3] = round((x[3] - x[4]) * 0.2);
                d_All[temp, 1] = (int)Math.Round(Average(a, x[3], x[4]));

                p[10] = x[4] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[2] = round((x[4] - x[5]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[4] = CheckKizu(a, x[4], x[5], beta, alpha);
                    //w[4] = CheckWave(a, x[4], x[5], beta, alpha, 2);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[5] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[6];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[6] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[5] = CheckKizu(a, x[5], x[6], 0, y0);
                    //w[5] = CheckWave(a, x[5], x[6], 0, y0, 1);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[5], x[6]));
                l[1] = round((x[5] - x[6]) * 0.2);
                p[14] = x[6] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3);

                x[7] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[6] - x[7]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[6] = CheckKizu(a, x[6], x[7] + 30, beta, alpha);
                    //w[6] = CheckWave(a, x[6], x[7] + 30, beta, alpha, 0);
                }

                l[0] = round((x[6] - x[7]) * 0.2);
                l[1] = round((x[5] - x[6]) * 0.2);
                l[2] = round((x[4] - x[5]) * 0.2);
                l[3] = round((x[3] - x[4]) * 0.2);
                l[4] = round((x[2] - x[3]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);
                l[6] = round((x[0] - x[1]) * 0.2);
                l[7] = round((p[0] - x[0]) * 0.2);

                if (bUseCheckKizu)
                {
                    //for (int i = 0; i <= 6; i++)
                    //{
                    //    bKizu = bKizu * b[i];
                    //    bWave = bWave * w[i];
                    //}
                }

            }
            else if (ItemName == "13CA018-0-VN")
            {
                p[1] = p[0] - (m_iPara[24] - 2) * 5;
                y0 = Average(a, p[0] - 60, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - m_iPara[23] * 5;
                        p[3] = p[2] - (m_iPara[22] - 7) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[22] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[0] = CheckKizu(a, p[0] - 20, x[0], 0, y0);
                    //w[0] = CheckWave(a, p[0] - 20, x[0], 0, y0, 7);
                }
                d_All[temp, 3] = (int)Math.Round(Average(a, p[0] - 50, x[0]));
                l[7] = round((p[0] - x[0]) * 0.2);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[21] + 1) * 5;
                        p[5] = p[4] - (m_iPara[18] - 8) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[18] * 5;
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
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta) - 10);
                    else
                        return false;
                    x[2] = x[1] - 5;
                }
                d_All[temp, 2] = (int)Math.Round(Average(a, x[1], x[2]));
                l[6] = round((x[0] - x[1]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                p[6] = x[2] - (m_iPara[17]) * 5;
                p[7] = p[6] - (m_iPara[16] - 10) * 5;

                y0 = Average(a, p[6], p[7]) - 5;

                x[3] = (int)Math.Round((y0 - alpha2) / beta2);
                l[4] = round((x[2] - x[3]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }


                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 10) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    //Đang báo lỗi không phân tích được ở đây ERROR

                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }



                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }
                l[3] = round((x[3] - x[4]) * 0.2);
                d_All[temp, 1] = (int)Math.Round(Average(a, x[3], x[4]));

                p[10] = x[4] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[2] = round((x[4] - x[5]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[4] = CheckKizu(a, x[4], x[5], beta, alpha);
                    //w[4] = CheckWave(a, x[4], x[5], beta, alpha, 2);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[5] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[6];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[6] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[5] = CheckKizu(a, x[5], x[6], 0, y0);
                    //w[5] = CheckWave(a, x[5], x[6], 0, y0, 1);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[5], x[6]));
                l[1] = round((x[5] - x[6]) * 0.2);
                p[14] = x[6] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3);

                x[7] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[6] - x[7]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[6] = CheckKizu(a, x[6], x[7] + 30, beta, alpha);
                    //w[6] = CheckWave(a, x[6], x[7] + 30, beta, alpha, 0);
                }

                l[0] = round((x[6] - x[7]) * 0.2);
                l[1] = round((x[5] - x[6]) * 0.2);
                l[2] = round((x[4] - x[5]) * 0.2);
                l[3] = round((x[3] - x[4]) * 0.2);
                l[4] = round((x[2] - x[3]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);
                l[6] = round((x[0] - x[1]) * 0.2);
                l[7] = round((p[0] - x[0]) * 0.2);

                if (bUseCheckKizu)
                {
                    //for (int i = 0; i <= 6; i++)
                    //{
                    //    bKizu = bKizu * b[i];
                    //    bWave = bWave * w[i];
                    //}
                }

            }
            else if (ItemName == "09CA002-0")
            {
                p[1] = p[0] - (m_iPara[24] - 2) * 5;
                y0 = Average(a, p[0] - 60, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - m_iPara[23] * 5;
                        p[3] = p[2] - (m_iPara[22] - 7) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[22] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[0] = CheckKizu(a, p[0] - 20, x[0], 0, y0);
                    //w[0] = CheckWave(a, p[0] - 20, x[0], 0, y0, 7);
                }
                d_All[temp, 3] = (int)Math.Round(Average(a, p[0] - 50, x[0]));
                l[7] = round((p[0] - x[0]) * 0.2);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[21] + 1) * 5;
                        p[5] = p[4] - (m_iPara[18] - 8) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[18] * 5;
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
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta) - 10);
                    else
                        return false;
                    x[2] = x[1] - 5;
                }
                d_All[temp, 2] = (int)Math.Round(Average(a, x[1], x[2]));
                l[6] = round((x[0] - x[1]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                p[6] = x[2] - (m_iPara[17]) * 5;
                p[7] = p[6] - (m_iPara[16] - 10) * 5;

                y0 = Average(a, p[6], p[7]) - 5;

                x[3] = (int)Math.Round((y0 - alpha2) / beta2);
                l[4] = round((x[2] - x[3]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }


                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 10) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    //Đang báo lỗi không phân tích được ở đây ERROR

                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }



                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }
                l[3] = round((x[3] - x[4]) * 0.2);
                d_All[temp, 1] = (int)Math.Round(Average(a, x[3], x[4]));

                p[10] = x[4] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[2] = round((x[4] - x[5]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[4] = CheckKizu(a, x[4], x[5], beta, alpha);
                    //w[4] = CheckWave(a, x[4], x[5], beta, alpha, 2);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[5] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[6];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[6] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[5] = CheckKizu(a, x[5], x[6], 0, y0);
                    //w[5] = CheckWave(a, x[5], x[6], 0, y0, 1);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[5], x[6]));
                l[1] = round((x[5] - x[6]) * 0.2);
                p[14] = x[6] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3);

                x[7] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[6] - x[7]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[6] = CheckKizu(a, x[6], x[7] + 30, beta, alpha);
                    //w[6] = CheckWave(a, x[6], x[7] + 30, beta, alpha, 0);
                }

                l[0] = round((x[6] - x[7]) * 0.2);
                l[1] = round((x[5] - x[6]) * 0.2);
                l[2] = round((x[4] - x[5]) * 0.2);
                l[3] = round((x[3] - x[4]) * 0.2);
                l[4] = round((x[2] - x[3]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);
                l[6] = round((x[0] - x[1]) * 0.2);
                l[7] = round((p[0] - x[0]) * 0.2);

                if (bUseCheckKizu)
                {
                    //for (int i = 0; i <= 6; i++)
                    //{
                    //    bKizu = bKizu * b[i];
                    //    bWave = bWave * w[i];
                    //}
                }

            }
            else if (ItemName == "15CA001-0-VN")
            {
                p[1] = p[0] - (m_iPara[24] - 2) * 5;
                y0 = Average(a, p[0] - 60, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - m_iPara[23] * 5;
                        p[3] = p[2] - (m_iPara[22] - 7) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[22] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[0] = CheckKizu(a, p[0] - 20, x[0], 0, y0);
                    //w[0] = CheckWave(a, p[0] - 20, x[0], 0, y0, 7);
                }
                d_All[temp, 3] = (int)Math.Round(Average(a, p[0] - 50, x[0]));
                l[7] = round((p[0] - x[0]) * 0.2);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[21] + 1) * 5;
                        p[5] = p[4] - (m_iPara[18] - 8) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[18] * 5;
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
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta) - 10);
                    else
                        return false;
                    x[2] = x[1] - 5;
                }
                d_All[temp, 2] = (int)Math.Round(Average(a, x[1], x[2]));
                l[6] = round((x[0] - x[1]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                p[6] = x[2] - (m_iPara[17]) * 5;
                p[7] = p[6] - (m_iPara[16] - 10) * 5;

                y0 = Average(a, p[6], p[7]) - 5;

                x[3] = (int)Math.Round((y0 - alpha2) / beta2);
                l[4] = round((x[2] - x[3]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }


                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 10) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    //Đang báo lỗi không phân tích được ở đây ERROR

                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }



                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }
                l[3] = round((x[3] - x[4]) * 0.2);
                d_All[temp, 1] = (int)Math.Round(Average(a, x[3], x[4]));

                p[10] = x[4] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[2] = round((x[4] - x[5]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[4] = CheckKizu(a, x[4], x[5], beta, alpha);
                    //w[4] = CheckWave(a, x[4], x[5], beta, alpha, 2);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[5] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[6];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[6] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[5] = CheckKizu(a, x[5], x[6], 0, y0);
                    //w[5] = CheckWave(a, x[5], x[6], 0, y0, 1);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[5], x[6]));
                l[1] = round((x[5] - x[6]) * 0.2);
                p[14] = x[6] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3);

                x[7] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[6] - x[7]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[6] = CheckKizu(a, x[6], x[7] + 30, beta, alpha);
                    //w[6] = CheckWave(a, x[6], x[7] + 30, beta, alpha, 0);
                }

                l[0] = round((x[6] - x[7]) * 0.2);
                l[1] = round((x[5] - x[6]) * 0.2);
                l[2] = round((x[4] - x[5]) * 0.2);
                l[3] = round((x[3] - x[4]) * 0.2);
                l[4] = round((x[2] - x[3]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);
                l[6] = round((x[0] - x[1]) * 0.2);
                l[7] = round((p[0] - x[0]) * 0.2);

                if (bUseCheckKizu)
                {
                    //for (int i = 0; i <= 6; i++)
                    //{
                    //    bKizu = bKizu * b[i];
                    //    bWave = bWave * w[i];
                    //}
                }

            }
            else if (ItemName == "12CA016-0-VN")
            {
                p[1] = p[0] - (m_iPara[24] - 2) * 5;
                y0 = Average(a, p[0] - 60, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - m_iPara[23] * 5;
                        p[3] = p[2] - (m_iPara[22] - 7) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[22] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[0] = CheckKizu(a, p[0] - 20, x[0], 0, y0);
                    //w[0] = CheckWave(a, p[0] - 20, x[0], 0, y0, 7);
                }
                d_All[temp, 3] = (int)Math.Round(Average(a, p[0] - 50, x[0]));
                l[7] = round((p[0] - x[0]) * 0.2);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[21] + 1) * 5;
                        p[5] = p[4] - (m_iPara[18] - 8) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[18] * 5;
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
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta) - 10);
                    else
                        return false;
                    x[2] = x[1] - 5;
                }
                d_All[temp, 2] = (int)Math.Round(Average(a, x[1], x[2]));
                l[6] = round((x[0] - x[1]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                p[6] = x[2] - (m_iPara[17]) * 5;
                p[7] = p[6] - (m_iPara[16] - 10) * 5;

                y0 = Average(a, p[6], p[7]) - 5;

                x[3] = (int)Math.Round((y0 - alpha2) / beta2);
                l[4] = round((x[2] - x[3]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }


                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 10) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    //Đang báo lỗi không phân tích được ở đây ERROR

                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }



                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }
                l[3] = round((x[3] - x[4]) * 0.2);
                d_All[temp, 1] = (int)Math.Round(Average(a, x[3], x[4]));

                p[10] = x[4] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[2] = round((x[4] - x[5]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[4] = CheckKizu(a, x[4], x[5], beta, alpha);
                    //w[4] = CheckWave(a, x[4], x[5], beta, alpha, 2);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[5] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[6];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[6] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[5] = CheckKizu(a, x[5], x[6], 0, y0);
                    //w[5] = CheckWave(a, x[5], x[6], 0, y0, 1);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[5], x[6]));
                l[1] = round((x[5] - x[6]) * 0.2);
                p[14] = x[6] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3);

                x[7] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[6] - x[7]) * 0.2);
                l[0] = 50;
                if (bUseCheckKizu)
                {
                    //b[6] = CheckKizu(a, x[6], x[7] + 30, beta, alpha);
                    //w[6] = CheckWave(a, x[6], x[7] + 30, beta, alpha, 0);
                }

                //l[0] = round((x[6] - x[7]) * 0.2);
                //l[1] = round((x[5] - x[6]) * 0.2);
                //l[2] = round((x[4] - x[5]) * 0.2);
                //l[3] = round((x[3] - x[4]) * 0.2);
                //l[4] = round((x[2] - x[3]) * 0.2);
                //l[5] = round((x[1] - x[2]) * 0.2);
                //l[6] = round((x[0] - x[1]) * 0.2);
                //l[7] = round((p[0] - x[0]) * 0.2);

                if (bUseCheckKizu)
                {
                    //for (int i = 0; i <= 6; i++)
                    //{
                    //    bKizu = bKizu * b[i];
                    //    bWave = bWave * w[i];
                    //}
                }

            }
            //#Bắt đầu 12CA022
            else if (ItemName == "12CA022-0-VN")
            {
                p[1] = p[0] - (m_iPara[24] - 2) * 5;
                y0 = Average(a, p[0] - 60, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - m_iPara[23] * 5;
                        p[3] = p[2] - (m_iPara[22] - 7) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[22] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[0] = CheckKizu(a, p[0] - 20, x[0], 0, y0);
                    //w[0] = CheckWave(a, p[0] - 20, x[0], 0, y0, 7);
                }
                d_All[temp, 3] = (int)Math.Round(Average(a, p[0] - 50, x[0]));
                l[7] = round((p[0] - x[0]) * 0.2);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[21] + 1) * 5;
                        p[5] = p[4] - (m_iPara[18] - 8) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[18] * 5;
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
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta) - 10);
                    else
                        return false;
                    x[2] = x[1] - 5;
                }
                d_All[temp, 2] = (int)Math.Round(Average(a, x[1], x[2]));
                l[6] = round((x[0] - x[1]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                p[6] = x[2] - (m_iPara[17]) * 5;
                p[7] = p[6] - (m_iPara[16] - 10) * 5;

                y0 = Average(a, p[6], p[7]) - 5;

                x[3] = (int)Math.Round((y0 - alpha2) / beta2);
                l[4] = round((x[2] - x[3]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }


                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 10) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    //Đang báo lỗi không phân tích được ở đây ERROR

                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }



                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }
                l[3] = round((x[3] - x[4]) * 0.2);
                d_All[temp, 1] = (int)Math.Round(Average(a, x[3], x[4]));

                p[10] = x[4] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[2] = round((x[4] - x[5]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[4] = CheckKizu(a, x[4], x[5], beta, alpha);
                    //w[4] = CheckWave(a, x[4], x[5], beta, alpha, 2);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[5] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[6];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[6] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[5] = CheckKizu(a, x[5], x[6], 0, y0);
                    //w[5] = CheckWave(a, x[5], x[6], 0, y0, 1);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[5], x[6]));
                l[1] = round((x[5] - x[6]) * 0.2);
                p[14] = x[6] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 1);//sua tu 3 ve 1

                x[7] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[6] - x[7]) * 0.2);
                l[0] = 40;
                if (bUseCheckKizu)
                {
                    //b[6] = CheckKizu(a, x[6], x[7] + 30, beta, alpha);
                    //w[6] = CheckWave(a, x[6], x[7] + 30, beta, alpha, 0);
                }

                //l[0] = round((x[6] - x[7]) * 0.2);
                //l[1] = round((x[5] - x[6]) * 0.2);
                //l[2] = round((x[4] - x[5]) * 0.2);
                //l[3] = round((x[3] - x[4]) * 0.2);
                //l[4] = round((x[2] - x[3]) * 0.2);
                //l[5] = round((x[1] - x[2]) * 0.2);
                //l[6] = round((x[0] - x[1]) * 0.2);
                //l[7] = round((p[0] - x[0]) * 0.2);

                if (bUseCheckKizu)
                {
                    //for (int i = 0; i <= 6; i++)
                    //{
                    //    bKizu = bKizu * b[i];
                    //    bWave = bWave * w[i];
                    //}
                }

            }
            //#Hết 12CA022
            //# Bắt đầu 12CA002-0-VN
            else if (ItemName == "12CA002-0-VN")
            {
                p[1] = p[0] - (m_iPara[24] - 2) * 5;
                y0 = Average(a, p[0] - 60, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - m_iPara[23] * 5;
                        p[3] = p[2] - (m_iPara[22] - 7) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[22] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[0] = CheckKizu(a, p[0] - 20, x[0], 0, y0);
                    //w[0] = CheckWave(a, p[0] - 20, x[0], 0, y0, 7);
                }
                d_All[temp, 3] = (int)Math.Round(Average(a, p[0] - 50, x[0]));
                l[7] = round((p[0] - x[0]) * 0.2);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[21] + 1) * 5;
                        p[5] = p[4] - (m_iPara[18] - 8) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[18] * 5;
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
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta) - 10);
                    else
                        return false;
                    x[2] = x[1] - 5;
                }
                d_All[temp, 2] = (int)Math.Round(Average(a, x[1], x[2]));
                l[6] = round((x[0] - x[1]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                p[6] = x[2] - (m_iPara[17]) * 5;
                p[7] = p[6] - (m_iPara[16] - 10) * 5;

                y0 = Average(a, p[6], p[7]) - 5;

                x[3] = (int)Math.Round((y0 - alpha2) / beta2);
                l[4] = round((x[2] - x[3]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }


                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 10) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    //Đang báo lỗi không phân tích được ở đây ERROR

                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }



                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }
                l[3] = round((x[3] - x[4]) * 0.2);
                d_All[temp, 1] = (int)Math.Round(Average(a, x[3], x[4]));

                p[10] = x[4] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[2] = round((x[4] - x[5]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[4] = CheckKizu(a, x[4], x[5], beta, alpha);
                    //w[4] = CheckWave(a, x[4], x[5], beta, alpha, 2);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[5] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[6];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[6] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[5] = CheckKizu(a, x[5], x[6], 0, y0);
                    //w[5] = CheckWave(a, x[5], x[6], 0, y0, 1);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[5], x[6]));
                l[1] = round((x[5] - x[6]) * 0.2);
                p[14] = x[6] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3);

                x[7] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[6] - x[7]) * 0.2);
                l[0] = 50;

                if (bUseCheckKizu)
                {
                    //b[6] = CheckKizu(a, x[6], x[7] + 30, beta, alpha);
                    //w[6] = CheckWave(a, x[6], x[7] + 30, beta, alpha, 0);
                }

                //////l[0] = round((x[6] - x[7]) * 0.2);
                //////l[1] = round((x[5] - x[6]) * 0.2);
                //////l[2] = round((x[4] - x[5]) * 0.2);
                //////l[3] = round((x[3] - x[4]) * 0.2);
                //////l[4] = round((x[2] - x[3]) * 0.2);
                //////l[5] = round((x[1] - x[2]) * 0.2);
                //////l[6] = round((x[0] - x[1]) * 0.2);
                //////l[7] = round((p[0] - x[0]) * 0.2);

                if (bUseCheckKizu)
                {
                    //for (int i = 0; i <= 6; i++)
                    //{
                    //    bKizu = bKizu * b[i];
                    //    bWave = bWave * w[i];
                    //}
                }

            }
            //# Hết 12CA002-0-VN
            //# Bắt đầu 12CA009-0-VN
            else if (ItemName == "12CA009-0-VN")
            {
                p[1] = p[0] - (m_iPara[24] - 2) * 5;
                y0 = Average(a, p[0] - 60, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - m_iPara[23] * 5;
                        p[3] = p[2] - (m_iPara[22] - 7) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[22] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[0] = CheckKizu(a, p[0] - 20, x[0], 0, y0);
                    //w[0] = CheckWave(a, p[0] - 20, x[0], 0, y0, 7);
                }
                d_All[temp, 3] = (int)Math.Round(Average(a, p[0] - 50, x[0]));
                l[7] = round((p[0] - x[0]) * 0.2);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[21] + 1) * 5;
                        p[5] = p[4] - (m_iPara[18] - 8) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[18] * 5;
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
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta) - 10);
                    else
                        return false;
                    x[2] = x[1] - 5;
                }
                d_All[temp, 2] = (int)Math.Round(Average(a, x[1], x[2]));
                l[6] = round((x[0] - x[1]) * 0.2);
                l[5] = round((x[1] - x[2]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                p[6] = x[2] - (m_iPara[17]) * 5;
                p[7] = p[6] - (m_iPara[16] - 10) * 5;

                y0 = Average(a, p[6], p[7]) - 5;

                x[3] = (int)Math.Round((y0 - alpha2) / beta2);
                l[4] = round((x[2] - x[3]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }


                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 10) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    //Đang báo lỗi không phân tích được ở đây ERROR

                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }



                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }
                l[3] = round((x[3] - x[4]) * 0.2);
                d_All[temp, 1] = (int)Math.Round(Average(a, x[3], x[4]));

                p[10] = x[4] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[2] = round((x[4] - x[5]) * 0.2);

                if (bUseCheckKizu)
                {
                    //b[4] = CheckKizu(a, x[4], x[5], beta, alpha);
                    //w[4] = CheckWave(a, x[4], x[5], beta, alpha, 2);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[5] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[6];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[6] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[5] = CheckKizu(a, x[5], x[6], 0, y0);
                    //w[5] = CheckWave(a, x[5], x[6], 0, y0, 1);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[5], x[6]));
                l[1] = round((x[5] - x[6]) * 0.2);
                p[14] = x[6] - (m_iPara[9]) * 5;


                y0 = Average(a, p[14], 3);

                x[7] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[6] - x[7]) * 0.2) + 8;
                //CHú ý: Đoạn L[0] hiện tại không đo được. vì đoạn đo dài hơn thiết kế của máy. nên ko tính toán nữa mà Gán l[0]=50
                //   l[0] = 50;
                if (bUseCheckKizu)
                {
                    //b[6] = CheckKizu(a, x[6], x[7] + 30, beta, alpha);
                    //w[6] = CheckWave(a, x[6], x[7] + 30, beta, alpha, 0);
                }

                //////l[0] = round((x[6] - x[7]) * 0.2);
                //////l[1] = round((x[5] - x[6]) * 0.2);
                //////l[2] = round((x[4] - x[5]) * 0.2);
                //////l[3] = round((x[3] - x[4]) * 0.2);
                //////l[4] = round((x[2] - x[3]) * 0.2);
                //////l[5] = round((x[1] - x[2]) * 0.2);
                //////l[6] = round((x[0] - x[1]) * 0.2);
                //////l[7] = round((p[0] - x[0]) * 0.2);

                if (bUseCheckKizu)
                {
                    //for (int i = 0; i <= 6; i++)
                    //{
                    //    bKizu = bKizu * b[i];
                    //    bWave = bWave * w[i];
                    //}
                }

            }
            //# Hết 12CA009-0-VN

            //********NHÓM 3********
            else if (ItemName == "7CA006-E-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 4) * 5;
                y0 = Average(a, p[0] - 30, p[1]) + 5;

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - (m_iPara[19] - 3) * 5;
                        p[3] = p[2] - (m_iPara[18] - 3) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 30, x[0]));
                l[5] = round((p[0] - x[0]) * 0.2);
                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[17] - 3) * 5;
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }
                l[4] = round((x[0] - x[1]) * 0.2);


                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[2] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));
                l[3] = round((x[1] - x[2]) * 0.2);

                p[10] = x[2] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[3] = (int)Math.Round((y0 - alpha) / beta);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[3] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[4];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));
                l[2] = round((x[2] - x[3]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);

                p[14] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 1) + 10;//trc bù +300

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[4] - x[5]) * 0.2);

                //l[0] = round((x[4] - x[5]) * 0.2);
                //l[1] = round((x[3] - x[4]) * 0.2);
                //l[2] = round((x[2] - x[3]) * 0.2);
                //l[3] = round((x[1] - x[2]) * 0.2);
                //l[4] = round((x[0] - x[1]) * 0.2);
                //l[5] = round((p[0] - x[0]) * 0.2);
            }
            //#het 12CA022
            else if (ItemName == "4VC006-0-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 4) * 5;
                y0 = Average(a, p[0] - 60, p[1]) + 5;

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - (m_iPara[19] - 3) * 5;
                        p[3] = p[2] - (m_iPara[18] - 3) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[17] - 3) * 5;
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[2] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                p[10] = x[2] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[3] = (int)Math.Round((y0 - alpha) / beta);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[3] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[4];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[14] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3) + 10;//trc bù +300

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);
            }
            else if (ItemName == "17CA010-0-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 4) * 5;
                y0 = Average(a, p[0] - 60, p[1]) + 5;

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - (m_iPara[19] - 3) * 5;
                        p[3] = p[2] - (m_iPara[18] - 3) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[17] - 3) * 5;
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[2] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                p[10] = x[2] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[3] = (int)Math.Round((y0 - alpha) / beta);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[3] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[4];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[14] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3) + 10;//trc bù +300

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);
            }
            else if (ItemName == "10CA005-0-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 4) * 5;
                y0 = Average(a, p[0] - 60, p[1]) + 5;

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - (m_iPara[19] - 3) * 5;
                        p[3] = p[2] - (m_iPara[18] - 3) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[17] - 3) * 5;
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[2] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                p[10] = x[2] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[3] = (int)Math.Round((y0 - alpha) / beta);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[3] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[4];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[14] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3) + 10;//trc bù +300

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);
            }
            else if (ItemName == "09CA006-0-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 4) * 5;
                y0 = Average(a, p[0] - 60, p[1]) + 5;

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - (m_iPara[19] - 3) * 5;
                        p[3] = p[2] - (m_iPara[18] - 3) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[17] - 3) * 5;
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[2] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                p[10] = x[2] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[3] = (int)Math.Round((y0 - alpha) / beta);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[3] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[4];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[14] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3) + 10;//trc bù +300

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);
            }
            else if (ItemName == "09VC003-0-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 4) * 5;
                y0 = Average(a, p[0] - 60, p[1]) + 5;

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - (m_iPara[19] - 3) * 5;
                        p[3] = p[2] - (m_iPara[18] - 3) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[17] - 3) * 5;
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[2] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                p[10] = x[2] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[3] = (int)Math.Round((y0 - alpha) / beta);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[3] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[4];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[14] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3) + 10;//trc bù +300

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);
            }
            else if (ItemName == "1CA001-B-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 4) * 5;
                y0 = Average(a, p[0] - 60, p[1]) + 5;

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - (m_iPara[19] - 3) * 5;
                        p[3] = p[2] - (m_iPara[18] - 3) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[17] - 3) * 5;
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[2] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                p[10] = x[2] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[3] = (int)Math.Round((y0 - alpha) / beta);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[3] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[4];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[14] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3) + 10;//trc bù +300

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);
            }
            else if (ItemName == "8CA001-B-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 4) * 5;
                y0 = Average(a, p[0] - 60, p[1]) + 5;

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - (m_iPara[19] - 3) * 5;
                        p[3] = p[2] - (m_iPara[18] - 3) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[17] - 3) * 5;
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[2] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                p[10] = x[2] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[3] = (int)Math.Round((y0 - alpha) / beta);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[3] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[4];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[14] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3) + 10;//trc bù +300

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);
            }
            else if (ItemName == "5CA004-C-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 4) * 5;
                y0 = Average(a, p[0] - 60, p[1]) + 5;

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - (m_iPara[19] - 3) * 5;
                        p[3] = p[2] - (m_iPara[18] - 3) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[17] - 3) * 5;
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[2] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                p[10] = x[2] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[3] = (int)Math.Round((y0 - alpha) / beta);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[3] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[4];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[14] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3) + 10;//trc bù +300

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);
            }
            else if (ItemName == "0CA002-A-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 4) * 5;
                y0 = Average(a, p[0] - 60, p[1]) + 5;

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - (m_iPara[19] - 3) * 5;
                        p[3] = p[2] - (m_iPara[18] - 3) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[17] - 3) * 5;
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[2] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                p[10] = x[2] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[3] = (int)Math.Round((y0 - alpha) / beta);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[3] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[4];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[14] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3) + 10;//trc bù +300

                x[5] = (int)Math.Round((y0 - alpha) / beta);

                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);  //bù 0.5
            }
            else if (ItemName == "09CA001-0-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 4) * 5;
                y0 = Average(a, p[0] - 60, p[1]) + 5;

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - (m_iPara[19] - 3) * 5;
                        p[3] = p[2] - (m_iPara[18] - 3) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[17] - 3) * 5;
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[2] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                p[10] = x[2] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[3] = (int)Math.Round((y0 - alpha) / beta);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[3] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[4];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[14] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3) + 10;//trc bù +300

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);
            }
            else if (ItemName == "7CA016-D-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 4) * 5;
                y0 = Average(a, p[0] - 60, p[1]) + 5;

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - (m_iPara[19] - 3) * 5;
                        p[3] = p[2] - (m_iPara[18] - 3) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[17] - 3) * 5;
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[16]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[15]) * 5;
                        p[9] = p[8] - (m_iPara[14] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[14]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[2] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                p[10] = x[2] - (m_iPara[13]) * 5;
                p[11] = p[10] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[10], p[11]) - 5;

                x[3] = (int)Math.Round((y0 - alpha) / beta);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[12] = x[3] - (m_iPara[11]) * 5;
                        p[13] = p[12] - (m_iPara[10] - 15) * 5;
                    }
                    else
                    {
                        p[12] = x[4];
                        p[13] = p[12] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[12], p[13]);
                    tbY = Average(a, p[12], p[13]);
                    Sxx = ConsiderSxx(p[12], p[13], tbX);
                    Sxy = ConsiderSxy(a, p[12], p[13], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[14] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[14], 3) + 10;//trc bù +300

                x[5] = (int)Math.Round((y0 - alpha) / beta);
                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);
            }

            //********NHÓM 4********
            //Bắt đầu 1CA004-A-VN

            else if (ItemName == "1CA004-A-VN")
            {
                p[1] = p[0] - (m_iPara[16] - 4) * 5;
                int p1 = p[1];

                y0 = Average(a, p[0] - 30, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        //p[2] vị trí ước lượng dựa trên tiêu chuẩn MAX đoạn L3
                        p[2] = p[0] - (m_iPara[15]) * 5;
                        p[3] = p[2] - (m_iPara[14]) * 5;
                    }
                    else
                    {
                        //p[2] vị trí ước lượng dựa trên tiêu chuẩn MAX đoạn L3 sau khi tính chính xác
                        p[2] = x[0];
                        p[3] = p[2] - (m_iPara[14]) * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    //Phương trình đường thẳng qua P2,P3
                    alpha = tbY - beta * tbX;
                    //Vì giao điểm của 2 đường thẳng qua P2,P3 và P0,P1 sẽ phải cắt ở đâu đó. Vị trí ấy tọa độ M(X[0],Y[0])
                    //Tọa độ X[0] sát hơn so với p[2] nên sử dụng tính sẽ sát hơn
                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                //Đường kính D2
                d_All[temp, 1] = (int)Math.Round(Average(a, p[0] - 50, x[0]));


                //Độ dài đoạn đo đường kính D2
                l[3] = round((p[0] - x[0]) * 0.2);  //Đoạn L4
                double DoDaiL3 = round((p[0] - x[0]) * 0.2);
                //#Hết điểm đo D2

                //Điểm đo D1
                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[13]) * 5;
                        p[7] = p[6] - (m_iPara[12] - 2) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[12]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }
                //Độ dài Đoạn L3: 30+-3
                l[2] = round((x[0] - x[1]) * 0.2);  //Đoạn L3

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[11]) * 5;
                        p[9] = p[8] - (m_iPara[10]) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX); //Tổng bình phương 1 hiệu giá trị từ p[8]-tbX đến p[9]-tbX
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);  // Tổng của tích 2 giá trị (p[8]-tbx)* (a[8]-tbY) đến p[8]
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;  //tỉ số Sxx/Sxy
                    else
                        return false;
                    alpha = tbY - beta * tbX;  //trung bình đương kính - beta*tbX

                    x[2] = (int)Math.Round((y0 - alpha) / beta);  // (đương kính trung bình- alpha)/beta;
                }
                //Độ dài đoạn L2: 250+-10
                l[1] = round((x[1] - x[2]) * 0.2);  //Đoạn L2 
                int viTriX2 = x[2];

                //Đường kính đoạn D1
                d_All[temp, 0] = (int)Math.Round(Average(a, x[1], x[2]));

                //Tính độ dài đoạn L1: Đoạn 50+-10
                p[10] = x[2] - (m_iPara[9]) * 5;

                y0 = Average(a, p[10], 3); //Gán =3 vì nó gần hết Core nên gắn mặc định mà không tính 

                x[3] = (int)Math.Round((y0 - alpha) / beta);
                //Chương trình đang lỗi đoạn L1. Tính đéo ra cái hồn gì ??? Vị trí các định tiến sát về 0
                l[0] = round((x[2] - x[3]) * 0.2) + 5;  //Đoạn L1 Bù 5 giá trị

                //x[3] = x[2] - Convert.ToInt32(averageX(m_iPara[9], m_iPara[10])) * 5;
                //l[0] = round((x[2] - x[3]) * 0.2);  //Đoạn L1

                //d_All[temp,1] = Average(a, x[1], x[2]);

                //p[10] = x[2] - (m_iPara[9]) * 5;

                //y0 = Average(a, p[10], 3);

                //x[3] = (y0 - alpha) / beta;



                ////l[0] = round((x[2] - x[3]) * 0.2);  //Đoạn L1
                ////l[1] = round((x[1] - x[2]) * 0.2);  //Đoạn L2
                ////l[2] = round((x[0] - x[1]) * 0.2);  //Đoạn L3
                // l[3] = round((p[0] - x[0]) * 0.2);  //Đoạn L4

            }
            //# Kết thúc 1CA004-A-VN
            //Bắt đầu 10CA014-0-VN
            else if (ItemName == "10CA014-0-VN")
            {

                p[1] = p[0] - (m_iPara[16] - 4) * 5;
                int p1 = p[1];

                y0 = Average(a, p[0] - 30, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        //p[2] vị trí ước lượng dựa trên tiêu chuẩn MAX đoạn L3
                        p[2] = p[0] - (m_iPara[15]) * 5;
                        p[3] = p[2] - (m_iPara[14]) * 5;
                    }
                    else
                    {
                        //p[2] vị trí ước lượng dựa trên tiêu chuẩn MAX đoạn L3 sau khi tính chính xác
                        p[2] = x[0];
                        p[3] = p[2] - (m_iPara[14]) * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }
                int vitriX0 = (int)Math.Round((y0 - alpha) / beta);
                //Đường kính D2
                d_All[temp, 1] = (int)Math.Round(Average(a, p[0] - 50, x[0]));


                //Độ dài đoạn đo đường kính D2
                l[3] = round((p[0] - x[0]) * 0.2);  //Đoạn L4
                double DoDaiL3 = round((p[0] - x[0]) * 0.2);
                //#Hết điểm đo D2

                //Điểm đo D1
                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[13]) * 5;
                        p[7] = p[6] - (m_iPara[12] - 2) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[12]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }
                //Độ dài Đoạn L3: 30+-3
                l[2] = round((x[0] - x[1]) * 0.2);  //Đoạn L3

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[11]) * 5;
                        p[9] = p[8] - (m_iPara[10]) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX); //Tổng bình phương 1 hiệu giá trị từ p[8]-tbX đến p[9]-tbX
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);  // Tổng của tích 2 giá trị (p[8]-tbx)* (a[8]-tbY) đến p[8]
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;  //tỉ số Sxx/Sxy
                    else
                        return false;
                    alpha = tbY - beta * tbX;  //trung bình đương kính - beta*tbX

                    x[2] = (int)Math.Round((y0 - alpha) / beta);  // (đương kính trung bình- alpha)/beta;
                }
                //Độ dài đoạn L2: 250+-10
                l[1] = round((x[1] - x[2]) * 0.2);  //Đoạn L2 
                int viTriX2 = x[2];

                //Đường kính đoạn D1
                d_All[temp, 0] = (int)Math.Round(Average(a, x[1], x[2]));

                //Tính độ dài đoạn L1: Đoạn 50+-10
                p[10] = x[2] - (m_iPara[9]) * 5;

                y0 = Average(a, p[10], 1); //Gán =3 vì nó gần hết Core nên gắn mặc định mà không tính 

                x[3] = (int)Math.Round((y0 - alpha) / beta);
                //Chương trình đang lỗi đoạn L1. Tính đéo ra cái hồn gì ??? Vị trí các định tiến sát về 0
                l[0] = round((x[2] - x[3]) * 0.2);  //Đoạn L1

                //x[3] = x[2] - Convert.ToInt32(averageX(m_iPara[9], m_iPara[10])) * 5;
                //l[0] = round((x[2] - x[3]) * 0.2);  //Đoạn L1

                //d_All[temp,1] = Average(a, x[1], x[2]);

                //p[10] = x[2] - (m_iPara[9]) * 5;

                //y0 = Average(a, p[10], 3);

                //x[3] = (y0 - alpha) / beta;



                ////l[0] = round((x[2] - x[3]) * 0.2);  //Đoạn L1
                ////l[1] = round((x[1] - x[2]) * 0.2);  //Đoạn L2
                ////l[2] = round((x[0] - x[1]) * 0.2);  //Đoạn L3
                // l[3] = round((p[0] - x[0]) * 0.2);  //Đoạn L4

            }
            //Kết thúc 10CA014-0-VN
            // Bắt đầu 07CA001-0-VN
            else if (ItemName == "07CA001-0-VN")
            {

                p[1] = p[0] - (m_iPara[16] - 4) * 5;
                int p1 = p[1];

                y0 = Average(a, p[0] - 30, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        //p[2] vị trí ước lượng dựa trên tiêu chuẩn MAX đoạn L3
                        p[2] = p[0] - (m_iPara[15]) * 5;
                        p[3] = p[2] - (m_iPara[14]) * 5;
                    }
                    else
                    {
                        //p[2] vị trí ước lượng dựa trên tiêu chuẩn MAX đoạn L3 sau khi tính chính xác
                        p[2] = x[0];
                        p[3] = p[2] - (m_iPara[14]) * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }
                int vitriX0 = (int)Math.Round((y0 - alpha) / beta);
                //Đường kính D2
                d_All[temp, 1] = (int)Math.Round(Average(a, p[0] - 50, x[0]));


                //Độ dài đoạn đo đường kính D2
                l[3] = round((p[0] - x[0]) * 0.2);  //Đoạn L4
                double DoDaiL3 = round((p[0] - x[0]) * 0.2);
                //#Hết điểm đo D2

                //Điểm đo D1
                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[13]) * 5;
                        p[7] = p[6] - (m_iPara[12] - 2) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[12]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }
                //Độ dài Đoạn L3: 30+-3
                l[2] = round((x[0] - x[1]) * 0.2);  //Đoạn L3

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[11]) * 5;
                        p[9] = p[8] - (m_iPara[10]) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX); //Tổng bình phương 1 hiệu giá trị từ p[8]-tbX đến p[9]-tbX
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);  // Tổng của tích 2 giá trị (p[8]-tbx)* (a[8]-tbY) đến p[8]
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;  //tỉ số Sxx/Sxy
                    else
                        return false;
                    alpha = tbY - beta * tbX;  //trung bình đương kính - beta*tbX

                    x[2] = (int)Math.Round((y0 - alpha) / beta);  // (đương kính trung bình- alpha)/beta;
                }
                //Độ dài đoạn L2: 250+-10
                l[1] = round((x[1] - x[2]) * 0.2);  //Đoạn L2 
                int viTriX2 = x[2];

                //Đường kính đoạn D1
                d_All[temp, 0] = (int)Math.Round(Average(a, x[1], x[2]));

                //Tính độ dài đoạn L1: Đoạn 50+-10
                p[10] = x[2] - (m_iPara[9]) * 5;

                y0 = Average(a, p[10], 1); //Gán =3 vì nó gần hết Core nên gắn mặc định mà không tính 

                x[3] = (int)Math.Round((y0 - alpha) / beta);
                //Chương trình đang lỗi đoạn L1. Tính đéo ra cái hồn gì ??? Vị trí các định tiến sát về 0
                l[0] = round((x[2] - x[3]) * 0.2);  //Đoạn L1

                //x[3] = x[2] - Convert.ToInt32(averageX(m_iPara[9], m_iPara[10])) * 5;
                //l[0] = round((x[2] - x[3]) * 0.2);  //Đoạn L1

                //d_All[temp,1] = Average(a, x[1], x[2]);

                //p[10] = x[2] - (m_iPara[9]) * 5;

                //y0 = Average(a, p[10], 3);

                //x[3] = (y0 - alpha) / beta;



                ////l[0] = round((x[2] - x[3]) * 0.2);  //Đoạn L1
                ////l[1] = round((x[1] - x[2]) * 0.2);  //Đoạn L2
                ////l[2] = round((x[0] - x[1]) * 0.2);  //Đoạn L3
                // l[3] = round((p[0] - x[0]) * 0.2);  //Đoạn L4

            }
            // Kết thúc 07CA001-0-VN
            // Bắt đầu 06VC003-B-VN
            else if (ItemName == "06VC003-B-VN")
            {

                p[1] = p[0] - (m_iPara[16] - 4) * 5;
                int p1 = p[1];

                y0 = Average(a, p[0] - 30, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        //p[2] vị trí ước lượng dựa trên tiêu chuẩn MAX đoạn L3
                        p[2] = p[0] - (m_iPara[15]) * 5;
                        p[3] = p[2] - (m_iPara[14]) * 5;
                    }
                    else
                    {
                        //p[2] vị trí ước lượng dựa trên tiêu chuẩn MAX đoạn L3 sau khi tính chính xác
                        p[2] = x[0];
                        p[3] = p[2] - (m_iPara[14]) * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }
                int vitriX0 = (int)Math.Round((y0 - alpha) / beta);
                //Đường kính D2
                d_All[temp, 1] = (int)Math.Round(Average(a, p[0] - 50, x[0]));


                //Độ dài đoạn đo đường kính D2
                l[3] = round((p[0] - x[0]) * 0.2);  //Đoạn L4
                double DoDaiL3 = round((p[0] - x[0]) * 0.2);
                //#Hết điểm đo D2

                //Điểm đo D1
                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[6] = x[0] - (m_iPara[13]) * 5;
                        p[7] = p[6] - (m_iPara[12] - 2) * 5;
                    }
                    else
                    {
                        p[6] = x[1];
                        p[7] = p[6] - (m_iPara[12]) * 5;
                    }
                    y0 = Average(a, p[6], p[7]);

                    x[1] = (int)Math.Round((y0 - alpha) / beta);
                }
                //Độ dài Đoạn L3: 30+-3
                l[2] = round((x[0] - x[1]) * 0.2);  //Đoạn L3

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[1] - (m_iPara[11]) * 5;
                        p[9] = p[8] - (m_iPara[10]) * 5;
                    }
                    else
                    {
                        p[8] = x[2];
                        p[9] = p[8] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX); //Tổng bình phương 1 hiệu giá trị từ p[8]-tbX đến p[9]-tbX
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);  // Tổng của tích 2 giá trị (p[8]-tbx)* (a[8]-tbY) đến p[8]
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;  //tỉ số Sxx/Sxy
                    else
                        return false;
                    alpha = tbY - beta * tbX;  //trung bình đương kính - beta*tbX

                    x[2] = (int)Math.Round((y0 - alpha) / beta);  // (đương kính trung bình- alpha)/beta;
                }
                //Độ dài đoạn L2: 250+-10
                l[1] = round((x[1] - x[2]) * 0.2);  //Đoạn L2 
                int viTriX2 = x[2];

                //Đường kính đoạn D1
                d_All[temp, 0] = (int)Math.Round(Average(a, x[1], x[2]));

                //Tính độ dài đoạn L1: Đoạn 50+-10
                p[10] = x[2] - (m_iPara[9]) * 5;

                y0 = Average(a, p[10], 1); //Gán =3 vì nó gần hết Core nên gắn mặc định mà không tính 

                x[3] = (int)Math.Round((y0 - alpha) / beta);
                //Chương trình đang lỗi đoạn L1. Tính đéo ra cái hồn gì ??? Vị trí các định tiến sát về 0
                l[0] = round((x[2] - x[3]) * 0.2);  //Đoạn L1

                //x[3] = x[2] - Convert.ToInt32(averageX(m_iPara[9], m_iPara[10])) * 5;
                //l[0] = round((x[2] - x[3]) * 0.2);  //Đoạn L1

                //d_All[temp,1] = Average(a, x[1], x[2]);

                //p[10] = x[2] - (m_iPara[9]) * 5;

                //y0 = Average(a, p[10], 3);

                //x[3] = (y0 - alpha) / beta;



                ////l[0] = round((x[2] - x[3]) * 0.2);  //Đoạn L1
                ////l[1] = round((x[1] - x[2]) * 0.2);  //Đoạn L2
                ////l[2] = round((x[0] - x[1]) * 0.2);  //Đoạn L3
                // l[3] = round((p[0] - x[0]) * 0.2);  //Đoạn L4

            }
            // Kết thúc 06VC003-B-VN

            //********NHÓM 5********
            else if (ItemName == "4CA004-B-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 2) * 5;
                y0 = Average(a, p[0] - 60, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - m_iPara[19] * 5;
                        p[3] = p[2] - (m_iPara[18] - 6) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[0] = CheckKizu(a, p[0] - 20, x[0], 0, y0);
                    //w[0] = CheckWave(a, p[0] - 20, x[0], 0, y0, 7);
                }
                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[17]) * 5;
                        p[5] = p[4] - (m_iPara[14] - 8) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[14] * 5;
                    }
                    tbX2 = averageX(p[4], p[5]);
                    tbY2 = Average(a, p[4], p[5]);
                    Sxx2 = ConsiderSxx(p[4], p[5], tbX2);
                    Sxy2 = ConsiderSxy(a, p[4], p[5], tbX2, tbY2);
                    if ((Sxx2 != 0) && (Sxy2 != 0))
                        beta2 = Sxy2 / Sxx2;
                    else return false;

                    alpha2 = tbY2 - beta2 * tbX2;


                    if (beta != beta2)
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta));
                    else return false;
                    x[2] = x[1] - 5;
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                p[6] = x[2] - (m_iPara[13]) * 5;
                p[7] = p[6] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[6], p[7]);

                x[3] = (int)Math.Round((y0 - alpha2) / beta2);

                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[11]) * 5;
                        p[9] = p[8] - (m_iPara[10] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[10] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[10], 3);

                x[5] = (int)Math.Round((y0 - alpha) / beta);

                //int i = x[4];
                //while ((a[i]<=y0-17)&&(i>=1))
                //{
                //	x[5] = i;
                //	i--;
                //}

                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);

                if (bUseCheckKizu)
                {
                    //    for (int i = 0; i <= 6; i++)
                    //    {
                    //        bKizu = bKizu * b[i];
                    //        bWave = bWave * w[i];
                    //    }
                }
            }
            else if (ItemName == "7CA004-B-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 2) * 5;
                y0 = Average(a, p[0] - 60, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - m_iPara[19] * 5;
                        p[3] = p[2] - (m_iPara[18] - 6) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[0] = CheckKizu(a, p[0] - 20, x[0], 0, y0);
                    //w[0] = CheckWave(a, p[0] - 20, x[0], 0, y0, 7);
                }
                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[17]) * 5;
                        p[5] = p[4] - (m_iPara[14] - 8) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[14] * 5;
                    }
                    tbX2 = averageX(p[4], p[5]);
                    tbY2 = Average(a, p[4], p[5]);
                    Sxx2 = ConsiderSxx(p[4], p[5], tbX2);
                    Sxy2 = ConsiderSxy(a, p[4], p[5], tbX2, tbY2);
                    if ((Sxx2 != 0) && (Sxy2 != 0))
                        beta2 = Sxy2 / Sxx2;
                    else return false;

                    alpha2 = tbY2 - beta2 * tbX2;


                    if (beta != beta2)
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta));
                    else return false;
                    x[2] = x[1] - 5;
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                p[6] = x[2] - (m_iPara[13]) * 5;
                p[7] = p[6] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[6], p[7]);

                x[3] = (int)Math.Round((y0 - alpha2) / beta2);

                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[11]) * 5;
                        p[9] = p[8] - (m_iPara[10] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[10] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[10], 3);

                x[5] = (int)Math.Round((y0 - alpha) / beta);

                //int i = x[4];
                //while ((a[i]<=y0-17)&&(i>=1))
                //{
                //	x[5] = i;
                //	i--;
                //}

                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);

                if (bUseCheckKizu)
                {
                    //    for (int i = 0; i <= 6; i++)
                    //    {
                    //        bKizu = bKizu * b[i];
                    //        bWave = bWave * w[i];
                    //    }
                }
            }
            else if (ItemName == "6CA007-B-VN")
            {
                p[1] = p[0] - (m_iPara[20] - 2) * 5;
                y0 = Average(a, p[0] - 20, p[1]);

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[2] = p[0] - m_iPara[19] * 5;
                        p[3] = p[2] - (m_iPara[18] - 6) * 5;
                    }
                    else
                    {
                        p[2] = x[0];
                        p[3] = p[2] - m_iPara[18] * 5;
                    }
                    tbX = averageX(p[2], p[3]);
                    tbY = Average(a, p[2], p[3]);
                    Sxx = ConsiderSxx(p[2], p[3], tbX);
                    Sxy = ConsiderSxy(a, p[2], p[3], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;

                    alpha = tbY - beta * tbX;

                    x[0] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[0] = CheckKizu(a, p[0] - 20, x[0], 0, y0);
                    //w[0] = CheckWave(a, p[0] - 20, x[0], 0, y0, 7);
                }
                d_All[temp, 2] = (int)Math.Round(Average(a, p[0] - 50, x[0]));

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[4] = x[0] - (m_iPara[17]) * 5;
                        p[5] = p[4] - (m_iPara[14] - 8) * 5;
                    }
                    else
                    {
                        p[4] = x[2];
                        p[5] = p[4] - m_iPara[14] * 5;
                    }
                    tbX2 = averageX(p[4], p[5]);
                    tbY2 = Average(a, p[4], p[5]);
                    Sxx2 = ConsiderSxx(p[4], p[5], tbX2);
                    Sxy2 = ConsiderSxy(a, p[4], p[5], tbX2, tbY2);
                    if ((Sxx2 != 0) && (Sxy2 != 0))
                        beta2 = Sxy2 / Sxx2;
                    else return false;

                    alpha2 = tbY2 - beta2 * tbX2;


                    if (beta != beta2)
                        x[1] = (int)Math.Round((alpha - alpha2) / (beta2 - beta));
                    else return false;
                    x[2] = x[1] - 5;
                }

                d_All[temp, 1] = (int)Math.Round(Average(a, x[1], x[2]));

                if (bUseCheckKizu)
                {
                    //b[1] = CheckKizu(a, x[0], x[1], beta, alpha);
                    //w[1] = CheckWave(a, x[0], x[1], beta, alpha, 6);
                }

                p[6] = x[2] - (m_iPara[13]) * 5;
                p[7] = p[6] - (m_iPara[12] - 10) * 5;

                y0 = Average(a, p[6], p[7]);

                x[3] = (int)Math.Round((y0 - alpha2) / beta2);

                if (bUseCheckKizu)
                {
                    //b[2] = CheckKizu(a, x[2], x[3], beta2, alpha2);
                    //w[2] = CheckWave(a, x[2], x[3], beta2, alpha2, 4);
                }

                for (int i = 0; i <= REPEAT; i++)
                {
                    if (i == 0)
                    {
                        p[8] = x[3] - (m_iPara[11]) * 5;
                        p[9] = p[8] - (m_iPara[10] - 5) * 5;
                    }
                    else
                    {
                        p[8] = x[4];
                        p[9] = p[8] - (m_iPara[10]) * 5;
                    }

                    tbX = averageX(p[8], p[9]);
                    tbY = Average(a, p[8], p[9]);
                    Sxx = ConsiderSxx(p[8], p[9], tbX);
                    Sxy = ConsiderSxy(a, p[8], p[9], tbX, tbY);
                    if ((Sxx != 0) && (Sxy != 0))
                        beta = Sxy / Sxx;
                    else return false;
                    alpha = tbY - beta * tbX;

                    x[4] = (int)Math.Round((y0 - alpha) / beta);
                }

                if (bUseCheckKizu)
                {
                    //b[3] = CheckKizu(a, x[3], x[4], 0, y0);
                    //w[3] = CheckWave(a, x[3], x[4], 0, y0, 3);
                }

                d_All[temp, 0] = (int)Math.Round(Average(a, x[3], x[4]));

                p[10] = x[4] - (m_iPara[9]) * 5;

                y0 = Average(a, p[10], 3);

                x[5] = (int)Math.Round((y0 - alpha) / beta);

                //int i = x[4];
                //while ((a[i]<=y0-17)&&(i>=1))
                //{
                //	x[5] = i;
                //	i--;
                //}

                l[0] = round((x[4] - x[5]) * 0.2);
                l[1] = round((x[3] - x[4]) * 0.2);
                l[2] = round((x[2] - x[3]) * 0.2);
                l[3] = round((x[1] - x[2]) * 0.2);
                l[4] = round((x[0] - x[1]) * 0.2);
                l[5] = round((p[0] - x[0]) * 0.2);

                if (bUseCheckKizu)
                {
                    //    for (int i = 0; i <= 6; i++)
                    //    {
                    //        bKizu = bKizu * b[i];
                    //        bWave = bWave * w[i];
                    //    }
                }
            }
            return true;
        }

    

        /// <summary>
        /// Tính trung bình cộng của TẤT CẢ các điểm bắt đầu đến điểm kết thúc
        /// </summary>
        /// <param name="a"></param>
        /// <param name="begin">Vị trí bắt đầu</param>
        /// <param name="end"></param>
        /// <returns></returns>
        public double Average(int[] a, int begin, int end)
        {
            if ((begin < 0) || (begin >= 3000) || (end < 0) || (end >= 3000) || (begin == end))
                return 0;
            double res, total = 0;
            for (int i = begin; i >= end; i--)
                total = total + a[i];
            res = total / (double)(begin - end + 1);
            return res;
        }
        /// <summary>
        /// Tính trung bình cộng từ vị trí s đến vị trí e => lấy gia điểm trung bình trên mảng a
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
        /// 
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
        /// Lấy phần dư
        /// </summary>
        /// <param name="fl"></param>
        /// <returns></returns>
        public double round(double fl)
        {
            double result;
            double temp = (int)(fl * 10) % 10;
            if (temp >= 5)
                result = fl + 1;
            else
                result = fl;
            return result;
        }
   
    }
}
