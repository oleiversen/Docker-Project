#!/bin/bash
echo "Startup Initialize MongoDB replica-set"
while :
do

echo "Waiting for all MongoDb servers to become ready before Initialize MongoDB replica-set"
timeout=0
while : ; do
  echo "Trying to connect to mongo1"
  mongo=$(curl -s http://mongo1:28017/serverStatus\?text\=1 2>&1 | grep uptime | head -1)
  echo $mongo | grep -q uptime
  if [ $? -eq 0 ];then
    echo $mongo
    echo "Found mongo1!" 
    break
  fi    
  echo "mongo1 not ready"

  sleep 1s
  timeout=$((timeout + 1))
  if [ "$timeout" -gt 60 ]; then
    echo "Timeout waiting for mongo1"
    break
  fi 
done

timeout=0
while : ; do
  echo "Trying to connect to mongo2"
  mongo=$(curl -s http://mongo2:28017/serverStatus\?text\=1 2>&1 | grep uptime | head -1)
  echo $mongo | grep -q uptime
  if [ $? -eq 0 ];then
    echo $mongo
    echo "Found mongo2!" 
    break
  fi    
  echo "mongo2 not ready"

  sleep 1s
  timeout=$((timeout + 1))
  if [ "$timeout" -gt 60 ]; then
    echo "Timeout waiting for mongo2"
    break
  fi 
done

timeout=0
while : ; do
  echo "Trying to connect to mongo3"
  mongo=$(curl -s http://mongo3:28017/serverStatus\?text\=1 2>&1 | grep uptime | head -1)
  echo $mongo | grep -q uptime
  if [ $? -eq 0 ];then
    echo $mongo
    echo "Found mongo3!" 
    break
  fi    
  echo "mongo3 not ready"

  sleep 1s
  timeout=$((timeout + 1))
  if [ "$timeout" -gt 60 ]; then
    echo "Timeout waiting for mongo3"
    break
  fi 
done

echo "Sending script to Initialize MongoDB replica-set"

echo SETUP.sh time now: `date +"%T" `
mongo --host mongo1:27017 <<EOF
   var cfg = {
        "_id": "rs",
        "version": 1,
        "members": [
            {
                "_id": 0,
                "host": "mongo1:27017",
                "priority": 2
            },
            {
                "_id": 1,
                "host": "mongo2:27017",
                "priority": 1
            },
            {
                "_id": 2,
                "host": "mongo3:27017",
                "priority": 0
            }
        ]
    };
    rs.initiate(cfg, { force: true });
    rs.reconfig(cfg, { force: true });
    db.getMongo().setReadPref('nearest');
EOF

echo "Sleeping 120 sec before reinitialize MongoDB replica-set"
sleep 120s
done