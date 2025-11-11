using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolucionProfesorA2._10
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    /*
      LogiTrack - Sistema de Envíos
      - Código compacto: reutilización, constructores en cadena, máxima utilización de LINQ
      - Propiedades automáticas cuando es posible, validación que convierte negativos a 0.0
      - Costo base: 2.0 € por kilogramo; la propiedad CostoBase devuelve el coste en euros (Peso * 2.0)
    */

    /* ===========================
       Clase base: Envio
       - Descripcion: propiedad automática
       - Peso: propiedad no automática con validación (no negativo)
       - CostoBase: propiedad de solo lectura que calcula Peso * 2.0 (euros)
       - CalcularCostoTotal: virtual (por defecto, devuelve CostoBase)
       - ToString: virtual para impresión de atributos comunes
       =========================== */
    abstract class Envio
    {
        // Descripción como propiedad automática pública.
        public string Descripcion { get; set; }

        // Campo privado para peso y propiedad con validación.
        double peso;
        public double Peso
        {
            get => peso;
            set => peso = value < 0.0 ? 0.0 : value; // si negativo -> 0.0
        }

        // Costo base en euros: 2.0 € por kilogramo; propiedad de solo lectura.
        // Devuelve el coste base absoluto (Peso * 2.0).
        public double CostoBase => Peso * 2.0;

        // Constructor que inicializa descripción y peso (la validación ocurre en el setter).
        protected Envio(string descripcion, double peso)
        {
            Descripcion = descripcion ?? string.Empty;
            Peso = peso;
        }

        // Método polimórfico: por defecto la tarifa total es el costo base.
        public virtual double CalcularCostoTotal() => CostoBase;

        // Representación textual básica; las subclases la extenderán.
        public override string ToString() =>
            $"{GetType().Name} | Descripción: {Descripcion} | Peso(kg): {Peso:0.00} | CostoBase: {CostoBase:0.00}€";
    }

    /* ===========================
       PaqueteEstandar
       - Hereda de Envio
       - TarifaPlana: propiedad no automática con validación (no negativo)
       - CostoTotal = CostoBase + TarifaPlana
       =========================== */
    class PaqueteEstandar : Envio
    {
        double tarifaPlana;
        public double TarifaPlana
        {
            get => tarifaPlana;
            set => tarifaPlana = value < 0.0 ? 0.0 : value;
        }

        // Constructor: reutiliza constructor base.
        public PaqueteEstandar(string descripcion, double peso, double tarifaPlana = 10.0)
            : base(descripcion, peso)
        {
            TarifaPlana = tarifaPlana;
        }

        // Reusar CostoBase y sumar la tarifa plana.
        public override double CalcularCostoTotal() => CostoBase + TarifaPlana;

        public override string ToString() =>
            $"{base.ToString()} | TarifaPlana: {TarifaPlana:0.00}€";
    }

    /* ===========================
       PaqueteExpress
       - Hereda de Envio
       - RecargoUrgencia: propiedad no automática con validación (porcentaje, p.ej. 0.10 = 10%)
       - CostoTotal = CostoBase + RecargoUrgencia * Peso
       =========================== */
    class PaqueteExpress : Envio
    {
        double recargoUrgencia;
        public double RecargoUrgencia
        {
            get => recargoUrgencia;
            set => recargoUrgencia = value < 0.0 ? 0.0 : value;
        }

        public PaqueteExpress(string descripcion, double peso, double recargoUrgencia)
            : base(descripcion, peso)
        {
            RecargoUrgencia = recargoUrgencia;
        }

        // Calculo: costo base + recargo por urgencia multiplicado por el peso.
        public override double CalcularCostoTotal() => CostoBase + (RecargoUrgencia * Peso);

        public override string ToString() =>
            $"{base.ToString()} | RecargoUrgencia: {RecargoUrgencia:0.00}€/kg";
    }

    /* ===========================
       Programa principal (consola)
       - Colección polimórfica List<Envio>
       - Menú: Crear Envío, Ver Costos, Calcular Ingreso Total, Salir
       - Uso de LINQ para listar y sumar
       =========================== */
    class Program
    {
        static void Main()
        {
            var envios = new List<Envio>();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("=== LogiTrack - Sistema de Envíos ===");
                Console.WriteLine("1) Crear Envío");
                Console.WriteLine("2) Ver Costos Individuales");
                Console.WriteLine("3) Calcular Ingreso Total");
                Console.WriteLine("4) Salir");
                Console.Write("Opción (1-4): ");
                var opt = Console.ReadLine()?.Trim();

                switch (opt)
                {
                    case "1":
                        CrearEnvio(envios);
                        break;
                    case "2":
                        VerCostos(envios);
                        break;
                    case "3":
                        CalcularIngresoTotal(envios);
                        break;
                    case "4":
                        Console.WriteLine("Saliendo. ¡Hasta pronto!");
                        return;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
        }

        static void CrearEnvio(List<Envio> envios)
        {
            Console.WriteLine();
            Console.WriteLine("Tipos de paquete:");
            Console.WriteLine("a) Paquete Estándar");
            Console.WriteLine("b) Paquete Express");
            Console.Write("Elige (a-b): ");
            var tipo = Console.ReadLine()?.Trim().ToLower();

            Console.Write("Descripción: ");
            var desc = Console.ReadLine() ?? string.Empty;

            var peso = LeerDouble("Peso en kg (ej: 5.25): ");

            if (tipo == "a")
            {
                // Tarifa plana por defecto 10.0€, pero permitimos personalizar.
                var tarifa = LeerDouble("Tarifa plana (por defecto 10.0): ");
                if (Math.Abs(tarifa) < 1e-9) tarifa = 10.0; // si el usuario puso 0 tras leer mal, mantenemos 10 por defecto
                envios.Add(new PaqueteEstandar(desc, peso, tarifa));
                Console.WriteLine("Paquete Estándar creado.");
            }
            else if (tipo == "b")
            {
                var recargo = LeerDouble("Recargo por urgencia (€/kg): ");
                envios.Add(new PaqueteExpress(desc, peso, recargo));
                Console.WriteLine("Paquete Express creado.");
            }
            else
            {
                Console.WriteLine("Tipo no reconocido. Operación cancelada.");
            }
        }

        static void VerCostos(List<Envio> envios)
        {
            Console.WriteLine();
            if (!envios.Any())
            {
                Console.WriteLine("No hay envíos registrados.");
                return;
            }

            // LINQ: construir líneas y mostrarlas
            envios
                .Select(e => $"{e} | CostoTotal: {e.CalcularCostoTotal():0.00}€")
                .ToList()
                .ForEach(line => Console.WriteLine(line));
        }

        static void CalcularIngresoTotal(List<Envio> envios)
        {
            Console.WriteLine();
            var total = envios.Sum(e => e.CalcularCostoTotal());
            Console.WriteLine($"Ingreso total esperado ({envios.Count} envío(s)): {total:0.00}€");
        }

        // LeerDouble: intenta parsear, si falla devuelve 0.0 (filosofía preventiva).
        static double LeerDouble(string prompt)
        {
            Console.Write(prompt);
            var raw = Console.ReadLine();
            if (double.TryParse(raw, out double v)) return v;
            Console.WriteLine("Entrada no numérica. Se asignará 0.0.");
            return 0.0;
        }
    }
}
