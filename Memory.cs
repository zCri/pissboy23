namespace pissboy23 {
    class Memory : Component {
        public bool mapBios;

        byte[] bios;
        byte[] rom;
        byte[] ram;
        byte[] eram;
        byte[] zram;

        public void Reset() {
            mapBios = true;
        }

        //TODO: write byte and cpu and shit and aisdjiasdoj
        public byte ReadByte(ushort addr) {
            switch(addr & 0xF000) {
                case 0x0000: {
                        if(mapBios) {
                            if(addr < 0x0100) {
                                return 0; //return bios
                            } else if(GameBoy.CPU.registers.PC == 0x0100) {
                                mapBios = false;
                            }
                        }
                        return 0;
                    }

                case 0x1000:
                case 0x2000:
                case 0x3000: {
                        return rom[addr];
                    }

                case 0x4000:
                case 0x5000:
                case 0x6000:
                case 0x7000: {
                        return 0; // return carcridfge rom otrher bank
                    }

                case 0x8000:
                case 0x9000: {
                        return 0; // return gpu vram
                    }

                case 0xA000:
                case 0xB000: {
                        return eram[addr & 0x1FFF]; //return external ram
                    }

                case 0xC000:
                case 0xD000:

                case 0xE000: {
                        return ram[addr & 0x1FFF];
                    }

                case 0xF000: {
                        switch(addr & 0x0F00) {
                            case 0x000:
                            case 0x100:
                            case 0x200:
                            case 0x300:
                            case 0x400:
                            case 0x500:
                            case 0x600:
                            case 0x700:
                            case 0x800:
                            case 0x900:
                            case 0xA00:
                            case 0xB00:
                            case 0xC00:
                            case 0xD00: {
                                    return ram[addr & 0x1FFF];
                                }

                            case 0xE00: {
                                    if(addr < 0xFEA0) {
                                        return 0; //return oam
                                    } else {
                                        return 0;
                                    }
                                }

                            case 0xF00:
                                if(addr < 0XFF80) {
                                    return 0; // mmio
                                } else {
                                    return zram[addr & 0x7F];
                                }
                            default: {
                                    return 0;
                                }
                        }
                    }
                default: {
                        return 0;
                    }
            }
        }

        public ushort ReadWord(ushort addr) {
            ushort upper = ReadByte(addr);
            ushort lower = (ushort) (ReadByte((ushort) (addr + 1)) << 8);
            return (ushort) (upper + lower);
        }

        public void WriteByte(ushort addr, byte val) {
            switch(addr & 0xF000) {
                // bios or rom (both read only)
                case 0x0000:
                case 0x1000:
                case 0x2000:
                case 0x3000:

                case 0x4000:
                case 0x5000:
                case 0x6000:
                case 0x7000:
                    break;

                case 0x8000:
                case 0x9000: {
                        break; // vram
                    }

                case 0xA000:
                case 0xB000: {
                        eram[addr & 0x1FFF] = val;
                        break;
                    }

                case 0xC000:
                case 0xD000:
                case 0xE000: {
                        ram[addr & 0x1FFF] = val;
                        break;
                    }

                case 0xF000: {
                        switch(addr & 0x0F00) {
                            case 0x000:
                            case 0x100:
                            case 0x200:
                            case 0x300:
                            case 0x400:
                            case 0x500:
                            case 0x600:
                            case 0x700:
                            case 0x800:
                            case 0x900:
                            case 0xA00:
                            case 0xB00:
                            case 0xC00:
                            case 0xD00: {
                                    ram[addr & 0x1FFF] = val;
                                    break;
                                }

                            case 0xE00: {
                                    break; //oam
                                }

                            case 0xF00: {
                                    if(addr > 0xFF7F) {
                                        zram[addr & 0x7F] = val;
                                    } else {
                                        switch(addr & 0xF0) {
                                            
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
