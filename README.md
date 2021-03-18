# Skinet

## Table of Contents

**[Description](#Description)**<br>
**[Features](#Features)**<br>
**[Visuals](#Visuals)**<br>
**[Usage](#Usage)**<br>
**[Commands](#Commands)**<br>
**[Roadmap](#Roadmap)**<br>
**[Roadmap_Done](#Roadmap_Done)**<br>
**[Current_Bugs](#Current_Bugs)**<br>
**[Fixed_Bugs](#Fixed_Bugs)**<br>
**[Contributions](#Contributions)**<br>

## Description

A proof of concept e-commerce store using Angular, .Net Core and Stripe for payment processing

### Features

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

![Homepage](ReadMeImages/Home.PNG 'Homepage')

### Shop

![Shop](ReadMeImages/Shop.PNG 'Shop')

### Basket

![Basket](ReadMeImages/Basket.PNG 'Basket')

### Checkout

![Checkout](ReadMeImages/Checkout.PNG 'Checkout')

### Orders

![Orders](ReadMeImages/Orders.PNG 'Orders')

### Login

![Login](ReadMeImages/Login.PNG 'Login')

### Register

![Register](ReadMeImages/Register.PNG 'Register')

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

## Commands

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

- Moving inline style to stylesheets

- Improve Home page

### Backend

- Refactor code for orders

### Front/Backend

- Order filtering by ordered items or date

- Order caching

- Order email

- Inventory system

- Email confirmation

- Password forget feature

- Password reset feature

- Product review system

- Code refactoring

- Testing

## Roadmap_Done

### Frontend

- Migrated to ESLint and fixed Lint Errors

### Backend

- SwaggerUI Documentation

- Moved ProductRepo calls to ProductService

### Front/Backend

- Order filtering by status

- Order sorting by date or price

- Reset order filters

- Order pagination

## Current_Bugs

### SwaggerUI

- /api/orders bug in SwaggerUI locks the browser if endpoint is opened in SwaggerUI probably because the example value is too big

## Fixed_Bugs

### Shop

- When going to the next page of products in the shop, the caching is gone of the detailed productpage of the previous paginated products

## Contributions

Many thanks to Neil Cummings for his course on .Net Core and Angular. This poc was created with the help of his course and further additions have been made by me.

- His [website](https://trycatchlearn.com/)
- His courses on [Udemy](https://www.udemy.com/user/neil-cummings-2/)

Thanks to Ostranme for the material theme for SwaggerUI

- His [GitHub Repo](https://github.com/ostranme/swagger-ui-themes)
