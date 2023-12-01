
# Team 15

## Building the project

```sh
$ cd NACTAM
$ dotnet build
```

`npm install` and `npm run build` are called automatically

## Running the project

```sh
$ cd NACTAM
$ dotnet run
$ # or
$ dotnet watch # for automatic reloading
```

## Changing the database schema

```sh
$ cd NACTAM
$ dotnet ef migrations add <descriptive name>
$ dotnet ef database update
```

In case you want to reset your database, you can just call the latter.

## Formatting

```sh
$ dotnet format
```

## Unit Testing

## UI Testing

## Generating Documentation

```sh
$ cd NACTAM
$ docfx metadata docfx_project/docfx.json
$ docfx build docfx_project/docfx.json --serve
```

and then open `localhost:8080`



