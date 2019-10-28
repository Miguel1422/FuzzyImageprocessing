using Microsoft.VisualStudio.TestTools.UnitTesting;
using FuzzyLogic.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FuzzyLogic.ImageProcessing.Tests
{
    [TestClass()]
    public class CorrectorTests
    {
        [TestMethod()]
        public void HsvToColorTest()
        {
            Color c = Color.FromArgb(17, 25, 33);
            Color c2 = Corrector.HsvToColor(c.GetHue(), c.GetSaturation(), c.GetBrightness());
            Assert.AreEqual(c.R, c2.R);
            Assert.AreEqual(c.G, c2.G);
            Assert.AreEqual(c.B, c2.B);
        }
    }
}