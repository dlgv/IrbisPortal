
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BotUAC
{
    [Serializable()]
    public class TPermissionGridRow
    {
        // http://aspdotnetcodebook.blogspot.com/2008/07/using-arraylist-as-datasource-for.html
        private string _Id;
        private string _Extension;
        private bool _Allow;
        private bool _Deny;

        public TPermissionGridRow(string Id, string Extension, bool Allow, bool Deny)
        {
            _Id = Id;
            _Extension = Extension;
            _Allow = Allow;
            _Deny = Deny;
        }

        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public string Extension
        {
            get { return _Extension; }
            set { _Extension = value; }
        }

        public bool Allow
        {
            get { return _Allow; }
            set { _Allow = value; }
        }

        public bool Deny
        {
            get { return _Deny; }
            set { _Deny = value; }
        }

    }  // class TPermissionGridRow

}