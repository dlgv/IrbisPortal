
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//---------------
using System.Xml;
using System.Xml.Linq;          // XElement


namespace BotUAC
{
    [Serializable]
    public class TUserRole
    {
        private string roleName;
        private TPermissions permissions;
        public TUserRole()  // конструктор  - нельзя без прараметров для структуры !!!
        {
            roleName = null;
            permissions = new TPermissions();
        }
        public TUserRole(string RoleName, TPermissions Permissions)
        {
            roleName = RoleName;
            permissions = Permissions;
        }
        public TUserRole(XmlNode nodeMain)     
        {
            roleName = null;
            permissions = new TPermissions();
            foreach (XmlNode nodeChild2 in nodeMain.ChildNodes)
            {
                switch (nodeChild2.Name.ToUpper())
                {
                    case "ROLENAME":
                        roleName = nodeChild2.InnerText; 
                        break; //------->

                    case "PERMISSIONS":
                        permissions =  new TPermissions(nodeChild2); 
                        break; //------->
                }
            }
        }

        public string RoleName { get { return roleName; } set { roleName = value; } }
        public TPermissions Permissions { get { return permissions; } set { permissions = value; } }
        public XElement ToXElement()
        {
            XElement xe = new XElement("userRole");
            xe.Add(new XElement("roleName", new XText(this.roleName)));
            xe.Add(permissions.ToXElement());
            return xe; //==============>
        }

        public TUserRole Clone()
        {
            TUserRole retUserRole = new TUserRole(this.roleName, this.permissions.Clone());
            return retUserRole; //==============>
        }

        public override string ToString()
        {
            return roleName + Environment.NewLine + permissions.ToString();
        }

    } // class TUserRole
}
