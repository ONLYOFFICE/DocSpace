import React, { Suspense, lazy } from "react";
import { Router, Route, Switch } from "react-router-dom";
import { Loader } from "asc-web-components";
import {
  history,
  PrivateRoute,
  PublicRoute,
  Login,
  Error404,
  StudioLayout,
  Offline,
  ComingSoon,
} from "asc-web-common";

import Home from "./components/pages/Home";

const About = lazy(() => import("./components/pages/About"));
const Confirm = lazy(() => import("./components/pages/Confirm"));
const Settings = lazy(() => import("./components/pages/Settings"));
const Wizard = lazy(() => import("./components/pages/Wizard"));
const Payments = lazy(() => import("./components/pages/Payments"));
const App = () => {
  return navigator.onLine ? (
    <Router history={history}>
      <StudioLayout>
        <Suspense
          fallback={<Loader className="pageLoader" type="rombs" size="40px" />}
        >
          <Switch>
            <Route exact path="/wizard" component={Wizard} />
            <PublicRoute
              exact
              path={[
                "/login",
                "/login/error=:error",
                "/login/confirmed-email=:confirmedEmail",
              ]}
              component={Login}
            />
            <Route path="/confirm" component={Confirm} />
            <PrivateRoute
              exact
              path={["/", "/error=:error"]}
              component={Home}
            />
            <PrivateRoute exact path="/about" component={About} />
            <PrivateRoute restricted path="/settings" component={Settings} />
            <PrivateRoute
              exact
              path={["/coming-soon"]}
              component={ComingSoon}
            />
            <PrivateRoute path="/payments" component={Payments} />
            <PrivateRoute component={Error404} />
          </Switch>
        </Suspense>
      </StudioLayout>
    </Router>
  ) : (
    <Offline />
  );
};

export default App;
