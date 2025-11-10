using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjercicioRepasoA2._10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<EnvioBase> envios = new List<EnvioBase>();
            bool salir = false;

            while (!salir)
            {
                Console.WriteLine("\nMenú LogiTrack S.A.");
                Console.WriteLine("1. Crear Envío");
                Console.WriteLine("2. Ver Costos Individuales");
                Console.WriteLine("3. Calcular Ingreso Total");
                Console.WriteLine("4. Salir");
                Console.Write("Seleccione una opción: ");
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        Console.WriteLine("\nTipo de Paquete:");
                        Console.WriteLine("1. Estándar");
                        Console.WriteLine("2. Express");
                        Console.Write("Seleccione tipo: ");
                        string tipo = Console.ReadLine();

                        Console.Write("Descripción: ");
                        string descripcion = Console.ReadLine();

                        Console.Write("Peso en kg (no negativo): ");
                        double peso = double.TryParse(Console.ReadLine(), out double p) ? p : 0.0;

                        if (tipo == "1")
                        {
                            Console.Write("Tarifa plana (no negativa): ");
                            double tarifa = double.TryParse(Console.ReadLine(), out double t) ? t : 0.0;
                            envios.Add(new PaqueteEstandar(descripcion, peso, tarifa));
                        }
                        else if (tipo == "2")
                        {
                            Console.Write("Recargo urgencia (no negativo, en euros por kg): ");
                            double recargo = double.TryParse(Console.ReadLine(), out double r) ? r : 0.0;
                            envios.Add(new PaqueteExpress(descripcion, peso, recargo));
                        }
                        else
                        {
                            Console.WriteLine("Opción de tipo inválida.");
                        }
                        break;

                    case "2":
                        Console.WriteLine("\nCostos individuales de envíos:");
                        foreach (var envio in envios)
                        {
                            Console.WriteLine(envio.ToString());
                            Console.WriteLine($"Costo Total: {envio.CalcularCostoTotal():0.00} €");
                        }
                        break;

                    case "3":
                        double total = 0.0;
                        foreach (var envio in envios)
                        {
                            total += envio.CalcularCostoTotal();
                        }
                        Console.WriteLine($"\nIngreso total por envíos: {total:0.00} €");
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

    public abstract class EnvioBase
    {
        public string Descripcion { get; set; }

        private double pesoKg;
        public double PesoKg
        {
            get { return pesoKg; }
            set { pesoKg = value < 0 ? 0.0 : value; }
        }

        public double CostoBase
        {
            get { return 2.0 * PesoKg; }
        }

        public EnvioBase(string descripcion, double pesoKg)
        {
            Descripcion = descripcion;
            PesoKg = pesoKg;
        }

        public abstract double CalcularCostoTotal();

        public override string ToString()
        {
            return $"Descripción: {Descripcion}, Peso: {PesoKg:0.00} kg, Costo Base: {CostoBase:0.00} €";
        }
    }

    public class PaqueteEstandar : EnvioBase
    {
        private double tarifaPlana;
        public double TarifaPlana
        {
            get { return tarifaPlana; }
            set { tarifaPlana = value < 0 ? 0.0 : value; }
        }

        public PaqueteEstandar(string descripcion, double pesoKg, double tarifaPlana)
            : base(descripcion, pesoKg)
        {
            TarifaPlana = tarifaPlana;
        }

        public override double CalcularCostoTotal()
        {
            return CostoBase + TarifaPlana;
        }

        public override string ToString()
        {
            return base.ToString() + $", Tarifa Plana: {TarifaPlana:0.00} €";
        }
    }

    public class PaqueteExpress : EnvioBase
    {
        private double recargoUrgencia;
        public double RecargoUrgencia
        {
            get { return recargoUrgencia; }
            set { recargoUrgencia = value < 0 ? 0.0 : value; }
        }

        public PaqueteExpress(string descripcion, double pesoKg, double recargoUrgencia)
            : base(descripcion, pesoKg)
        {
            RecargoUrgencia = recargoUrgencia;
        }

        public override double CalcularCostoTotal()
        {
            return CostoBase + (RecargoUrgencia * PesoKg);
        }

        public override string ToString()
        {
            return base.ToString() + $", Recargo Urgencia: {RecargoUrgencia:0.00} €/kg";
        }
    }
}
