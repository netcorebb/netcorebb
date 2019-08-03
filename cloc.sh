#!/bin/bash

cloc --exclude-dir=$(tr '\n' ',' < .clocignore) .
