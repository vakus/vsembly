namespace Vsembly.Test.Unit
{
    public class ProcessorInstructionTest
    {
        private Processor processor;

        [SetUp]
        public void Setup()
        {
            processor = new();
        }

        [Test]
        public void Nop_Increments_Ip()
        {
            processor.memory[0] = 0;
            processor.Step();
            Assert.That(processor.ip == 1);
            Assert.That(processor.registers.All(reg => reg == 0));
            Assert.True(processor.memory.All(mem => mem == 0));
        }

        [Test]
        [TestCase(0, 1)]
        [TestCase(1, 69)]
        [TestCase(2, 420)]
        [TestCase(3, 1337)]
        [TestCase(2, -255)]
        public void Mov_MovesValueToRegister(int register, int value)
        {
            processor.memory[0] = 1;
            processor.memory[1] = register;
            processor.memory[2] = value;

            processor.Step();

            Assert.That(processor.ip == 3, "Instruction pointer should be incremented by 3 to prevent execution of data");
            Assert.That(processor.registers[register] == value, $"The register {register} value has not been updated");
            Assert.That(processor.registers.Where((v, i) => i != register).All(reg => reg == 0), "Non referenced registers value should remain unchanged");
        }

        [Test]
        [TestCase(1, 2, 123)]
        [TestCase(3, 0, 987)]
        [TestCase(2, 2, 555)]
        public void Mov_MovesRegisterToRegister(int sourceRegister, int targetRegister, int value)
        {
            processor.memory[0] = 2;
            processor.memory[1] = targetRegister;
            processor.memory[2] = sourceRegister;
            processor.registers[sourceRegister] = value;

            processor.Step();

            Assert.That(processor.ip == 3, "Instruction pointer should be incremented by 3 to prevent execution of data");
            Assert.That(processor.registers[targetRegister] == value, $"The target register {targetRegister} value should be {value}");
            Assert.That(processor.registers[sourceRegister] == value, $"The source register {sourceRegister} value should be {value}");
            Assert.That(processor.registers.Where((v, i) => i != sourceRegister && i != targetRegister).All(reg => reg == 0), "Non referenced registers value should remain unchanged");
        }

        [Test]
        [TestCase(0, 420, 123)]
        [TestCase(1, 4, 456)]
        [TestCase(2, 159, 987)]
        public void Mov_MovesMemoryValueToRegister(int targetRegister, int memoryLocation, int value)
        {
            if (memoryLocation < 3)
            {
                Assert.Fail("Memory Location would overwrite executable command.");
            }

            processor.memory[0] = 3;
            processor.memory[1] = targetRegister;
            processor.memory[2] = memoryLocation;
            processor.memory[memoryLocation] = value;

            processor.Step();

            Assert.That(processor.ip == 3, "Instruction pointer should be incremented by 3 to prevent execution of data");
            Assert.That(processor.registers[targetRegister] == value, $"The target register {targetRegister} value should be {value}");
            Assert.That(processor.memory[memoryLocation] == value, $"The value in memory at location {memoryLocation} should be unchanged");
            Assert.That(processor.registers.Where((v, i) => i != targetRegister).All(reg => reg == 0), "Non referenced registers value should remain unchanged");
        }

        [Test]
        [TestCase(0, 1, 123, 456)]
        [TestCase(3, 3, 123, 456)]
        public void Mov_MovesMemoryValueFromRegisterToRegister(int targetRegister, int sourceRegister, int memoryLocation, int value)
        {
            if (memoryLocation < 3)
            {
                Assert.Fail("Memory Location would overwrite executable command.");
            }

            processor.memory[0] = 4;
            processor.memory[1] = targetRegister;
            processor.memory[2] = sourceRegister;
            processor.memory[memoryLocation] = value;
            processor.registers[sourceRegister] = memoryLocation;

            processor.Step();

            Assert.That(processor.ip == 3, "Instruction pointer should be incremented by 3 to prevent execution of data");
            Assert.That(processor.registers[targetRegister] == value, $"The target register {targetRegister} value should be {value}");
            if (targetRegister != sourceRegister)
                Assert.That(processor.registers[sourceRegister] == memoryLocation, $"The source register {sourceRegister} should be unchanged.");
            Assert.That(processor.memory[memoryLocation] == value, $"The value in memory at location {memoryLocation} should be unchanged.");
            Assert.That(processor.registers.Where((v, i) => i != sourceRegister && i != targetRegister).All(reg => reg == 0), "Non referenced registers value should remain unchanged");
        }

        [Test]
        [TestCase(0, 123, 456)]
        [TestCase(1, 2, 159)]
        public void Mov_MovesRegisterValueToMemory(int sourceRegister, int memoryLocation, int value)
        {
            processor.memory[0] = 5;
            processor.memory[1] = memoryLocation;
            processor.memory[2] = sourceRegister;

            processor.registers[sourceRegister] = value;

            processor.Step();

            Assert.That(processor.ip == 3, "Instruction pointer should be incremented by 3 to prevent execution of data");
            Assert.That(processor.memory[memoryLocation] == value, $"The value in memory at location {memoryLocation} should be {value}");
            Assert.That(processor.registers[sourceRegister] == value, $"The value in source register {sourceRegister} should be {value}");
            Assert.That(processor.registers.Where((v, i) => i != sourceRegister).All(reg => reg == 0), "Non referenced registers value should remain unchanged");
        }

        [Test]
        [TestCase(0,1,123,456)]
        [TestCase(1,2,456,789)]
        public void Mov_MovesRegisterValueToMemoryByRegister(int targetRegister, int sourceRegister, int memoryLocation, int value)
        {
            if(targetRegister == sourceRegister)
                Assert.Fail("Target and source registers can not be the same");

            processor.memory[0] = 6;
            processor.memory[1] = targetRegister;
            processor.memory[2] = sourceRegister;

            processor.registers[targetRegister] = memoryLocation;
            processor.registers[sourceRegister] = value;

            processor.Step();
            
            Assert.That(processor.ip == 3, "Instruction pointer should be incremented by 3 to prevent execution of data");
            Assert.That(processor.memory[memoryLocation] == value, $"The value in memory at location {memoryLocation} should be {value}");
            Assert.That(processor.registers[targetRegister] == memoryLocation, $"The value in {targetRegister} should be {memoryLocation}");
            Assert.That(processor.registers[sourceRegister] == value, $"The value in {sourceRegister} should be {value}");
        }
    }
}