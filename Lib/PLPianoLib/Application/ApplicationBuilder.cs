using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLPianoLib.Application
{
    public class ApplicationBuilder
    {
        public ApplicationBuilder() {
            _repositories = new Repositories();
        }
        /// <summary>
        /// Build all logical modules of the application
        /// </summary>
        public void Build()
        {
            _repositories.Build();
        }

        Repositories _repositories;
    }
}
