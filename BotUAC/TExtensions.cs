
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
    //public class TExtensions : ITExtensions, IEnumerator //, IEnumerable 
    //{
    //} // class TExtensions

    public interface ITExtensions
    {
        int Count();
        TExtension Item(int i);
        //TExtension Enum();
        void Add(TExtension inValue);
        IEnumerator GetEnumerator();    // \
        bool MoveNext();                //  |  для перебора элементов !
        void Reset();                   // /
        TExtension FindExtension(string inSymbol);
        void Clear();
    }

    public class TExtensions : ITExtensions, IEnumerator //, IEnumerable 
    {
        private ArrayList myAL = new ArrayList();       // array of TExtension

        private int Position = -1;

        internal TExtensions()                      // !!! not creatable !!!
        {
        }
        public TExtensions(XmlNode nodeMain)     
        {
            // дочерние узлы - переносим в массив объектов
            foreach (XmlNode nodeChild in nodeMain.ChildNodes)
            {
                if (nodeChild.Name.ToUpper() == "EXTENSION")
                { 
                    TExtension extension = new TExtension(nodeChild);
                    this.myAL.Add(extension);
                }
            }
        }




        public int Count()
        {
            return myAL.Count;
        }

        public TExtension Item(int i)
        {
            return (TExtension)myAL[i];
        }

        //public TExtension Enum()
        //{
        //    return null;
        //}

        public void Add(TExtension inObj)
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

        internal int IndexByExtensionId(string inExtensionId)             // internal - только в классе или в той-же программе т(сборке)
        {
            int nIndex = -1;    // defaul -1 (not found)
            for (int n = 0; n < myAL.Count; n++)
            {
                if (((TExtension)myAL[n]).ExtensionId == inExtensionId)
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


        public TExtension FindExtension(string ExtensionId)
        {
            int nIndex = -1;
            if (myAL.Count > 0)
            {
                for (int nInd = 0; nInd < myAL.Count; nInd++)
                {
                    if (((TExtension)myAL[nInd]).ExtensionId == ExtensionId)
                    {
                        nIndex = nInd;
                        break; //------------->
                    }
                }
            }
            if (nIndex >= 0)
                return (TExtension)myAL[nIndex];

            else
                return null;
        }

        public TExtension FindExtensionName(string ExtensionName)
        {
            int nIndex = -1;
            if (myAL.Count > 0)
            {
                for (int nInd = 0; nInd < myAL.Count; nInd++)
                {
                    if (((TExtension)myAL[nInd]).ExtensionName == ExtensionName)
                    {
                        nIndex = nInd;
                        break; //------------->
                    }
                }
            }
            if (nIndex >= 0)
                return (TExtension)myAL[nIndex];

            else
                return null;
        }

        public void Clear()
        {
            myAL.Clear();
        }

        public XElement ToXElement()
        {
            XElement xe = new XElement("extensions");
            foreach (TExtension p in this.myAL)
            {
                xe.Add(p.ToXElement());
            }
            return xe; //==============>
        }

        public TExtensions Clone()
        {
            TExtensions retExtensions = new TExtensions();
            foreach (TExtension p in this.myAL)
            {
                retExtensions.Add(p.Clone());
            }
            return retExtensions; //==============>
        }

        public override string ToString()
        {
            string s = "";
            foreach (TExtension x in myAL)
            {
                s = s + (s == "" ? "" : Environment.NewLine) + x.ToString();
            }
            return s;
        }

    }
}
