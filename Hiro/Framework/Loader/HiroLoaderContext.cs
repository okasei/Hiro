using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Hiro.Framework.Loader
{
    internal class HiroLoaderContext : AssemblyLoadContext
    {
        public HiroLoaderContext() : base(isCollectible: true) { }
    }
}
