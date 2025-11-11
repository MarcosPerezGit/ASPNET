using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ===========================
   Clase base: Empleado
   - Nombre: propiedad automática (requerido)
   - SalarioBase: propiedad NO automática con validación en el setter
   - CalcularNomina: método polimórfico virtual (comportamiento por defecto: SalarioBase)
   - ToString: virtual para permitir que las clases derivadas añadan información
   Nota: la clase es abstracta para cumplir el requisito de diseño común, pero
   aporta una implementación por defecto de CalcularNomina (comportamiento "empleado base").
   =========================== */
abstract class Empleado
{
    // Nombre como propiedad automática (lectura/escritura pública).
    public string Nombre { get; set; }

    // Campo privado para el salario base. Usamos propiedad no automática para añadir validación.
    double salarioBase;
    public double SalarioBase
    {
        get => salarioBase;
        set
        {
            // Regla: si se intenta asignar valor negativo, se pone 0.0
            salarioBase = value < 0.0 ? 0.0 : value;
        }
    }

    // Constructor que inicializa propiedades comunes.
    protected Empleado(string nombre, double salarioBase)
    {
        Nombre = nombre ?? string.Empty;
        SalarioBase = salarioBase; // el setter hará la validación
    }

    // Método polimórfico: por defecto, la nómina mensual es SalarioBase.
    // Es virtual para que las subclases lo sobreescriban cuando proceda.
    public virtual double CalcularNomina() => SalarioBase;

    // ToString virtual para imprimir atributos comunes; las subclases pueden extenderlo.
    public override string ToString() =>
        $"{GetType().Name} | Nombre: {Nombre} | SalarioBase: {SalarioBase:0.00}";
}

/* ===========================
   EmpleadoBase
   - Representa al "Empleado Base" (no añade campos nuevos)
   - Simplemente hereda el comportamiento base (CalcularNomina devuelve SalarioBase)
   - Se incluye como clase concreta para distinguir tipos en la colección.
   =========================== */
class EmpleadoBase : Empleado
{
    public EmpleadoBase(string nombre, double salarioBase)
        : base(nombre, salarioBase) { }

    // No es necesario sobreescribir CalcularNomina; heredará SalarioBase.
    // Pero sobreescribimos ToString para dejar claro que es un EmpleadoBase.
    public override string ToString() => base.ToString();
}

/* ===========================
   EmpleadoFijo
   - Hereda de Empleado
   - BonoAnual: propiedad NO automática con validación (requisito)
   - Nómina mensual = SalarioBase + BonoAnual / 12
   =========================== */
class EmpleadoFijo : Empleado
{
    double bonoAnual;
    public double BonoAnual
    {
        get => bonoAnual;
        set => bonoAnual = value < 0.0 ? 0.0 : value;
    }

    public EmpleadoFijo(string nombre, double salarioBase, double bonoAnual)
        : base(nombre, salarioBase)
    {
        BonoAnual = bonoAnual; // validación en setter
    }

    // Reusar la lógica base cuando tenga sentido; aquí calculemos sobre SalarioBase.
    public override double CalcularNomina() =>
        // prorrateo del bono anual
        SalarioBase + (BonoAnual / 12.0);

    public override string ToString() =>
        $"{base.ToString()} | BonoAnual: {BonoAnual:0.00}";
}

/* ===========================
   EmpleadoPorHora
   - Hereda de Empleado
   - TarifaHora, HorasTrabajadasMes: propiedades NO automáticas con validación
   - Nómina mensual = SalarioBase + TarifaHora * HorasTrabajadasMes
   =========================== */
class EmpleadoPorHora : Empleado
{
    double tarifaHora;
    public double TarifaHora
    {
        get => tarifaHora;
        set => tarifaHora = value < 0.0 ? 0.0 : value;
    }

    double horasTrabajadasMes;
    public double HorasTrabajadasMes
    {
        get => horasTrabajadasMes;
        set => horasTrabajadasMes = value < 0.0 ? 0.0 : value;
    }

    public EmpleadoPorHora(string nombre, double salarioBase, double tarifaHora, double horasTrabajadasMes)
        : base(nombre, salarioBase)
    {
        TarifaHora = tarifaHora;
        HorasTrabajadasMes = horasTrabajadasMes;
    }

    // Reusar SalarioBase y sumar la parte variable de horas.
    public override double CalcularNomina() =>
        SalarioBase + (TarifaHora * HorasTrabajadasMes);

    public override string ToString() =>
        $"{base.ToString()} | TarifaHora: {TarifaHora:0.00} | HorasMes: {HorasTrabajadasMes:0.00}";
}

/* ===========================
   Programa principal (consola)
   - Colección de Empleado (polimorfismo con LINQ para operaciones)
   - Menú: Contratar, Ver Nóminas Individuales, Calcular Coste Total, Salir
   =========================== */
