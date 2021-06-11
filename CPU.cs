using System;

namespace pissboy23 {

    public class CPU : Component {

        public Clock clock;
        public Registers registers;
        public Flags flags;

        public Func<int>[] opcodes8;

        public void Reset() {
            clock.m = clock.t = 0;
            registers.A = registers.B = registers.C = registers.D = registers.E = registers.H = registers.L = registers.F = 0;
            registers.PC = registers.SP = 0;
            registers.M = registers.T = 0;

            #region Opcodes

            opcodes8 = new Func<int>[] { // returns machine cycles taken
            () => { // 0x00 NOP
                return 1;
            },
            () => { // 0x01 LD BC, d16
                registers.BC = GameBoy.memory.ReadWord(registers.PC);
                registers.PC += 2;
                return 3;
            },
            () => { // 0x02 LD (BC), A
                GameBoy.memory.WriteByte(registers.BC, registers.A);
                return 2;
            },
            () => { // 0x03 INC BC
                registers.BC++;
                return 2;
            },
            () => { // 0x04 INC B
                INC(registers.B);
                return 1;
            },
            () => { // 0x05 DEC B
                DEC(registers.B);
                return 1;
            },
            () => { // 0x06 LD B, d8
                registers.B = GameBoy.memory.ReadByte(registers.PC);
                registers.PC++;
                return 2;
            },
            () => { // 0x07 RLCA
                flags.Z = false;
                flags.N = false;
                flags.H = false;
                flags.C = (registers.A & 0x80) != 0;
                registers.A = (byte) ((registers.A >> 7) | (registers.A << 1));
                return 1;
            },
            () => { // 0x08 LD (a16), SP
                GameBoy.memory.WriteWord(GameBoy.memory.ReadWord(registers.PC), registers.SP);
                registers.PC += 2;
                return 5;
            },
            () => { // 0x09 ADD
                ADD(registers.BC);
                return 2;
            }
        };
            #endregion
        }

        #region ALU

        void ADD(byte val) {
            int res = registers.A + val;
            SetFlagZ(res);
            flags.N = false;
            SetFlagHAdd(registers.A, val);
            SetFlagC(res);
            registers.A = (byte) res;
        }

        void ADD(ushort val) {
            int res = registers.HL + val;
            SetFlagZ(res);
            flags.N = false;
            SetFlagHAdd(registers.HL, val);
            flags.C = res >> 16 != 0;
            registers.HL = (ushort) res;
        }

        ushort ADDr8(byte val) {
            flags.Z = false;
            flags.N = false;
            SetFlagHAdd((byte) registers.SP, val);
            SetFlagC((byte) registers.SP + val);
            return (ushort) (registers.SP + (sbyte) val);
        }

        void ADC(byte val) {
            int res = registers.A + val + (flags.C ? 1 : 0);
            SetFlagZ(res);
            flags.N = false;
            if(flags.C) {
                SetFlagHAddCarry(registers.A, val);
            } else {
                SetFlagHAdd(registers.A, val);
            }
            SetFlagC(res);
            registers.A = (byte) res;
        }

        void SUB(byte val) {
            int res = registers.A - val;
            SetFlagZ(res);
            flags.N = true;
            SetFlagHSub(registers.A, val);
            SetFlagC(res);
            registers.A = (byte) res;
        }

        void SBC(byte val) {
            int res = registers.A - val - (flags.C ? 1 : 0);
            SetFlagZ(res);
            flags.N = true;
            if(flags.C) {
                SetFlagHSubCarry(registers.A, val);
            } else {
                SetFlagHSub(registers.A, val);
            }
            SetFlagC(res);
            registers.A = (byte) res;
        }

        void AND(byte val) {
            int res = val & registers.A;
            SetFlagZ(res);
            flags.N = false;
            flags.H = true;
            flags.C = false;
            registers.A = (byte) res;
        }

        void OR(byte val) {
            int res = val | registers.A;
            SetFlagZ(res);
            flags.N = false;
            flags.H = false;
            flags.C = false;
            registers.A = (byte) res;
        }

