using System.Net.Sockets;

namespace Assignment_6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(Directory.GetCurrentDirectory());
            //Console.WriteLine(getPersonFromFile("omar", "initialInvestmentUSD.txt"));
            //Console.WriteLine(getPersonFromFile("omar", "clientBC.txt"));
            //buyBitCoin(getDollarPrice(getData()));
            //getCurrentValue(getDollarPrice(getData()));
            while (true)
            {
                Console.WriteLine("One BitCoin is currently worth $" + getDollarPrice(getData()) + "\n\r1.Buy Bitcoin\n\r2.See everyones current value in USD\n\r3.See one persons gain / loss\n\r4.Quit");

                try
                {
                    int choice = Convert.ToInt32(Console.ReadLine());
                    switch (choice)
                    {
                        default:
                            throw new FormatException();
                        case 1:
                            buyBitCoin(getDollarPrice(getData()));
                            break;
                        case 2:
                            getCurrentValue(getDollarPrice(getData()));
                            break;
                        case 3:
                            Console.WriteLine("Enter a name");
                            string person = Console.ReadLine().Trim().ToLower();
                            Console.WriteLine();
                            double ii = getPersonFromFile(person, "initialInvestmentUSD.txt");
                            double bc = getPersonFromFile(person, "clientBC.txt");
                            if (ii == -1 || bc == -1)
                            {
                                break;
                            }
                            double curval = bc * getDollarPrice(getData());
                            Console.WriteLine("Original Investment: " + ii);
                            Console.WriteLine("Number of Bitcoin: " + bc);
                            Console.WriteLine("Current value: " + curval);
                            Console.WriteLine("Change in value: " + (curval - ii));
                            break;
                        case 4:
                            System.Environment.Exit(0);
                            break;
                    }
                }
                catch (FormatException) { Console.WriteLine("Not a valid choice"); }
            }
        }
        public static double getPersonFromFile(string nquery, string fquery)
        {
            try
            {
                StreamReader sr = new(fquery);
                string line = "";
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] data = line.Split(':');
                    if ((data[0].ToLower().Trim()).Equals(nquery.ToLower()))
                    {
                        return (Convert.ToDouble(data[1]));
                    }
                }
                throw new PersonNotFound();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
            catch (PersonNotFound ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
        public class PersonNotFound : Exception
        {
            public override string Message
            {
                get
                {
                    return ("Person does not exist");
                }
            }
        }
        public static void getCurrentValue(double curval)
        {
            try
            {
                StreamReader sr = new("clientBC.txt");
                string line = "";
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] bc = line.Split(':');
                    bc[1] = ((Convert.ToDouble(bc[1])) * curval).ToString();
                    Console.WriteLine(bc[0] + ":" + bc[1]);

                }
            }
            catch (IOException ex) { Console.WriteLine(ex.Message); }
        }
        public static void buyBitCoin(float price)
        {
            StreamWriter? sw = null;
            try
            {
                StreamReader sr = new("initialInvestmentUSD.txt");
                sw = new StreamWriter("clientBC.txt");
                string line = "";
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] bc = line.Split(':');
                    bc[1] = ((Convert.ToDouble(bc[1])) / price).ToString();
                    sw.WriteLine(bc[0] + ":" + bc[1]);
                }
                sr.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (sw != null) sw.Close();
            }
        }
        public static List<string> getData()
        {
            List<string> list = new List<string>();
            try
            {

                TcpClient client = new TcpClient("api.coindesk.com", 80);
                NetworkStream stream = client.GetStream();
                StreamReader sr = new StreamReader(stream);
                StreamWriter sw = new StreamWriter(stream);
                sw.WriteLine("GET http://api.coindesk.com/v1/bpi/currentprice.json HTTP/1.0\n\n");
                sw.Flush();
                string dataline = "";
                while (!sr.EndOfStream)
                {
                    dataline = sr.ReadLine();
                    list.Add(dataline);
                    //Console.WriteLine(dataline);
                }
                sr.Close();
                sw.Close();
                stream.Close();
                client.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return (list);
        }
        public static float getDollarPrice(List<string> lines)
        {
            bool header = true;
            String json = "";
            foreach (string line in lines)
            {
                if (line.Equals(""))
                {
                    header = false;
                    continue;
                }
                if (header == false)
                {
                    json = line;
                    break;
                }
            }
            //Console.WriteLine("Json: "+json);
            String[] jsonParts = json.Split(":");
            String priceLine = jsonParts[19];
            String justPrice = priceLine.Replace("},\"GBP\"", "");
            float price = Convert.ToSingle(justPrice);
            return price;
        }
    }
}