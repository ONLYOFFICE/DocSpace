import React, { Suspense } from "react";
import { connect } from "react-redux";
import { Router, Switch, Redirect } from "react-router-dom";
import { Loader } from "asc-web-components";
import Home from "./components/pages/Home";
import Profile from "./components/pages/Profile";
import ProfileAction from "./components/pages/ProfileAction";
import GroupAction from "./components/pages/GroupAction";
import Reassign from "./components/pages/Reassign";
import {
  history,
  PrivateRoute,
  PublicRoute,
  Login,
  Error404,
  Error520,
  StudioLayout,
  Offline
} from "asc-web-common";

/*const Profile = lazy(() => import("./components/pages/Profile"));
const ProfileAction = lazy(() => import("./components/pages/ProfileAction"));
const GroupAction = lazy(() => import("./components/pages/GroupAction"));*/

const App = ({ settings }) => {
  const { homepage } = settings;
  return navigator.onLine ? (
    <Router history={history}>
      <StudioLayout>
        <Suspense
          fallback={<Loader className="pageLoader" type="rombs" size="40px" />}
        >
          <Switch>
            <Redirect exact from="/" to={`${homepage}`} />
            <PrivateRoute
              exact
              path={[homepage, `${homepage}/filter`]}
              component={Home}
            />
            <PrivateRoute
              path={`${homepage}/view/:userId`}
              component={Profile}
            />
            <PrivateRoute
              path={`${homepage}/edit/:userId`}
              component={ProfileAction}
              restricted
              allowForMe
            />
            <PrivateRoute
              path={`${homepage}/create/:type`}
              component={ProfileAction}
              restricted
            />
            <PrivateRoute
              path={`${homepage}/group/edit/:groupId`}
              component={GroupAction}
              restricted
            />
            <PrivateRoute
              path={`${homepage}/group/create`}
              component={GroupAction}
              restricted
            />
            <PrivateRoute
              path={`${homepage}/reassign/:userId`}
              component={Reassign}
              restricted
            />
            <PublicRoute
              exact
              path={[
                "/login",
                "/login/error=:error",
                "/login/confirmed-email=:confirmedEmail"
              ]}
              component={Login}
            />
            <PrivateRoute exact path={`/error=:error`} component={Error520} />
            <PrivateRoute component={Error404} />
          </Switch>
        </Suspense>
      </StudioLayout>
    </Router>
  ) : (
    <Offline />
  );
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings
  };
}

export default connect(mapStateToProps)(App);
