using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRiscuit.Rule
{
    public class ScoreCoefficient
    {
        public float NormWeight = 10f;
        public float RdirWeight1 = 10f;
        public float RdirWeight2 = 10f;
        public float AngleWeight = 100f;
        public float c4 = 1;
        public float EpsWeight1 = 400;
        public float EpsWeight2 = 15;
        public float NormLeveling = 1;
        public float RdirLeveling1 = 10000;
        public float RdirLeveling2 = 10000;
        public float AngleLeveling = 30;
        public float w4 = 1;

        public static ScoreCoefficient operator* (ScoreCoefficient a, ScoreCoefficient b)
        {
            return new ScoreCoefficient()
            {
                NormWeight = a.NormWeight * b.NormWeight,
                RdirWeight1 = a.RdirWeight1 * b.RdirWeight1,
                RdirWeight2 = a.RdirWeight2 * b.RdirWeight2,
                AngleWeight = a.AngleWeight * b.AngleWeight,
                c4 = a.c4 * b.c4,
                EpsWeight1 = a.EpsWeight1 * b.EpsWeight1,
                EpsWeight2 = a.EpsWeight2 * b.EpsWeight2,
                NormLeveling = a.NormLeveling * b.NormLeveling,
                RdirLeveling1 = a.RdirLeveling1 * b.RdirLeveling1,
                RdirLeveling2 = a.RdirLeveling2 * b.RdirLeveling2,
                AngleLeveling = a.AngleLeveling * b.AngleLeveling,
                w4 = a.w4 * b.w4
            };
        }

        public static ScoreCoefficient Identity = new ScoreCoefficient()
        {
            NormWeight = 1f,
            RdirWeight1 = 1f,
            RdirWeight2 = 1f,
            AngleWeight = 1f,
            c4 = 1,
            EpsWeight1 = 1,
            EpsWeight2 = 1,
            NormLeveling = 1,
            RdirLeveling1 = 1,
            RdirLeveling2 = 1,
            AngleLeveling = 1,
            w4 = 1
        };
    }

    
}
