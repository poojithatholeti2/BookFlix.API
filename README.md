# BookFlix — Intelligent Book Management & Recommendation API
![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![ASP.NETCore](https://img.shields.io/badge/ASP.NETCore-8-orange)
![EntityFramework](https://img.shields.io/badge/EntityFrameworkCore-8.0-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-blue)
![SQLServer](https://img.shields.io/badge/SQLServer-16-blue)
![LLM-Powered](https://img.shields.io/badge/LLMAPI-Groq-red)
![Python](https://img.shields.io/badge/Python-3-yellow)

BookFlix is a scalable, AI-enhanced backend platform for intelligent book management and semantic recommendations integrated with Groq LLM — built with ASP.NET Core, PostgreSQL, pgvector. 

<p align="center">
  <img src="./Assets/EmbeddingService_Image.jpg" width="30%" alt="Demo of BookFlix Book Creation followed by Embedding" />
  <img src="./Assets/RecommendationService_Image.jpg" width="37.3%" alt="Demo of BookFlix Recommendation Flow" />
  <!-- <div align="center"><p3>(Animated explanations below)</p3></div> -->
</p>

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

## API Endpoints & Access Control
### Authentication Endpoints

**Database used**: SQL Server 16  
**Purpose**: Handles user identity management, registration, login, and JWT issuance for secure access.  

| HTTP Method | Endpoint           | Description                        | Access         |
|--------|--------------------|------------------------------------|----------------|
| POST   | /api/auth/register | Register new user with roles       | Public         |
| POST   | /api/auth/login    | Login and receive JWT token        | Public         |

### Roles and Permissions Overview

These define access levels for users to enforce role-based authorization across all API operations.  
**Available Roles**: Admin, Writer, Reader 

| Role    | Permissions                                 |
|---------|---------------------------------------------|
| Admin   | Full access to all endpoints                |
| Writer  | Can manage all resources (create, update, delete)   |
| Reader  | Can view books, categories, and ratings     |

### Consumer Endpoints

**Database Used**: PostgreSQL 17  
**Purpose**: Handles client-facing CRUD operations across resources such as Books, Categories, and Ratings. Enforces strict role-based access control for secure API usage.  
**Supported Roles (OverAll)**: Admin, Writer, Reader  

| HTTP Method | Endpoint              | Roles Supported          |
|-------------|------------------------|---------------------------|
| POST        | /api/resource          | Admin, Writer             |
| GET         | /api/resource/{id}     | Admin, Writer, Reader     |
| GET         | /api/resource          | Admin, Writer, Reader     |
| PUT         | /api/resource/{id}     | Admin, Writer             |
| DELETE      | /api/resource/{id}     | Admin, Writer             |

---

## Comprehensive CRUD Endpoint Overview

Below is a consolidated view of all major CRUD endpoints for authentication, books, categories, and ratings. These endpoints enable Create, Read, Update, and Delete (CRUD) functionality with role-based access control for secure interactions.

| Resource     | Action        | HTTP Method | Endpoint                          | Roles Allowed         | Description                                              |
|--------------|---------------|-------------|-----------------------------------|------------------------|----------------------------------------------------------|
| **Auth**     | Register       | POST        | /api/auth/register                | Public                 | Register a new user with assigned role(s).              |
|              | Login          | POST        | /api/auth/login                   | Public                 | Authenticate user and return a JWT token.               |
| **Books**    | Create         | POST        | /api/books                        | Writer, Admin          | Add a new book to the catalog.                          |
|              | Bulk Create    | POST        | /api/books/bulk                   | Writer, Admin          | Insert multiple books in a single request.              |
|              | Read All       | GET         | /api/books                        | Reader, Writer, Admin  | Retrieve all available books with optional filters.     |
|              | Read by ID     | GET         | /api/books/{id}                   | Reader, Writer, Admin  | Fetch a specific book by its unique ID.                 |
|              | Update         | PUT         | /api/books/{id}                   | Writer, Admin          | Modify an existing book’s details.                      |
|              | Delete         | DELETE      | /api/books/{id}                   | Writer, Admin          | Remove a book permanently from the database.            |
|              | Recommend      | POST        | /api/books/recommend              | Reader, Writer, Admin  | Get AI-powered book recommendations.                    |
| **Categories**| Create        | POST        | /api/categories                   | Admin                  | Add a new category for books.                           |
|              | Read All       | GET         | /api/categories                   | Reader, Writer, Admin  | Retrieve a list of all categories.                      |
|              | Read by ID     | GET         | /api/categories/{id}              | Reader, Writer, Admin  | Get details of a specific category by ID.               |
|              | Update         | PUT         | /api/categories/{id}              | Admin                  | Update an existing category.                            |
|              | Delete         | DELETE      | /api/categories/{id}              | Admin                  | Delete a category permanently.                          |
| **Ratings**  | Create         | POST        | /api/ratings                      | Admin                  | Add a new book rating.                                  |
|              | Read All       | GET         | /api/ratings                      | Reader, Writer, Admin  | Fetch all ratings across books.                         |
|              | Read by ID     | GET         | /api/ratings/{id}                 | Reader, Writer, Admin  | View rating details by its ID.                          |
|              | Update         | PUT         | /api/ratings/{id}                 | Admin                  | Modify a rating entry.                                  |
|              | Delete         | DELETE      | /api/ratings/{id}                 | Admin                  | Remove a rating from the system.                        |


### Book Listing Features

BookFlix provides dynamic data operations via filterable, sortable, and pageable endpoints.

- **Filtering:** Filter books by Title or Author.
- **Sorting:** Sort by Title, Author, or Price.
- **Pagination:** Customize with pageNumber and pageSize.
- **Parameter Validation:** Only specific query keys are allowed for safety and predictability.
- **Role-based access:** Access to Endpoints based on roles (Reader, Writer, Admin).

## Example Request

### POST `/books`

**Request Body:**  

```json
{
    "title": "One Hundred Years of Solitude",
    "description": null,
    "author": "Gabriel Garcia Marquez",
    "price": 6990,
    "categoryid": "17ea39ed-3066-44f6-a0c1-d97be6b15de9",
    "ratingid": "4bb3890e-2acc-4ebe-9e5f-e0527b4b33cb"
}
```
**Validation Rules for Request body:**

| Field        | Type     | Constraints                                                                 |
|--------------|----------|------------------------------------------------------------------------------|
| `Title`      | string   | Required, MinLength: 3, MaxLength: 50                                       |
| `Description`| string  | Optional, MaxLength: 1000                                                    |
| `Author`     | string   | Required, MaxLength: 50                                                      |
| `Price`      | int      | Required, Range: 0–100000                                                    |
| `CategoryId` | GUID     | Required                                                                     |
| `RatingId`   | GUID     | Required                                                                     |


### GET `/books`

`GET /api/books?filterOn=Title&filterQuery=finance&sortBy=Price&isAscending=true&pageNumber=1&pageSize=10`

**Query Parameters:**

| Parameter     | Type   | Description                                                                 |
|---------------|--------|-----------------------------------------------------------------------------|
| `filterOn`    | string | Column to apply filter on. Allowed values: `Title`, `Author`.               |
| `filterQuery` | string | Value to match in the specified filter column.                              |
| `sortBy`      | string | Column to sort by. Allowed values: `Title`, `Author`, `Price`.              |
| `isAscending` | bool   | `true` for ascending order, `false` for descending.                         |
| `pageNumber`  | int    | Page number to retrieve (starting from 1).                                  |
| `pageSize`    | int    | Number of records per page (default: 1000, allows large datasets).          |

> ⚠️ Only the above parameters are accepted. Any extra or invalid query keys will result in a `400 Bad Request` with a list of allowed parameters.

---

## Embedding Service Architecture

Embeddings are generated asynchronously via a Python-based microservice integrated into the .NET application. This design ensures that the API remains highly responsive while handling AI workloads efficiently.

[Book Creation and Embedding Service Codeflow]
![Book Creation Flow](./Assets/EmbeddingService_Image.jpg)

### 1. **Book Creation Request:**
- When a client sends a `POST /api/books` request, the book data is passed from the controller to the service layer and saved into the database via the repository layer. A success response is returned immediately without waiting for embedding generation.

### 2. **Queueing for Embedding Generation:** 
- After the book is successfully stored, the `IBookService` enqueues the book data into a singleton `IEmbeddingQueue`, allowing the embedding process to run asynchronously without blocking the API response.

### 3. **Asynchronous Background Execution:** 
- A hosted background service continuously monitors the `IEmbeddingQueue`. Once it detects new book data, it dequeues the request and invokes the embedding generation process.
- The `IEmbeddingService` interacts with a Python module (`embedding_service.py`) via `PythonNet`. It passes the book's textual data (e.g., title and description) to `IEmbeddingService` that returns a 384-dimensional semantic embedding vector.

### 4. **Validation and Embedding Storage:** 
- The returned float array is validated to ensure it has 384 elements. If valid, it is wrapped into a `pgvector.Vector` object suitable for storage in PostgreSQL.
- The generated vector is stored in the PostgreSQL database alongside the book record. These vectors are later used for semantic similarity searches in the recommendation process.

---

## AI-Powered Recommendation Engine Workflow

BookFlix delivers intelligent recommendations through a multi-step, AI-augmented pipeline that blends powerful vector-based similarity search with LLM-driven contextual filtering.

[Recommendation Service Codeflow]
![Recommendation Flow](./Assets/RecommendationService_Image.jpg)

### 1. User Query Initiation
- The user sends a plain-text request like:
 `"Give a book that is related to finance or money."`

### 2. Semantic Query Embedding Generation
- The query is sent to a Python-based microservice using `IEmbeddingService`, where it is converted into a high-dimensional vector for semantic understanding.

### 3. Vector Similarity Search
- The .NET backend uses `pgvector` with PostgreSQL to compare the query vector against stored book vectors, identifying semantically similar books using cosine similarity.

### 4. Top Match Selection with Confidence Filtering
- The system retrieves the top 5 most relevant books, emphasizing high confidence and discarding ambiguous results to improve recommendation quality.

### 5. LLM-Based Filtering and Optimization
- The curated list of the top 5 semantically similar books is passed to `ILLMService`, along with a structured system prompt and the original query.
- The LLM analyzes the context and returns:
  - At most 2 GUIDs (book IDs) that are most relevant.
  - A `<think>` block that explains the model's thought process.

The system uses strict rules in the prompt to ensure consistent output format and accuracy.

### 6. Structured DTO Mapping and Explanation Generation
- The selected book IDs are matched to full book details and wrapped in a `RecommendationsDto` object, which includes:
  - A user-friendly message
  - The recommended books
  - Optional reasoning/explanation from the LLM (if `IsExplanationNeeded` is provided as `true` by the user)

## Example Query

- **Request:** Recommend books related to history, preferably around ₹500.
- **Response:** Returns two curated book recommendations that align with history, filtered for affordability and category relevance using semantic search and LLM refinement.

### POST `/recommend`

**Request Body:**  

```json
{
  "query": "Recommend books related to history, preferably around ₹500",
  "isExplanationNeeded": true
}
```

**Validation Rules for Request Body:**

| Field               | Type    | Constraints                          |
|---------------------|---------|--------------------------------------|
| `query`             | string  | Required, minimum 24 characters     |
| `isExplanationNeeded` | boolean | Optional          |

**Response:**
  
```json
{
  "message": "Here are the recommended Books based on your query among our available Books.",
  "books": [
    {
      "id": "01972f77-7756-7bff-b5cc-2c8998fc89c3",
      "title": "India After Gandhi",
      "description": "A comprehensive and readable history of India post-independence.",
      "author": "Ramachandra Guha",
      "price": 750,
      "categoryName": "History",
      "ratingName": "Good"
    },
    {
      "id": "01972f78-e10c-7d19-a750-179c23ec1f8e",
      "title": "The Discovery of India",
      "description": "Jawaharlal Nehru's sweeping reflection on India's rich heritage and its journey through time.",
      "author": "Jawaharlal Nehru",
      "price": 520,
      "categoryName": "History",
      "ratingName": "Good"
    }
  ],
  "explanation": "Okay, so I need to help the user by recommending books based on their query. The user is asking for books related to history, preferably around ₹500. I have five books to choose from, and I can recommend at most two. First, I'll look at each book's details. The first book is \"The Guide\" by R. K. Narayan. It's in the Literature category and costs ₹789. Since the user is interested in history, this doesn't fit, and the price is a bit higher than preferred. Next is \"The Little Book of Common Sense Investing\" by John C. Bogle. It's in the Finance category and priced at ₹990. That's way over the user's preferred price and not related to history, so I'll skip this one. The third book is \"The History of the Ancient World\" by Susan Wise Bauer. It's in the History category, which is exactly what the user wants. However, the price is ₹1399, which is significantly higher than ₹500. I'll note this but might not prioritize it unless there are no better options. The fourth book is \"India After Gandhi\" by Ramachandra Guha. It's also in the History category and priced at ₹750. This is closer to the user's price range but still a bit higher. It's a strong candidate since it fits the category. The fifth book is \"The Discovery of India\" by Jawaharlal Nehru. This one is in the History category as well and priced at ₹520, which is very close to the user's preferred price of around ₹500. This seems like the best fit. Now, I need to choose the top two. \"The Discovery of India\" is the closest in price and fits the category perfectly. \"India After Gandhi\" is a good second choice because it's also in the History category, even though it's a bit pricier. \"The History of the Ancient World\" is too expensive, so I'll leave it out. So, the two books I'll recommend are \"The Discovery of India\" and \"India After Gandhi.\" Their GUIDs are \"01972f78-e10c-7d19-a750-179c23ec1f8e\" and \"01972f77-7756-7bff-b5cc-2c8998fc89c3\" respectively. I'll format them as a comma-separated string without any spaces."
}

```

---

## Book Recommendations – Design Optimizations Summary

The recommendation engine is engineered for speed, accuracy, and scale. This allows the system to deliver highly relevant book suggestions with minimal latency, even across millions of books, through the following optimizations:

- **PostgreSQL + pgvector Integration:** Enables efficient high-dimensional vector similarity search with support for semantic relevance.  
- **HNSW Indexing:** Uses Hierarchical Navigable Small World (HNSW) graphs for sub-millisecond vector search performance, even at scale.  
- **Stateless & Modular Architecture:** Each service is loosely coupled and stateless, allowing seamless horizontal scaling under heavy load.  
- **Offloaded LLM & Embedding Tasks:** All resource-intensive processing, including LLM-based filtering and vector embedding, is handled outside the main API thread—ensuring minimal latency during API calls.  
- **Real-Time Recommendations at Scale:** The engine remains performant and responsive even as the catalog of books grows exponentially.  
- **Strict Prompting Strategy:** Groq LLM is guided with precise rules—returning only valid IDs, limiting to two results, and prioritizing category match. This ensures structured and reliable outputs.  
- **Fallback for No Match Scenarios:** If no high-confidence match is found, the system gracefully responds with a clear, fallback message—avoiding forced or irrelevant suggestions.  
- **Asynchronous Embedding Pipeline:** Embedding generation is fully decoupled and handled in the background using a task queue, boosting throughput without blocking user-facing endpoints.  
  
---

## Performance and Scalability Highlights

- **Modular Architecture:** Follows clean coding practices with separation of controller, service, and repository layers.
- **Role-Based Access Control:** Ensures only authorized users access specific actions.
- **Optimized DTO Usage:** Reduces payload size and boosts performance with AutoMapper.
- **Asynchronous Embedding Pipeline:** Improves throughput and decouples AI logic from core APIs.
- **Logging with Serilog:** Logs API activity to both console and persistent storage.
- **Scalable Storage:** PostgreSQL and SQL Server integration support high data volume and fast access.

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
JWT_Issuer=https://localhost:7164/  
JWT_Audience=https://localhost:7164/  

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
