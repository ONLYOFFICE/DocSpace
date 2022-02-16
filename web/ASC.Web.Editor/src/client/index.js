import React from "react";
import { registerSW } from "@appserver/common/sw/helper";
import App from "../App.js";

const propsObj = window.__STATE__;
delete window.__STATE__;

const { props } = propsObj;

React.hydrate(<App {...props} />, document.getElementById("root"));

registerSW();
