using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;
using System.Diagnostics;
using System.Threading;


namespace AlphamaConverter
{
    public partial class Converter : Form
    {
        private ToolStripMenuItem options1;
        private ToolStripMenuItem options2;

        string source_text;
        string des_text;

        string source_lang;
        string des_lang;

        List<string> words = new List<string>();
        List<string> convertedwords = new List<string>();

        List<string> templatewords = new List<string>();
        List<string> templateconvertedwords = new List<string>();

        public Converter()
        {
            InitializeComponent();
            this.comboBox1.Text = "en";
            this.comboBox2.Text = "vi";
            options1 = this.showResultsContainInterlinksOnlyToolStripMenuItem;
            options2 = this.predictTranslateCategoryToolStripMenuItem;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            try
            {
                progressBar1.Minimum = 0;
                progressBar1.Value = 1;
                progressBar1.Step = 1;
                source_text = "";
                Convert();
            }
            catch
            {
                MessageBox.Show("There are some errors happened. Please restart this program!", "Error");
                System.Diagnostics.Process.Start(Application.ExecutablePath); // to start new instance of application
                this.Close(); //to turn off current app
            }

            progressBar1.Maximum = 1;
            progressBar1.PerformStep();
        }

        private void Converter_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            this.textBox2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox2_KeyDown);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode.ToString() == "A")
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.KeyPreview = true;
                this.textBox1.SelectAll();
            }
        }

        private void Converter_KeyDown(object sender, KeyEventArgs e)
        {

            //if (e.Control && e.KeyCode.ToString() == "A")
            //{

            //    this.KeyPreview = true;

            //}

        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode.ToString() == "A")
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                this.KeyPreview = true;
                this.textBox2.SelectAll();
            }
        }

        public void GetTemplateWords(string text)
        {
            templatewords = new List<string>();
            templateconvertedwords = new List<string>();
            string result = text;

            for (int i = 0; i < text.Length - 1; i++)
            {
                if (text[i] == '{' && text[i + 1] == '{')
                {
                    string temp = "";

                    for (int j = i + 2; j < text.Length - 2; j++)
                    {
                        if (text[j] == '}' && text[j + 1] == '}')
                        {
                            i = j;
                            break;
                        }
                        else
                        {
                            if (text[j] == '{' && text[j + 1] == '{')
                            {
                                // not balance
                                i = j - 1;
                                temp = "";
                                break;
                            }
                            else
                            {
                                if (text[j] == '}' && text[j + 1] != '}')
                                {
                                    //  not balance
                                    i = j + 1;

                                    break;
                                }
                                else
                                {
                                    temp += text[j];
                                }
                            }
                        }
                    }

                    if (temp != "")
                    {
                        templatewords.Add(temp);
                    }

                }

            }

            progressBar1.Maximum = words.Count * 2;

            foreach (string word in templatewords)
            {
                progressBar1.PerformStep();

                //Chỉ convert bản mẫu đơn
                if (!word.Contains("|"))
                {

                    templateconvertedwords.Add(Convert1Word("Template:" + word));
                }
                else
                {
                    templateconvertedwords.Add(word);
                }
            }
        }

        public void GetConvertedWords(string text)
        {
            words = new List<string>();
            convertedwords = new List<string>();
            string result = text;

            #region get words
            for (int i = 0; i < text.Length - 1; i++)
            {
                if (text[i] == '[' && text[i + 1] == '[')
                {
                    string temp = "";
                    
                    for (int j = i + 2; j < text.Length - 2; j++)
                    {
                        if (text[j] == ']' && text[j + 1] == ']')
                        {
                            i = j;
                            break;
                        }
                        else
                        {
                            if (text[j] == '[' && text[j + 1] == '[')
                            {
                                // not balance
                                i = j - 1;
                                temp = "";
                                break;
                            }
                            else
                            {
                                if (text[j] == ']' && text[j + 1] != ']')
                                {
                                    //  not balance
                                    i = j + 1;
                                   
                                    break;
                                }
                                else
                                {
                                    temp += text[j];
                                }
                            }
                        }
                    }

                    if (temp != "")
                    {
                        words.Add(temp);
                    }
                }
            }

            progressBar1.Maximum = words.Count * 2;
            #endregion
            
            #region convert word

            foreach (string word in words)
            {
                progressBar1.PerformStep();
                
                convertedwords.Add(Convert1Word(word));

            }

            #endregion

        }


        public string TermFormat(string word)
        {
            //Uppercase first letter and remove whitespace at the head
            // Ex: [[      Category:ABC]] ==> [[Category:ABC]]
            while (Char.IsWhiteSpace(word[0]))
            {
                word = word.Substring(1);
            }

            word = char.ToUpper(word[0]) + word.Substring(1);

           

            //Remove whitespace at the end
            while (Char.IsWhiteSpace(word[word.Length-1]))
            {
                word = word.Substring(0, word.Length - 1);
            }

            if (word.Contains(":"))
            {

                string temp1 = String.Empty;

                temp1 = word.Substring(0, word.IndexOf(":"));


                if (temp1.Length != 0)
                {

                    while (Char.IsWhiteSpace(temp1[temp1.Length - 1]))
                    {
                        temp1 = temp1.Substring(0, temp1.Length - 1);
                    }
                }
            
                string temp = String.Empty;

                temp = word.Substring(word.IndexOf(":") + 1, word.Length - word.IndexOf(":") - 1);

                while (Char.IsWhiteSpace(temp[0]))
                {
                    temp = temp.Substring(1);
                }
                temp = char.ToUpper(temp[0]) + temp.Substring(1);



                word = temp1 + ":" + temp;
            }

            //Remove |
            if (word.Contains("|"))
            {
                word = word.Substring(0, word.IndexOf("|"));
            }



            return word;
        }

        public string CheckRedirect(string word, bool show)
        {
            
            //Check redirects
            string contentlink = "https://" + source_lang + ".wikipedia.org/w/api.php?action=query&prop=revisions&rvprop=content&format=xml&titles=" + word;
            string contentdata = String.Empty;
            try
            {
                contentdata = GetWikiData(contentlink);
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(contentdata);
                XmlNode page = xd.SelectSingleNode("/api/query/pages/page/revisions/rev");
                contentdata = page.InnerText;
            }
            catch
            {
                if (options1.Checked == true) word = String.Empty;
               
                if (show == true) return word = ":" + word;
                else return word;

            }

            string temp1 = contentdata.ToLower();

            Match m = Regex.Match(contentdata, @"#\s*[Rr][Ee][Dd][Ii][Rr][Ee][Cc][Tt]");
            Match m1 = Regex.Match(contentdata, @"#[Đđ][Ổổ][Ii]");
            Match m2 = Regex.Match(contentdata, @"\{\{[Đđ]ổi\s*hướng\s*thể\s*loại");

            // Đa số bài redirect bắt đầu = dấu # hay #REDIRECT?  +  //API check redirect
            // Vị trí dò cụm từ phải đầu tiên của trang
            if (m.Success && m.Index < 10)
            {
                word = contentdata.Substring(contentdata.IndexOf("[["), contentdata.Length - contentdata.IndexOf("[["));
                word = word.Replace("[[", "");
                word = word.Substring(0, word.IndexOf("]]"));
            }

            return word;
        }

        public string GetWord(string word, string originWord, bool show, ref bool result)
        {
            string wordpart1 = String.Empty;
            string wordpart2 = String.Empty;
            string link = "https://www.wikidata.org/w/api.php?action=wbgetentities&format=xml&sites=" + source_lang + "wiki&props=sitelinks&redirects=no&sitefilter=" + des_lang + "wiki&titles=" + Uri.EscapeDataString(word);
            string temp = String.Empty;
            try
            {

                temp = GetWikiData(link);

                if (temp.Contains("missing=")) temp = originWord;
                else
                {
                    XmlDocument xd = new XmlDocument();
                    xd.LoadXml(temp);
                    XmlNodeList pages = xd.SelectNodes("/api/entities/*/sitelinks/sitelink");
                    temp = pages[0].Attributes["title"].InnerText;
                    //Convert OK
                    result = true;
                }
            }
            catch
            {
                //Bật option2 trường hợp muốn convert danh mục chứa tồn tại, trường hợp này sẽ không kiểm tra #redirect

                string temp2 = String.Empty;

                bool checktemp2 = false;

                if (options2.Checked == true && word.Contains(":"))
                {

                    wordpart1 = word.Substring(0, word.IndexOf(":"));
                    wordpart2 = word.Substring(word.IndexOf(":") + 1, word.Length - word.IndexOf(":") - 1);

                    if (wordpart2.Length > 0)
                    {
                        //Search in Wikidata
                        //link = "https://www.wikidata.org/w/api.php?action=wbgetentities&format=xml&sites=" + source_lang + "wiki&props=sitelinks&titles=" + wordpart2;

                        link = "https://www.wikidata.org/w/api.php?action=wbgetentities&format=xml&sites=" + source_lang + "wiki&props=sitelinks&redirects=no&sitefilter=" + des_lang + "wiki&titles=" + Uri.EscapeDataString(wordpart2);

                        //link = "https://www.wikidata.org/w/api.php?action=wbgetentities&format=xml&sites=" + source_lang + "wiki&props=labels&languages=" + des_lang + "&titles=" + wordpart2;
                        //nonexistcat = true;
                    }


                    try
                    {

                        temp2 = GetWikiData(link);

                        if (temp2.Contains("missing=")) temp2 = originWord;
                        else
                        {

                            XmlDocument xd = new XmlDocument();
                            xd.LoadXml(temp2);
                            XmlNodeList pages = xd.SelectNodes("/api/entities/*/sitelinks/sitelink");
                            temp2 = pages[0].Attributes["title"].InnerText;
                            checktemp2 = true;

                            //Convert OK
                            result = true;


                        }

                    }
                    catch
                    {
                        //Empty

                    }

                }

                if (checktemp2 == true)
                {
                    if (show == true)
                    {
                        return ":" + wordpart1 + ":" + temp2 + "<!--Link đỏ hoặc chưa có Interwiki -->";
                    }
                    else
                    {
                        return wordpart1 + ":" + temp2 + "<!-- Link đỏ hoặc chưa có Interwiki -->";
                    }
                }
                else
                {
                    if (options1.Checked == true)
                    {
                        if (temp.Contains("<?xml version=\"1.0\"?>"))
                            word = String.Empty;
                    }

                    if (show == true)
                    {
                        return word = ":" + word;
                    }

                    else
                    {
                        return word;
                    }

                }

            }


            if (show == true)
            {
                //Viết hoa chữ cái đầu
                temp = ":" + char.ToUpper(temp[0]) + temp.Substring(1);

            }
            else
            {

                //Viết hoa chữ cái đầu
                temp = char.ToUpper(temp[0]) + temp.Substring(1);
            }

            return temp;
            

        }

        public string Convert1Word(string word)
        {         
            bool show = false;
           // bool nonexistcat = false;

            //Không dịch Image, File dùng cho tiếng Anh
            //[[Tập tin:ABC.JPG|thumb]]

            if (word.Contains("File:") || word.Contains("Image:") || word.Contains("Tập tin:") || word.Contains("Hình ảnh:"))
            {
                return word;
            }

            word = TermFormat(word);

            string originWord = word;

            if (word.IndexOf('|') != -1)
            {
                word = word.Substring(0, word.IndexOf('|'));
            }
            
            //Nếu chứa : ở đầu tức muốn hiện cụm từ này như văn bản, ví dụ [[:Category:Love]] ==> Thể loại:Tình yêu trên văn bản
            if (word.Substring(0, 1) == ":")
            {
                word = word.Substring(1);
                show = true;
            }

            //Cụm từ chứa dấu #, ví dụ Love#abc thì loại bỏ chỉ còn Love
            if (word.Contains("#"))
            {
                word = word.Substring(0, word.IndexOf("#"));
            }

            bool result = false;

            //word = GetWord(word, originWord, show, ref result);

           // if (result != true)
            //{
                word = CheckRedirect(word, show);
                word = GetWord(word, originWord, show, ref result);
           // }

            return word;
        }

        public void Convert()
        {

            if (this.comboBox1.Text != String.Empty) source_lang = this.comboBox1.Text;
            if (this.comboBox2.Text != String.Empty) des_lang = this.comboBox2.Text;
           
            source_text = this.textBox1.Text;

            if (source_text != String.Empty)
            {

                des_text = source_text;
                GetConvertedWords(source_text);
                GetTemplateWords(source_text);
                this.textBox2.Text = "";

                #region Apply changes

                for (int i = 0; i < words.Count; i++)
                {

                    progressBar1.PerformStep();
                    //Lowercase first letter
                    string temp = String.Empty;
                    string temp1 = String.Empty;

                    temp = words[i];
                    temp1 = convertedwords[i];

                    if (temp1 == String.Empty) des_text = des_text.Replace("[[" + temp + "]]", "");
                    else des_text = des_text.Replace("[[" + temp + "]]", "[[" + convertedwords[i] + "]]");
                    
                }

                for (int i = 0; i < templatewords.Count; i++)
                {
                    progressBar1.PerformStep();
                    string temp = String.Empty;
                    string temp1 = String.Empty;
                    
                    temp = templatewords[i];
                    temp1 = templateconvertedwords[i];

                    //Bản mẫu có nhiều tham số cách nhau bởi | thì không thay thế
                    if (!temp1.Contains("|")) temp1 = temp1.Substring(temp1.IndexOf(":") + 1, temp1.Length - temp1.IndexOf(":") - 1);
                    
                    if (temp1 == String.Empty) des_text = des_text.Replace("{{" + temp + "}}", "");
                    else des_text = des_text.Replace("{{" + temp + "}}", "{{" + temp1 + "}}");
                }

                this.textBox2.Text = Translation(source_lang, des_lang, des_text);
                #endregion

            }
  
        }

       
        //Dịch các cụm từ phổ biến
        public string Translation(string language, string deslanguage, string text)
        {
            if (language == "en" && deslanguage == "vi")
            {
                string pattern = "==\\s?(E|e)xternal links\\s?==";
                Regex rgx = new Regex(pattern);
                text = rgx.Replace(text, "== Liên kết ngoài ==");

                pattern = "==\\s?(F|f)urther reading\\s?==";
                rgx = new Regex(pattern);
                text = rgx.Replace(text, "== Đọc thêm ==");

                pattern = "==\\s?(R|r)eferences\\s?==";
                rgx = new Regex(pattern);
                text = rgx.Replace(text, "== Tham khảo ==");

                pattern = "==\\s?(S|s)ee also\\s?==";
                rgx = new Regex(pattern);
                text = rgx.Replace(text, "== Xem thêm ==");

                pattern = "==\\s?(N|n)otes\\s?==";
                rgx = new Regex(pattern);
                text = rgx.Replace(text, "== Ghi chú ==");
            }

            if (language=="en" && deslanguage == "ur")
            {
                string pattern = "==\\s?(E|e)xternal links\\s?==";
                Regex rgx = new Regex(pattern);
                text = rgx.Replace(text, "== بیرونی روابط ==");

                pattern = "==\\s?(F|f)urther reading\\s?==";
                rgx = new Regex(pattern);
                text = rgx.Replace(text, "== مزید پڑھیں ==");

                pattern = "==\\s?(R|r)eferences\\s?==";
                rgx = new Regex(pattern);
                text = rgx.Replace(text, "== حوالہ جات ==");

                pattern = "==\\s?(S|s)ee also\\s?==";
                rgx = new Regex(pattern);
                text = rgx.Replace(text, "== مزید دیکھیے ==");

                pattern = "==\\s?(N|n)otes\\s?==";
                rgx = new Regex(pattern);
                text = rgx.Replace(text, "== نوٹ ==");
            }

            return text;
        }


        public static string GetWikiData(string link)
        {

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.DefaultConnectionLimit = 9999;
            //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12

            string result = String.Empty;
            WebRequest objWebRequest = WebRequest.Create(link);
            objWebRequest.Credentials = CredentialCache.DefaultCredentials;
            ((HttpWebRequest)objWebRequest).UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";

            WebResponse objWebResponse = objWebRequest.GetResponse();
            Stream receiveStream = objWebResponse.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
            result = readStream.ReadToEnd();
            objWebResponse.Close();
            readStream.Close();
            return result;

        }

        private void authorToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            if (MessageBox.Show("My name is Alphama. I am from Vietnamese Wikipedia. My email is \"alphamawikipedia@gmail.com\". Do you want to contact me?", "Author", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://vi.wikipedia.org/wiki/User:Alphama");
            }
        }

        private void versionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Alphama Converter 1.0 \r\nThis tool is used for converting words and terms between different Wikipedias. ", "Version 1.1.10");
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This tool is used for translating terms, templates and categories between Wikipedia language editions. \r\n\r\n 1. Copy & paste content to left textbox. \r\n 2. Choose a source language and a translated language. \r\n 3. Click \"Convert\" button and receive results in the right textbox.", "Help");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();


                Clipboard.SetText(this.textBox2.Text);
            }
            catch
            {
                Clipboard.SetText(" ");
            }
            
        }
        private void showResultsContainInterlinksOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (options1.Checked == false)
            {
                options1.CheckState = CheckState.Checked;

            }
            else
            {
                options1.CheckState = CheckState.Unchecked;
            }

            // uncheck the old one
            //temp.CheckState = CheckState.Unchecked;
            //temp = (ToolStripMenuItem)sender;
            // check the new one
            //temp.CheckState = CheckState.Checked;
        }

        private void predictTranslateCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (options2.Checked == false)
            {
                options2.CheckState = CheckState.Checked;

            }
            else
            {
                options2.CheckState = CheckState.Unchecked;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

       
    }
}
