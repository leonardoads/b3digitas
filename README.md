# b3digitas

O projeto se organiza da seguinte forma:
Api -> é o entrypoint da aplicação, onde fica  o registro dos serviços, DI, e gerencia a comunição web através de uma minimal api 
Application -> é o nucleo da aplicação onde fica todas as regras de negocio
Infrastrcture -> isola a utilização de serviços externos
Domain -> objetos de dominios utilizados por todas as camadas

Pra executar o projeto é necessario uma instancia do MongoDB, uma forma facil de obter é instanlando via container podman

```
$ podman run --detach --name todoDB -p 3000:27017 docker.io/mongodb/mongodb-community-server:latest
```
ou docker 
```
$ docker run --detach --name todoDB -p 3000:27017 docker.io/mongodb/mongodb-community-server:latest
```
