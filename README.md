# NexTrack

NexTrack is an ASP.NET Core MVC application designed for hierarchical item tracking and processing management. It allows users to track raw material batches, process them into components, and visualize these parent-child relationships in an intuitive tree structure.

## 🚀 Features
- **Item Management**: Add new items (raw materials, components, etc.) and specify their properties like weight and processing status.
- **Hierarchical Processing**: Track parent-child relationships as raw materials get processed or broken down into smaller components.
- **Tree Visualization**: View the full lifecycle of an item and its derivatives using an interactive tree layout.
- **Robust Persistence**: Data is securely and permanently stored using Entity Framework Core and MS SQL Server Express.
- **Modern UI**: Styled with Bootstrap for a clean and responsive user experience. 

## 🛠 Technology Stack
- **Framework**: .NET 8, ASP.NET Core MVC
- **Database ORM**: Entity Framework Core
- **Database**: MS SQL Server (via `.\SQLEXPRESS`)
- **Frontend**: Razor Pages / Views, HTML, CSS, JavaScript, Bootstrap

## 💻 Running Locally

### 1. Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- SQL Server Express (`.\SQLEXPRESS`)

### 2. Setup Database Connection
Ensure your application is configured to connect to your local SQL Server Express instance. Check `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=NexTrackDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

### 3. Build & Run
Open your terminal in the root directory (where `NexTrack.csproj` is located) and run:
```bash
dotnet run
```
On the very first run, Entity Framework Core will automatically create the `NexTrackDb` database and its tables in your SQL Server instance, and seed it with initial tracking mock data.

### 4. View in Browser
Once started, the terminal will indicate where the app is running. Open your browser and navigate to:
`http://localhost:5208`

## 📖 Database Schema
The core application relies on a self-referencing `Item` structure:
- `Id`: Unique Identifier
- `Name`: Description of the item (e.g., 'Raw Material Batch A', 'Component X')
- `Weight`: The weight or quantity of the item
- `Status`: Current state (`pending` or `processed`)
- `ParentId`: References the source item it was derived from
- `CreatedAt`: Timestamp of entry
