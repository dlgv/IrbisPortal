
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
    //public class TActions : ITActions, IEnumerator //, IEnumerable 
    //{
    //} // class TActions

    public interface ITActions
    {
        int Count();
        TAction Item(int i);
        //TAction Enum();
        void Add(TAction inValue);
        IEnumerator GetEnumerator();    // \
        bool MoveNext();                //  |  для перебора элементов !
        void Reset();                   // /
        TAction FindAction(string inSymbol);
        void Clear();
    }

    public class TActions : ITActions, IEnumerator //, IEnumerable 
    {
        private ArrayList myAL = new ArrayList();       // array of TAction

        private int Position = -1;

        internal TActions()                      // !!! not creatable !!!
        {
        }
        public TActions(XmlNode nodeMain)
        {
            // дочерние узлы - переносим в массив объектов
            foreach (XmlNode nodeChild in nodeMain.ChildNodes)
            {
                if (nodeChild.Name.ToUpper() == "ACTION")
                {
                    TAction action = new TAction(nodeChild);
                    this.myAL.Add(action);
                }
            }
        }




        public int Count()
        {
            return myAL.Count;
        }

        public TAction Item(int i)
        {
            return (TAction)myAL[i];
        }

        //public TAction Enum()
        //{
        //    return null;
        //}

        public void Add(TAction inObj)
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

        internal int IndexByActionId(string inActionId)             // internal - только в классе или в той-же программе т(сборке)
        {
            int nIndex = -1;    // defaul -1 (not found)
            for (int n = 0; n < myAL.Count; n++)
            {
                if (((TAction)myAL[n]).ActionId == inActionId)
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


        public TAction FindAction(string inActionId)
        {
            int nIndex = -1;
            if (myAL.Count > 0)
            {
                for (int nInd = 0; nInd < myAL.Count; nInd++)
                {
                    if (((TAction)myAL[nInd]).ActionId == inActionId)
                    {
                        nIndex = nInd;
                        break; //------------->
                    }
                }
            }
            if (nIndex >= 0)
                return (TAction)myAL[nIndex];

            else
                return null;
        }

        public void Clear()
        {
            myAL.Clear();
        }

        public XElement ToXElement()
        {
            XElement xe = new XElement("actions");
            foreach (TAction p in this.myAL)
            {
                xe.Add(p.ToXElement());
            }
            return xe; //==============>
        }

        public TActions Clone()
        {
            TActions retActions = new TActions();
            foreach (TAction p in this.myAL)
            {
                retActions.Add(p.Clone());
            }
            return retActions; //==============>
        }

        public override string ToString()
        {
            string s = "";
            foreach (TAction x in myAL)
            {
                s = s + (s == "" ? "" : Environment.NewLine) + x.ToString();
            }
            return s;
        }

    }
}
