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
- Profiling with MiniProfiler at `https://localhost:5001/mini-profiler-resources/results-index`
- Logging events and HTTP requests in console and to PostGreSQL database with Serilog
- HealthCheck with UI available at `https://localhost:5001/healthchecks-ui#/healthchecks`
- API Versioning

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
- Ngx-Gallery, Spinner, Toastr, Dropzone, Image-Cropper, Hamburgers, Navbar
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

### Admin

![Dashboard](ReadMeImages/AdminProductDash.PNG 'Admin Dash')
![CreateProduct](ReadMeImages/AdminProductCreate1.PNG 'Create New Product')
![AddImageToProduct](ReadMeImages/AdminProductCreate2.PNG 'Add Image to Product')
![EditProduct](ReadMeImages/AdminProductEdit1.PNG 'Edit Existing Product')

### MiniProfiler

![MiniProfiler](ReadMeImages/MiniProfiler.PNG 'MiniProfiler')

### HealthCheck

![HealthCheck](ReadMeImages/HealthCheck.PNG 'HealthCheck')

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

- Postman or any equivalent, API contains SwaggerUI at `https://localhost:5001/swagger/index.html`

## First steps before running the app for the first time

1) "npm install" in the client folder
2) "dotnet restore" in root folder
3) create appsettings.json in API folder and copy content of appsettings.Development.json into it, add or change keys for Stripe and SendGrid
4) "docker-compose up --detach" in the root folder
5) add server.crt and server.key (self signed or by a CA) and place them in client/ssl

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

`dotnet ef migrations add "#Name of migration#" -p .\Infrastructure\ -s .\API\ -c StoreContext -o Data/Migrations`

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

In any terminal run the following command, Stripe CLI is needed

`stripe listen -f https://localhost:5001/api/payments/webhook -e payment_intent.succeeded,payment_intent.payment_failed`

## Roadmap

### Frontend

- Making the frontend responsive

### Backend

- Refactor code for orders

- Refactor product controller

- Improve HealthCheck implementation

### Front/Backend

- Order filtering by ordered items or date

- Order caching

- Order email

- Email confirmation

- Password forget feature

- Password reset feature

- Product review system

- Testing

## Roadmap_Done

### Frontend

- Migrated to ESLint and fixed Lint Errors

- Improve Home page

- Improve Nav Bar

- Moving inline style to stylesheets

### Backend

- SwaggerUI Documentation

- Moved ProductRepo calls to ProductService

- Added Profiler (MiniProfiler)

- Logging events and HTTP requests in console and to PostGreSQL database with Serilog

- HealthCheck implementation 

- API Versioning

### Front/Backend

- Order filtering by status

- Order sorting by date or price

- Reset order filters

- Order pagination

- Inventory system

- Product picture feature

- Product add review feature

## Current_Bugs

## Fixed_Bugs

### Shop

- When going to the next page of products in the shop, the caching is gone of the detailed productpage of the previous paginated products

### Orders

- Photo of products are not displayed in order detail page because they are not included in call for the ordered product in database so no pictureURL is inserted into database
- Id property of OrderItemDto in backend was not the same as IOrder ProductId property so the route to the product page could not be resolved and redirected to shop page, fixed by refactoring 

### Basket-summary

- When not logged in the user is not redirected to the login page when clicking on the checkout button

### SwaggerUI

- POST /api/orders bug in SwaggerUI locks the browser if endpoint is opened in SwaggerUI because of the wrong return type

## Contributions

Many thanks to Neil Cummings for his course on .Net Core and Angular. This poc was created with the help of his course and further additions have been made by me.

- Neil's [website](https://trycatchlearn.com/)
- Neil's courses on [Udemy](https://www.udemy.com/user/neil-cummings-2/)

Thanks to Ostranme for the material theme for SwaggerUI

- Ostranme's [GitHub Repo](https://github.com/ostranme/swagger-ui-themes)
