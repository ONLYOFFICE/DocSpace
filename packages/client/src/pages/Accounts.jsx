import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import { Redirect, Switch } from "react-router-dom";
import ErrorBoundary from "@docspace/common/components/ErrorBoundary";
import toastr from "@docspace/components/toast/toastr";
import PrivateRoute from "@docspace/common/components/PrivateRoute";
import AppLoader from "@docspace/common/components/AppLoader";
import { /*combineUrl,*/ updateTempContent } from "@docspace/common/utils";
import Home from "./AccountsHome";
import Profile from "./Profile";

import Filter from "@docspace/common/api/people/filter";
import { showLoader, hideLoader } from "@docspace/common/utils";

const Error404 = React.lazy(() => import("client/Error404"));

const PeopleSection = React.memo(() => {
  return (
    <Switch>
      <PrivateRoute exact path={["/accounts/view/@self"]} component={Profile} />
      <PrivateRoute
        exact
        path={["/accounts"]}
        component={HomeRedirectToFilter}
      />
      <PrivateRoute path={"/accounts/filter"} restricted component={Home} />
      <PrivateRoute component={Error404Route} />
    </Switch>
  );
});

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
  return <Redirect to={`/accounts/filter?${urlFilter}`} />;
};

const PeopleContent = (props) => {
  const { loadBaseInfo, isLoading, setFirstLoad } = props;

  useEffect(() => {
    setFirstLoad(false);
  }, [setFirstLoad]);

  useEffect(() => {
    if (isLoading) {
      showLoader();
    } else {
      hideLoader();
    }
  }, [isLoading]);

  useEffect(() => {
    loadBaseInfo()
      .catch((err) => toastr.error(err))
      .finally(() => {
        updateTempContent();
      });
  }, []);

  return <PeopleSection />;
};

const People = inject(({ auth, filesStore, peopleStore }) => {
  const { setFirstLoad, isLoading } = filesStore;

  return {
    loadBaseInfo: async () => {
      await peopleStore.init();
      //auth.setProductVersion(config.version);
    },
    setFirstLoad,
    isLoading,
  };
})(observer(PeopleContent));

export default (props) => <People {...props} />;
