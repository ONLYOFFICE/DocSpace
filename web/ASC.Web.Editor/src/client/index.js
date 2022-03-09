import React from "react";
import { hydrate } from "react-dom";
import { registerSW } from "@appserver/common/sw/helper";
import App from "../App.js";

const propsObj = window.__ASC_INITIAL_STATE__;
delete window.__ASC_INITIAL_STATE__;

const stateJS = document.getElementById("__ASC_INITIAL_STATE__");
stateJS.parentNode.removeChild(stateJS);

const { props } = propsObj;

hydrate(<App {...props} />, document.getElementById("root"));

registerSW();
