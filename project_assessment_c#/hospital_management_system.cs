using System;

// Delegate for billing strategy
public delegate double BillingStrategy(double amount);

// Custom EventArgs
public class HospitalEventArgs : EventArgs
{
    public string Message;

    public HospitalEventArgs(string message)
    {
        Message = message;
    }
}

// Abstract Patient class
public abstract class Patient
{
    public int PatientId;
    public string Name;

    public abstract double CalculateBaseBill();
}

// Patient Types
public class GeneralPatient : Patient
{
    public override double CalculateBaseBill() => 2000;
}

public class EmergencyPatient : Patient
{
    public override double CalculateBaseBill() => 5000;
}

public class InsurancePatient : Patient
{
    public override double CalculateBaseBill() => 3000;
}

// Hospital class
public class Hospital
{
    public event EventHandler<HospitalEventArgs> PatientAdmitted;
    public event EventHandler<HospitalEventArgs> BillGenerated;

    public void AdmitPatient(Patient patient)
    {
        PatientAdmitted?.Invoke(this,
            new HospitalEventArgs("Patient " + patient.Name + " admitted."));
    }

    public double ApplyBilling(Patient patient, BillingStrategy strategy)
    {
        return strategy(patient.CalculateBaseBill());
    }

    public void GenerateBill(Patient patient, double amount)
    {
        BillGenerated?.Invoke(this,
            new HospitalEventArgs("Final bill for " + patient.Name + " is Rs." + amount));
    }
}

class Program
{
    static void Main()
    {
        Hospital hospital = new Hospital();

        hospital.PatientAdmitted += ReceptionNotification;
        hospital.BillGenerated += AccountsNotification;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Hospital Management System ===");
            Console.WriteLine("1. Admit New Patient");
            Console.WriteLine("2. View Hospital Services");
            Console.WriteLine("3. Emergency Contact Info");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");

            int menuChoice;
            if (!int.TryParse(Console.ReadLine(), out menuChoice))
                continue;

            switch (menuChoice)
            {
                case 1:
                    AdmitPatientFlow(hospital);
                    break;

                case 2:
                    ShowServices();
                    break;

                case 3:
                    ShowEmergencyContacts();
                    break;

                case 4:
                    Console.WriteLine("Thank you for using Hospital System.");
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    // ================= Patient Admission Flow =================
    static void AdmitPatientFlow(Hospital hospital)
    {
        Console.Write("Enter Patient ID: ");
        int id = int.Parse(Console.ReadLine());

        Console.Write("Enter Patient Name: ");
        string name = Console.ReadLine();

        Console.WriteLine("\nSelect Patient Type");
        Console.WriteLine("1. General");
        Console.WriteLine("2. Emergency");
        Console.WriteLine("3. Insurance");
        Console.Write("Choice: ");
        int choice = int.Parse(Console.ReadLine());

        Patient patient;

        if (choice == 1) patient = new GeneralPatient();
        else if (choice == 2) patient = new EmergencyPatient();
        else if (choice == 3) patient = new InsurancePatient();
        else
        {
            Console.WriteLine("Invalid Choice");
            return;
        }

        patient.PatientId = id;
        patient.Name = name;

        hospital.AdmitPatient(patient);

        BillingStrategy billing =
            (patient is InsurancePatient) ? InsuranceBilling : NormalBilling;

        double finalBill = hospital.ApplyBilling(patient, billing);
        hospital.GenerateBill(patient, finalBill);
    }

    // ================= Extra Menus =================
    static void ShowServices()
    {
        Console.WriteLine("\n--- Hospital Services ---");
        Console.WriteLine("• 24x7 Emergency Care");
        Console.WriteLine("• OPD Consultation");
        Console.WriteLine("• Diagnostic Labs");
        Console.WriteLine("• Pharmacy Services");
        Console.WriteLine("• Health Insurance Support");
    }

    static void ShowEmergencyContacts()
    {
        Console.WriteLine("\n--- Emergency Contacts ---");
        Console.WriteLine("Ambulance: 108");
        Console.WriteLine("Reception: 044-23456789");
        Console.WriteLine("Emergency Desk: 044-98765432");
    }

    // ================= Billing Methods =================
    static double InsuranceBilling(double amount)
    {
        return amount * 0.5;
    }

    static double NormalBilling(double amount)
    {
        return amount + (amount * 0.18);
    }

    // ================= Event Handlers =================
    static void ReceptionNotification(object sender, HospitalEventArgs e)
    {
        Console.WriteLine("[Reception] " + e.Message);
    }

    static void AccountsNotification(object sender, HospitalEventArgs e)
    {
        Console.WriteLine("[Accounts] " + e.Message);
    }
}