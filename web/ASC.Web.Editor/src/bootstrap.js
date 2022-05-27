import React from "react";
import ReactDOM from "react-dom";
import App from "./App";
import { registerSW } from "@appserver/common/sw/helper";

ReactDOM.render(<App />, document.getElementById("root"));

registerSW();
