
# Job Notes App

Job Notes App is a WPF (Windows Presentation Foundation) application designed to manage job records efficiently. It allows users to add, view, search, and delete job entries, track job completion, and analyze job statistics over time.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Screenshots](#screenshots)
- [Architecture](#architecture)
- [Installation](#installation)
- [Usage](#usage)
- [Technologies Used](#technologies-used)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Contributing](#contributing)
- [License](#license)

## Overview

The Job Notes App is a desktop application that interacts with a backend REST API to manage job-related data. Users can perform operations like adding new jobs, updating existing jobs, marking jobs as completed, and analyzing job trends over months and years.

## Features

- **User Authentication**: Secure login using JWT (JSON Web Token) authentication.
- **Add/Edit/Delete Jobs**: Easily manage job entries, including setting job completion status.
- **Search and Filter**: Search for jobs based on location, client name, or notes.
- **Automatic Token Refresh**: Keeps the user logged in by refreshing the JWT token periodically.
- **Statistics and Analysis**: View job statistics like total jobs, best year, best month, and average jobs per year.
- **Responsive UI**: User-friendly interface with dynamic updates based on user interactions.

## Screenshots

### Main Window

![Main Window](C:\Users\matth\OneDrive\Desktop\MainWindow.png)

### Job List Window

![Job List Window](C:\Users\matth\OneDrive\Desktop\JobListWindow.png)

*Note: Replace the paths with the actual paths to the screenshots when you upload them.*

## Architecture

The application follows the MVVM (Model-View-ViewModel) architecture pattern, ensuring a clear separation of concerns and enhancing maintainability.

- **Model**: Represents the application's data. In this project, `Job` and `User` are primary models.
- **View**: The UI elements defined in XAML files, such as `MainWindow.xaml` and `JobListWindow.xaml`.
- **ViewModel**: Handles the data logic and interactions between the Model and the View. `MainViewModel` and `JobListViewModel` are the main view models used.

The application also utilizes services for handling API calls and authentication:

- `AuthenticationService`: Manages user authentication and token refreshing.
- `JobService`: Handles CRUD operations for job entries.

## Installation

To set up the project locally, follow these steps:

1. **Clone the repository**:
   ```bash
   git clone https://github.com/yourusername/job-notes-app.git
   ```
2. **Navigate to the project directory**:
   ```bash
   cd job-notes-app
   ```
3. **Install dependencies** (if applicable, e.g., using NuGet packages):
   - Ensure all required packages are restored. Visual Studio should handle this automatically on project load.

4. **Set up the environment variables**:
   - Create a `.env` file in the root directory with the following content:
     ```
     USERNAME=your_username
     PASSWORD=your_password
     ```

5. **Start the backend server**:
   - Follow the backend project README instructions to set up and run the backend server.

6. **Run the application**:
   - Open the solution in Visual Studio and run the project using `F5` or by selecting `Debug > Start Debugging`.

## Usage

1. **Login**: Start the application, and it will automatically log in using the credentials specified in the `.env` file.
2. **View Jobs**: The main window displays a list of jobs. You can filter jobs using the search bar or navigate between months and years.
3. **Add/Edit/Delete Jobs**: Use the buttons to add new jobs or delete existing ones. Mark jobs as completed by checking the corresponding checkbox.
4. **View Statistics**: Switch to the Job List Window to view job statistics, including total jobs, best year, best month, and average jobs per year.

## Technologies Used

- **C# and .NET**: The core programming language and framework.
- **WPF (Windows Presentation Foundation)**: For building the desktop application's user interface.
- **MVVM (Model-View-ViewModel)**: Architectural pattern for separation of concerns.
- **Entity Framework**: Used for data access and ORM (if applicable).
- **ASP.NET Core**: Backend framework for the REST API (if part of the project).
- **JWT (JSON Web Tokens)**: For secure user authentication.
- **Docker**: Containerization for backend services and MSSQL database.

## Configuration

- **Backend API Base URL**: Modify the `apiBaseUrl` in `App.xaml.cs` to point to your backend API.
- **Environment Variables**: Store sensitive data like usernames and passwords in the `.env` file to avoid hardcoding them in the codebase.

## API Documentation

The Job Notes App interacts with a backend REST API. Below is a brief overview of the endpoints used:

- `POST /api/auth/login`: Authenticates a user and returns a JWT token.
- `GET /api/jobs`: Retrieves a list of all jobs.
- `GET /api/jobs/{id}`: Retrieves a job by its ID.
- `POST /api/jobs`: Creates a new job.
- `PUT /api/jobs/{id}`: Updates an existing job.
- `DELETE /api/jobs/{id}`: Deletes a job by its ID.
- `GET /api/jobs/search`: Searches jobs based on query parameters like location and client name.

*For detailed API usage, refer to the backend API documentation.*

