namespace ClassLibrary {
    public class Class1 {

        public static void Main() {

        Console.WriteLine("Podaj dwie cyfry.");
        double a = double.Parse(Console.ReadLine());
        double b = double.Parse(Console.ReadLine());
        Class1 klasa = new Class1();
        double result0 = klasa.Add(a, b);
        double result1 = klasa.Subtraction(a, b);
        double result2 = klasa.Multiplication(a, b);
        double result3 = klasa.Division(a, b);
        Console.WriteLine("Dodawanie:");
        Console.WriteLine(result0);
        Console.WriteLine("Odejmowanie:");
        Console.WriteLine(result1);
        Console.WriteLine("Mnożenie:");
        Console.WriteLine(result2);
        Console.WriteLine("Dzielenie:");
        Console.WriteLine(result3);

        }
        public double Add(double a, double b) {
            return a + b;
        }
        public double Subtraction(double a, double b) {
            return a - b;
        }
        public double Multiplication(double a, double b) {
            return a * b;
        }
        public double Division(double a, double b) {
            return a / b;
        }

    }
 }
