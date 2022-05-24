using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DeepCopyDemo
{
    internal class Program
    {
        private static void Main()
        {
            var person = new Person("wang", 18);

            Person person2 = person;
            var newPerson = DeepCopy(person);

            Console.WriteLine(person2.GetHashCode().ToString());
            person2.SayHello();
            Console.WriteLine(person.GetHashCode().ToString());
            person.SayHello();
            Console.WriteLine(newPerson.GetHashCode().ToString());
            newPerson.SayHello();
        }

        public static Person DeepCopy(Person p)
        {
            object obj;

            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();

                bf.Serialize(ms, p);
                ms.Seek(0, SeekOrigin.Begin);

                obj = bf.Deserialize(ms);
                ms.Close();
            }

            return (Person)obj;
        }
    }

    [Serializable]
    internal class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public void SayHello()
        {
            Console.WriteLine($"{Name} is {Age} years old, Say hello to you!");
        }
    }
}
