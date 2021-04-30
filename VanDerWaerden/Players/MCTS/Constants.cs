﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanDerWaerden
{
    public static class Constants
    {
        public const MoveSelection moveSelection = MoveSelection.MostVisited;
    }

    public enum MoveSelection
    {
        MostVisited,
        BestScore,
    }
}