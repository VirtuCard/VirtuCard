#!/bin/bash

#Bash File that tests whether the RoomCode is of the
# appropriate format. The RoomCode should be 6 characters
# long and should only contain letters.

ROOMCODE=$( cat RoomCode.txt )

#Number Check
if egrep -q [0-9] RoomCode.txt ; then
	echo "TEST FAILED: The Room Code contains a number."
	echo "ROOM CODE IN FILE: $ROOMCODE"
	exit 0;
fi

#Special Character Check
if egrep -q [#$\+%@?] RoomCode.txt ; then
	echo "TEST FAILED: The Room Code contains a special character such as #, $, etc."
	echo "ROOM CODE IN FILE: $ROOMCODE"
	exit 0;
fi

#String Length Check
STRING_LEN=${#ROOMCODE}

if [ $STRING_LEN -gt 6 ]; then
	echo "TEST FAILED: The Room Code is longer than 6 characters long."
	echo "ACTUAL STRING LENGTH: $STRING_LEN"
	exit 0;
fi

if [ $STRING_LEN -lt 6 ]; then
	echo "TEST FAILED: The Room Code is shorter than 6 characters long."
	echo "ACTUAL STRING LENGTH: $STRING_LEN"
	exit 0;
fi

echo "TEST PASSED: $ROOMCODE"

