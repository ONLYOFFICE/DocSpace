import React from "react";
import { hydrate } from "react-dom";
import { registerSW } from "@appserver/common/sw/helper";
import App from "../App.js";

const propsObj = window.__STATE__;
delete window.__STATE__;

const { props } = propsObj;

hydrate(<App {...props} />, document.getElementById("root"));

registerSW();
