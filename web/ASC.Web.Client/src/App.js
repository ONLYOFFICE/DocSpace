import React, { Suspense, lazy } from "react";
import { Router, Route, Switch } from "react-router-dom";
import { Loader } from "asc-web-components";
import { history, PrivateRoute, PublicRoute, Login, Error404, StudioLayout, Offline} from "asc-web-common";
import About from "./components/pages/About";

const Home = lazy(() => import("./components/pages/Home"));
const Confirm = lazy(() => import("./components/pages/Confirm"));
const Settings = lazy(() => import("./components/pages/Settings"));

const App = () => {
  return (
    navigator.onLine ? 
    <Router history={history}>
      <StudioLayout>
        <Suspense
          fallback={<Loader className="pageLoader" type="rombs" size='40px' />}
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
    </Router> :
    <Offline/>
  );
};

export default App;
