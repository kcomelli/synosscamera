cd src
docker build -t shcome-api -f shcome.api/Dockerfile .

docker tag shcome-api:latest kcomelli/shcome-api

docker push kcomelli/shcome-api:latest

cd ..