using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjercicioRepasoA2._11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<VehiculoBase> flota = new List<VehiculoBase>();
            bool salir = false;

            while (!salir)
            {
                Console.WriteLine("\nMenú FleetManager S.A.");
                Console.WriteLine("1. Registrar Vehículo");
                Console.WriteLine("2. Ver Costos Operacionales");
                Console.WriteLine("3. Calcular Costo Total de Flota (100,000 km)");
                Console.WriteLine("4. Salir");
                Console.Write("Seleccione una opción: ");
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        Console.WriteLine("\nTipo de Vehículo:");
                        Console.WriteLine("1. Autobús");
                        Console.WriteLine("2. Camión");
                        Console.Write("Seleccione tipo: ");
                        string tipo = Console.ReadLine();

                        Console.Write("Matrícula: ");
                        string matricula = Console.ReadLine();

                        Console.Write("Consumo de combustible (L/100km, no negativo): ");
                        double consumo = double.TryParse(Console.ReadLine(), out double c) ? c : 0.0;

                        if (tipo == "1")
                        {
                            Console.Write("Capacidad máxima (no negativo): ");
                            double capacidad = double.TryParse(Console.ReadLine(), out double cap) ? cap : 0.0;
                            flota.Add(new Autobus(matricula, consumo, capacidad));
                        }
                        else if (tipo == "2")
                        {
                            Console.Write("Peaje anual (€): ");
                            double peaje = double.TryParse(Console.ReadLine(), out double p) ? p : 0.0;
                            flota.Add(new Camion(matricula, consumo, peaje));
                        }
                        else
                        {
                            Console.WriteLine("Opción de tipo inválida.");
                        }
                        break;

                    case "2":
                        Console.WriteLine("\nCostos operacionales de vehículos:");
                        foreach (var vehiculo in flota)
                        {
                            Console.WriteLine(vehiculo.ToString());
                            Console.WriteLine($"Costo Operacional por Km: {vehiculo.CalcularCostoPorKm():0.00} €");
                        }
                        break;

                    case "3":
                        double total = 0.0;
                        foreach (var vehiculo in flota)
                        {
                            total += vehiculo.CalcularCostoPorKm() * 100000.0;
                        }
                        Console.WriteLine($"\nCosto total de la flota (100,000 km por vehículo): {total:0.00} €");
                        break;

                    case "4":
                        salir = true;
                        break;

                    default:
                        Console.WriteLine("Opción inválida. Intente de nuevo.");
                        break;
                }
            }
            Console.WriteLine("Fin del programa.");
        }
    }

    public abstract class VehiculoBase
    {
        public string Matricula { get; set; }

        private double consumo;
        public double Consumo
        {
            get { return consumo; }
            set { consumo = value < 0 ? 0.0 : value; }
        }

        public double CostoOperacionalBase
        {
            get { return 0.15; }
        }

        public VehiculoBase(string matricula, double consumo)
        {
            Matricula = matricula;
            Consumo = consumo;
        }

        public abstract double CalcularCostoPorKm();

        public override string ToString()
        {
            return $"Matrícula: {Matricula}, Consumo: {Consumo:0.00} L/100km, Costo Operacional Base: {CostoOperacionalBase:0.00} €/L";
        }
    }

    public class Autobus : VehiculoBase
    {
        private double capacidadMaxima;
        public double CapacidadMaxima
        {
            get { return capacidadMaxima; }
            set { capacidadMaxima = value < 0 ? 0.0 : value; }
        }

        public double FactorDesgaste
        {
            get { return 1.2; }
        }

        public Autobus(string matricula, double consumo, double capacidadMaxima)
            : base(matricula, consumo)
        {
            CapacidadMaxima = capacidadMaxima;
        }

        public override double CalcularCostoPorKm()
        {
            return Consumo * CostoOperacionalBase * FactorDesgaste;
        }

        public override string ToString()
        {
            return base.ToString() + $", Capacidad Máxima: {CapacidadMaxima:0.00} personas, Factor Desgaste: {FactorDesgaste:0.00}";
        }
    }

    public class Camion : VehiculoBase
    {
        private double peajeAnual;
        public double PeajeAnual
        {
            get { return peajeAnual; }
            set { peajeAnual = value < 0 ? 0.0 : value; }
        }

        public Camion(string matricula, double consumo, double peajeAnual)
            : base(matricula, consumo)
        {
            PeajeAnual = peajeAnual;
        }

        public override double CalcularCostoPorKm()
        {
            return Consumo * CostoOperacionalBase * (PeajeAnual / 100000.0);
        }

        public override string ToString()
        {
            return base.ToString() + $", Peaje Anual: {PeajeAnual:0.00} €";
        }
    }
}
