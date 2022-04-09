using System;
using System.Runtime.InteropServices;

namespace BaseService
{
    public static class BusinessLogic
    {
        public static string ProcessingContent(this string contents)
        {
                if ((contents.Contains("Nhập")
                    || contents.Contains("nhập"))
                    && !contents.Contains("Nhập.")
                    && !contents.Contains("nhập."))
                {
                    contents = contents.Replace("Nhập chấm phẩy.", "; ");
                    contents = contents.Replace(" nhập chấm phẩy.", "; ");
                    contents = contents.Replace("Nhập chấm phẩy", "; ");
                    contents = contents.Replace("nhập chấm phẩy", "; ");

                    //dấu chấm hỏi
                    contents = contents.Replace("Nhập chấm hỏi.", "? ");
                    contents = contents.Replace(" nhập chấm hỏi.", "? ");
                    contents = contents.Replace("Nhập chấm hỏi", "? ");
                    contents = contents.Replace("nhập chấm hỏi", "? ");

                    //dấu chấm than
                    contents = contents.Replace("Nhập chấm than.", "! ");
                    contents = contents.Replace(" nhập chấm than.", "! ");
                    contents = contents.Replace("Nhập chấm than", "! ");
                    contents = contents.Replace("nhập chấm than", "! ");

                    //dấu chấm
                    contents = contents.Replace("Nhập chấm.", ". ");
                    contents = contents.Replace(" nhập chấm.", ". ");
                    contents = contents.Replace("Nhập chấm", ". ");
                    contents = contents.Replace("nhập chấm", ". ");

                    //dấu phẩy
                    contents = contents.Replace("Nhập phẩy.", ", ");
                    contents = contents.Replace(" nhập phẩy.", ", ");
                    contents = contents.Replace("Nhập phẩy", ", ");
                    contents = contents.Replace("nhập phẩy", ", ");

                    //dấu xuống dòng
                    contents = contents.Replace("Nhập xuống dòng.", "\r\n");
                    contents = contents.Replace(" nhập xuống dòng.", "\r\n");
                    contents = contents.Replace("Nhập Xuống dòng ", "\r\n");
                    contents = contents.Replace("nhập xuống dòng", "\r\n");

                    //dấu hai chấm
                    contents = contents.Replace("Nhập hai chấm.", ": ");
                    contents = contents.Replace(" nhập hai chấm.", ": ");
                    contents = contents.Replace("Nhập hai chấm", ": ");
                    contents = contents.Replace("nhập hai chấm", ": ");

                    //dấu gạch ngang 
                    contents = contents.Replace("Nhập gạch ngang.", "- ");
                    contents = contents.Replace(" nhập gạch ngang.", "- ");
                    contents = contents.Replace("Nhập gạch ngang", "- ");
                    contents = contents.Replace("nhập gạch ngang", "- ");

                    //dấu 3 chấm
                    contents = contents.Replace("nhập 3 chấm", "... ");
                    contents = contents.Replace("nhậpXuống dòng Alo ba chấm", "... ");

                    // Đóng mửo ngoặc 
                    contents = contents.Replace("nhập ngoặc đơn", "' ");
                    contents = contents.Replace("nhập ngoặc đơn", "' ");
                    contents = contents.Replace("nhập ngoặc kép", "\" ");
                    contents = contents.Replace("nhập ngoặc kép", "\" ");
                    contents = contents.Replace("nhập mở ngoặc", "( ");
                    contents = contents.Replace("nhập đóng ngoặc", ") ");
                    //Nhập ký tự đặc biệt 
                    contents = contents.Replace("Nhập thăng", "#");
                    contents = contents.Replace("Nhập phần trăm", "%");
                    contents = contents.Replace("Nhập và", "&");
                    contents = contents.Replace("Nhập sao", "*");
                    contents = contents.Replace("Nhập gạch dưới", "_");
                    contents = contents.Replace("Nhập bằng", "=");
                    contents = contents.Replace("Nhập cộng", "+");
                    contents = contents.Replace("Nhập phần trăm", "%");
                    contents = contents.Replace("Nhập ngoặc nhọn mở", "{");
                    contents = contents.Replace("Nhập ngoặc nhọn đóng", "}");
                }

                // xoa tu 
                if (contents.IndexOf("từ") - contents.IndexOf("xóa đi") == 9 || contents.IndexOf("từ") - contents.IndexOf("xóa đi") == 10)
                {
                    String s = contents.Substring(contents.IndexOf("xóa đi"));
                    String[] re = s.Split(' ');
                    int t = (int)Int64.Parse(re[2]);
                    contents = contents.Replace(s, "");
                    String[] re2 = contents.Split(' ');
                    for (int i = re2.Length - 1; i >= 0; i--)
                    {
                        if (t >= 0)
                        {
                            re2[i] = "";
                            t--;
                        }
                    }
                    s = "";
                    for (int i = 0; i < re2.Length; i++)
                    {
                        s = s + re2[i] + " ";
                    }
                    contents = s;
                }

                // viết hoa chữ cái đầu tiên
                contents = char.ToUpper(contents[0]) + contents.Substring(1);


                // viết hoa sau dấu câu 
                string[] punctuators = { "?", "!", ",", ":", ";", "." };
                for (int i = 0; i < 6; i++)
                {
                    int pos = contents.IndexOf(punctuators[i]);
                    while (pos != -1 && pos < contents.Length - 2)
                    {
                        contents = contents.Insert(pos + 2, char.ToUpper(contents[pos + 2]).ToString());
                        contents = contents.Remove(pos + 3, 1);
                        pos = contents.IndexOf(punctuators[i], pos + 1);
                    }
                }
                return contents;
            }
    }
}