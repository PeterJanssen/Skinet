# Skinet

## Description

A proof of concept e-commerce store using Angular, .Net Core and Stripe for payment processing

### What was used in this app?

#### Backend
- .Net Core 5
- Entity Framework
- Identity
- JWT
- Swagger
- C# Generics
- Repository and Unit of Work Pattern
- Sorting, Filtering, Searching and Pagination
- Validation
- Specification Pattern
- Caching with Redis
- Accepting payments using Stripe

#### Frontend
- Angular 11
- JWT
- SCSS
- Caching with Redis
- Sorting, Filtering, Searching and Pagination
- Angular Lazy loading
- Angular Routing
- Angular Reactive Forms
- Angular Creating a MultiStep form wizard
- Accepting payments using Stripe
- Angular Re-usable form components
- Angular validation and async validation

## Visuals

### HomePage

<img src="ReadMeImages/Home.png?raw=true" width="350" alt="Homepage"/>

### Shop

<img src="ReadMeImages/Shop.png?raw=true" width="350" alt="Shop"/>

### Basket

<img src="ReadMeImages/Basket.png?raw=true" width="350" alt="Basket"/>

### Checkout

<img src="ReadMeImages/Checkout.png?raw=true" width="350" alt="Checkout"/>

### Orders

<img src="ReadMeImages/Orders.png?raw=true" width="350" alt="Orders"/>

### Login

<img src="ReadMeImages/Login.png?raw=true" width="350" alt="Login"/>

### Register

<img src="ReadMeImages/Register.png?raw=true" width="350" alt="Register"/>

### Usage

## What is needed to run or improve this app?

(version numbers are what was used during development)

- Any IDE e.g. VSCode/Visual Studio/etc.

- .NET 5.0.103

- Angular 11.2.4

- Node >= 10.13.0

- npm ^6.11.0 || ^7.5.6

- Angular CLI 11.2.4

- Docker Desktop

- Stripe CLI 1.5.11

- A Stripe account with the WebhookSecret, key and secret placed in appsettings.json

- A server certification and key (self signed or by a CA) and placed in client/ssl

- Postman or any equivalant

## What commands are used to run this app?

### Creating the docker containers

In the root folder run the following command

`docker-compose up --detach`

Creates redis and redis database container for caching purposes

Creates adminer and redis commander container for viewing the databases

Login and passwords are defined in docker-compose.yml

### Creating and seeding the databases

In the API folder run the following command

`dotnet watch run`

Creates and/or seeds the databases if they are non-existent

### Adding migration

In any folder run the following command, replace the text between #

#### For the skinet database containing store information:

`dotnet ef migrations add "#Name of migration#" -p .\Infrastructure\ -s .\API\ -c StoreContext -o Data/Migrations`

#### For the identity database containing account information:

`dotnet ef migrations add "#Name of migration#" -p .\Infrastructure\ -s .\API\ -c AppIdentityDbContext -o Identity/Migrations`

### Running the backend

In the API folder run the following command

`dotnet watch run`

### Running the frontend

In the client folder run the following command

`ng serve`

### Building the frontend to the wwwroot folder

In the client folder run the following command

`ng build --prod`

### Publishing the app

In the root folder run the following command

`dotnet publish -c Release -o publish skinet.sln`

### Listening to the Stripe CLI webhook

In any terminal run the following command

`stripe listen -f https://localhost:5001/api/payments/webhook -e payment_intent.succeeded,payment_intent.payment_failed`

## Roadmap

### Frontend

- Making the frontend responsive

- Code refactoring

### Backend

- Code refactoring

### Front/Backend

- Order filtering

- Order caching

- Order Pagination

- Inventory system

- Email Service

- Password forgetting

- Password resetting

- Product Review system

## Contributions

Many thanks to Neil Cummings for his course on .Net Core and Angular. This poc was created with the help of his course and further additions have been made by me.

- His [website](https://trycatchlearn.com/)
- His courses on [Udemy](https://www.udemy.com/user/neil-cummings-2/)
