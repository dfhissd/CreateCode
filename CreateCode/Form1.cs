﻿using System;
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


namespace CreateCode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }
        // <summary>
        // 二维码公共处理类
        // </summary>
        public static class QRCoderHelper
        {
            // <summary>
            // 创建二维码返回文件路径名称
            // </summary>
            // <param name="plainText">二维码内容</param>
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
                    string filePath = @"C:\Images\QR\";
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
        }

        // <summary>
        // 创建二维码返回byte数组
        //</summary>
        // <param name="plainText">二维码内容</param>
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

        private void button1_Click_1(object sender, EventArgs e)
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
                plainText = "ID编码：" + textBox_ID.Text + "\r\n客户：" + textBox_Name.Text + "\r\n出货日期：" + textBox_Time.Text;
            }
            else {
                //表示需要打印普通格式文本
                if (textBox_text.Text.Trim() == string.Empty) {
                    pictureBox1.Image = null;
                    return;
                }
                // 生成二维码的内容
                plainText = textBox_text.Text;

            }
            //创建二维码返回文件路径名称
            string fileName = QRCoderHelper.CreateQRCodeToFile(plainText, Convert.ToInt32(comboBox_size.Text));

            pictureBox1.Image = Image.FromFile(fileName);
        }


    }
}
