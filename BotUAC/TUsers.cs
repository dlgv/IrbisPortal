
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
    public class TUsers : IEnumerator //, IEnumerable 
    {
        private ArrayList myAL = new ArrayList();       // array of TUser
        private int Position = -1;

        // конструктор
        internal TUsers()                      // !!! not creatable !!!
        {
        }
        public TUsers(XmlNode nodeMain)     
        {
            // дочерние узлы - переносим в массив объектов
            foreach (XmlNode nodeChild in nodeMain.ChildNodes)
            {
                if (nodeChild.Name.ToUpper() == "USER")
                {
                    TUser user = new TUser(nodeChild);
                    this.myAL.Add(user);
                }
            }
        }

        // свойства
        public int Count { get { return myAL.Count; } }

        // методы
        public TUser Item(int i)
        {
            return (TUser)myAL[i];
        }

        //public TUser Enum()
        //{
        //    return null;
        //}

        public void Add(TUser inObj)
        {
            // append
            myAL.Add(inObj);
        }

        public void Set(TUser User)
        {
            // ищем старые - заменяем или добавляем, если старого нет
            // удаляем старый
            this.Drop(User.UserName);   // может и не быть для удаления (для нового)!
            // добавляем нового
            myAL.Add(User);
        }
        internal void Drop(int inIndex)                 // internal - только в классе или в той-же программе т(сборке)
        {
            // !!! physically remove from array !
            if (inIndex >= 0 && inIndex < myAL.Count)  // allways !
            {
                myAL.RemoveAt(inIndex);
            }
        }

        internal void Drop(string userName)                 // internal - только в классе или в той-же программе т(сборке)
        {
            // !!! physically remove from array !
            for (int i = myAL.Count - 1; i >= 0; i--) // удаляем с конца - ВЕСЬ СПИСОК !!!
            {
                if (((TUser)myAL[i]).UserName == userName)
                {
                    myAL.RemoveAt(i);
                }
            }
        }

        internal int IndexByUserName(string inUserName)             // internal - только в классе или в той-же программе т(сборке)
        {
            int nIndex = -1;    // defaul -1 (not found)
            for (int n = 0; n < myAL.Count; n++)
            {
                if (((TUser)myAL[n]).UserName == inUserName)
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


        public TUser FindUser(string inUserName)
        {
            int nIndex = -1;
            if (myAL.Count > 0)
            {
                for (int nInd = 0; nInd < myAL.Count; nInd++)
                {
                    if (((TUser)myAL[nInd]).UserName == inUserName)
                    {
                        nIndex = nInd;
                        break; //------------->
                    }
                }
            }
            if (nIndex >= 0)
                return (TUser)myAL[nIndex];

            else
                return null;
        }

        public void Clear()
        {
            myAL.Clear();
        }

        //------------------------------
        // Like() - проверяет только ПОИМЕННЫЙ состав списков !!!
        public bool Like(TUsers users)
        {
            bool bRet = true;
            // имя 
            if (bRet)
            {
                if (myAL.Count != users.Count)
                {
                    bRet = false;
                }
            }

            // 1. все члены оригинала должны содержаться в проверяемом
            if (bRet)
            {
                if (myAL.Count != users.Count)
                {
                    bRet = false;
                }
                else
                {
                    if (myAL.Count > 0)
                    {
                        for (int nInd = 0; nInd < myAL.Count; nInd++)
                        {
                            if (users.FindUser(((TUser)myAL[nInd]).UserName) == null)
                            {
                                bRet = false; // break не делать - плохо себя ведет нумератор класса !!!
                                break; //------------->
                            }
                        }
                    }
                }
            }
            // . все члены проверяемого должны содержаться в оригинале
            if (bRet)
            {
                foreach (TUser user in users)
                {
                    if (this.FindUser(user.UserName) == null)
                    {
                        bRet = false; // break не делать - плохо себя ведет нумератор класса !!!
                    }
                }
            }
            return bRet; //==============>
        }

        public TUsers Clone()
        {
            TUsers retUsers = new TUsers();
            foreach (TUser p in this.myAL)
            {
                retUsers.Add(p.Clone());
            }
            return retUsers; //==============>
        }

        public XElement ToXElement()
        {
            //XElement xe = new XElement("users");
            //foreach (TUser p in this.myAL)
            //{
            //    xe.Add(p.ToXElement());
            //}

            // сначала создаем отсортированный массив имен - в этом порядке будем формировать список результата
            List<string> aTmp = new List<string>();
            foreach (TUser p in this.myAL)
            {
                aTmp.Add(p.UserName);
            }
            // сорируем массив имен
            aTmp.Sort();
            // добавляем в результат по алфавиту
            XElement xe = new XElement("users");
            foreach (string s in aTmp)
            {
                xe.Add(this.FindUser(s).ToXElement());  // должен быть - сами массив сортировали!
            }

            return xe; //==============>
        }

        public override string ToString()
        {
            string s = "";
            foreach (TUser x in myAL)
            {
                s = s + (s == "" ? "" : Environment.NewLine) + x.ToString();
            }
            return s;
        }

    }
}
