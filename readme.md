# ASP.NET Web API Project README

## Introduction

This is a Task Managament Backend API made with ASP.NET Web API. This API provided the following features.

1. User Management - Authentication and Authorization 
2. Task Management System 
3. Notification Management 


## Installation

Follow these steps to set up the project locally:

1. Clone the repository:

   ```bash
   git clone https://github.com/Sketodee/TMS.git

   ```bash 
   cd TMS 

   ```bash 
   dotnet run 

# Usage 

## Running the API Locally 

To run the api locally 

### 1. Start the API 

```bash
dotnet run 

The API Endpoints will be accessible at https://localhost:44368/swagger/index.html

Endpoint methods, request body, parameters, headers and schemas are well detailed in the swagger link

Background task was handled using hangfire, dashboard accessible at https://localhost:44368/mydashboard



### 2. Authentication with JWT

Our API uses JSON Web Tokens (JWT) for authentication. JWT is a widely adopted standard for secure authentication and authorization in web applications.

### Obtaining a JWT Token

To access protected endpoints, you need to obtain a JWT token by authenticating with our API using your credentials. Here's how to do it:

1. **Registration**: If you haven't registered yet, create an account on our platform.

2. **Login**: Use your credentials (username and password) to send a POST request to the `/api/auth/login` endpoint to authenticate. The API will respond with a JWT token if the credentials are valid.


### 3. Testing
To test the API:

1. Use Postman or your preferred API testing tool.
2. Send requests to the API endpoints.
3. Verify that the responses match the expected results as described in the API documentation.


