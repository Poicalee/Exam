using System;
using System.Data;
using System.Net.Http.Headers;
using System;
using System.Numerics;
using static KOLOS.Zadanie;
namespace KOLOS
{
    class Zadanie
    {
        private const string sciezkaTXT = "history.txt";
        private static StreamWriter historyWriter;
        public static void Main(string[] args)
        {
            historyWriter = new StreamWriter(sciezkaTXT, true);
            Produkt produkt1 = new Produkt("pepsi", 3, new DateOnly(5110, 05, 01));
            Produkt produkt = new Produkt("czekolad", 32, new DateOnly(5110,01,01));
            MiniMunchMachine machine = new MiniMunchMachine();
            //Test działania ToString
            Console.WriteLine(produkt.ToString());
            machine.AddProdukt(produkt1);
            machine.AddProdukt(produkt,'F');
            for (int i = 0; i < 18; i++)
            {
                machine.AddProdukt(new Produkt("jaranie", 2, new DateOnly(5110, 01, 01)));
            }
            machine.showProducts();
            while (true) 
            {
                Console.WriteLine("A - pokaz produkt\nB - wrzuc monete\nC - kup produkt\nD - pokaz ilosc wrzuconych monet\nE - wyciagnij monety\nF - wyjscie");
                Console.WriteLine("Wybierz opcje");
                string choice = Console.ReadLine();
                historyWriter.WriteLine($"Wybrano opcję: {choice}");
                switch (choice)
                {
                    case "A":
                        machine.showProducts();
                        break;
                    case "B":
                        Console.WriteLine("Wrzuc monety");
                        int moneta = 0;
                        try
                        {
                            moneta = Int32.Parse(Console.ReadLine());
                            machine.AddMoney(moneta);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Zła moneta");
                        }
                        break;
                    case "C":
                        Console.WriteLine("Podaj kod produktu ktory chcesz kupic");
                        try
                        {
                            machine.BuyProdukt(Char.Parse(Console.ReadLine()));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Bledny kod");
                        }
                        break;
                    case "D":
                        Console.WriteLine("Aktualna ilosc wrzuconych pieniedzy: " + machine.kasetkaMaszyny);
                        break;
                    case "E":
                        Console.WriteLine("Monety wyciagniete");
                        machine.ReturnCoins();
                        break;
                    case "F":
                        historyWriter.Close();
                        Console.WriteLine("Bye bye");
                        Environment.Exit(0);
                        break;
                    case "ACAB":
                        Console.WriteLine("Menu dodania produktu");
                        try
                        {
                            Console.WriteLine("Podaj nazwe produktu");
                            String nazwaProduktu = Console.ReadLine();
                            Console.WriteLine("Podaj cene produktu");
                            int cenaProduktu = Int32.Parse(Console.ReadLine());
                            Console.WriteLine("Podaj date przydatnosci produktu");
                            Console.WriteLine("Rok");
                            int rok = Int32.Parse(Console.ReadLine());
                            Console.WriteLine("Miesiac");
                            int miesiac = Int32.Parse(Console.ReadLine());
                            Console.WriteLine("Dzien");
                            int dzien = Int32.Parse(Console.ReadLine());
                            Console.WriteLine("Podaj kod produktu");
                            char kodproduktu = Char.Parse(Console.ReadLine());
                            machine.AddProdukt(new Produkt(nazwaProduktu, cenaProduktu, new DateOnly(rok, miesiac, dzien)), kodproduktu);
                            Console.WriteLine("Dodano produkt");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Bledne dane");
                        }
                        break;
                    case "HISTORY":
                        Console.WriteLine("Historia operacji:");
                        historyWriter.Close();
                        DisplayHistory();
                        historyWriter = new StreamWriter(sciezkaTXT, true);
                        break;
                    default:
                        Console.WriteLine("Nic nie wybrales");
                        break;
                }
            }
        }
        private static void DisplayHistory()
        {
            try
            {
                using (StreamReader sr = new StreamReader(sciezkaTXT))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Błąd podczas odczytu historii: " + e.Message);
            }
        }
    }


