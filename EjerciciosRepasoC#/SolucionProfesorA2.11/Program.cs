using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 FleetManager - Sistema de Costes Operacionales
 - Minimizar código repetido, maximizar LINQ y propiedades automáticas.
 - Validación: cualquier valor numérico crítico negativo se convierte a 0.0.
 - Jerarquía:
     Vehiculo (abstract)
      ├─ TransportePasajeros (abstract) -> Capacidad
      │    └─ Autobus
      └─ TransporteCarga (abstract) -> PeajeAnual
           └─ Camion
 - Interpretaciones concretas:
     * CostoOperacionalBase = 0.15 € por litro (readonly).
     * Consumo expresado en L/100km.
     * Coste combustible por km = (ConsumoLPor100km / 100) * CostoOperacionalBase
     * Autobus: Coste/km = coste_combustible_por_km * FactorDesgaste (1.2)
     * Camion: Coste/km = coste_combustible_por_km + (PeajeAnual / 100000.0)
     * Cálculo total de flota asume 100000.0 km por vehículo cuando se solicita.
*/

/* ===========================
   Clase base: Vehiculo
   - Matricula: propiedad automática
   - ConsumoLPor100km: propiedad con validación
   - CostoOperacionalBase: readonly (0.15 €/L)
   - CalcularCostoPorKm: virtual (comportamiento por defecto: coste combustible)
   - ToString: virtual
   =========================== */
abstract class Vehiculo
{
    // Matrícula: automática, pública
    public string Matricula { get; set; }

    // Campo privado para consumo; setter valida no-negativo.
    double consumoLPor100km;
    public double ConsumoLPor100km
    {
        get => consumoLPor100km;
        set => consumoLPor100km = value < 0.0 ? 0.0 : value;
    }

    // Costo operativo por litro (readonly).
    public double CostoOperacionalBase => 0.15;

    // Constructor protegido: usado por las subclases.
    protected Vehiculo(string matricula, double consumoLPor100km)
    {
        Matricula = matricula ?? string.Empty;
        ConsumoLPor100km = consumoLPor100km;
    }

    // Cálculo base: consumo L/100km -> litros/km = Consumo/100
    // coste combustible por km = litros_por_km * CostoOperacionalBase
    public virtual double CalcularCostoPorKm() =>
        (ConsumoLPor100km / 100.0) * CostoOperacionalBase;

    public override string ToString() =>
        $"{GetType().Name} | Matrícula: {Matricula} | Consumo(L/100km): {ConsumoLPor100km:0.00}";
}

/* ===========================
   TransportePasajeros (intermedia)
   - Añade Capacidad (nº pasajeros) validada
   - Permite compartir atributos entre vehículos de pasajeros
   =========================== */
abstract class TransportePasajeros : Vehiculo
{
    double capacidad;
    public double Capacidad
    {
        get => capacidad;
        set => capacidad = value < 0.0 ? 0.0 : value;
    }

    protected TransportePasajeros(string matricula, double consumoLPor100km, double capacidad)
        : base(matricula, consumoLPor100km)
    {
        Capacidad = capacidad;
    }

    public override string ToString() => $"{base.ToString()} | Capacidad: {Capacidad:0.##}";
}

/* ===========================
   TransporteCarga (intermedia)
   - Añade PeajeAnual validado y propiedad de solo lectura PeajePorKm
   - Permite compartir atributos entre vehículos de carga
   =========================== */
abstract class TransporteCarga : Vehiculo
{
    double peajeAnual;
    public double PeajeAnual
    {
        get => peajeAnual;
        set => peajeAnual = value < 0.0 ? 0.0 : value;
    }

    protected TransporteCarga(string matricula, double consumoLPor100km, double peajeAnual)
        : base(matricula, consumoLPor100km)
    {
        PeajeAnual = peajeAnual;
    }

    // Peaje prorrateado por km usando distancia de referencia 100000 km
    public double PeajePorKm => PeajeAnual / 100000.0;

    public override string ToString() => $"{base.ToString()} | PeajeAnual: {PeajeAnual:0.00}€";
}

/* ===========================
   Autobus
   - FactorDesgaste fijo 1.2
   - CostePorKm = coste_combustible_por_km * FactorDesgaste
   =========================== */
class Autobus : TransportePasajeros
{
    // Factor de desgaste (readonly)
    public double FactorDesgaste => 1.2;

    public Autobus(string matricula, double consumoLPor100km, double capacidad)
        : base(matricula, consumoLPor100km, capacidad) { }

    // Reutiliza el cálculo base y lo multiplica por el factor
    public override double CalcularCostoPorKm() =>
        base.CalcularCostoPorKm() * FactorDesgaste;

