import React from "react";
import ReactDOM from "react-dom";
import App from "./App";
//import { registerSW } from "@docspace/common/sw/helper";
const root = document.getElementById("root");

if (root) ReactDOM.render(<App />, root);

//registerSW();
