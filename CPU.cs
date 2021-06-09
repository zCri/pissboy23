using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pissboy23 {
    class CPU : Component {
        public byte A, B, C, D, E, H, L, F;
        public ushort PC, SP;

        public void reset() {
        }
    }
}
