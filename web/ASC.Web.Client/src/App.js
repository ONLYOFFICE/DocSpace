import React, { Suspense, lazy } from "react";
import { BrowserRouter, Route, Switch } from "react-router-dom";
import { Loader } from "asc-web-components";
import StudioLayout from "./components/Layout/index";
import Login from "./components/pages/Login";
import { PrivateRoute } from "./helpers/privateRoute";
import { PublicRoute } from "./helpers/publicRoute";
import { Error404 } from "./components/pages/Error";

const Home = lazy(() => import("./components/pages/Home"));
const About = lazy(() => import("./components/pages/About"));
const Confirm = lazy(() => import("./components/pages/Confirm"));

const App = () => {
  return (
    <BrowserRouter>
      <StudioLayout>
        <Suspense
          fallback={<Loader className="pageLoader" type="rombs" size={40} />}
        >
          <Switch>
            <PublicRoute exact path={["/login","/login/:error"]} component={Login} />
            <PublicRoute path="/confirm" component={Confirm} />
            <PrivateRoute exact path="/" component={Home} />
            <PrivateRoute exact path="/about" component={About} />
            <PrivateRoute component={Error404} />
          </Switch>
        </Suspense>
      </StudioLayout>
    </BrowserRouter>
  );
};

export default App;
