using System;

namespace EventDetailDemo
{
    internal class Program
    {
        static void Main()
        {
            var customer = new Customer() {Name = "Wang"};
            var waiter = new Waiter();
            customer.OrderEvent += waiter.Action;
            customer.Action();
        }
    }

    public delegate void OrderEventHandler(Customer customer, OrderEventArgs eventArgs);

    public class Customer
    {
        public event OrderEventHandler OrderEvent;
        public double Bill { get; set; }
        public string Name { get; set; }

        public void GoEat()
        {
            Console.WriteLine($"{Name} is going the street.");
            Console.WriteLine($"{Name} go to the restaurant.\nSeat in the arm.");
        }

        public void Thinking()
        {
            Console.WriteLine("Let me thinking.");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Thinking!!");
            }

            if (OrderEvent == null) return;
            var o = new OrderEventArgs {Size = "small", DishName = "Pizza"};
            OrderEvent.Invoke(this,o);
        }

        public void Action()
        {
            GoEat();
            Thinking();
        }
    }

    public class OrderEventArgs : EventArgs
    {
        public string DishName { get; set; }
        public string Size { get; set; }
    }

    public class Waiter
    {
        public void Action(Customer customer, OrderEventArgs eventArgs)
        {
            double price = 10;
            switch (eventArgs.Size)
            {
                case "large":
                    customer.Bill = price * 15;
                    break;
                case "small":
                    customer.Bill = price * 5;
                    break;
            }

            Console.WriteLine($"{customer.Name} eats a {eventArgs.Size} {eventArgs.DishName}.\nNeeds pay {customer.Bill}.");
        }
    }
}
