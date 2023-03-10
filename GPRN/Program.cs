namespace GPRN
{
    /// <summary>Pseudo-Random Generation Сlass with referenced implementation</summary>
    public class Random1
    {
        /// <value>numbers sequence</value>
        List<uint> numbers;
        /// <value>Coefficient a in linear congruential generator</value>
        uint a = 1664525;
        /// <value>Coefficient b in linear congruential generator</value>
        uint b = 1013904223;
        /// <value>Counter value</value>
        int count;

        /// <summary>Constructor</summary>
        /// <param name="seed">X0 = seed for generator</param>
        public Random1(uint? seed = null)
        {
            numbers = new List<uint>();
            count = 1;
            if (seed == null)
                numbers.Add(Convert.ToUInt32(Convert.ToUInt64(DateTime.Now.Ticks) % UInt32.MaxValue));
            else numbers.Add(seed.Value);
        }

        /// <summary>Fuction generates the number in [0; mod - 1]</summary>
        /// <param name="mod">Max possible number plus 1</param>
        /// <returns>Following pseudo-random number in [0; mod - 1]</returns>
        public uint Next(uint mod)
        {
            count++;
            uint data;
            if (count == 2)
                data = AXplusB(numbers[0]);

            else
                data = AXplusB(numbers[Convert.ToInt32(AXplusB(numbers[count - 2]) % (count - 1))] * Convert.ToUInt32(count));
            numbers.Add(data);
            return data % mod;
        }

        /// <summary>Linear operation</summary>
        /// <param name="data">Some number</param>
        /// <returns>y = a*data + b</returns>
        uint AXplusB(uint data)
        {
            return (data * a + b) % UInt32.MaxValue;
        }
    }
    /// <summary>Pseudo-Random Generation Сlass with optimized implementation (keeps only 2000 elements in memory)</summary>
    public class Random2
    {
        /// <value>Numbers sequence (last 2000)</value>
        List<uint> numbers;
        /// <value>Coefficient a in linear congruential generator</value>
        uint a = 1664525;
        /// <value>Coefficient b in linear congruential generator</value>
        uint b = 1013904223;
        /// <value>Counter value</value>
        int count;

        /// <summary>Constructor</summary>
        /// <param name="seed">X0 = seed for generator</param>
        public Random2(uint? seed = null)
        {
            numbers = new List<uint>();
            count = 1;
            if (seed == null)
                numbers.Add(Convert.ToUInt32(Convert.ToUInt64(DateTime.Now.Ticks) % UInt32.MaxValue));
            else numbers.Add(seed.Value);
        }

        /// <summary>Fuction generates the number in [0; mod - 1]</summary>
        /// <param name="mod">Max possible number plus 1</param>
        /// <returns>Following pseudo-random number in [0; mod - 1]</returns>
        public uint Next(uint mod)
        {
            uint data;
            if (count == 1)
            {
                data = AXplusB(numbers[0]);
                numbers.Add(data);
            }

            else if (count < 2000)
            {
                data = AXplusB(numbers[Convert.ToInt32(AXplusB(numbers[count - 1]) % count)] * Convert.ToUInt32(count));
                numbers.Add(data);
            }
            else
            {
                data = AXplusB(numbers[Convert.ToInt32(AXplusB(numbers[((count % 2000) - 1 + 2000) % 2000]) % 2000)] * Convert.ToUInt32(count));
                numbers[count % 2000] = data;
            }
            count++;
            return data % mod;
        }

        /// <summary>Linear operation</summary>
        /// <param name="data">Some number</param>
        /// <returns>y = a*data + b</returns>
        uint AXplusB(uint data)
        {
            return (data * a + b) % UInt32.MaxValue;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            uint N = 10000;
            int[] sizes = { 1000, 2000, 4000, 6000, 8000,
                            10000, 50000, 100000, 500000, 1000000 };
            foreach (int k in sizes)
            {
                using (StreamWriter sw = new StreamWriter($"RefImp-{k}.csv"))
                {
                    Random1 rand = new Random1();
                    List<uint> nums = new List<uint>();
                    DateTime start = DateTime.Now;
                    for (int i = 0; i < k; i++)
                    {
                        uint data = rand.Next(N);
                        nums.Add(data);
                        sw.WriteLine(data);
                    }
                    DateTime end = DateTime.Now;

                    Console.WriteLine($"RefImp, {k} elements, {(end - start).TotalMilliseconds} ms");
                }
                using (StreamWriter sw = new StreamWriter($"OptImp-{k}.csv"))
                {
                    Random2 rand = new Random2();
                    List<uint> nums = new List<uint>();
                    DateTime start = DateTime.Now;
                    for (int i = 0; i < k; i++)
                    {
                        uint data = rand.Next(N);
                        nums.Add(data);
                        sw.WriteLine(data);
                    }
                    DateTime end = DateTime.Now;

                    Console.WriteLine($"OptImp, {k} elements, {(end - start).TotalMilliseconds} ms");
                }
                using (StreamWriter sw = new StreamWriter($"ByNext-{k}.csv"))
                {
                    Random rand = new Random();
                    List<int> nums = new List<int>();
                    DateTime start = DateTime.Now;
                    for (int i = 0; i < k; i++)
                    {
                        int data = rand.Next((int)N);
                        nums.Add(data);
                        sw.WriteLine(data);
                    }
                    DateTime end = DateTime.Now;

                    Console.WriteLine($"ByNext, {k} elements, {(end - start).TotalMilliseconds} ms\n");
                }
            }
        }
    }
}