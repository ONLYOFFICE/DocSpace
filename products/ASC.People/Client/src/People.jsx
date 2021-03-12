import React, { Suspense, useEffect } from "react";
import { Provider as PeopleProvider, inject, observer } from "mobx-react";
import { Router, Switch } from "react-router-dom";
import PeopleStore from "./store/PeopleStore";
import Home from "./components/pages/Home";
import Loader from "@appserver/components/loader";
import toastr from "studio/toastr";
import PrivateRoute from "@appserver/common/components/PrivateRoute";
import { updateTempContent } from "@appserver/common/utils";
const Profile = React.lazy(() => import("./components/pages/Profile"));
const ProfileAction = React.lazy(() =>
  import("./components/pages/ProfileAction")
);
const GroupAction = React.lazy(() => import("./components/pages/GroupAction"));
const Reassign = React.lazy(() => import("./components/pages/Reassign"));
import config from "../package.json";
import "./custom.scss";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import history from "@appserver/common/history";

const Error404 = React.lazy(() => import("studio/Error404"));

const PeopleContent = (props) => {
  const { homepage, isLoaded, loadBaseInfo } = props;

  useEffect(() => {
    loadBaseInfo()
      .catch((err) => toastr.error(err))
      .finally(() => {
        //this.props.setIsLoaded(true);
        updateTempContent();
      });
  }, [loadBaseInfo]);

  useEffect(() => {
    if (isLoaded) updateTempContent();
  }, [isLoaded]);

  return (
    <Suspense
      fallback={<Loader className="pageLoader" type="rombs" size="40px" />}
    >
      <Router history={history}>
        <Switch>
          <PrivateRoute
            exact
            path={`${homepage}/view/:userId`}
            component={Profile}
          />
          <PrivateRoute
            path={`${homepage}/edit/:userId`}
            restricted
            allowForMe
            component={ProfileAction}
          />
          <PrivateRoute
            path={`${homepage}/create/:type`}
            restricted
            component={ProfileAction}
          />
          <PrivateRoute
            path={[
              `${homepage}/group/edit/:groupId`,
              `${homepage}/group/create`,
            ]}
            restricted
            component={GroupAction}
          />
          <PrivateRoute
            path={`${homepage}/reassign/:userId`}
            restricted
            component={Reassign}
          />
          <PrivateRoute exact path={homepage} component={Home} />
          <PrivateRoute path={`${homepage}/filter`} component={Home} />
          <PrivateRoute component={Error404} />
        </Switch>
      </Router>
    </Suspense>
  );
};

const People = inject(({ auth, peopleStore }) => ({
  homepage: auth.settingsStore.homepage || config.homepage,
  loadBaseInfo: async () => {
    //auth.init();
    await peopleStore.init();
    auth.setProductVersion(config.version);
  },
  isLoaded: auth.isLoaded && peopleStore.isLoaded,
}))(observer(PeopleContent));

const peopleStore = new PeopleStore();

export default (props) => (
  <PeopleProvider peopleStore={peopleStore}>
    <I18nextProvider i18n={i18n}>
      <People {...props} />
    </I18nextProvider>
  </PeopleProvider>
);
