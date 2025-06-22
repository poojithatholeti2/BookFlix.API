# BookFlix — Intelligent Book Management & Recommendation API
BookFlix is a scalable, AI-enhanced backend platform for intelligent book management and semantic recommendations integrated with Groq LLM — built with ASP.NET Core, PostgreSQL, pgvector. 

## Description

**BookFlix** is a secure, modular, and scalable API-first backend platform built with **ASP.NET Core Web API** and **Entity Framework Core**. It manages books, categories, and ratings with advanced filtering, sorting, and pagination, following scalable software design principles. Featuring robust JWT-based authentication with role-based authorization, clean service-repository separation, and scalable dependency injection, all structured within a maintainable and extensible architecture.  

The project includes intelligent, vector-based AI recommendations using pgvector, powered by the Groq LLM API, coupled with advanced semantic similarity search using pgvector in PostgreSQL. A dedicated Python microservice asynchronously generates embeddings, decoupled via a queue-based pipeline to ensure optimal performance and modularity.

---

## Tech Stack

| Component                  | Technology Used                                              |
|---------------------------|--------------------------------------------------------------|
| Backend Framework         | ASP.NET Core Web API                                         |
| ORM & Databases           | Entity Framework Core, PostgreSQL, SQL Server               |
| Vector Similarity Search  | pgvector (PostgreSQL extension)                             |
| AI/LLM Integration        | Groq LLM API                                                 |
| Authentication & Roles   | ASP.NET Identity, JWT                                        |
| DTO Mapping               | AutoMapper                                                  |
| Embedding Service         | Python (via pythonnet for embedding generation)             |
| Logging & Monitoring      | Serilog                                                     |

---

## Endpoints Overview
### Authentication Endpoints

**Database used**: SQL Server 16.

**Purpose**: Handles user identity management, registration, login, and JWT issuance for secure access.  

| Method | Endpoint           | Description                        | Access         |
|--------|--------------------|------------------------------------|----------------|
| POST   | /api/auth/register | Register new user with roles       | Public         |
| POST   | /api/auth/login    | Login and receive JWT token        | Public         |

### Roles and Permissions Overview

These define access levels for users to enforce role-based authorization across all API operations. 

| Role    | Permissions                                 |
|---------|---------------------------------------------|
| Admin   | Full access to all endpoints                |
| Writer  | Can manage books (create, update, delete)   |
| Reader  | Can view books, categories, and ratings     |

---

## CRUD Operations Overview

Below is a consolidated view of all major CRUD endpoints for authentication, books, categories, and ratings.

These endpoints enable full Create, Read, Update, and Delete functionality with role-based access control for secure interactions.

| Resource     | Action      | Method | Endpoint                          | Roles Allowed       |
|--------------|-------------|--------|-----------------------------------|----------------------|
| Auth         | Register     | POST   | /api/auth/register                | Public               |
|              | Login        | POST   | /api/auth/login                   | Public               |
| Books        | Create       | POST   | /api/books                        | Writer, Admin        |
|              | Bulk Create  | POST   | /api/books/bulk                   | Writer, Admin        |
|              | Read All     | GET    | /api/books                        | Reader, Writer, Admin|
|              | Read by ID   | GET    | /api/books/{id}                   | Reader, Writer, Admin|
|              | Update       | PUT    | /api/books/{id}                   | Writer, Admin        |
|              | Delete       | DELETE | /api/books/{id}                   | Writer, Admin        |
|              | Recommend    | POST   | /api/books/recommend              | Reader, Writer, Admin|
| Categories   | Create       | POST   | /api/categories                   | Admin                |
|              | Read All     | GET    | /api/categories                   | Reader, Writer, Admin|
|              | Read by ID   | GET    | /api/categories/{id}              | Reader, Writer, Admin|
|              | Update       | PUT    | /api/categories/{id}              | Admin                |
|              | Delete       | DELETE | /api/categories/{id}              | Admin                |
| Ratings      | Create       | POST   | /api/ratings                      | Admin                |
|              | Read All     | GET    | /api/ratings                      | Reader, Writer, Admin|
|              | Read by ID   | GET    | /api/ratings/{id}                 | Reader, Writer, Admin|
|              | Update       | PUT    | /api/ratings/{id}                 | Admin                |
|              | Delete       | DELETE | /api/ratings/{id}                 | Admin                |

