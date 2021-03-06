﻿//
//  GlobalSettings.cs
//
//  Author:
//       Christopher Stefancik <gutenvirt@gmail.com>
//
//  Copyright (c) 2015 2015 CD Stefancik
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;

namespace CSDataMiner2
{
    public static class GlobalSettings
    {
        public static MethodOfDelete DeleteOption { get; set; }

        public static bool ReplaceCR { get; set; }

        public static bool GenerateCSV { get; set; }

        public static bool GenerateHTML { get; set; }

        public static bool GenerateReferences { get; set; }

        public static bool GenerateZScores { get; set; }

        public static bool HasMC { get; set; }

        public static bool HasMS { get; set; }

        public static bool HasGR { get; set; }

        public static bool HasCR { get; set; }

        public static string FileFIlter { get; set; }

        public static bool GenZScores { get; set; }

    }

    public enum MethodOfDelete
    {
        Listwise,
        Pairwise,
        ZeroReplace
    }
}

