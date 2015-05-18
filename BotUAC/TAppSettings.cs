
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//-----------------
using System.IO;                  // для FileStream 
using System.Xml;
using System.Xml.Linq;          // для сохранения на диске !
using System.Globalization;     // CultureInfo
using System.Collections;  // ArrayList

namespace BotUAC
{
    public class TAppSettings
    {

        private int portNum;        
        private string logsDir;
        private string permissionsFilePath;
        // вспомогательные:
        private string fileName;
        private string fileNameFull;
        private string message;

        //-----------------------------------------------------------------
        public TAppSettings(string FileName)  // конструктор  (для структуры нельзя без прараметров !!!)
        {
            logsDir = "";
            permissionsFilePath = "";
            portNum = -1;
            fileName = FileName;
            fileNameFull = "";
            message = "";
        }
        // свойства
        public int PortNum { get { return portNum; } }
        public string LogsDir { get { return logsDir; } }
        public string PermissionsFilePath { get { return permissionsFilePath; } }
        // вспомогательные:
        public string FileName { get { return fileName; } }
        public string FileNameFull { get { return fileNameFull; } }
        public string Message { get { return message; } }


        //-------------------------------------------
        // загрузка параметров приложения из xml-файла (из папки приложения)
        //
        // Для FW 3.0:
        //   using  System.Xml.Linq;
        //   XDocument xDocument = XDocument.Load(filname);
        //   var vv = xDocument.Element("Заказы").Element("Vopros7").Attribute("A").Value;
        //   textBox.Text = vv;
        //

        public bool Load()
        {
            bool bRet = false;
            string sErr = "";

            //string sXmlFile = "Irbis.xml";

            ////  c:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\bin
            //string sXmlFilepath = ".\\bin\\" + this.fileName;   // в ТЕКУЩЕЙ папке (брать из папки с .EXE ????)

            //string sXmlFilepath = HttpContext.Current.Server.MapPath("~/bin~/") + this.fileName;   // в ТЕКУЩЕЙ папке (брать из папки с .EXE ????)

            string sXmlFilepath = this.fileName;   //  !!! уже с путем (для универсальности классов !)

            string sXmlFileNameFull = "";
            FileStream fs = null;

            try
            {
                sXmlFileNameFull = Path.GetFullPath(sXmlFilepath);  // с косой в конце !!!
                if (sXmlFileNameFull.Substring(sXmlFileNameFull.Length - 1, 1) == "\\")
                {
                    sXmlFileNameFull = sXmlFileNameFull.Substring(0, sXmlFileNameFull.Length - 2); // отрезали косую в конце! 
                }
                // в свойства класса AppSetting (потом можно забтрать из объекта)
                this.fileNameFull = sXmlFileNameFull;

                // проверяем наличие файла
                if (!File.Exists(this.fileNameFull))
                    throw new TException(String.Format("AppSettings Load: Config file " + this.fileNameFull + " does not exist!"));  //====>

                XmlDocument xmlDoc = new XmlDocument();
                fs = File.Open(this.fileNameFull, FileMode.Open);
                xmlDoc.Load(new StreamReader(fs, Encoding.Default));

                // Получаем всех детей корневого элемента (В xml-е может быть только один корневой элемент !!!)
                if (xmlDoc.DocumentElement.Name != "appSettings")
                    throw new TException("AppSettings Load: Root element is not \"appSettings\"");  //====>

                // всегда должно быть !   (xmlDoc.DocumentElement - корневой элемент)
                foreach (XmlNode nodeMain in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (nodeMain.Name.ToUpper())
                    {
                        case "LOGSDIR":
                            this.logsDir = nodeMain.InnerText.Trim();
                            break; //------->

                        case "PERMISSIONSFILEPATH":
                            this.permissionsFilePath = nodeMain.InnerText.Trim();
                            break; //------->

                        case "PORTNUM":
                            this.portNum = Convert.ToInt16(nodeMain.InnerText);
                            break; //------->
                    } // case
                }

                // проверяем наличие атрибутов в файле инициализации
                if (this.PortNum == -1) throw new TException("Value \"this.portNum\" does not exist");  //====>
                if (this.LogsDir == null) throw new TException("Value \"this.pogsDir\" does not exist");  //====>
                if (this.PermissionsFilePath == null) throw new TException("Value \"this.permissionsFilePath\" does not exist");  //====>

                // нормально загрузили параметры прилоежения
                bRet = true;
            }
            catch (Exception e)
            {
                sErr = e.Message;
            }
            if (fs != null)
            {
                fs.Close();
            }
            if (bRet)
            {
                //logger.Info(String.Format("    this.portNum                = {0}", this.PortNum));
            }
            else
            {
                message = sErr;
            }
            return bRet; //=======================>

        } // Load()

    }  // class TAppSettings

}
