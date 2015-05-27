
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
    public class TParameters : IEnumerator //, IEnumerable 
    {
        private ArrayList myAL = new ArrayList();       // array of TParameter
        private int Position = -1;

        // контсруктор
        internal TParameters()                      // !!! not creatable !!!
        {
        }
        public TParameters(XmlNode nodeMain)     
        {
            // дочерние узлы - переносим в массив объектов
            foreach (XmlNode nodeChild in nodeMain.ChildNodes)
            {
                if (nodeChild.Name.ToUpper() == "PARAMETER")
                {
                    TParameter parameter = new TParameter(nodeChild);
                    this.myAL.Add(parameter);
                }
            }
        }

        // свойства
        public int Count { get { return myAL.Count; } }

        // методы
        public TParameter Item(int i)
        {
            return (TParameter)myAL[i];
        }

        //public TParameter Enum()
        //{
        //    return null;
        //}

        public void Add(TParameter inObj)
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

        internal int IndexByName(string inName)             // internal - только в классе или в той-же программе т(сборке)
        {
            int nIndex = -1;    // defaul -1 (not found)
            for (int n = 0; n < myAL.Count; n++)
            {
                if (((TParameter)myAL[n]).Name == inName)
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


        public TParameter FindParameter(string Name, string Value)
        {
            int nIndex = -1;
            if (myAL.Count > 0)
            {
                for (int nInd = 0; nInd < myAL.Count; nInd++)
                {
                    if (((TParameter)myAL[nInd]).Name == Name && ((TParameter)myAL[nInd]).Value == Value)
                    {
                        nIndex = nInd;
                        break; //------------->
                    }
                }
            }
            if (nIndex >= 0)
                return (TParameter)myAL[nIndex];
            else
                return null;
        }

        public void Clear()
        {
            myAL.Clear();
        }

        public XElement ToXElement()
        {
            XElement xe = new XElement("parameters");
            foreach (TParameter p in this.myAL)
            {
                xe.Add(p.ToXElement());
            }
            return xe; //==============>
        }

        public TParameters Clone()
        {
            TParameters retParameters = new TParameters();
            foreach (TParameter p in this.myAL)
            {
                retParameters.Add(p.Clone());
            }
            return retParameters; //==============>
        }

        public bool Like(TParameters pars)
        {
            bool bRet = true;
            // 1. все члены оригинала должны содержаться в проверяемом
            if (bRet)
            {
                if (myAL.Count != pars.Count)
                {
                    bRet = false;
                }
                else
                {
                    if (myAL.Count > 0)
                    {
                        for (int nInd = 0; nInd < myAL.Count; nInd++)
                        {
                            if (pars.FindParameter(((TParameter)myAL[nInd]).Name,
                                    ((TParameter)myAL[nInd]).Value) == null)
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
                foreach (TParameter par in pars)
                {
                    if (this.FindParameter(par.Name, par.Value) == null)
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
            foreach(TParameter x in myAL)
            {
                s = s + (s == "" ? "" : Environment.NewLine) + x.ToString();
            }
            return s;
        }

    }
}
