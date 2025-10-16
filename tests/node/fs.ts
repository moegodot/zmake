
import * as fs from "node:fs";

fs.writeFileSync("test.txt", "a test text file");
const read = fs.readFileSync("test.txt", "utf-8");

