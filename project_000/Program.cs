using Lab2.Composite;
using Lab2.Observer;
using System.Linq;
using System.IO;
using System;

namespace Lab2
{
    namespace Observer
    {
        public class Event<Type>
        {
            private HashSet<Action<Type>> Subscribers = new();
            public void Add(Action<Type> subscriber) => Subscribers.Add(subscriber);
            public void Remove(Action<Type> subscriber) => Subscribers.Remove(subscriber);
            public void SendMessage(Type message)
            {
                foreach (var subscriber in Subscribers)
                    subscriber(message);
            }
        }
    }
    namespace Composite
    {
        public interface ICalculatable { double calculate(); }
        public abstract class UnaryOperation : ICalculatable
        {
            public abstract double calculate();
            public ICalculatable Origin { get; set; }
            protected UnaryOperation(ICalculatable origin) => Origin = origin;
        }
        public abstract class BinaryOperation : ICalculatable
        {
            public abstract double calculate();
            public ICalculatable OriginLeft { get; set; }
            public ICalculatable OriginRight { get; set; }

            protected BinaryOperation():this(null, null) { }
            protected BinaryOperation(ICalculatable left, ICalculatable right)
            {
                OriginLeft = left;
                OriginRight = right;
            }
        }

        public class DoubleNumber : ICalculatable
        {
            public double Value { get; set; }
            public double calculate() => Value;
            public DoubleNumber(double value) => Value = value;
        }

        public class UnaryPlus : UnaryOperation
        {
            public UnaryPlus(ICalculatable origin) : base(origin) { }
            public override double calculate() => Origin.calculate();
        }
        public class UnaryMinus : UnaryOperation
        {
            public UnaryMinus(ICalculatable origin) : base(origin) { }
            public override double calculate() => -Origin.calculate();
        }

        public class Addition : BinaryOperation
        {
            public override double calculate() => OriginLeft.calculate() + OriginRight.calculate();
            public Addition(ICalculatable left, ICalculatable right) : base(left, right) { }
            public Addition() : base() { }
        }
        public class Subtraction : BinaryOperation
        {
            public override double calculate() => OriginLeft.calculate() - OriginRight.calculate();
            public Subtraction(ICalculatable left, ICalculatable right) : base(left, right) { }
            public Subtraction() : base() { }
        }
        public class Multiplication : BinaryOperation
        {
            public override double calculate() => OriginLeft.calculate() * OriginRight.calculate();
            public Multiplication(ICalculatable left, ICalculatable right) : base(left, right) { }
            public Multiplication() : base() { }
        }
        public class Division : BinaryOperation
        {
            public override double calculate() => OriginLeft.calculate() / OriginRight.calculate();
            public Division(ICalculatable left, ICalculatable right) : base(left, right) { }
            public Division() : base() { }
        }
        public class Exponentiation : BinaryOperation
        {
            public override double calculate() => Math.Pow(OriginLeft.calculate(), OriginRight.calculate());
            public Exponentiation(ICalculatable left, ICalculatable right) : base(left, right) { }
            public Exponentiation() : base() { }
        }
    }
    class Program
    {
        class CmdCommandReader
        {
            public readonly Observer.Event<string> OnCommand = new();
            public void Exec()
            {
                while(true)
                    OnCommand.SendMessage(Console.ReadLine());
            }
        }
        class ExampleCommandHandler
        {
            public void OnCommandRecived(string command) => Console.WriteLine($"Handle command: {command}");
        }

        static void Main(string[] argv)
        {
            {
                var expression = new Addition(new DoubleNumber(5), new DoubleNumber(7));
                Console.WriteLine($"Result => {expression.calculate()}");
            }
            {
                var reader = new CmdCommandReader();
                var handler_1 = new ExampleCommandHandler();
                var handler_2 = new ExampleCommandHandler();

                reader.OnCommand.Add(handler_1.OnCommandRecived);
                reader.OnCommand.Add(handler_2.OnCommandRecived);
                reader.Exec();
            }
        }
    }
}
