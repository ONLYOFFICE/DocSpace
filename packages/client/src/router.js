import React from "react";
import { createBrowserRouter } from "react-router-dom";

import routes from "./routes";
import Error404 from "SRC_DIR/pages/Errors/404";

import Root from "./Shell";

const router = createBrowserRouter([
  {
    path: "/",
    element: <Root />,
    errorElement: <Error404 />,
    children: [...routes],
  },
]);

export default router;
