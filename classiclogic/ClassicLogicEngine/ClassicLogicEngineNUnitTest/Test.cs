﻿using NUnit.Framework;
using System;
using System.Text;
using ClassicLogic.Engine;
using ClassicLogic.Utils;

namespace ClassicLogicEngineNUnitTest
{
  [TestFixture()]
  public class Test
  {
    public static readonly string Level =
      "                  S         " +
      "    $             S         " +
      "#######H#######   S         " +
      "       H----------S    $    " +
      "       H    ##H   #######H##" +
      "       H    ##H          H  " +
      "     0 H    ##H       $0 H  " +
      "##H#####    ########H#######" +
      "  H                 H       " +
      "  H           0     H       " +
      "#########H##########H       " +
      "         H          H       " +
      "       $ H----------H   $   " +
      "    H######         #######H" +
      "    H         &  $         H" +
      "############################";

    [Test()]
    public void TestCase()
    {
      var engine = new Engine(AIVersion.V1, new LevelReader(Level), Mode.Classic);
      
    }
  }
}
