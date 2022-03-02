#!/bin/bash

count=1
IFS=$'\n'

siginthand()
{
  count=`expr $count % 2`

  if [ $count == 0 ]
  then
    echo "2 ctrl+c"
    echo "Files:"
    for i in $a; do
      if [ -n "`find . -daystart -atime -1 -name "$i"`" ]
      then
        wc -c "$i" 2> /dev/null
      fi
    done
  else
    echo "$count ctrl+c"
  fi

  let count++
}
trap siginthand SIGINT

for line in `ls --time atime -au1`; do
  if [ -n "$a" ]; then
    a+=$'\n'
  fi
  a+="$line"
  sleep 10
done

child(){
  while read line; do
    if [ -n "`find . -daystart -atime -1 -name "$line"`" ]; then
      echo "$line"
    fi
  done
}

echo "$a" | child
