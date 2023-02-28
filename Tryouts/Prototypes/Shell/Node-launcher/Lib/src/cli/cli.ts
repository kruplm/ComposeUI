﻿#!/usr/bin/env node

"use strict";

import { executeScriptFile } from './executeScrtiptFile.js';

let fileName = process.argv[2];

try {
    executeScriptFile(fileName);
} catch (error) {
    console.error(error);
}