class Program
{
    static void Main()
    {
        // Lista polimórfica: guardamos instancias de las distintas subclases.
        var empleados = new List<Empleado>();

        // Bucle principal del menú
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== TechSolutions - HRSystem (Consola) ===");
            Console.WriteLine("1) Contratar Empleado");
            Console.WriteLine("2) Ver Nóminas Individuales");
            Console.WriteLine("3) Calcular Coste Total de Nóminas");
            Console.WriteLine("4) Salir");
            Console.Write("Elige una opción (1-4): ");
            var opcion = Console.ReadLine()?.Trim();

            if (opcion == "1")
            {
                ContratarEmpleado(empleados);
            }
            else if (opcion == "2")
            {
                VerNominas(empleados);
            }
            else if (opcion == "3")
            {
                CalcularCosteTotal(empleados);
            }
            else if (opcion == "4")
            {
                Console.WriteLine("Saliendo... ¡Hasta pronto!");
                break;
            }
            else
            {
                Console.WriteLine("Opción no válida. Intenta de nuevo.");
            }
        }
    }

    // --------------------------
    // ContratarEmpleado: pide tipo y datos, añade a la colección.
    // Uso intensivo de pequeños métodos auxiliares para minimizar repetición.
    // --------------------------
    static void ContratarEmpleado(List<Empleado> empleados)
    {
        Console.WriteLine();
        Console.WriteLine("Tipo de empleado a contratar:");
        Console.WriteLine("a) Empleado Base");
        Console.WriteLine("b) Empleado Fijo");
        Console.WriteLine("c) Empleado Por Hora");
        Console.Write("Elige (a-c): ");
        var tipo = Console.ReadLine()?.Trim().ToLower();

        Console.Write("Nombre: ");
        var nombre = Console.ReadLine() ?? string.Empty;

        // SalarioBase: pedimos valor y dejamos que el setter haga la validación.
        var salarioBase = LeerDouble("Salario base mensual (ej: 1200.50): ");

        switch (tipo)
        {
            case "a":
                empleados.Add(new EmpleadoBase(nombre, salarioBase));
                Console.WriteLine("Empleado Base contratado correctamente.");
                break;

            case "b":
                var bono = LeerDouble("Bono anual (se prorratea): ");
                empleados.Add(new EmpleadoFijo(nombre, salarioBase, bono));
                Console.WriteLine("Empleado Fijo contratado correctamente.");
                break;

            case "c":
                var tarifa = LeerDouble("Tarifa por hora: ");
                var horas = LeerDouble("Horas trabajadas en el mes: ");
                empleados.Add(new EmpleadoPorHora(nombre, salarioBase, tarifa, horas));
                Console.WriteLine("Empleado Por Hora contratado correctamente.");
                break;

            default:
                Console.WriteLine("Tipo no reconocido. Operación cancelada.");
                break;
        }
    }

    // --------------------------
    // VerNominas: recorre la colección y muestra ToString + nómina mensual.
    // Usamos LINQ Select para construir las líneas de salida y ToList().ForEach para imprimir.
    // --------------------------
    static void VerNominas(List<Empleado> empleados)
    {
        Console.WriteLine();
        if (!empleados.Any())
        {
            Console.WriteLine("No hay empleados contratados.");
            return;
        }

        // Construimos una lista de strings con LINQ para separar la lógica de presentación.
        var lineas = empleados
            .Select(e => $"{e} | Nómina mensual: {e.CalcularNomina():0.00}")
            .ToList();

        // Imprimimos cada línea.
        lineas.ForEach(line => Console.WriteLine(line));
    }

    // --------------------------
    // CalcularCosteTotal: suma todas las nóminas mensuales con LINQ Sum.
    // --------------------------
    static void CalcularCosteTotal(List<Empleado> empleados)
    {
        Console.WriteLine();
        var total = empleados.Sum(e => e.CalcularNomina());
        Console.WriteLine($"Coste total mensual de nóminas ({empleados.Count} empleado(s)): {total:0.00}");
    }

    // --------------------------
    // LeerDouble: auxiliar que solicita un valor numérico al usuario.
    // Si la entrada no es un número válido, se devuelve 0.0 (seguimos la filosofía preventiva).
    // Observa que si el usuario introduce un número negativo, los setters de las propiedades
    // convertirán ese valor a 0.0 automáticamente; aquí devolvemos el double tal cual.
    // --------------------------
    static double LeerDouble(string prompt)
    {
        Console.Write(prompt);
        var raw = Console.ReadLine();
        if (double.TryParse(raw, out double valor))
        {
            // devolvemos el valor tal cual; la validación final la realizará la propiedad.
            return valor;
        }
        Console.WriteLine("Entrada no numérica. Se asignará 0.0 por defecto.");
        return 0.0;
    }
}
