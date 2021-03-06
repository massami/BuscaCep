﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuscaCep.Models.Widenet
{
    public class Resultado
    {
        public int status { get; set; }
        public bool ok { get; set; }
        public string code { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string address { get; set; }
        public string statusText { get; set; }
    }
}