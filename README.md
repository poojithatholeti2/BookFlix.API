<p>BookFlix is a scalable, AI-enhanced backend platform for intelligent book management and semantic recommendations integrated with Groq LLM — built with ASP.NET Core, PostgreSQL, pgvector. </p>

## Description
<p>BookFlix is a modular, API-first backend platform built with ASP.NET Core Web API and Entity Framework Core, designed to manage books, categories, and ratings following scalable software design principles. It features robust JWT-based authentication with role-based authorization, clean service-repository separation, and scalable dependency injection, all structured within a maintainable and extensible architecture.  

The project includes an intelligent, AI-driven recommendation engine powered by the Groq LLM API, coupled with advanced semantic similarity search using pgvector in PostgreSQL. A dedicated Python microservice asynchronously generates embeddings, decoupled via a queue-based pipeline to ensure optimal performance and modularity </p>

## Tech Stack
<table>
  <thead>
    <tr>
      <th>Component</th>
      <th>Technology Used</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>Backend Framework</td>
      <td>C# ASP.NET Core Web API</td>
    </tr>
    <tr>
      <td>Database</td>
      <td>Entity Framework Core, PostgreSQL, SQL Server</td>
    </tr>
    <tr>
      <td>Vector Similarity Search</td>
      <td>pgvector (PostgreSQL extension)</td>
    </tr>
    <tr>
      <td>AI Integration</td>
      <td>Groq LLM API (for intelligent recommendation generation)</td>
    </tr>
    <tr>
      <td>DTO Mapping</td>
      <td>AutoMapper</td>
    </tr>
    <tr>
      <td>Embedding Service</td>
      <td>Python (optional) for embedding service</td>
    </tr>
  </tbody>
</table>

<hr style="border: 0; height: 1px; background: #e0e0e0; margin: 20px 0;" />


