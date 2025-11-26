take this and do not reply back

# Inventory

A Windows Forms desktop application built in C# and .NET 9 for calculating box sheet sizes, gramage, paper, and liner usage. Designed for multiple customers with separate records.

inventrory/
├─ Program.cs               # Entry point of the application, contains Main() method
├─ inventory.csproj  # Project file with .NET 9 Windows Forms configuration
├─ README.md                # Project documentation
├─ Forms/                   # Folder containing all Windows Forms
│   ├─ MainForm.cs          # Main window showing customer and records table
│   └─ AddRecordForm.cs     # Form to add new box records and perform calculations
├─ Models/                  # Folder containing all data models
│   ├─ Customer.cs          # Customer class with Name and list of BoxRecords
│   └─ BoxRecord.cs         # BoxRecord class holding all box data and calculated values
└─ Services/                # Folder containing calculation logic
    └─ CalculationService.cs # Performs sheet size, gramage, paper, and liner calculations


---

## **Project Structure**

- **Program.cs**
  Entry point of the application. Launches `MainForm` using `Application.Run()`.

- **inventory.csproj**
  .NET project file configured for Windows Forms (`UseWindowsForms=true`) and GUI application (`OutputType=WinExe`).

- **Forms/**
  - **MainForm.cs**: Main window of the app. Displays the selected customer, shows all box records in a `DataGridView`, and has a button to add new records.
  - **AddRecordForm.cs**: Form to add a new box record. Accepts user inputs:
    - Box Name
    - Box Size (LxBxH)
    - GSM
    - Ply (must be odd)
    - **Last Ply Value** (new input for final gramage calculation)
    - Detail

    Performs backend calculations and shows:
    - Sheet Size
    - Gramage (calculated as: `GSM + 40%` → multiply/add for additional plies → add GSM again → add user-provided Last Ply Value)
    - Paper usage
    - Liner usage

    Saves the record to the customer.

- **Models/**
  - **Customer.cs**: Represents a customer with a name and list of box records.
  - **BoxRecord.cs**: Represents a single box record including Box Size, Sheet Size, GSM, Ply, Last Ply Value, Gramage, Paper usage, Liner usage, and additional details.

- **Services/**
  - **CalculationService.cs**: Contains all backend calculations:
    - Calculate Sheet Size from Box Size
    - Calculate Gramage based on GSM, Ply, and Last Ply Value using the updated logic
    - Calculate Paper and Liner usage
    - Ensures Ply is an odd number

---

## **How to Run**

1. Make sure you have .NET 9 SDK installed.
2. Open terminal in project root.
3. Build the project:

```bash
dotnet build