
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//----------------
using System.Xml;
using System.Xml.Linq;          // XElement
using System.Collections;  // ArrayList


namespace BotUAC
{
    [Serializable]
    public class TUserRoles : IEnumerator //, IEnumerable 
    {
        private ArrayList myAL = new ArrayList();       // array of TUserRole
        private int Position = -1;

        // конструктор
        internal TUserRoles()                      // !!! not creatable !!!
        {
        }
        public TUserRoles(XmlNode nodeMain)     
        {
            // дочерние узлы - переносим в массив объектов
            foreach (XmlNode nodeChild in nodeMain.ChildNodes)
            {
                if (nodeChild.Name.ToUpper() == "USERROLE")
                {
                    TUserRole userRole = new TUserRole(nodeChild);
                    this.myAL.Add(userRole);
                }
            }
        }

        // методы
        public int Count()
        {
            return myAL.Count;
        }

        public TUserRole Item(int i)
        {
            return (TUserRole)myAL[i];
        }

        //public TUserRole Enum()
        //{
        //    return null;
        //}

        public void Add(TUserRole inObj)
        {
            // append
            myAL.Add(inObj);
        }

        internal void Drop(int inIndex)                 // internal - только в классе или в той-же программе т(сборке)
        {
            // !!! physically remove from array !
            if (inIndex >= 0 && inIndex < myAL.Count)  // allways !
            {
                myAL.RemoveAt(inIndex);
            }
        }

        internal int IndexByRoleName(string inRoleName)             // internal - только в классе или в той-же программе т(сборке)
        {
            int nIndex = -1;    // defaul -1 (not found)
            for (int n = 0; n < myAL.Count; n++)
            {
                if (((TUserRole)myAL[n]).RoleName == inRoleName)
                {
                    nIndex = n;
                    break; //----------->
                }
            }
            return nIndex;
        }







        /* === Needed since Implementing IEnumerable*/
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }

        /* ===Needed since Implementing IEnumerator*/
        public bool MoveNext()
        {
            if (Position < myAL.Count - 1)
            {
                ++Position;
                return true; //----------------->
            }
            else
            {
                this.Reset(); // сбрасываем позицию на -1
                return false; //------------------>
            }
        }

        public void Reset()
        {
            Position = -1;
        }
        public Object Current
        {
            get
            {
                return (object)myAL[Position];
            }
        }


        public TUserRole FindUserRole(string inRoleName)
        {
            int nIndex = -1;
            if (myAL.Count > 0)
            {
                for (int nInd = 0; nInd < myAL.Count; nInd++)
                {
                    if (((TUserRole)myAL[nInd]).RoleName == inRoleName)
                    {
                        nIndex = nInd;
                        break; //------------->
                    }
                }
            }
            if (nIndex >= 0)
                return (TUserRole)myAL[nIndex];

            else
                return null;
        }

        public void Clear()
        {
            myAL.Clear();
        }

        public XElement ToXElement()
        {
            XElement xe = new XElement("userRoles");
            foreach (TUserRole p in this.myAL)
            {
                xe.Add(p.ToXElement());
            }
            return xe; //==============>
        }

        public TUserRoles Clone()
        {
            TUserRoles retUserRoles = new TUserRoles();
            foreach (TUserRole p in this.myAL)
            {
                retUserRoles.Add(p.Clone());
            }
            return retUserRoles; //==============>
        }

        public override string ToString()
        {
            string s = "";
            foreach (TUserRole x in myAL)
            {
                s = s + (s == "" ? "" : Environment.NewLine) + x.ToString();
            }
            return s;
        }

    }
}
