namespace pissboy23 {
    class Memory : Component {
        bool mapBios;

        public void reset() {
            mapBios = true;
        }
        //TODO: write byte and cpu and shit and aisdjiasdoj
        byte ReadByte(ushort addr) {
            switch(addr & 0xF000) {
                case 0x0000: {
                        if(mapBios) {
                            if(addr < 0x0100) {
                                return 0; //return bios
                            } else if(GameBoy.CPU.PC == 0x0100) {
                                mapBios = false;
                            }
                        }
                        return 0;
                    }

                case 0x1000:
                case 0x2000:
                case 0x3000:
                case 0x4000:
                case 0x5000:
                case 0x6000:
                case 0x7000: {
                        return 0; // return carcridfge rom
                    }

                case 0x8000:
                case 0x9000: {
                        return 0; // return gpu vram
                    }

                case 0xA000:
                case 0xB000: {
                        return 0; //return external ram
                    }

                case 0xC000:
                case 0xD000: {
                        return 0; //return actual ram
                    }

                case 0xE000: {
                        return 0; // return ram copy
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
                                    return 0; // return ram copy
                                }

                            case 0xE00: {
                                    if(addr < 0xFEA0) {
                                        return 0; //return oam
                                    } else {
                                        return 0; // mmio
                                    }
                                }

                            case 0xF00:
                                if(addr >= 0XFF80) {
                                    return 0; //zram
                                } else {
                                    return 0;
                                }
                        }
                        break;
                    }
            }
        }

        public ushort ReadWord(ushort addr) {
            ushort upper = ReadByte(addr);
            ushort lower = (ushort) (ReadByte((ushort) (addr + 1)) << 8);
            return (ushort)(upper + lower);
        }
    }
}
