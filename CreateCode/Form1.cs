using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QRCoder;
using System.IO;
using System.Security.Cryptography;

namespace CreateCode
{
    public partial class Form1 : Form
    {
        public string ProductInfoStartFilePath = Application.StartupPath + @"\ProductInfoLogs";
        public bool TxtHead_Flag = false;
        public int Code_Number = 0;

        public Form1()
        {
            InitializeComponent();
        }
        // <summary>
        // 二维码公共处理类
        // </summary>
        public static class QRCoderHelper
        {
            /// <summary>
            /// 创建二维码返回文件路径名称
            /// </summary>
            /// <param name="plainText">二维码内容</param>
            public static string CreateQRCodeToFile(string plainText, int size)
            {
                try
                {
                    string fileName = "";
                    if (String.IsNullOrEmpty(plainText))
                    {
                        return "";
                    }

                    //二维码文件目录
                    string filePath = Application.StartupPath + @"\Images\QR\";
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    //创建二维码文件路径名称
                    fileName = filePath + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 1000) + ".jpeg";

                    QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
                    //QRCodeGenerator.ECCLevel:纠错能力,Q级：约可纠错25%的数据码字
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrcode = new QRCode(qrCodeData);
                    //Bitmap qrCodeImage = qrcode.GetGraphic(5);
                    Bitmap qrCodeImage = qrcode.GetGraphic(size, Color.Black, Color.White, false);
                    qrCodeImage.Save(fileName, ImageFormat.Jpeg);
                    return fileName;
                }
                catch (Exception ex)
                {
                    throw new Exception("创建二维码返回文件路径名称方法异常", ex);
                }
            }
            /// <summary>
            /// 创建二维码返回byte数组
            /// </summary>
            /// <param name="plainText">二维码内容</param>
            public static byte[] CreateQRCodeToBytes(string plainText)
            {
                try
                {
                    if (String.IsNullOrEmpty(plainText))
                    {
                        return null;
                    }

                    QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
                    //QRCodeGenerator.ECCLevel:纠错能力,Q级：约可纠错25%的数据码字
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrcode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrcode.GetGraphic(5);
                    MemoryStream ms = new MemoryStream();
                    qrCodeImage.Save(ms, ImageFormat.Jpeg);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    ms.Close();

                    return arr;
                }
                catch (Exception ex)
                {
                    throw new Exception("创建二维码返回byte数组方法异常", ex);
                }
            }
            /// <summary>
            /// 创建二维码返回Base64字符串
            /// </summary>
            /// <param name="plainText">二维码内容</param>
            public static string CreateQRCodeToBase64(string plainText, bool hasEdify = true)
            {
                try
                {
                    string result = "";
                    if (String.IsNullOrEmpty(plainText))
                    {
                        return "";
                    }

                    QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
                    //QRCodeGenerator.ECCLevel:纠错能力,Q级：约可纠错25%的数据码字
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrcode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrcode.GetGraphic(5);
                    MemoryStream ms = new MemoryStream();
                    qrCodeImage.Save(ms, ImageFormat.Jpeg);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    ms.Close();
                    if (hasEdify)
                    {
                        result = "data:image/jpeg;base64," + Convert.ToBase64String(arr);
                    }
                    else
                    {
                        result = Convert.ToBase64String(arr);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("创建二维码返回Base64字符串方法异常", ex);
                }
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            string plainText = "";

            if (tabControl1.SelectedTab == tabPage1) {
                //表示需要打印格式文本
                if ((textBox_ID.Text.Trim() == string.Empty)&& (textBox_Name.Text.Trim() == string.Empty)&&(textBox_Time.Text.Trim() == string.Empty))
                {
                    pictureBox1.Image = null;
                    return;
                }
                // 生成二维码的内容
                plainText = "ID編碼：" + textBox_ID.Text + "\r\n客戶名稱：" + textBox_Name.Text + "\r\n出貨日期：" + textBox_Time.Text + "\r\n聯繫電話：" + textBox_phone.Text + "\r\n備註：" + textBox_txt.Text ;
            }
            //else {
            //    //表示需要打印普通格式文本
            //    if (textBox_text.Text.Trim() == string.Empty) {
            //        pictureBox1.Image = null;
            //        return;
            //    }

            //    // 生成二维码的内容
            //    //plainText = "file:///C:/Users/12783/Desktop/C%23、halcon练习/C%23生成二维码/CreateCode/CreateCode/bin/Debug/uncompile.html" + MD5Encrypt(textBox_text.Text) + "&password=" + "123456";
            //}
            
            //创建二维码返回文件路径名称
            string fileName = QRCoderHelper.CreateQRCodeToFile(plainText, Convert.ToInt32(comboBox_size.Text));
            Code_Number++;
            SaveProductInfoData();
            pictureBox1.Image = Image.FromFile(fileName);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool Write(string mFile, List<List<string>> Data, string Separator)
        {
            try
            {
                if (Data == null)
                    return false;

                using (StreamWriter sw = new StreamWriter(mFile, true, Encoding.Default))
                {
                    foreach (List<string> T in Data)
                    {
                        int len = T.Count;
                        if (len > 0)
                        {
                            string data_0 = T[0];
                            if (len > 1)
                                for (int i = 1; i < len; i++)
                                {
                                    data_0 = string.Format("{0}{1}{2}", data_0, Separator, T[i]);
                                }
                            sw.WriteLine(data_0);
                        }
                    }

                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取信息数据
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<List<string>> GetProductInfoToData()
        {
            List<List<string>> Data = new List<List<string>>();
            string Time = DateTime.Now.ToString("[HH時mm分ss秒]");
            Data.Add(new List<string>());
            Data[Data.Count - 1].Add(Time);//时间
            if (TxtHead_Flag == false) { 
                Data.Add(new List<string>());
                Data[Data.Count - 1].Add("编号");//简述
                Data[Data.Count - 1].Add("ID");//简述
                Data[Data.Count - 1].Add("名称");//简述
                Data[Data.Count - 1].Add("日期");//简述
                Data[Data.Count - 1].Add("电话");//简述
                Data[Data.Count - 1].Add("备注");//简述
                TxtHead_Flag = true;
            }
            Data.Add(new List<string>());
            Data[Data.Count - 1].Add(Convert.ToString(Code_Number));//编号
            Data[Data.Count - 1].Add(textBox_ID.Text);//ID编码
            Data[Data.Count - 1].Add(textBox_Name.Text);//客户名称
            Data[Data.Count - 1].Add(textBox_Time.Text);//日期
            Data[Data.Count - 1].Add(textBox_phone.Text);//电话
            Data[Data.Count - 1].Add(textBox_txt.Text);//备注


            return Data;
        }
        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public void SaveProductInfoData()
        {
            try
            {
                //判断产品信息文件夹是否存在
                if (!Directory.Exists(ProductInfoStartFilePath))
                    Directory.CreateDirectory(ProductInfoStartFilePath);
                string Time = DateTime.Now.ToString("yyyy年MM月dd日");
                string Path = ProductInfoStartFilePath + "\\" + Time;
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);
                Path += "\\" + Time + ".csv";
                Write(Path, GetProductInfoToData(), ",");
            }
            catch { }
        }

        /// <summary>
        /// 用MD5加密字符串，可选择生成16位或者32位的加密字符串
        /// </summary>
        /// <param name="password">待加密的字符串</param>
        /// <param name="bit">位数，一般取值16 或 32</param>
        /// <returns>返回的加密后的字符串</returns>
        public string MD5Encrypt(string password, int bit)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(password));
            StringBuilder tmp = new StringBuilder();
            foreach (byte i in hashedDataBytes)
            {
                tmp.Append(i.ToString("x2"));
            }
            if (bit == 16)
                return tmp.ToString().Substring(8, 16);
            else
            if (bit == 32) return tmp.ToString();//默认情况
            else return string.Empty;
        }
        /// <summary>
        /// 用MD5加密字符串
        /// </summary>
        /// <param name="password">待加密的字符串</param>
        /// <returns></returns>
        public string MD5Encrypt(string password)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(password));
            StringBuilder tmp = new StringBuilder();
            foreach (byte i in hashedDataBytes)
            {
                tmp.Append(i.ToString("x2"));
            }
            return tmp.ToString();
        }

    }
}
