using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Feedbook.Sync
{
    internal interface ISynchronizer
    {
        void Start();

        void Stop();
    }
}
