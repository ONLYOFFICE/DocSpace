import React from "react";
import { hydrate } from "react-dom";
import { registerSW } from "@appserver/common/sw/helper";
import App from "../App.js";

const propsObj = window.__STATE__;
delete window.__STATE__;

const stateJS = document.getElementById("__STATE__");
//stateJS.parentNode.removeChild(stateJS);

const { props } = propsObj;
console.log(props);
hydrate(<App {...props} />, document.getElementById("root"));

registerSW();