---

## Embedding Service Architecture

Embeddings are generated asynchronously via a Python-based microservice integrated into the .NET application.

- When a book is created, its data is queued for embedding generation.
- A hosted background service listens to this queue.
- The Python service uses Groq's LLM to create vector embeddings.
- These embeddings are stored in PostgreSQL via pgvector for future recommendations.

This design ensures that the API remains highly responsive while handling AI workloads efficiently.

---

## AI-Powered Recommendation Engine

BookFlix includes a powerful vector-based recommendation engine that utilizes semantic similarity and LLM-generated embeddings.

- Groq LLM API generates context-aware embeddings from book titles and metadata.
- pgvector is used for high-speed vector similarity search in PostgreSQL.
- EmbeddingQueue handles asynchronous vector processing without blocking API operations.
- A custom RecommendationService retrieves top-matching books based on semantic embeddings.

This allows the system to deliver highly relevant book suggestions with minimal latency.

---

## Book Listing Features

BookFlix provides dynamic data operations via filterable, sortable, and pageable endpoints.

- Filtering: Filter books by Title or Author.
- Sorting: Sort by Title, Author, or Price.
- Pagination: Customize with pageNumber and pageSize.
- Parameter Validation: Only specific query keys are allowed for safety and predictability.


---

## Performance and Scalability Highlights

- Asynchronous Embedding Pipeline: Improves throughput and decouples AI logic from core APIs.
- Modular Architecture: Follows clean coding practices with separation of controller, service, and repository layers.
- Role-Based Access Control: Ensures only authorized users access specific actions.
- Optimized DTO Usage: Reduces payload size and boosts performance with AutoMapper.
- Logging with Serilog: Logs API activity to both console and persistent storage.
- Scalable Storage: PostgreSQL and SQL Server integration support high data volume and fast access.

---

## QuickStart

### Follow these steps to get BookFlix API up and running locally:

### 1. Clone the Repository
- git clone https://github.com/your-username/BookFlix.git
- cd BookFlix
### 2. Setup Environment Variables
#### Create a .env file at the root of the project with the following keys:

<details> <summary><strong>📄 .env Example</strong> (click to expand)</summary>

#### #PostgreSQL for Book Catalog
BookFlixConnectionString=Host=localhost;Port=5432;Database=BookFlixDb;Username=your_username;Password=your_password

#### #SQL Server or PostgreSQL for Auth Database
BookFlixAuthDbConnectionString=Server=localhost;Database=BookFlixAuthDb;Trusted_Connection=True;TrustServerCertificate=True

#### #JWT Configuration
JWT_Key=your_super_secret_key
JWT_Issuer=https://localhost:7016/
JWT_Audience=https://localhost:7016/

#### #Python Embedding Service (if using pythonnet)
PythonDLLPath=C:/Path/To/pythonXY.dll
PythonScriptsFolder=C:/Path/To/BookFlix/Python

#### #Groq AI Configuration
GroqApiKey=your_groq_api_key
GroqLLMModel=llama3-8b-instruct
</details>

## 3. Apply Migrations
#### Navigate to the API project directory
- cd BookFlix.API

#### Apply migrations to both databases
- dotnet ef database update --context BookFlixDbContext
- dotnet ef database update --context BookFlixAuthDbContext

## 4. Run the Project
- dotnet run --project BookFlix.API

## 5. Test with Swagger
- Visit: https://localhost:1234/swagger
