using System;
using System.Collections.Generic;

namespace Parser
{
    public static class Parser
    {
        public static bool Parse2200Cutoff(List<string> inputList, ref Items2200 output)
        {
            if (inputList.Count < 2) return false;
            if (!inputList[1].StartsWith("cutoff:")) return false;

            var value = decimal.Parse(inputList[1].Split(':')[1]);
            if (inputList[0].EndsWith("-T")) output.CutoffT = value;
            if (inputList[0].EndsWith("-B")) output.CutoffB = value;
            return true;
        }

        public static bool Parse2200MFD(List<string> inputList, ref Items2200 output)
        {
            if (inputList.Count < 2) return false;
            for (int i = 1; i < inputList.Count; i++)
            {
                var array = inputList[i].Split(':');
                if (array.Length < 2 || !array[0].EndsWith("MFD")) continue;

                var key = int.Parse(array[0].Replace("MFD", ""));
                var value = decimal.Parse(array[1]);
                if (inputList[0].EndsWith("-T")) output.MFDsT[key] = value;
                if (inputList[0].EndsWith("-B")) output.MFDsB[key] = value;
            }

            return true;
        }

        public static bool Parse2200Attenuation(List<string> inputList, ref Items2200 output)
        {
            if (inputList.Count < 2) return false;
            for (int i = 1; i < inputList.Count; i++)
            {
                var array = inputList[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length < 2) continue;

                var key = int.Parse(array[0]);
                var value = decimal.Parse(array[1]);
                output.Attenuations[key] = value;
            }

            return true;
        }
    }
}