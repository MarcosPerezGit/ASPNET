using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class Figura
{
    // Requisito técnico de polimorfismo
    public abstract double CalcularArea();
    // Requisito técnico de polimorfismo
    public abstract double CalcularPerimetro();

    protected double _area;
    protected double _perimetro;
}

// Requisito técnico de herencia
class Circulo : Figura
{
    // Requisito funcional
    // Propiedad no automática
    private double _radio;
    // Requisito de calidad si valores negativos
    public double Radio { get => _radio; set => _radio = value <= 0 ? 1 : value; }

    // Requisito técnico de propiedades de sólo lectura.
    public double Area { get => Math.PI * Math.Pow(Radio, 2); }
    // Requisito técnico de propiedades de sólo lectura.
    public double Perimetro { get => 2 * Math.PI * Radio; }
    // Requisito técnico de polimorfismo
    public override double CalcularArea()
    {
        //return Area;
        _area = Area;
        return _area;
    }

    // Requisito técnico de polimorfismo
    public override double CalcularPerimetro()
    {
        return Perimetro;
    }
    // Requisito funcional ver colección
    public override string ToString()
    {
        return $"Circulo de Radio {Radio} con área {Area} y perímetro {Perimetro}";
    }
}

// Requisito técnico de herencia
class Rectangulo : Figura
{
    // Requisito funcional
    // Propiedad no automática
    private double _base;
    // Requisito de calidad si valores negativos
    public double Base { get => _base; set => _base = value <= 0 ? 1 : value; }

    // Requisito funcional
    // Propiedad no automática
    private double _altura;
    // Requisito de calidad si valores negativos
    public double Altura { get => _altura; set => _altura = value <= 0 ? 1 : value; }

    // Requisito técnico de propiedades de sólo lectura.
    public double Area { get => Base * Altura; }
    // Requisito técnico de propiedades de sólo lectura.
    public double Perimetro { get => 2 * (Base + Altura); }
    // Requisito técnico de polimorfismo
    public override double CalcularArea()
    {
        return Area;
    }

    // Requisito técnico de polimorfismo
    public override double CalcularPerimetro()
    {
        return Perimetro;
    }
    // Requisito funcional ver colección
    public override string ToString()
    {
        return $"Rectángulo de Base {Base} y Altura {Altura} con área {Area} y perímetro {Perimetro}";
    }

}

// Requisito técnico de herencia
class Rombo : Figura
{
    // Requisito funcional
    // Propiedad no automática
    private double _diagonalMayor;
    // Requisito de calidad si valores negativos
    public double DiagonalMayor { get => _diagonalMayor; set => _diagonalMayor = value <= 0 ? 1 : value; }
    // Requisito funcional
    // Propiedad no automática
    private double _diagonalMenor;
    // Requisito de calidad si valores negativos
    public double DiagonalMenor { get => _diagonalMenor; set => _diagonalMenor = value <= 0 ? 1 : value; }
    // Requisito técnico de propiedades de sólo lectura.
    public double Area { get => DiagonalMayor * DiagonalMenor / 2; }
    // Requisito técnico de propiedades de sólo lectura.
    public double Perimetro { get => 2 * Math.Sqrt(Math.Pow(DiagonalMayor, 2) * Math.Pow(DiagonalMenor, 2)); }

    public override double CalcularArea()
    {
        return Area;
    }

    // Requisito técnico de polimorfismo
    public override double CalcularPerimetro()
    {
        return Perimetro;
    }

    // Requisito funcional ver colección
    public override string ToString()
    {
        return $"Rombo de DiagonalMayor {DiagonalMayor} y DiagonalMenor {DiagonalMenor} con área {Area} y perímetro {Perimetro}";
    }
}

class Programa
{
    static List<Figura> figuras = new List<Figura>();
    static int LeerOpcion()
    {
        while (true)
        {
            Console.Write("Introduzca una opción entre 1 y 5; ");
            int opcion = 0;
            if (int.TryParse(Console.ReadLine(), out opcion))
            {
                if (opcion >= 1 && opcion <= 5)
                    return opcion;
            }
        }
    }
    //  Requisito funcional      
    static void CrearFigura()
    {
        Console.WriteLine("Elija Circulo, Rectangulo o Rombo");
        var figura = Console.ReadLine().ToLower();

        if (figura.Equals("circulo"))
        {
            Console.Write("Radio: ");
            double radio;
            if (double.TryParse(Console.ReadLine(), out radio))
            {
                Circulo circulo = new Circulo();
                circulo.Radio = radio;
                figuras.Add(circulo);
            }
            else
            {
                Console.WriteLine("Incorrecto. No se ha creado");
            }
        }
    }
    //  Requisito funcional      
    static void VerColeccion()
    {
        figuras.ForEach(f => Console.WriteLine(f));
    }
    //  Requisito funcional      
    static void CalcularAreaTotal()
    {
        Console.WriteLine($"Area total = {figuras.Sum(f => f.CalcularArea())}");
    }
    //  Requisito funcional      
    static void CalcularPerimetroTotal()
    {
        Console.WriteLine($"Area total = {figuras.Sum(f => f.CalcularPerimetro())}");
    }

    //  Requisito funcional      
    static void Menu()
    {
        Console.WriteLine("1.- Crear Figura");
        Console.WriteLine("2.- Ver colección");
        Console.WriteLine("3.- Calcular Área Total");
        Console.WriteLine("4.- Calcular Perímetro Total");
        Console.WriteLine("5.- Terminar");
    }

    public static void Main()
    {
        while (true)
        {
            Menu();
            int opcion = LeerOpcion();

            switch (opcion)
            {
                case 1: CrearFigura(); break;
                case 2: VerColeccion(); break;
                case 3: CalcularAreaTotal(); break;
                case 4: CalcularPerimetroTotal(); break;
                case 5: break;
            }

            if (5 == opcion) break;
        }


    }

}