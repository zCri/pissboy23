namespace pissboy23 {

    class GameBoy : Component {
        public static CPU CPU;
        public static Memory memory;

        public void Reset() {
            CPU = new CPU();
            memory = new Memory();
        }
    }
}