        public class Produkt
        {
            private String _Nazwa;
            public string Nazwa
            {
                get
                {
                    return _Nazwa;
                }
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        throw new ArgumentNullException("value");
                    }
                    else
                    {
                        _Nazwa = value;
                    }
                }
            }

            private int _Cena;
            public int Cena
            {
                get
                {
                    return _Cena;
                }
                set {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException("cena nie moze byc 0");
                    }
                    else
                    {
                        _Cena = value;
                    }
                }
            }
            private DateOnly _dataważności;
            public DateOnly dataważności
            {
                get
                {
                    return _dataważności;
                }
                set
                {
                    if (value < DateOnly.MinValue)
                    {
                        throw new ArgumentOutOfRangeException("zła data");
                    }
                    else
                    {
                        _dataważności = value;
                    }
                }
            }
          
           public Produkt(String nazwa,int cena,DateOnly datawazności)
            {
                Nazwa = nazwa;
                Cena =  cena;
                dataważności = datawazności;
            }
            public override string ToString()
            {
                return $"Nazwa: {Nazwa}, Cena: {Cena}, Data ważności: {dataważności.ToString("yyyy-MM-dd")}";
            }


        }
        
    
    interface IVendingMachine
    {
        public void AddProdukt(Produkt produkt);
        public void AddMoney(int kwota);
        public void BuyProdukt(char kodProduktu);
        public void ReturnCoins();
     }
    abstract class IVendingMachineAbstract 
    {
       abstract public void AddProdukt(Produkt produkt);
       abstract public void AddMoney(int kwota);
       abstract public void BuyProdukt(char kodProduktu);
       abstract public void ReturnCoins();
    }
    class MiniMunchMachine : IVendingMachine
    {
        private const int pojemnosc = 20;
        public int kasetkaMaszyny = 0;
        String nazwaMaszyny;
        String wymiaryMaszyny;
        private Dictionary<char, Produkt> produkty = new Dictionary<char, Produkt>();
        public void AddMoney(int kwota)
        {
            if (kwota == 1 || kwota == 2 || kwota == 5)
            {
                kasetkaMaszyny += kwota;
            }
            else
            {
                throw new ArgumentException("zły nominał");
            }
        }
        public MiniMunchMachine()
        {
            nazwaMaszyny = "MiniMunchMachine";
            wymiaryMaszyny = "szer. 74 cm, gł. 87 cm, wys. 94 cm";
        }

        public void AddProdukt(Produkt produkt)
        {
            if (produkty.Count >= pojemnosc)
            {
                throw new ArgumentException("maszyna jest pełna");
            }
            char kodproduktu = 'A';
            while (produkty.ContainsKey(kodproduktu))
            {
                kodproduktu++;
                if (kodproduktu > 'Z')
                {
                    kodproduktu = 'A';
                }
            }
            produkty.Add(kodproduktu, produkt);

        }
        public void AddProdukt(Produkt produkt, char kodProduktu)
        {
            if (produkty.Count >= pojemnosc)
            {
                throw new ArgumentException("Maszyna jest pełna!");
            }

            if (!char.IsLetter(kodProduktu) || produkty.ContainsKey(kodProduktu))
            {
                throw new ArgumentException("Nieprawidłowy kod produktu");
            }
            produkty.Add(kodProduktu, produkt);
        }
        public void BuyProdukt(char kodProduktu)
        {
            if (produkty.TryGetValue(kodProduktu, out Produkt product))
            {
                if (kasetkaMaszyny >= product.Cena)
                {
                    Console.WriteLine($"Produkt zakupiony: {product.Nazwa}");
                    kasetkaMaszyny -= product.Cena;
                    produkty.Remove(kodProduktu);
                }
                else
                {
                    Console.WriteLine("Niewystarczająca ilość pieniędzy w kasetce maszyny.");
                }
            }
            else
            {
                Console.WriteLine("Produkt o podanym kodzie nie istnieje.");
            }
        }

        public void ReturnCoins()
        {
            Console.WriteLine($"Zwrócono {kasetkaMaszyny} monet.");
            kasetkaMaszyny = 0;
        }
        public void showProducts()
        {
            foreach (var kvp in produkty)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value.Nazwa} {kvp.Value.Cena} EUR");
            }
        }
    }
}
