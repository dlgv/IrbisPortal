
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//----------------
using System.Xml.Linq;          // XElement
using System.Xml;

namespace BotUAC
{
    public class TUser
    {
        private string userName;
        private string userRole;
        private TPermissions permissions;
        public TUser()  // конструктор  - нельзя без прараметров для структуры !!!
        {
            userName = null;
            userRole = null;
            permissions = new TPermissions();
        }
        public TUser(string UserName, string UserRole, TPermissions Permissions)
        {
            userName = UserName;
            userRole = UserRole;
            permissions = Permissions;
        }
        public TUser(XmlNode nodeMain)     
        {
            userName = null;
            userRole = null;
            permissions = new TPermissions();
            foreach (XmlNode nodeChild2 in nodeMain.ChildNodes)
            {
                switch (nodeChild2.Name.ToUpper())
                {
                    case "USERNAME": userName = nodeChild2.InnerText; break; //------->
                    case "USERROLE": userRole = nodeChild2.InnerText; break; //------->
                    case "PERMISSIONS": permissions = new TPermissions(nodeChild2); break; //------->
                }
            }
        }
        public string UserName { get { return userName; } set { userName = value; } }
        public string UserRole { get { return userRole; } set { userRole = value; } }
        public TPermissions Permissions { get { return permissions; } set { permissions = value; } }
        public XElement ToXElement()
        {
            XElement xe = new XElement("user");
            xe.Add(new XElement("userName", new XText(this.userName)) );
            //xe.Add(new XElement("userRole", new XText(this.userRole)) );  - пока без роей
            xe.Add(permissions.ToXElement());
            return xe; //==============>
        }
        public void PermissionSet(string actionName, string type, string extensionName, bool On)
        {
            // ищем разрешение
            TPermission perm = this.permissions.FindPermission(actionName, type);
            if (perm != null)
            {
                // ищем операцию в разрешении
                //TParameter par = perm.Parameters.FindParameter(
            }

        }
        public bool Like(TUser User)
        {
            bool bEqual = false;
            // сравниваем значения всех свойств
            if (bEqual) // имя
            {
                bEqual = (User.UserName == this.userName);
            }
            if (bEqual) // роль
            {
                bEqual = (User.userRole == this.userRole);
            }
            if (bEqual) // разрешения
            {
                if (!User.Permissions.Like(this.permissions))
                {
                    bEqual = false;
                }
            }
            return bEqual; //===========>
        }
        public TUser Clone()
        {
            TUser retUser  = new TUser(this.userName, this.userRole, this.Permissions.Clone());
            return retUser; //===========>
        }

        public override string ToString()
        {
            return userName + ": " + userRole + Environment.NewLine + permissions.ToString();
        }

    } // class TUser
}
