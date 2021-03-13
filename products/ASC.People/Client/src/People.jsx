import React, { useEffect } from "react";
import { Provider as PeopleProvider, inject, observer } from "mobx-react";
import { Router, Switch } from "react-router-dom";
import PeopleStore from "./store/PeopleStore";
import ErrorBoundary from "@appserver/common/components/ErrorBoundary";
import toastr from "studio/toastr";
import PrivateRoute from "@appserver/common/components/PrivateRoute";
import AppLoader from "@appserver/common/components/AppLoader";
import { updateTempContent } from "@appserver/common/utils";
import config from "../package.json";
import "./custom.scss";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";

const Profile = React.lazy(() => import("./components/pages/Profile"));
const ProfileAction = React.lazy(() =>
  import("./components/pages/ProfileAction")
);
const GroupAction = React.lazy(() => import("./components/pages/GroupAction"));
const Reassign = React.lazy(() => import("./components/pages/Reassign"));
const Error404 = React.lazy(() => import("studio/Error404"));
const Home = React.lazy(() => import("./components/pages/Home"));

const ProfileRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Profile {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const ProfileActionRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <ProfileAction {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const GroupActionRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <GroupAction {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const ReassignRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Reassign {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const HomeRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Home {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const Error404Route = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Error404 {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const PeopleContent = (props) => {
  const { homepage, isLoaded, loadBaseInfo } = props;

  useEffect(() => {
    loadBaseInfo()
      .catch((err) => toastr.error(err))
      .finally(() => {
        //this.props.setIsLoaded(true);
        updateTempContent();
      });
  }, []);

  useEffect(() => {
    if (isLoaded) updateTempContent();
  }, [isLoaded]);

  return (
    <Switch>
      <PrivateRoute
        exact
        path={`${homepage}/view/:userId`}
        component={ProfileRoute}
      />
      <PrivateRoute
        path={`${homepage}/edit/:userId`}
        restricted
        allowForMe
        component={ProfileActionRoute}
      />
      <PrivateRoute
        path={`${homepage}/create/:type`}
        restricted
        component={ProfileActionRoute}
      />
      <PrivateRoute
        path={[`${homepage}/group/edit/:groupId`, `${homepage}/group/create`]}
        restricted
        component={GroupActionRoute}
      />
      <PrivateRoute
        path={`${homepage}/reassign/:userId`}
        restricted
        component={ReassignRoute}
      />
      <PrivateRoute exact path={homepage} component={HomeRoute} />
      <PrivateRoute path={`${homepage}/filter`} component={HomeRoute} />
      <PrivateRoute component={Error404Route} />
    </Switch>
  );
};

const People = inject(({ auth, peopleStore }) => ({
  homepage: config.homepage, // auth.settingsStore.homepage
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
