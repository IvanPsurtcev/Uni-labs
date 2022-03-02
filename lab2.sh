#!/bin/sh

k=0 #interaption counter

handl()
{
	k=`expr $k + 1`
	if [ $k -eq 2 ]
	then
		s=0
		for i in $res
		do
			tm=`wc -c $i | awk '{print $1}'`
			s=`expr $s + $tm`
		done
		echo $s
		k=0
	fi
}
trap "handl" 2
res=`find . -type f -atime -1 -daystart -printf "%AT + %p\n" | sort -r | awk '{print $3 "\n"}'`
for i in $res
do
	echo $i
	sleep 1
done