    public override string ToString() =>
        $"{base.ToString()} | FactorDesgaste: {FactorDesgaste:0.00} | CostePorKm: {CalcularCostoPorKm():0.000000}€";
}

/* ===========================
   Camion
   - PeajeAnual (heredado); Coste/km = coste_combustible_por_km + PeajePorKm
   =========================== */
class Camion : TransporteCarga
{
    public Camion(string matricula, double consumoLPor100km, double peajeAnual)
        : base(matricula, consumoLPor100km, peajeAnual) { }

    public override double CalcularCostoPorKm() =>
        base.CalcularCostoPorKm() + PeajePorKm;

    public override string ToString() =>
        $"{base.ToString()} | Peaje/km: {PeajePorKm:0.000000} | CostePorKm: {CalcularCostoPorKm():0.000000}€";
}

/* ===========================
   Programa principal (consola)
   - Lista polimórfica List<Vehiculo>
   - Menú:
       1) Registrar Vehículo (Autobús / Camión)
       2) Ver Costos Operacionales (por km)
       3) Calcular Costo Total de Flota (asumiendo 100000.0 km/vehículo)
       4) Salir
   - Uso de LINQ para listar y sumar
   =========================== */
class Program
{
    static void Main()
    {
        var flota = new List<Vehiculo>();
        const double DistanciaReferencia = 100000.0; // km por vehículo para el cálculo agregado

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== FleetManager - Costes Operacionales ===");
            Console.WriteLine("1) Registrar Vehículo");
            Console.WriteLine("2) Ver Costos Operacionales (por km)");
            Console.WriteLine("3) Calcular Costo Total de Flota (100000 km / vehículo)");
            Console.WriteLine("4) Salir");
            Console.Write("Elige (1-4): ");
            var opcion = Console.ReadLine()?.Trim();

            switch (opcion)
            {
                case "1": RegistrarVehiculo(flota); break;
                case "2": VerCostos(flota); break;
                case "3":
                    {
                        var total = flota.Sum(v => v.CalcularCostoPorKm() * DistanciaReferencia);
                        Console.WriteLine($"Costo total estimado para la flota ({flota.Count} vehículo(s) * {DistanciaReferencia:0.##} km): {total:0.00}€");
                        break;
                    }
                case "4": Console.WriteLine("Saliendo. ¡Hasta pronto!"); return;
                default: Console.WriteLine("Opción no válida."); break;
            }
        }
    }

    // Registrar vehículo: selecciona tipo, lee datos y añade a la lista.
    static void RegistrarVehiculo(List<Vehiculo> flota)
    {
        Console.WriteLine();
        Console.WriteLine("Tipos:");
        Console.WriteLine("a) Autobús (transporte de pasajeros)");
        Console.WriteLine("b) Camión (transporte de carga)");
        Console.Write("Elige (a-b): ");
        var tipo = Console.ReadLine()?.Trim().ToLower();

        Console.Write("Matrícula: ");
        var matricula = Console.ReadLine() ?? string.Empty;

        var consumo = LeerDouble("Consumo (L/100km) (ej: 25.5): ");

        if (tipo == "a")
        {
            var capacidad = LeerDouble("Capacidad máxima (nº pasajeros): ");
            flota.Add(new Autobus(matricula, consumo, capacidad));
            Console.WriteLine("Autobús registrado correctamente.");
        }
        else if (tipo == "b")
        {
            var peaje = LeerDouble("Peaje anual (euros): ");
            flota.Add(new Camion(matricula, consumo, peaje));
            Console.WriteLine("Camión registrado correctamente.");
        }
        else
        {
            Console.WriteLine("Tipo no reconocido. Operación cancelada.");
        }
    }

    // VerCostos: muestra ToString() de cada vehículo (que incluye el coste por km).
    static void VerCostos(List<Vehiculo> flota)
    {
        Console.WriteLine();
        if (!flota.Any())
        {
            Console.WriteLine("No hay vehículos registrados.");
            return;
        }

        // Usamos LINQ para componer las líneas; ToList().ForEach para imprimir.
        flota
            .Select(v => $"{v}") // v.ToString() ya contiene información relevante y el coste por km
            .ToList()
            .ForEach(Console.WriteLine);
    }

    // Auxiliar para leer doubles; si la entrada no es válida devuelve 0.0
    // (la validación final de negativos ocurre en los setters de propiedades).
    static double LeerDouble(string prompt)
    {
        Console.Write(prompt);
        var raw = Console.ReadLine();
        if (double.TryParse(raw, out double valor)) return valor;
        Console.WriteLine("Entrada no numérica. Se asignará 0.0.");
        return 0.0;
    }
}
