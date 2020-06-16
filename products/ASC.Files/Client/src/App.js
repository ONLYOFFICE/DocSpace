import React, { Suspense } from "react";
import { connect } from "react-redux";
import { Router, Switch, Redirect } from "react-router-dom";
import { Loader } from "asc-web-components";
import Home from "./components/pages/Home";
import DocEditor from "./components/pages/DocEditor";
import { history, PrivateRoute, PublicRoute, Login, Error404, StudioLayout, Offline } from "asc-web-common";

const withStudioLayout = Component => props => <StudioLayout><Component {...props} /></StudioLayout>;

const App = ({ settings }) => {
  const { homepage } = settings;
  
  return (
    navigator.onLine ? 
    <Router history={history}>
        <Suspense
          fallback={<Loader className="pageLoader" type="rombs" size='40px' />}
        >
          <Switch>
            <Redirect exact from="/" to={`${homepage}`} />
            <PrivateRoute exact path={[homepage, `${homepage}/filter`]} component={withStudioLayout(Home)} />
            <PrivateRoute exact path={`${homepage}/doceditor`} component={DocEditor} />
            <PublicRoute exact path={["/login","/login/error=:error", "/login/confirmed-email=:confirmedEmail"]} component={Login} />
            <PrivateRoute component={withStudioLayout(Error404)} />
          </Switch>
        </Suspense>
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
