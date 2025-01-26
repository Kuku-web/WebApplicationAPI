
# WebApplicationAPI

How to run?
Open in this project in visual studio, build and run.   
The application includes Swagger UI for API testing.



What can be added/Improved?
1- Current implementation lacks proper query string validation for the endpoints.
2- The current unit tests cover some scenarios, but improvements are needed to cover edge cases.
3- The current rate limit is configured with a default value in appsettings.json.
      This limit needs to be fine-tuned based on:
        - Business Requirements: How many requests should be allowed per user or globally?
        - Third-party API Constraints: How many requests can the Frankfurter API handle without throttling
