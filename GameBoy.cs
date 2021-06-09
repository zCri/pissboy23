using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pissboy23 {

    class GameBoy : Component {
        public static CPU CPU;
        public static Memory memory;

        public void reset() {
            CPU = new CPU();
            memory = new Memory();
        }
    }
}
