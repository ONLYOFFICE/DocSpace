#!/usr/bin/env node

import inquirer from "inquirer";
import * as fs from "fs";
import * as path from "path";
import * as cp from "child_process";
import { fileURLToPath } from "url";

import createTemplate from "./createTemplate.js";

const __filename = fileURLToPath(import.meta.url);

const __dirname = path.dirname(__filename);

const CURR_DIR = process.cwd();
const TEMPLATES_PATH = path.join(__dirname, "../templates");

const CHOICES = [
  "default plugin",
  "context plugin",
  "main button plugin",
  "profile menu plugin",
];

const QUESTIONS = [
  {
    name: "plugin-type",
    type: "list",
    message: "What plugin template would you like to generate?",
    choices: CHOICES,
  },
  {
    name: "plugin-name",
    type: "input",
    message: "Plugin name:",
    validate: function (input) {
      if (/^([A-Za-z\_\-])+$/.test(input)) return true;
      else return "Plugin name may only include letters.";
    },
  },
];

inquirer.prompt(QUESTIONS).then((answers) => {
  const pluginType = answers["plugin-type"];
  const name = answers["plugin-name"];

  const splitName = name.replaceAll("-", "").replaceAll("_", "").split("");

  splitName[0] = splitName[0].toUpperCase();

  const pluginName = splitName.join("");

  let template = null;

  switch (pluginType) {
    case CHOICES[0]:
      template = "default";
      break;
    case CHOICES[1]:
      template = "context";
      break;
    case CHOICES[2]:
      template = "main_button";
      break;
    case CHOICES[3]:
      template = "profile_menu";
      break;
    default:
      template = "default";
  }

  const templatePath = `${TEMPLATES_PATH}/${template}`;

  fs.mkdirSync(`${CURR_DIR}/${name}`);

  console.log(`Cloning ${template} plugin template`);

  createTemplate(templatePath, name, pluginName).then(() => {
    console.log("Installing dependencies...");
    process.chdir(name);
    cp.exec(`yarn`);
  });
});
