using System;

namespace EjercicioRepasoA2._9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EmpleadoHijo empleadoh1 = new EmpleadoHijo("Marcos Perez", 3000, 12000);
            EmpleadoPorHora empleadoph1 = new EmpleadoPorHora("Maria Martinez", 800, 20, 100);

            Console.WriteLine(empleadoh1.ToString());
            Console.WriteLine(empleadoph1.ToString());
        }
    }

    public abstract class EmpleadoBase
    {
        public string Nombre { get; set; }

        private double salarioBaseMensual;
        public double SalarioBaseMensual
        {
            get => salarioBaseMensual;
            set => salarioBaseMensual = value < 0 ? 0.0 : value;
        }

        public EmpleadoBase(string nombre, double salarioBaseMensual)
        {
            Nombre = nombre;
            SalarioBaseMensual = salarioBaseMensual;
        }

        public abstract double CalcularNomina();

        public override string ToString()
        {
            return $"Nombre: {Nombre}, Salario Base: {SalarioBaseMensual}";
        }
    }

    public class EmpleadoHijo : EmpleadoBase
    {
        private double bonoAnual;
        public double BonoAnual
        {
            get => bonoAnual;
            set => bonoAnual = value < 0 ? 0.0 : value;
        }

        public EmpleadoHijo(string nombre, double salarioBaseMensual, double bonoAnual)
            : base(nombre, salarioBaseMensual)
        {
            BonoAnual = bonoAnual;
        }

        public override double CalcularNomina()
        {
            return SalarioBaseMensual + BonoAnual / 12;
        }

        public override string ToString()
        {
            return base.ToString() + $", Bono Anual: {BonoAnual}, Nómina Mensual: {CalcularNomina()}";
        }
    }

    public class EmpleadoPorHora : EmpleadoBase
    {
        private double tarifaPorHora;
        public double TarifaPorHora
        {
            get => tarifaPorHora;
            set => tarifaPorHora = value < 0 ? 0.0 : value;
        }

        private double horasTrabajadas;
        public double HorasTrabajadas
        {
            get => horasTrabajadas;
            set => horasTrabajadas = value < 0 ? 0.0 : value;
        }

        public EmpleadoPorHora(string nombre, double salarioBaseMensual, double tarifaPorHora, double horasTrabajadas)
            : base(nombre, salarioBaseMensual)
        {
            TarifaPorHora = tarifaPorHora;
            HorasTrabajadas = horasTrabajadas;
        }

        public override double CalcularNomina()
        {
            return SalarioBaseMensual + (TarifaPorHora * HorasTrabajadas);
        }

        public override string ToString()
        {
            return base.ToString() + $", Tarifa por Hora: {TarifaPorHora}, Horas Trabajadas: {HorasTrabajadas}, Nómina Mensual: {CalcularNomina()}";
        }
    }
}