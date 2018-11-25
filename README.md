# Task-basedAsynchronousPattern
Task-based Asynchronous Pattern DotNetCore,  InMemory DB, Http Communication, Unit testing, integration testing ..

Task-based Asynchronous Pattern (TAP), which uses a single method to represent the initiation and completion of an asynchronous operation. 
TAP was introduced in the .NET Framework 4. It's the recommended approach to asynchronous programming in .NET.
https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/

# Bussiness requirements, rules and workflow
* An ASP.NET Core, single-page application (with front-end framework of choice) with Web API back-ends to manage the Food Orders 
* The requirements for this system are below
* It is about Async services but for simple work flow will use restaurant analogy
* Waiter enters orders which include dishes and every dish has ingredients which requires different cooking times
* Waiter could add more than one order which shouldn’t block the other orders
* Each order is tracked via ID
* Order and dishes status could be tracked anytime as well as total order progress
* For simplicity we will use numerical representation for dish readiness and thread sleep for cooking time 
### Web App
 *	The application will require the Waiter to enter two numbers between 1 and 10
 **	The first number, X, is how many numbers of dishes should be processed
 **	The second, Y, is how many ingredients will be processed per dish
*	A start button will be available to click once the input is ready
*	Once started, the application will trigger back-end work
*	A grid of the dishes, their remaining numbers of ingredients to process, and their current totals should be displayed
*	The grid should update every 2 seconds
*	Total order status should be displayed
*	Once all dishes are processed, the user can start another order, clearing previous results
### Web API
*	An endpoint will exist to start processing X dishes with Y ingredients per dishes managed by a kitchen service
*	The kitchen will manage the backend work flow
*	The DishManager will request generated numbers from a separate service(generate number here is simulation for ingredients orders)
*	The CookingManager will be requested from that same service
*	For each dish, the kitchen service will ask the DishManager to request X new ingredients
**	When the DishManager receives a number, it will trigger the kitchen service identifying the dish and number
* For each ingredients generated number, the kitchen service will ask the CookingManager to cook and process the ingredient simulated here by multiply the number
*	When the CookingManager receives a multiplied number, it will trigger the kitchen service identifying the ingredient and number
*	The kitchen service will take each status for a ingredients and aggregate them as received
*	An endpoint will exist for retrieving the current processing state
*	An endpoint will exist to relay a generated number for a given ingredient to the kitchen service
*	An endpoint will exist to relay a multiplied number for a given batch to the kitchen service
### Secondary Web API 
*	An endpoint will exist to start ingredient orders (simulated by numbers generation) for a given dish managed by a DishManager
*	The DishManager will generate Y random integers between 1 and 100
**	For each number, a random delay of 5 to 10 seconds should be used to simulate work
** The generated number should be returned to the web application via its endpoint
*	An endpoint will exist to start cooking(simulated by multiplying number for a given ingredient) managed by a CookingManager
*	The CookingManager will multiply a number by 2, 3, or 4 (chosen at random)
**	A random delay of 5 to 10 seconds should be used to simulate work
** The generated number should be returned to the web application via its endpoint


# TODO:
1. ~~Add project bussiness rules~~
2. Replace UI with correct FW
3. Seprate DB models vs controllers models
4. Replace commiunication channel with interfaces and ammend dev enviroment
5. Add requests filters and expection handelers 
