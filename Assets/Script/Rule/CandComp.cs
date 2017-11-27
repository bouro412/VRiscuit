using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRiscuit.Rule {
    class CandComp : IComparer<Candidate> {
        int IComparer<Candidate>.Compare(Candidate x, Candidate y) {
            return x.Score.CompareTo(y.Score);
        }
    }
}
