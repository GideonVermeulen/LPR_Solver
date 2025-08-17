This is a .NET 8 Blazor Server application that functions as a linear programming (LP) and integer programming (IP) solver.

### Project Structure

- **`LPR_Solver.csproj`**: Defines the project as a .NET 8 web application.
- **`Program.cs`**: The application's entry point, configured as a standard Blazor Server app.
- **`Components/`**: Contains the Blazor components that make up the UI.
  - **`Pages/Home.razor`**: The main and only page of the application. It provides the entire user interface for the solver.
- **`wwwroot/`**: Contains static assets like CSS and JavaScript.
- **`test_lp.txt`**: An example input file demonstrating the format for defining an LP problem.

### Current Functionality (Implemented)

The user interface in `Home.razor` currently allows users to:

- **Define an LP/IP Problem**:
  - Specify the objective function (maximize or minimize) and its coefficients.
  - Add, remove, and define constraints with `<=`, `>=`, or `=` operators.
  - Define variable sign restrictions (e.g., integer, binary).
- **Import Problem File**:
  - Import a problem from a `.txt` file, with correct parsing of coefficients, operators, and RHS values.
- **Basic UI Elements**:
  - Algorithm selection cards (visual only, no backend logic connected yet).
  - A "Solve Problem" button (visual only, no backend logic connected yet).
  - Placeholder areas for Canonical Form, Current Tableau, Solution Results, and All Tableau Iterations.

### Input File Format (`test_lp.txt`)

The application can import `.txt` files with a specific format:

```
<max/min> <c1> <c2> ... <cn>
<a11> <a12> ... <a1n> <=/=/=> <b1>
...
<am1> <am2> ... <amn> <=/=/=> <bm>
<type1> <type2> ... <typen>
```

- The first line defines the objective function.
- The following lines define the constraints.
- The last line defines the variable types (e.g., `+`, `int`, `bin`).

### Remaining Tasks (To Be Implemented)

Based on the "Project LPR381 (Programming)" PDF and current progress:

1.  **Backend Solver Implementation**:
    *   Implement the **Primal Simplex Algorithm** (C# backend).
    *   Connect the "Solve Problem" button to trigger the Primal Simplex solver.
    *   Ensure the solver reads LP problem data from the UI content (objective function, constraints, variable restrictions).
    *   Display the calculated values for decision variables in the UI.
    *   Display the pivot history (all tableau iterations) in the UI.
2.  **Other Algorithms**:
    *   Implement Dual Simplex Algorithm.
    *   Implement Revised Primal Simplex Algorithm.
    *   Implement Branch & Bound Simplex Algorithm.
    *   Implement Cutting Plane Algorithm.
    *   Implement Branch & Bound Knapsack Algorithm.
3.  **Output/Export Functionality**:
    *   Implement the "Export" button functionality to export results to a text file.
4.  **Sensitivity Analysis & Duality**:
    *   Implement all specified sensitivity analysis operations (ranges, changes, shadow prices).
    *   Implement Duality analysis (transform to dual model, show dual solution, check duality).
5.  **Special Case Handling**:
    *   Implement logic to identify and resolve infeasible or unbounded programming models.
6.  **Variable Type Handling**:
    *   Ensure the solver correctly handles Integer and Binary variable types (requires extending the core simplex logic, e.g., with Branch & Bound for IP).

### Troubleshooting Notes

- **Browser Caching**: When making changes to JavaScript files (`wwwroot/js/demo.js`) or Blazor components, the browser may serve a cached, outdated version. To ensure the latest code is loaded, perform a clean build (`dotnet clean`, `dotnet build`), restart the application, and force refresh the browser (Ctrl+F5 or Cmd+Shift+R). Adding a cache-busting query string (e.g., `js/demo.js?v=TIMESTAMP`) to the script tag in `Components/App.razor` can help prevent this.
