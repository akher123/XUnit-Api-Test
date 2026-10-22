using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTestProject
{
    public class DataSources
    {
        public static TheoryData<int, bool> OddValues =>
          new()
          {
            { 3, true },
            { 5, true },
            { 6, false }
          };
    }
}
