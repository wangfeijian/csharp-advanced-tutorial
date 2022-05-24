using System;
using System.Collections.Generic;

namespace ClassGenericDemo
{
    class Program
    {
        static void Main()
        {
            var stuOne = new Student() { Name = "Wang", Id = 100001 };
            var stuTwo = new Student() { Name = "Liu", Id = 100002 };
            var stuThree = new Student() { Name = "Li", Id = 100003 };
            var stuFour = new Student() { Name = "Zhang", Id = 100004 };
            stuOne.AddToQueue();
            stuTwo.AddToQueue();
            stuThree.AddToQueue();
            stuFour.AddToQueue();
            bool b = false;
            do
            {
                if (Student.GetInstance().StudentDatas.Count > 0)
                {
                    var data = Student.GetInstance().StudentDatas.Dequeue();
                    Console.WriteLine($"{data.Name} is #{data.Info[data.Name]}.");
                    b = true;
                }
            } while (b);

        }
    }

    public class Student : BaseClassDemo<Student>
    {
        StudentData stuData = new StudentData();
        public Student()
        {

        }
        public Student(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public void AddToQueue()
        {
            stuData.Name = Name;
            stuData.Id = Id;
            stuData.AddStudent();
            GetInstance().StudentDatas.Enqueue(stuData);
        }
    }

    public class Animal : BaseClassDemo<Animal>
    {

    }

    public class StudentData
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public Dictionary<string, int> Info = new Dictionary<string, int>();

        public void AddStudent()
        {
            Info[Name] = Id;
        }
    }
    public class BaseClassDemo<T> where T : class, new()
    {
        public Queue<StudentData> StudentDatas = new Queue<StudentData>();

        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Syslock = new object();
        public string Name { get; set; }

        public int Id { get; set; }

        private static T _mInstance;

        public void ShowInfo()
        {
            Console.WriteLine($"{Name}'s id is {Id}.");
        }

        public static T GetInstance()
        {
            if (_mInstance == null)
            {
                lock (Syslock)
                {
                    if (_mInstance == null)
                        _mInstance = Activator.CreateInstance<T>();
                }
            }
            return _mInstance;
        }
    }
}
