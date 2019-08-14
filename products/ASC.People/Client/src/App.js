import React, { Suspense } from "react";
import { connect } from "react-redux";
import { BrowserRouter, Switch } from "react-router-dom";
import { Loader, ErrorContainer } from "asc-web-components";
import PeopleLayout from "./components/Layout";
import Home from "./components/pages/Home";
import PrivateRoute from "./helpers/privateRoute";
import Profile from './components/pages/Profile';
import ProfileAction from './components/pages/ProfileAction';
import GroupAction from './components/pages/GroupAction';
import { isAdmin } from "./store/auth/selectors";
import { Error404 } from "./components/pages/Error";

/*const Profile = lazy(() => import("./components/pages/Profile"));
const ProfileAction = lazy(() => import("./components/pages/ProfileAction"));
const GroupAction = lazy(() => import("./components/pages/GroupAction"));*/

const App = ({ settings }) => {
  const { homepage } = settings;
  return (
    <BrowserRouter>
      <PeopleLayout>
        <Suspense
          fallback={<Loader className="pageLoader" type="rombs" size={40} />}
        >
          <Switch>
            <PrivateRoute exact path={homepage} component={Home} />
            <PrivateRoute
              path={`${homepage}/view/:userId`}
              component={Profile}
            />
            <PrivateRoute
              path={`${homepage}/edit/:userId`}
              component={ProfileAction}
              restricted
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
            <PrivateRoute component={Error404} />
          </Switch>
        </Suspense>
      </PeopleLayout>
    </BrowserRouter>
  );
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings
  };
}

export default connect(mapStateToProps)(App);
