using System.Net.Sockets;

namespace Assignment_6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(Directory.GetCurrentDirectory());

            buyBitCoin(getDollarPrice(getData()));
            getCurrentValue(getDollarPrice(getData()));

        }

        public class PersonNotFound : Exception
        {
            public override string Message
            {
                get {
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
            catch(IOException ex) { Console.WriteLine(ex.Message); }
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
            finally {
                if(sw != null) sw.Close();
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