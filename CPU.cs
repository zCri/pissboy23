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
                    registers.BC = (ushort) GameBoy.memory.ReadWord(registers.PC);
                    registers.PC++;
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
                    ALU.INC(registers.B);
                    return 1;
                },
                () => { // 0x05 DEC B
                    ALU.DEC(registers.B);
                    return 1;
                },
                () => { // 0x06 LD B, d8
                    registers.B = GameBoy.memory.ReadByte(registers.PC);
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
                    GameBoy.memory.WriteWord((ushort) GameBoy.memory.ReadWord(registers.PC), (short) registers.SP);
                    registers.PC++;
                    return 5;
                },
                () => { // 0x09 ADD HL, BC
                    ALU.ADD(registers.BC);
                    return 2;
                },
                () => { // 0x0A LD A, (BC)
                    registers.A = GameBoy.memory.ReadByte(registers.BC);
                    return 2;
                },
                () => { // 0x0B DEC BC
                    registers.BC--;
                    return 2;
                },
                () => { // 0x0C INC C
                    ALU.INC(registers.C);
                    return 1;
                },
                () => { // 0x0D DEC C
                    ALU.DEC(registers.C);
                    return 1;
                },
                () => { // 0x0E LD C, d8
                    registers.C = GameBoy.memory.ReadByte(registers.PC);
                    return 2;
                },
                () => { // 0x0F RRCA
                    return 0; // TODO
                },
                () => { // 0x10 STOP
                    return 1; // TODO should anything be here?
                },
                () => { // 0x11 LD DE, d16
                    registers.DE = (ushort) GameBoy.memory.ReadWord(registers.PC);
                    registers.PC++;
                    return 3;
                },
                () => { // 0x12 LD (DE), A
                    GameBoy.memory.WriteByte(registers.DE, registers.A);
                    return 2;
                },
                () => { // 0x13 INC DE
                    registers.DE++;
                    return 2;
                },
                () => { // 0x14 INC D
                    ALU.INC(registers.D);
                    return 1;
                },
                () => { // 0x15 DEC D
                    ALU.DEC(registers.D);
                    return 1;
                },
                () => { // 0x16 LD D, d8
                    registers.D = GameBoy.memory.ReadByte(registers.PC);
                    return 2;
                },
                () => { // 0x17 RLA
                    return 0; // TODO
                },
                () => { // 0x18 JR s8
                    // TODO check if correct
                    registers.PC += (ushort) ((sbyte) GameBoy.memory.ReadByte(registers.PC) + 1);
                    return 3;
                },
                () => { // 0x19 ADD HL, DE
                    ALU.ADD(registers.DE);
                    return 2;
                },
                () => { // 0x1A LD A, (DE)
                    registers.A = GameBoy.memory.ReadByte(registers.DE);
                    return 2;
                },
                () => { // 0x1B DEC DE
                    registers.DE--;
                    return 2;
                },
                () => { // 0x1C INC E
                    ALU.INC(registers.E);
                    return 1;
                },
                () => { // 0x1D DEC E
                    ALU.DEC(registers.E);
                    return 1;
                },
                () => { // 0x1E LD E, d8
                    registers.E = GameBoy.memory.ReadByte(registers.PC);
                    return 2;
                },
                () => { // 0x1F RRA
                    return 0; // TODO
                },
                () => { // 0x20 JR NZ, s8
                    // TODO check if correct
                    if(flags.Z) {
                        return 2;
                    } else {
                        registers.PC += (ushort) ((sbyte) GameBoy.memory.ReadByte(registers.PC) + 1);
                        return 3;
                    }
                },
                () => { // 0x21 LD HL, d16
                    registers.HL = (ushort) GameBoy.memory.ReadWord(registers.PC);
                    registers.PC++;
                    return 3;
                },
                () => { // 0x22 LD (HL+), A
                    GameBoy.memory.WriteByte(registers.HL++, registers.A);
                    return 2;
                },
                () => { // 0x23 INC HL
                    registers.HL++;
                    return 2;
                },
                () => { // 0x24 INC H
                    ALU.INC(registers.H);
                    return 1;
                },
                () => { // 0x25 DEC H
                    ALU.DEC(registers.H);
                    return 1;
                },
                () => { // 0x26 LD H, d8
                    registers.H = GameBoy.memory.ReadByte(registers.PC);
                    return 2;
                },
                () => { // 0x27 DAA
                    return 0; // TODO fuck you
                },
                () => { // 0x28 JR Z, s8
                    // TODO check if correct
                    if(!flags.Z) {
                        return 2;
                    } else {
                        registers.PC += (ushort) ((sbyte) GameBoy.memory.ReadByte(registers.PC) + 1);
                        return 3;
                    }
                },
                () => { // 0x29 ADD HL, HL
                    ALU.ADD(registers.HL);
                    return 2;
                },
                () => { // 0x2A LD A, (HL+)
                    // TODO check
                    registers.A = GameBoy.memory.ReadByte(registers.HL++);
                    return 2;
                },
                () => { // 0x2B DEC HL
                    registers.HL--;
                    return 2;
                },
                () => { // 0x2C INC L
                    ALU.INC(registers.L);
                    return 1;
                },
                () => { // 0x2D DEC L
                    ALU.DEC(registers.L);
                    return 1;
                },
                () => { // 0x2E LD L, d8
                    registers.L = GameBoy.memory.ReadByte(registers.PC);
                    return 2;
                },
                () => { // 0x2F CPL
                    flags.N = true;
                    flags.H = true;

                    registers.A = (byte) ~registers.A;
                    return 1;
                },
                () => { // 0x30 JR NC, s8
                    // TODO check if correct
                    if(flags.C) {
                        return 2;
                    } else {
                        registers.PC += (ushort) ((sbyte) GameBoy.memory.ReadByte(registers.PC) + 1);
                        return 3;
                    }
                },
                () => { // 0x31 LD SP, d16
                    registers.SP = (ushort) GameBoy.memory.ReadWord(registers.PC);
                    registers.PC++;
                    return 3;
                },
                () => { // 0x32 LD (HL-), A
                    GameBoy.memory.WriteByte(registers.HL--, registers.A);
                    return 2;
                },
                () => { // 0x33 INC SP
                    registers.SP++;
                    return 2;
                },
                () => { // 0x34 INC (HL)
                    // TODO
                    return 3;
                },
                () => { // 0x35 DEC (HL)
                    // TODO
                    return 3;
                },
                () => { // 0x36 LD H, d8
                    GameBoy.memory.WriteByte(registers.HL, GameBoy.memory.ReadByte(registers.PC));
                    return 3;
                },
                () => { // 0x37 SCF
                    flags.N = false;
                    flags.H = false;
                    flags.C = true;

                    return 1;
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
