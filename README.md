# Trading Futures
A simple system for convenient trading (Futures, Huobi).
The system is fully functional and successfully used in the production environment.

## Architecture

The system is built using a microservices-oriented architecture with multiple autonomous microservices. Each microservice is dedicated to a specific task. Microservices communicate with each other using asynchronous messages over the event bus (RabbitMQ). All synchronous communications are done via gRPC. Envoy is used as API Gateway to shield all microservices and it allows access to only specific microservices (Web API).
