
# CreateUser Rabbitmq

Repostory for teste some definitions with rabbitmq using minimal api

queue and exchange with binds, deadletter and rpc





## Installation

Navigate to ./ and run

```bash
  docker compose --file docker-compose.uat.yml up
```
use the script.sql into ./ for create a new database in mysql container

```
./excuteme.sql
```
## API Documentation

#### Single endpoint in http://localhost:8080

```
  POST /api/v1/user
```
| Parâmetro   | Tipo       | Descrição                           |
| :---------- | :--------- | :---------------------------------- |
| `{"name": "rodolfo","email": "rodolfo@gmail.com","document": "12345678995","phone": "9999598998"}` | `string` | um usuário qualquer |

#### return http code
```
  201
  400
  400
```

