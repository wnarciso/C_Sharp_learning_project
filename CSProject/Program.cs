using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSProject
{
    class Staff 
    {
        private float hourlyRate;
        private int hWorked;

        public float TotalPay { get; protected set; }
        public float BasicPay { get; private set; }
        public string NameOfStaff { get; private set; }
        public int HoursWorked
        {
            get 
            {
                return hWorked;
            }
            set 
            {
                if (value > 0)
                    hWorked = value;
                else
                    hWorked = 0;
            }
        }
        public Staff(string name, float rate) 
        {
            NameOfStaff = name;
            hourlyRate = rate;
        }
        public virtual void CalculatePay() 
        {
            Console.WriteLine("Calculating Pay");
            BasicPay = hWorked * hourlyRate;
            TotalPay = BasicPay;
        }
        public override string ToString()
        {
            return ("Name of Staff = " + NameOfStaff + ", Hours Worked = " + HoursWorked
                + ", Basic Pay = " + BasicPay + ", Total Pay = " + TotalPay);
        }

    }

    class Manager : Staff 
    {
        private const float managerHourlyRate = 50;
        public int Allowance { get; private set; }
        public Manager(string name) : base(name, managerHourlyRate) 
        {
        
        }
        public override void CalculatePay()
        {
            base.CalculatePay();
            Allowance = 1000;
            if (HoursWorked > 160)
                TotalPay = TotalPay + Allowance;
        }
        public override string ToString()
        {
            return ("Name of Staff = " + NameOfStaff + ", Hours Worked = " + HoursWorked
                + ", Basic Pay = " + BasicPay + ", Total Pay = " + TotalPay);
        }
    }

    class Admin : Staff 
    {
        private const float overtimeRate = 15.5f;
        private const float adminHourlyRate = 30;
        public float Overtime { get; private set; }
        public Admin(string name) : base(name, adminHourlyRate) 
        { 
        }
        public override void CalculatePay()
        {
            base.CalculatePay();
            if (HoursWorked > 160) {
                Overtime = overtimeRate * (HoursWorked - 160);
                TotalPay = TotalPay + Overtime;
            }
        }
        public override string ToString()
        {
            return ("Name of Staff = " + NameOfStaff  + ", Hours Worked = " + HoursWorked
                + ", Basic Pay = " + BasicPay + ", Total Pay = " + TotalPay);
        }
    }

    class FileReader 
    {
        public List<Staff> ReadFile() 
        {
            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "staff.txt";
            string[] separator = {", "};
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        string sname = sr.ReadLine();
                        result[0] = sname.Split(separator, StringSplitOptions.None)[0];
                        result[1] = sname.Split(separator, StringSplitOptions.None)[1];
                        if (result[1].Equals("Manager"))
                        {
                            myStaff.Add(new Manager(result[0]));
                        }
                        else if (result[1].Equals("Admin"))
                        {
                            myStaff.Add(new Admin(result[0]));
                        }
                    }
                    sr.Close();
                }
            }
            else {
                Console.WriteLine("File does not exist");
            }
            return myStaff;

        }
    }

    class PaySlip 
    {
        private int month;
        private int year;
        enum MonthsOfYear{JAN, FEB, MAR, APR, MAY, JUNE, JULY, AUG, SEP, OCT, NOV, DEC}
        public PaySlip(int payMonth, int payYear) 
        {
            month = payMonth;
            year = payYear;
        }
        public void GeneratePaySlip(List<Staff> myStaff) 
        {
            string path;
            foreach (Staff f in myStaff) 
            {
                path = f.NameOfStaff + ".txt";
                using (StreamWriter sw = new StreamWriter(path)) 
                {
                    sw.WriteLine("PAYSLIP FOR {0} {1}", (MonthsOfYear)month, year);
                    sw.WriteLine("==========================");
                    sw.WriteLine("Name of Staff: {0}", f.NameOfStaff);
                    sw.WriteLine("Hours Worked: {0}", f.HoursWorked);
                    sw.WriteLine("");
                    sw.WriteLine("Basic Pay: {0:C}", f.BasicPay);
                    if (f.GetType() == typeof(Manager))
                        sw.WriteLine("Allowance: {0:C}", ((Manager)f).Allowance);
                    else if (f.GetType() == typeof(Admin))
                        sw.WriteLine("Overtime: {0:C}", ((Admin)f).Overtime);
                    sw.WriteLine("");
                    sw.WriteLine("==========================");
                    sw.WriteLine("Total Pay: {0:C}", f.TotalPay);
                    sw.WriteLine("==========================");
                    sw.Close();
                }
            }
        }
        public void GenerateSummary(List<Staff> myStaff) 
        {
            var result =
                from staff in myStaff
                where staff.HoursWorked < 10
                orderby staff.NameOfStaff ascending
                select new { staff.NameOfStaff, staff.HoursWorked };
            string path = "summary.txt";
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("Staff with less than 10 working hours");
                sw.WriteLine("");
                foreach (Staff f in myStaff) {
                    sw.WriteLine("Name of Staff: {0}, Hours Worked: {1}", f.NameOfStaff, f.HoursWorked);
                }
                sw.Close();
            }

        }
        public override string ToString()
        {
            return "Month: " + month + ", Year: " + year;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            List<Staff> myStaff;
            FileReader fr = new FileReader();
            int month = 0;
            int year = 0;
            while (year == 0) 
            {
                Console.WriteLine("\nPlease enter the year: ");
                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException) 
                {
                    //Code to handle the exception
                    Console.WriteLine("Please enter a valid number.");
                }
            }
            while (month == 0)
            {
                Console.WriteLine("\nPlease enter the month: ");
                try
                {
                    month = Convert.ToInt32(Console.ReadLine());
                    if (month < 1 || month > 12) {
                        Console.WriteLine("Please enter a valid number between 1 and 12");
                        month = 0;
                    }
                }
                catch (FormatException)
                {
                    //Code to handle the exception
                    Console.WriteLine("Please enter a valid number.");
                }
            }
            myStaff = fr.ReadFile();
            for (int i = 0; i < myStaff.Count; i++)
            {
                try
                {
                    Console.WriteLine("Enter hours worked for {0}: ", myStaff[i].NameOfStaff);
                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    Console.WriteLine(myStaff[i].ToString());
                }
                catch (Exception e) 
                {
                    Console.WriteLine("Error: {0}", e);
                    i--;
                }
            }
            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);
            Console.Read();

        }
    }
}
