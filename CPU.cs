using System;

namespace pissboy23 {

    public class CPU : Component {

        public Clock clock;
        public Registers registers;

        public Func<int>[] opcodes8;

        public void Reset() {
            clock.m = clock.t = 0;
            registers.A = registers.B = registers.C = registers.D = registers.E = registers.H = registers.L = registers.F = 0;
            registers.PC = registers.SP = 0;
            registers.M = registers.T = 0;

            opcodes8 = new Func<int>[] { // returns machine cycles taken
            () => { // 0x00 NOP
                registers.PC++;
                return 1;
            },
            () => { // 0x01 LD BC, d16
                registers.B = GameBoy.memory.ReadByte(registers.PC);
                registers.PC++;
                registers.C = GameBoy.memory.ReadByte(registers.PC);
                registers.PC++;
                return 3;
            },
            () => { // 0x02 LD (BC), A
                ushort address = (ushort) ((registers.C << 8) + registers.B);
                GameBoy.memory.WriteByte(address, registers.A);
                return 2;
            },
            () => { // 0x03 INC BC
                registers.B++;
                registers.C++;
                return 2;
            },
            () => { // 0x04 INC B
                registers.B++;
                return 1;
            },
            () => { // 0x05 DEC B
                registers.B--;
                return 1;
            },
            () => { // 0x06 LD B, d8
                registers.B = GameBoy.memory.ReadByte(registers.PC);
                registers.PC++;
                return 2;
            },
            () => { // 0x07 RLCA
                registers.A = (byte) ((registers.A >> 7) | ((registers.A & 0x7f) << 1));
                return 1;
            }
        };
        }
    }

    public struct Clock {
        public int m, t;
    }

    public struct Registers {
        public byte A, B, C, D, E, H, L, F;
        public ushort PC, SP;
        public int M, T;
    }
}
