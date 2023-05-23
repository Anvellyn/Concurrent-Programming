namespace test_app;
using ClassLibrary;


public class UnitTest1
{
    [Fact]
    public void TestAdd()
    {
        double a = 5;
        double b = 10;
        Class1 klasa = new Class1();
        Assert.Equal(15, klasa.Add(a, b));
    }

    [Fact]
    public void TestSubtraction()
    {
        double a = 5;
        double b = 10;
        Class1 klasa = new Class1();
        Assert.Equal(-5, klasa.Subtraction(a, b));
    }

    [Fact]
    public void TestMultiplication()
    {
        double a = 5;
        double b = 10;
        Class1 klasa = new Class1();
        Assert.Equal(50, klasa.Multiplication(a, b));
    }

    [Fact]
    public void TestDivision()
    {
        double a = 10;
        double b = 5;
        Class1 klasa = new Class1();
        Assert.Equal(2, klasa.Division(a, b));
    }
}
