import React from "react";
import { createBrowserRouter } from "react-router-dom";

import routes from "./routes";
import { NotFoundError } from "./routes/utils";

import Root from "./Shell";

const router = createBrowserRouter([
  {
    path: "/",
    element: <Root />,
    errorElement: <NotFoundError />,
    children: [...routes],
  },
]);

export default router;
