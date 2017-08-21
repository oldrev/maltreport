using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting.Tests.Common
{
    public class TestingDataSet
    {

        public SimpleObject SimpleObject { get; private set; } = new SimpleObject();

        public SimpleObject NullSimpleObject { get; private set; } = null;

        public Table Table1 { get; private set; } = new Table();

    }
}
