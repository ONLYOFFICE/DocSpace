import React, { Suspense } from "react";
import { connect } from "react-redux";
import { Router, Switch, Redirect } from "react-router-dom";
import { Loader } from "asc-web-components";
import Home from "./components/pages/Home";
import { history, PrivateRoute, PublicRoute, Login, Error404, StudioLayout, Offline } from "asc-web-common";

const App = ({ settings }) => {
  const { homepage } = settings;
  return (
    navigator.onLine ? 
    <Router history={history}>
      <StudioLayout>
        <Suspense
          fallback={<Loader className="pageLoader" type="rombs" size='40px' />}
        >
          <Switch>
            <Redirect exact from="/" to={`${homepage}`} />
            <PrivateRoute exact path={[homepage, `${homepage}/filter`]} component={Home} />
            <PublicRoute exact path={["/login","/login/error=:error", "/login/confirmed-email=:confirmedEmail"]} component={Login} />
            <PrivateRoute component={Error404} />
          </Switch>
        </Suspense>
      </StudioLayout>
    </Router>
    : <Offline/>
  );
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings
  };
}

export default connect(mapStateToProps)(App);
