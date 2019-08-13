import React, { Suspense, lazy } from "react";
import { connect } from "react-redux";
import { BrowserRouter, Switch } from "react-router-dom";
import { Loader, ErrorContainer } from "asc-web-components";
import PeopleLayout from "./components/Layout";
import Home from "./components/pages/Home";
import PrivateRoute from "./helpers/privateRoute";

const Profile = lazy(() => import("./components/pages/Profile"));
const ProfileAction = lazy(() => import("./components/pages/ProfileAction"));
const GroupAction = lazy(() => import("./components/pages/GroupAction"));

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
            />
            <PrivateRoute
              path={`${homepage}/create/:type`}
              component={ProfileAction}
            />
            <PrivateRoute
              path={`${homepage}/group/edit/:groupId`}
              component={GroupAction}
            />
            <PrivateRoute
              path={`${homepage}/group/create`}
              component={GroupAction}
            />
            <PrivateRoute
              component={() => (
                <ErrorContainer>
                  Sorry, the resource cannot be found.
                </ErrorContainer>
              )}
            />
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
