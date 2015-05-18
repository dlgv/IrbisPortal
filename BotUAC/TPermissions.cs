
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
    public class TPermissions : IEnumerator //, IEnumerable 
    {
        private ArrayList myAL = new ArrayList();       // array of TPermission
        private int Position = -1;

        // конструктор
        internal TPermissions()                      // !!! not creatable !!!
        {
        }
        public TPermissions(XmlNode nodeMain)     
        {
            // дочерние узлы - переносим в массив объектов
            foreach (XmlNode nodeChild in nodeMain.ChildNodes)
            {
                if (nodeChild.Name.ToUpper() == "PERMISSION")
                {
                    TPermission permission = new TPermission(nodeChild);
                    this.myAL.Add(permission);
                }
            }
        }

        // свойства
        public int Count { get { return myAL.Count; } }

        // методы
        public TPermission Item(int i)
        {
            return (TPermission)myAL[i];
        }

        //public TPermission Enum()
        //{
        //    return null;
        //}

        public void Add(TPermission Permission)
        {
            // append
            myAL.Add(Permission);
        }

        public void Set(TPermission Permission)
        {
            // ищем старые - заменяем или добавляем, если старого нет
            // удаляем старЫЕ
            this.Drop(Permission);
            //string sAction = Permission.Action;
            //string sType = Permission.Type;
            //for (int i = myAL.Count-1; i >= 0; i--) // удаляем с конца ВЕСЬ СПИСОК!!!
            //{
            //    if (((TPermission)myAL[i]).Action == sAction && ((TPermission)myAL[i]).Type == sType)
            //    {
            //        myAL.RemoveAt(i); 
            //    }
            //}
            // добавляем новые
            myAL.Add(Permission);
        }

        public void Drop(int inIndex)                 // internal - только в классе или в той-же программе т(сборке)
        {
            // !!! physically remove from array !
            if (inIndex >= 0 && inIndex < myAL.Count)  // allways !
            {
                myAL.RemoveAt(inIndex);
            }
        }
        public void Drop(TPermission permission)      // internal - только в классе или в той-же программе т(сборке)
        {
            // !!! physically remove from array !
            //foreach (TPermission perm in myAL)
            for (int i = myAL.Count - 1; i >= 0; i--)   /// удаляем С КОНЦА !!
            {
                if (((TPermission)myAL[i]).Action == permission.Action && ((TPermission)myAL[i]).Type == permission.Type)
                    {
                    myAL.RemoveAt(i); break; //---------->
                }
            }
        }
        public void Drop(string Action, string Type)
        {
            // !!! physically remove from array !
            for (int i = myAL.Count - 1; i >= 0; i--) // удаляем с конца - ВЕСЬ СПИСОК !!!
            {
                if (((TPermission)myAL[i]).Action == Action && ((TPermission)myAL[i]).Type == Type)
                {
                    myAL.RemoveAt(i); 
                }
            }
        }
        public TPermissions Clone()      // internal - только в классе или в той-же программе т(сборке)
        {
            TPermissions retPermissions = new TPermissions();
            for (int i = 0; i< myAL.Count; i++)
            {
                retPermissions.Add(((TPermission)myAL[i]).Clone());
            }
            return retPermissions; //===========>
        }

        public int Index(string Action, string Type)             // internal - только в классе или в той-же программе т(сборке)
        {
            int nIndex = -1;    // defaul -1 (not found)
            for (int n = 0; n < myAL.Count; n++)
            {
                if (((TPermission)myAL[n]).Action == Action && ((TPermission)myAL[n]).Type == Type)
                {
                    nIndex = n;
                    //break; - не прекращаем, вдруг еще будет ! //----------->
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

        public TPermission FindPermission(string Action, string Type)
        {
            int nIndex = -1;
            if (myAL.Count > 0)
            {
                for (int nInd = 0; nInd < myAL.Count; nInd++)
                {
                    if (((TPermission)myAL[nInd]).Action == Action && ((TPermission)myAL[nInd]).Type == Type)
                    {
                        nIndex = nInd;
                        break; //------------->
                    }
                }
            }
            if (nIndex >= 0)
                return (TPermission)myAL[nIndex];
            else
                return null;
        }

        public void Clear()
        {
            myAL.Clear();
        }

        public XElement ToXElement()
        {
            XElement xe = new XElement("permissions");
            foreach (TPermission p in this.myAL)
            {
                xe.Add(p.ToXElement());
            }
            return xe; //==============>
        }

        public bool Like(TPermissions perms)
        {
            bool bRet = true;
            // 1. все члены оригинала должны содержаться в проверяемом
            if (bRet)
            {
                if (myAL.Count != perms.Count)
                {
                    bRet = false;
                }
                else
                {
                    if (myAL.Count > 0)
                    {
                        for (int nInd = 0; nInd < myAL.Count; nInd++)
                        {
                            if (perms.FindPermission(((TPermission)myAL[nInd]).Action, ((TPermission)myAL[nInd]).Type) == null)
                            {
                                bRet = false; break; //------------->
                            }
                        }
                    }
                }
            }
            // 2. все члены проверяемого должны содержаться в оригинале
            if (bRet)
            {
                foreach (TPermission perm in perms)
                {
                    if (this.FindPermission(perm.Action, perm.Type) == null)
                    {
                        bRet = false; // break не делать - плохо себя ведет нумератор класса !!!
                    }
                }
            }
            return bRet; //==============>
        }
        
        public override string ToString()
        {
            string s = "";
            foreach (TPermission x in myAL)
            {
                s = s + (s == "" ? "" : Environment.NewLine) + x.ToString();
            }
            return s;
        }

    }
}
