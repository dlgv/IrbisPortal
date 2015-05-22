
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
        private string eventName;
        private string mutexName;
        private string permissionsFileNameAndPath;
        private string permissionsFileNameOnly;
        // вспомогательные:
        private string fileName;
        private string fileNameFull;
        private string message;

        //-----------------------------------------------------------------
        public TAppSettings(string FileNameFull)  // конструктор  (для структуры нельзя без прараметров !!!)
        {
            logsDir = "";
            eventName = "";
            mutexName = "";
            permissionsFileNameAndPath = "";
            permissionsFileNameOnly = "";
            portNum = -1;
            fileName = "";
            fileNameFull = FileNameFull;
            message = "";
        }
        // свойства
        public int PortNum { get { return portNum; } }
        public string LogsDir { get { return logsDir; } }
        public string EventName { get { return eventName; } }
        public string MutexName { get { return mutexName; } }
        public string PermissionsFileNameAndPath { get { return permissionsFileNameAndPath; } }
        public string PermissionsFileNameOnly { get { return permissionsFileNameOnly; } }
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
            //string sXmlFilePath = ".\\bin\\" + this.fileName;   // в ТЕКУЩЕЙ папке (брать из папки с .EXE ????)
            //string sXmlFilePath = HttpContext.Current.Server.MapPath("~/bin~/") + this.fileName;   // в ТЕКУЩЕЙ папке (брать из папки с .EXE ????)

            string sXmlFilePath = null;
            //string sXmlFileNameFull = "";

            FileStream fs = null;

            try
            {
                /*
                - !!! готовоеимя с ПУТЕМ взяли из web.config !!!
                sXmlFilePath = this.fileName;   //  !!! уже с путем (для универсальности классов !)
                sXmlFileNameFull = Path.GetFullPath(sXmlFilePath);  // с косой в конце !!!
                if (sXmlFileNameFull.Substring(sXmlFileNameFull.Length - 1, 1) == "\\")
                {
                    sXmlFileNameFull = sXmlFileNameFull.Substring(0, sXmlFileNameFull.Length - 2); // отрезали косую в конце! 
                }
                // в свойства класса AppSetting (потом можно забтрать из объекта)
                this.fileNameFull = sXmlFileNameFull;
                */

                //sXmlFilePath = Path.GetFullPath(fileNameFull);  // полное имя с путем !!!
                sXmlFilePath = Path.GetDirectoryName(fileNameFull);  // !!! без косых в конце "c:\\irbis"

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

                        case "EVENTNAME":
                            this.eventName = nodeMain.InnerText.Trim();
                            break; //------->

                        case "MUTEXNAME":
                            this.mutexName = nodeMain.InnerText.Trim();
                            break; //------->

                        case "PERMISSIONSFILENAME":
                            this.permissionsFileNameOnly = nodeMain.InnerText.Trim();
                            break; //------->

                        case "PORTNUM":
                            this.portNum = Convert.ToInt16(nodeMain.InnerText);
                            break; //------->
                    } // case
                }

                // проверяем наличие атрибутов в файле инициализации
                if (this.PortNum == -1) throw new TException("Value \"this.portNum\" does not exist");  //====>
                if (this.LogsDir == null) throw new TException("Value \"this.pogsDir\" does not exist");  //====>
                if (this.EventName == null) throw new TException("Value \"this.eventName\" does not exist");  //====>
                if (this.MutexName == null) throw new TException("Value \"this.mutexName\" does not exist");  //====>
                if (this.PermissionsFileNameOnly == null) throw new TException("Value \"this.PermissionsFileNameOnly\" does not exist");  //====>

                // файл разрешений в той же папке, что и Irbis.xml - добавляем путь к вычитанному имеи
                this.permissionsFileNameAndPath = sXmlFilePath  + "\\" + this.permissionsFileNameOnly;

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
