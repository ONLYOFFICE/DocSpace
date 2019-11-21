import React, { Suspense, lazy } from "react";
import { Router, Route, Switch } from "react-router-dom";
import { Loader } from "asc-web-components";
import StudioLayout from "./components/Layout/index";
import Login from "./components/pages/Login";
import PublicRoute from "./helpers/publicRoute";
import { Error404 } from "./components/pages/Error";
import { history, PrivateRoute } from "asc-web-common";

const Home = lazy(() => import("./components/pages/Home"));
const About = lazy(() => import("./components/pages/About"));
const Confirm = lazy(() => import("./components/pages/Confirm"));
const Settings = lazy(() => import("./components/pages/Settings"));

const App = () => {
  return (
    <Router history={history}>
      <StudioLayout>
        <Suspense
          fallback={<Loader className="pageLoader" type="rombs" size={40} />}
        >
          <Switch>
            <PublicRoute exact path={["/login","/login/error=:error", "/login/confirmed-email=:confirmedEmail"]} component={Login} />
            <Route path="/confirm" component={Confirm} />
            <PrivateRoute exact path={["/","/error=:error"]} component={Home} />
            <PrivateRoute exact path="/about" component={About} />
            <PrivateRoute restricted path="/settings" component={Settings} />
            <PrivateRoute component={Error404} />
          </Switch>
        </Suspense>
      </StudioLayout>
    </Router>
  );
};

export default App;
