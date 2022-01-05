cd src
docker build -t synosscamera-api -f synosscamera.api/Dockerfile .

docker tag synosscamera-api:latest kcomelli/synosscamera-api

docker push kcomelli/synosscamera-api:latest

cd ..