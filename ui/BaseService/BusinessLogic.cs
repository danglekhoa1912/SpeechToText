using System.Runtime.InteropServices;

namespace BaseService
{
    public static class BusinessLogic
    {
        public static string ProcessingContent(this string contents)
        {
            if ((contents.Contains("Nhập dấu")
                || contents.Contains("nhập dấu"))
                && !contents.Contains("Nhập dấu.")
                && !contents.Contains("nhập dấu."))
            {
                contents = contents.Replace("Nhập dấu chấm phẩy.", "; ");
                contents = contents.Replace(" nhập dấu chấm phẩy.", "; ");
                contents = contents.Replace("Nhập dấu chấm phẩy ", "; ");
                contents = contents.Replace(" nhập dấu chấm phẩy ", "; ");

                //dấu chấm hỏi
                contents = contents.Replace("Nhập dấu chấm hỏi.", "? ");
                contents = contents.Replace(" nhập dấu chấm hỏi.", "? ");
                contents = contents.Replace("Nhập dấu chấm hỏi ", "? ");
                contents = contents.Replace(" nhập dấu chấm hỏi ", "? ");

                //dấu chấm than
                contents = contents.Replace("Nhập dấu chấm than.", "! ");
                contents = contents.Replace(" nhập dấu chấm than.", "! ");
                contents = contents.Replace("Nhập dấu chấm than ", "! ");
                contents = contents.Replace(" nhập dấu chấm than ", "! ");

                //dấu chấm
                contents = contents.Replace("Nhập dấu chấm.", ". ");
                contents = contents.Replace(" nhập dấu chấm.", ". ");
                contents = contents.Replace("Nhập dấu chấm ", ". ");
                contents = contents.Replace(" nhập dấu chấm ", ". ");

                //dấu phẩy
                contents = contents.Replace("Nhập dấu phẩy.", ", ");
                contents = contents.Replace(" nhập dấu phẩy.", ", ");
                contents = contents.Replace("Nhập dấu phẩy", ", ");
                contents = contents.Replace(" nhập dấu phẩy ", ", ");

                //dấu xuống dòng
                contents = contents.Replace("Nhập dấu xuống dòng.", "\n");
                contents = contents.Replace(" nhập dấu xuống dòng.", "\n");
                contents = contents.Replace("Nhập dấu xuống dòng ", "\n");
                contents = contents.Replace(" nhập dấu xuống dòng ", "\n");

                //dấu hai chấm
                contents = contents.Replace("Nhập dấu hai chấm.", ": ");
                contents = contents.Replace(" nhập dấu hai chấm.", ": ");
                contents = contents.Replace("Nhập dấu hai chấm", ": ");
                contents = contents.Replace(" nhập dấu hai chấm ", ": ");

                //dấu gạch ngang 
                contents = contents.Replace("Nhập dấu gạch ngang.", "- ");
                contents = contents.Replace(" nhập dấu gạch ngang.", "- ");
                contents = contents.Replace("Nhập dấu gạch ngang", "- ");
                contents = contents.Replace(" nhập dấu gạch ngang ", "- ");
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