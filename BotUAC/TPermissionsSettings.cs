
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
    class TPermissionsSettings
    {

        TExtensions extensions;
        TActions actions;
        TUserRoles userRoles;
        TUsers users;
        // вспомогательные:
        private string fileName;
        private string fileNameFull;
        private string message;

        public const string NameAllow = "allow"; 
        public const string NameDeny = "deny"; 

        //-----------------------------------------------------------------
        public TPermissionsSettings(string FileName)  // конструктор  (для структуры нельзя без прараметров !!!)
        {
            actions = new TActions();
            extensions = new TExtensions();
            userRoles = new TUserRoles();
            users = new TUsers();
            fileName = FileName;
            fileNameFull = "";
            message = "";
        }

        protected TPermissionsSettings(string FileName, string FileNameFull, string Message)  // конструктор  (для структуры нельзя без прараметров !!!)
        {
            actions = new TActions();
            extensions = new TExtensions();
            userRoles = new TUserRoles();
            users = new TUsers();
            fileName = FileName;
            fileNameFull = FileNameFull;
            message = Message;
        }


        // свойства
        public TActions Actions { get { return actions; } set { actions = value; } }
        public TExtensions Extensions { get { return extensions; } set { extensions = value; } }
        public TUserRoles UserRoles { get { return userRoles; } set { userRoles = value; } }
        public TUsers Users { get { return users; } set { users = value; } }

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

            //string sXmlFilepath = ".\\" + this.fileName;   // в ТЕКУЩЕЙ папке (брать из папки с .EXE ????)
            string sXmlFilepath = this.fileName;   // !!! уже с путем (для универсальности классов !)
            string sXmlFileNameFull = "";
            FileStream fs = null;

            try
            {
                sXmlFileNameFull = Path.GetFullPath(sXmlFilepath);  // с косой в конце !!!
                if (sXmlFileNameFull.Substring(sXmlFileNameFull.Length - 1, 1) == "\\")
                {
                    sXmlFileNameFull = sXmlFileNameFull.Substring(0, sXmlFileNameFull.Length - 2); // отрезали косую в конце! 
                }
                // в свойства Объекта (потом можно забтрать из объекта)
                this.fileNameFull = sXmlFileNameFull;

                // проверяем наличие файла
                if (!File.Exists(this.fileNameFull))
                    throw new TException(String.Format("PermissionsSettings Load: Config file " + this.fileNameFull + " does not exist!"));  //====>

                // вычитываем файла в документ
                XmlDocument xmlDoc = new XmlDocument();
                fs = File.Open(this.fileNameFull, FileMode.Open);
                xmlDoc.Load(new StreamReader(fs, Encoding.Default));

                // Получаем всех детей корневого элемента (В xml-е может быть только один корневой элемент !!!)
                if (xmlDoc.DocumentElement.Name != "permissionsSettings")
                    throw new TException("PermissionsSettings Load: Root element is not \"permissionsSettings\"");  //====>

                // всегда должно быть !   (xmlDoc.DocumentElement - корневой элемент)
                foreach (XmlNode nodeMain in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (nodeMain.Name.ToUpper())
                    {
                        case "ACTIONS":
                            this.actions = new TActions(nodeMain);
                            break; //------->

                        case "EXTENSIONS":
                            this.extensions = new TExtensions(nodeMain);
                            break; //------->

                        case "USERROLES":
                            this.UserRoles = new TUserRoles(nodeMain);
                            break; //------->

                        case "USERS":
                            this.Users = new TUsers(nodeMain);
                            break; //------->

                    } // case

                    /*
                    //string s0 = nodeMain.InnerText;
                    // перебираем все атрибуты элемента
                    foreach (XmlAttribute nodeChild in nodeMain.Attributes)
                    {
                        // nodeChild.Name - имя текущего атрибута
                        // nodeChild.Value - значение текущего атрибута
                        string s1 = nodeChild.Name + ":" + nodeChild.Value;

                        // перебираем всех детей текущего узла
                        foreach (XmlNode ch in nodeMain.ChildNodes)
                        {
                            string s2 = ch.ToString();
                            //    ...
                        }
                    }
                     */
                }

                /*
                // проверяем наличие атрибутов в файле инициализации
                //if (this.Debug == null) throw new TException("Value \"this.Debug\" does not exist");  //====>
                if (this.Url == null) throw new TException("Value \"this.Url\" does not exist");  //====>
                if (this.PortNum <= 0) throw new TException("Value \"this.PortNum\" does not exist");  //====>
                */ 

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
                //logger.Info(String.Format("    this.Debug                          = {0}", this.Debug));
            }
            else
            {
                message = sErr;
            }
            return bRet; //=======================>

        } // Load()

        //-------------------------------------------
        // сохранение параметров приложения в xml-файле (в папке приложения)
        //
        public bool Save()
        {
            bool bRet = false;
            string sErr = "";
            System.Xml.Linq.XDocument docToSave = new XDocument();   // создаем документ (для нового файла)
            try
            {
                // добавляем User - работает
                //this.users.Add(new TUser("мое имя", "моя роль", new TPermissions()));

                //--------
                // работает:
                XElement xeActions = this.actions.ToXElement();
                XElement xeExtensions = this.extensions.ToXElement();
                XElement xeUserRoles = this.userRoles.ToXElement();
                XElement xeUsers = this.users.ToXElement();

                /*
                // ------------------------
                // вариант 1 - создаем документ "с 0"
                // добавляем элементы в корневой
                //XElement xePermissionsSettings = new XElement("permissionsSettings", xeLogsDir, xePortNum);
                //xePermissionsSettings.Add(new XElement("Test", new XText("999"))); // работает !
                XElement xePermissionsSettings = new XElement("permissionsSettings");
                xePermissionsSettings.Add(xeLogsDir);
                xePermissionsSettings.Add(xePortNum);
                xePermissionsSettings.Add(xeActions);
                xePermissionsSettings.Add(xeExtensions);
                xePermissionsSettings.Add(xeExtensions);
                xePermissionsSettings.Add(xeUserRoles); 
                xePermissionsSettings.Add(xeUsers); 
                // добавляем корневой в документ
                docToSave.Add(xePermissionsSettings);
                // ------------------------
                */

                // ------------------------
                // вариант 2 - вычитываем документ из файла и подменяем раздел
                docToSave = XDocument.Load(fileNameFull);
                // удаляем старый раздел "users"
                docToSave.Descendants().Where(e => e.Name == "users").Remove();  // работает!
                //docToSave.Descendants("permissionsSettings").Where(e => e.Name == "users").Remove();  // НЕ работает!
                // добавляем новый раздел "users"
                docToSave.Root.Add(xeUsers);   // работает!
                // ------------------------
                
                //======================
                // сохраняем откоректирванный документ в файле
                //docToSave.Save(fileNameFull+ "_new");
                //======================
                  
                // нормально выгрузили параметры прилоежения
                bRet = true;
            }
            catch (Exception e)
            {
                sErr = e.Message;
            }
            if (bRet)
            {
                message = "";
            }
            else
            {
                message = sErr;
            }
            return bRet; //=======================>

        } // Save()

        public TPermissionsSettings Clone()      // internal - только в классе или в той-же программе т(сборке)
        {
            TPermissionsSettings retermissionsSettings = new TPermissionsSettings(this.fileName, this.fileNameFull, this.message);
            retermissionsSettings.Actions = this.actions.Clone();
            retermissionsSettings.Extensions = this.extensions.Clone();
            retermissionsSettings.UserRoles = this.userRoles.Clone();
            retermissionsSettings.Users = this.users.Clone();

            return retermissionsSettings; //===========>
        }


        //public TActions Actions { get { return actions; } set { actions = value; } }
        //public TExtensions Extensions { get { return extensions; } set { extensions = value; } }
        //public TUserRoles UserRoles { get { return userRoles; } set { userRoles = value; } }
        //public TUsers Users { get { return users; } set { users = value; } }

        //public string FileName { get { return fileName; } }
        //public string FileNameFull { get { return fileNameFull; } }
        //public string Message { get { return message; } }


    }  // class TPermissionsSettings

}
