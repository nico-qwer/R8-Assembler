# R8 compiler
A compiler for the Reflex-8 instruction set I created for a Minecraft computer.

## Features:
- All 8 of the Reflex 8 instructions;
- Define byte (db) for adding a single byte;
- Recognises hexadecimal (0x), binary (0b), chars ('') and integers;
- Quick halt instruction that adds 0b11111000.

## Instruction Set:
- NOP: No operation. Does nothing.
- LDA: Load Ax. Loads the value of the specified address into Ax.
- STA: Store Ax. Stores the value of Ax at the specified address.
- LDI: Load Immediate. Loads the specified value into Ax.
- ADD: Addition. Stores the result of Ax + the value of the specified address into Ax.
- SUB: Subtraction. Stores the result of Ax - the value of the specified address into Ax.
- JMP: Jump. Jumps to the specified memory address. Note that this value will get incremented before fetching.
- JMZ: Jump if 0. If the result of the previous ADD or SUB instruction was 0, acts like JMP.  If it wasn’t 0, acts like NOP. 
