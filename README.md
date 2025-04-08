**Description:**  This is a full-stack web application that allows university employees to submit reimbursement requests by providing purchase details and uploading receipt files. It includes a .NET 6 backend API with MySQL for data storage and an Angular frontend for form submission and viewing past submissions. The system validates input data, handles file uploads, and stores receipt files securely, enabling a streamlined and user-friendly reimbursement process.


**How to run the app:** : 


**Backend** :
1  )  Clone the backend repo and open the solution in Visual Studio.

   
2  )  Update the MySQL password in appsettings.json:
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=reimbursementsdb;User=root;Password=YOUR_MYSQL_PASSWORD;"
}


3  )  Set up the database:


CREATE DATABASE reimbursementsdb;
Use reimbursementsdb;


4  )  Import the Schema


 The schema.sql file is located in the backend project directory, near the Program.cs file.



Open a new SQL tab


Go to File -> Open SQL Script -> Select schema.sql and run it


5  )  **Launch your project in Visual Studio using the correct launch profile (preferably https).**

   
If you're using IIS Express to run the backend in Visual Studio, the port may be different from 7125. Always make sure the apiUrl in the Angular frontend (reimbursement.service.ts) matches the actual port the backend is using — you can check this in launchSettings.json or the browser when the API runs.


To avoid inconsistencies and ensure stable communication between frontend and backend:

✅ Prefer running the backend using https with Kestrel (by selecting the project profile instead of IIS Express in Visual Studio).
✅ This keeps the port consistent (e.g., https://localhost:7125) and helps you avoid API failures and CORS issues due to mismatched URLs.


Swagger should now load and your endpoints should work without needing to run migrations.


**FrontEnd:**


1  )  Navigate to the frontend folder  (university-reimbursement-ui)


2  )  Install Dependencies 
Run npm install


3  )  Check the backend API URL in reimbursement.service.ts:
private apiUrl = 'https://localhost:7125/api/reimbursements';


This should match your backend HTTPS port.  If you're running the backend with IIS Express, the port may differ . 
You can find the correct port by checking launchSettings.json or Swagger UI URL when backend starts.


4  )  ng serve 








