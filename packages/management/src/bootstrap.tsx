import React from "react";
import { createRoot } from "react-dom/client";
import Client from "./Client";
//import { registerSW } from "@docspace/common/sw/helper";

const container = document.getElementById("root");
const root = createRoot(container);

if (root) root.render(<Client />);

//registerSW();
