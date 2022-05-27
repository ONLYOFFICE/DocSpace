import App from "./App";
import React from "react";
import ReactDOM from "react-dom";
import { registerSW } from "@appserver/common/sw/helper";

ReactDOM.render(<App />, document.getElementById("root"));

registerSW();
