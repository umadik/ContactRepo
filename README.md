Projenin çalışması için MongoDb ve RabbitMq nun ayağa kaldırılması gerekiyor.

Aşağıdaki kodları kullanarak bunu yapabilirsiniz:

docker run --name mongodb -d -p 27017:27017 mongo

docker run -d --hostname rabbitmq --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management

Ayrıca projeyi çalıştırdğınızda tarayıcıdan aşağıdaki adreslere giderek servisleri ve web projesini kontrol edebilirsiniz.

Contact Servis > http://localhost:5011/swagger/index.html

Report Servis  > http://localhost:5012/swagger/index.html

Web uygulaması > http://localhost:5015/
