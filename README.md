# Practice

## Links
1. [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html) (Gateway)
2. [http://localhost:5001/swagger/index.html](http://localhost:5001/swagger/index.html) (Authentication)
3. [http://localhost:5002/swagger/index.html](http://localhost:5002/swagger/index.html) (Catalog node 1)
4. [http://localhost:5003/swagger/index.html](http://localhost:5003/swagger/index.html) (Catalog node 2)
5. [http://localhost:8080](http://localhost:8080) (Nginx)

## Run instraction 
```
docker-compose up --build -d
docker-compose down
```

## Todo
- [x] Add rate limitter.
- [x] Add Load balancer using Nginx.
- [ ] Central logger.
- [ ] Use Grafana and Loki to view and query logs.
