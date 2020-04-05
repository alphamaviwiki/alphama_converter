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
    public  class  CustomThread
    {

        //public ParameterizedThreadStart Thread1(int beginindex, int endindex, string BeginCategory, ref string result, ref string PageList, string[] array)
        //{

        //    for (int i = beginindex; i < endindex; i++)
        //    {
        //        try
        //        {
                   
        //            string strContent = String.Empty;
        //            string link = "http://www.wikidata.org/w/api.php?action=wbgetentities&format=xml&sites=enwiki&props=labels&languages=vi&titles=" + array[i];

        //            strContent = WindowsFormsApplication1.Category.GetWikiData(link);

        //            XmlDocument xd = new XmlDocument();
        //            xd.LoadXml(strContent);
        //            XmlNode page = xd.SelectSingleNode("/api/entities/entity/labels/label");
        //            strContent = page.Attributes["value"].InnerText;


        //            //Kiểm tra đã có danh mục trong các bài tiếng Việt chưa?
        //            string Temp = strContent;
        //            strContent = strContent + "&" + BeginCategory;
        //            PageList += Temp + "\r\n";
        //            result += strContent + "\r\n";
        //        }
        //        catch
        //        {
        //            continue;
        //        }
        //        Thread.Sleep(1);
        //        return null;
        //    }
           

           
        //}
    }
}
