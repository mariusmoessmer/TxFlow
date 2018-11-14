using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.Debug.Adapter.Other
{
    public class GenericEventArgs<T> : EventArgs
    {
        private readonly T _data;

        public GenericEventArgs(T data)
        {
            this._data = data;
        }

        public T Data => _data;
    }
}
