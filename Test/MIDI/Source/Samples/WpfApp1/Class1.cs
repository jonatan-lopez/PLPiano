using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class SampleDataContext
    {
        public SampleDataContext() { Test1 = "PrimerTest";Test2 = "SgundoTest"; }
        private string test1;
        private string test2;

        public string Test1 { get => test1; set => test1 = value; }
        public string Test2 { get => test2; set => test2 = value; }
    }
}
