using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2016中招理化生准考证
{
    public static class Common
    {
        public static Bitmap GetCode39(string strSource)
        {
            int x = 5; //左邊界     
            int y = 0; //上邊界     
            int WidLength = 2; //粗BarCode長度     
            int NarrowLength = 1; //細BarCode長度     
            int BarCodeHeight = 24; //BarCode高度     
            int intSourceLength = strSource.Length;
            string strEncode = "010010100"; //編碼字串 初值為 起始符號 *      
            string AlphaBet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*"; //Code39的字母     
            string[] Code39 = //Code39的各字母對應碼
                {     
                    "000110100",      
                    "100100001",     
                    "001100001",      
                    "101100000",     
                    "000110001",      
                    "100110000",      
                    "001110000",      
                    "000100101",     
                    "100100100",      
                    "001100100",      
                    "100001001",      
                    "001001001",     
                    "101001000",      
                    "000011001",      
                    "100011000",      
                    "001011000",     
                    "000001101",      
                    "100001100",      
                    "001001100",      
                    "000011100",     
                    "100000011",      
                    "001000011",      
                    "101000010",      
                    "000010011",     
                    "100010010",      
                    "001010010",      
                    "000000111",      
                    "100000110",     
                    "001000110",      
                    "000010110",      
                    "110000001",      
                    "011000001",     
                    "111000000",      
                    "010010001",      
                    "110010000",      
                    "011010000",     
                    "010000101",      
                    "110000100",      
                    "011000100",     
                    "010101000",     
                    "010100010",      
                    "010001010",      
                    "000101010",      
                    "010010100" 
                };
            strSource = strSource.ToUpper();
            //實作圖片     
            Bitmap objBitmap = new Bitmap(((WidLength * 3 + NarrowLength * 7) * (intSourceLength + 2)) + (x * 2), BarCodeHeight + (y * 2));
            Graphics objGraphics = Graphics.FromImage(objBitmap);
            //宣告GDI+繪圖介面     
            //填上底色     
            objGraphics.FillRectangle(Brushes.White, 0, 0, objBitmap.Width, objBitmap.Height);
            for (int i = 0; i < intSourceLength; i++)
            {
                //檢查是否有非法字元         
                if (AlphaBet.IndexOf(strSource[i]) == -1 || strSource[i] == '*')
                {
                    objGraphics.DrawString("含有非法字元", SystemFonts.DefaultFont, Brushes.Red, x, y);
                    return objBitmap;
                }
                //查表編碼         
                strEncode = string.Format("{0}0{1}", strEncode, Code39[AlphaBet.IndexOf(strSource[i])]);
            }
            strEncode = string.Format("{0}0010010100", strEncode);
            //補上結束符號 *      
            int intEncodeLength = strEncode.Length;
            //編碼後長度     
            int intBarWidth;
            for (int i = 0; i < intEncodeLength; i++)
            //依碼畫出Code39 BarCode     
            {
                intBarWidth = strEncode[i] == '1' ? WidLength : NarrowLength;
                objGraphics.FillRectangle(i % 2 == 0 ? Brushes.Black : Brushes.White, x, y, intBarWidth, BarCodeHeight); x += intBarWidth;
            }
            return objBitmap;
        }
        public static byte[] Bitmap2Byte(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }
    }
}
