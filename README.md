BookFlix is a scalable, AI-enhanced backend platform for intelligent book management and semantic recommendations integrated with Groq LLM — built with ASP.NET Core, PostgreSQL, pgvector.

<h3>Description</h3>
<hr style="border: 0; height: 1px; background: #e0e0e0; margin: 20px 0;" />
BookFlix is a modular, API-first backend platform built with ASP.NET Core Web API and Entity Framework Core, designed to manage books, categories, and ratings following scalable software design principles. It features robust JWT-based authentication with role-based authorization, clean service-repository separation, and scalable dependency injection, all structured within a maintainable and extensible architecture.

The project includes an intelligent, AI-driven recommendation engine powered by the Groq LLM API, coupled with advanced semantic similarity search using pgvector in PostgreSQL. A dedicated Python microservice asynchronously generates embeddings, decoupled via a queue-based pipeline to ensure optimal performance and modularity.

<h3>Tech Stack</h3>
| Component                  | Technology Used                                              |
|---------------------------|--------------------------------------------------------------|
| Backend Framework          | C# ASP.NET Core Web API                                      |
| Database                   | Entity Framework Core, PostgreSQL, SQL Server                |
| Vector Similarity Search   | pgvector (PostgreSQL extension)                              |
| AI Integration             | Groq LLM API (for intelligent recommendation generation)     |
| DTO Mapping                | AutoMapper                                                   |
| Embedding Service          | Python (optional) for embedding service                      |