        void XOR(byte val) {
            int res = val ^ registers.A;
            SetFlagZ(res);
            flags.N = false;
            flags.H = false;
            flags.C = false;
            registers.A = (byte) res;
        }

        void CP(byte val) {
            int res = registers.A - val;
            SetFlagZ(res);
            flags.N = true;
            SetFlagHSub(registers.A, val);
            SetFlagC(res);
        }

        byte INC(byte val) {
            int res = val + 1;
            SetFlagZ(res);
            flags.N = false;
            SetFlagHAdd(val, 1);
            return (byte) res;
        }

        byte DEC(byte val) {
            int res = val - 1;
            SetFlagZ(res);
            flags.N = true;
            SetFlagHSub(val, 1);
            return (byte) res;
        }

        #endregion

        #region Flag utils

        void SetFlagZ(int val) {
            flags.Z = val == 0;
        }

        void SetFlagHAdd(byte val1, byte val2) {
            flags.H = ((val1 & 0x0F) + (val2 & 0x0F)) > 0x0F;
        }

        void SetFlagHAdd(ushort val1, ushort val2) {
            flags.H = ((val1 & 0x0FFF) + (val2 & 0x0FFF)) > 0x0FFF;
        }

        void SetFlagHAddCarry(ushort val1, ushort val2) {
            flags.H = ((val1 & 0x0F) + (val2 & 0x0F)) >= 0x0F;
        }

        void SetFlagHSub(byte val1, byte val2) {
            flags.H = (val1 & 0x0F) < (val2 & 0x0F);
        }

        void SetFlagHSubCarry(byte val1, byte val2) {
            flags.H = (val1 & 0x0F) < ((val2 & 0x0F) + (flags.C ? 1 : 0));
        }

        void SetFlagC(int val) {
            flags.C = (val >> 8) != 0;
        }

        #endregion

        public CPU() {
            Reset();
        }
    }

    public struct Clock {
        public ushort m, t;
    }

    public struct Registers {
        public byte A, B, C, D, E, H, L, F, M, T;
        public ushort PC, SP;

        public ushort AF {
            get {
                return (ushort) (A << 8 | F);
            }

            set {
                A = (byte) (value >> 8);
                F = (byte) (value & 0xF0);
            }
        }

        public ushort BC {
            get {
                return (ushort) (B << 8 | C);
            }

            set {
                B = (byte) (value >> 8);
                C = (byte) value;
            }
        }

        public ushort DE {
            get {
                return (ushort) (D << 8 | E);
            }

            set {
                D = (byte) (value >> 8);
                E = (byte) value;
            }
        }

        public ushort HL {
            get {
                return (ushort) (H << 8 | L);
            }

            set {
                H = (byte) (value >> 8);
                L = (byte) value;
            }
        }
    }

    public struct Flags {
        public bool Z {
            get {
                return (GameBoy.CPU.registers.F & 0x80) != 0;
            }

            set {
                GameBoy.CPU.registers.F = (byte) (value ? GameBoy.CPU.registers.F | 0x80 : GameBoy.CPU.registers.F & ~0x80);
            }
        }

        public bool N {
            get {
                return (GameBoy.CPU.registers.F & 0x40) != 0;
            }

            set {
                GameBoy.CPU.registers.F = (byte) (value ? GameBoy.CPU.registers.F | 0x40 : GameBoy.CPU.registers.F & ~0x40);
            }
        }

        public bool H {
            get {
                return (GameBoy.CPU.registers.F & 0x20) != 0;
            }

            set {
                GameBoy.CPU.registers.F = (byte) (value ? GameBoy.CPU.registers.F | 0x20 : GameBoy.CPU.registers.F & ~0x20);
            }
        }

        public bool C {
            get {
                return (GameBoy.CPU.registers.F & 0x10) != 0;
            }

            set {
                GameBoy.CPU.registers.F = (byte) (value ? GameBoy.CPU.registers.F | 0x10 : GameBoy.CPU.registers.F & ~0x10);
            }
        }
    }
}
