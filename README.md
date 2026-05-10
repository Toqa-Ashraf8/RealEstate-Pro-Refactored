# Real Estate Management System v2 (Clean Architecture & Dapper)

A specialized Full-Stack Application designed to digitize real estate operations. This is the **Refactored Version** of the original system, upgraded to follow **Clean Architecture** principles and optimized for high-performance data access.

## 🏗️ The Evolution (Refactoring Story)
While the first version focused on core business logic, this version (v2) was developed to solve architectural challenges:
* **From Monolithic to Clean Architecture:** Separated the project into layers (API, Core, Service, Repository) to achieve **Decoupling**.
* **From EF/ADO to Dapper:** Replaced standard data access with **Dapper ORM** for 10x faster query execution and full control over SQL.
* **Infrastructure Upgrades:** Implemented **Generic Repository Pattern** and **SQL Transactions** to ensure data consistency.

## 🏗️ Architectural Overview
This version follows **Clean Architecture** to ensure a clear **Separation of Concerns**:
* **API Layer:** RESTful endpoints with **Thin Controllers** and **DTOs**.
* **Repository Layer:** High-performance data access using **Dapper**.
* **Core Layer:** Centralizes Domain Models and Repository Interfaces.

## 🚀 Technical Highlights
* **Optimized Performance:** Used **Dapper's QueryMultiple** and **Async/Await** to minimize database round-trips.
* **Robust Transactions:** Managed complex "Unit Booking" workflows using **SQL Transactions** (Commit/Rollback) to guarantee data integrity.
* **Cleaner Logic:** Solved previous "Future Roadmap" challenges by centralizing business rules in the Service layer.

## 🛠️ Tech Stack
* **Frontend:** React.js, Redux Toolkit, Bootstrap.
* **Backend:** .NET 8 Web API, **Dapper**, SQL Server.
* **Database:** MS SQL Server (Complex Joins, Views).

## 🌟 Core Features (Legacy & New)
* **Secure RBAC:** Custom dashboards for Admins and Employees.
* **Master-Detail Inventory:** Dynamic management of Projects and Units.
* **Automated Installment Engine:** Real-time generation and tracking of payment schedules.
* **Advanced Negotiation Workflow:** Integrated "Managerial Decision Engine" for price approvals.

## 🔧 Installation & Setup
1. Clone the repo: `git clone https://github.com/Toqa-Ashraf8/RealEstate_Clean_Dapper.git`
2. **Backend:** - Update `appsettings.json` with your SQL connection string.
   - Run `dotnet run`.
3. **Frontend:** - Run `npm install` then `npm start`.
