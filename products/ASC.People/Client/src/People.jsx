import React, { useEffect } from "react";
import { Provider as PeopleProvider, inject, observer } from "mobx-react";
import { Redirect, Switch } from "react-router-dom";
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
import Home from "./pages/Home";
import Profile from "./pages/Profile";
import ProfileAction from "./pages/ProfileAction";
import GroupAction from "./pages/GroupAction";
import Filter from "@appserver/common/api/people/filter";

const homepage = config.homepage;

const Reassign = React.lazy(() => import("./pages/Reassign"));
const Error404 = React.lazy(() => import("studio/Error404"));

const ReassignRoute = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Reassign {...props} />
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

const HomeRedirectToFilter = (props) => {
  const filter = Filter.getDefault();
  const urlFilter = filter.toUrlParams();
  return <Redirect to={`${config.homepage}/filter?${urlFilter}`} />;
};

const PeopleContent = (props) => {
  const { isLoaded, loadBaseInfo } = props;

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
        path={[`${homepage}/group/edit/:groupId`, `${homepage}/group/create`]}
        restricted
        component={GroupAction}
      />
      <PrivateRoute
        path={`${homepage}/reassign/:userId`}
        restricted
        component={ReassignRoute}
      />
      <PrivateRoute exact path={homepage} component={HomeRedirectToFilter} />
      <PrivateRoute path={`${homepage}/filter`} component={Home} />
      <PrivateRoute component={Error404Route} />
    </Switch>
  );
};

const People = inject(({ auth, peopleStore }) => ({
  loadBaseInfo: async () => {
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
