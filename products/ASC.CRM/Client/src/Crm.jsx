import React, { useEffect } from "react";
import { Provider as PeopleProvider, inject, observer } from "mobx-react";
import { Switch } from "react-router-dom";
import CrmStore from "./store/CrmStore";
import ErrorBoundary from "@appserver/common/components/ErrorBoundary";
import toastr from "studio/toastr";
import PrivateRoute from "@appserver/common/components/PrivateRoute";
import AppLoader from "@appserver/common/components/AppLoader";
import { combineUrl, updateTempContent } from "@appserver/common/utils";
import config from "../package.json";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import Home from "./pages/Home";
import { AppServerConfig } from "@appserver/common/constants";

const { proxyURL } = AppServerConfig;
const homepage = config.homepage;
const PROXY_HOMEPAGE_URL = combineUrl(proxyURL, homepage);

const Error404 = React.lazy(() => import("studio/Error404"));

const Error404Route = (props) => (
  <React.Suspense fallback={<AppLoader />}>
    <ErrorBoundary>
      <Error404 {...props} />
    </ErrorBoundary>
  </React.Suspense>
);

const CrmContent = (props) => {
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
      <PrivateRoute exact path={PROXY_HOMEPAGE_URL} component={Home} />
      <PrivateRoute component={Error404Route} />
    </Switch>
  );
};

const Crm = inject(({ auth, crmStore }) => ({
  loadBaseInfo: async () => {
    await crmStore.init();
    auth.setProductVersion(config.version);
  },
  isLoaded: auth.isLoaded && crmStore.isLoaded,
}))(observer(CrmContent));

const crmStore = new CrmStore();

export default (props) => (
  <PeopleProvider crmStore={crmStore}>
    <I18nextProvider i18n={i18n}>
      <Crm {...props} />
    </I18nextProvider>
  </PeopleProvider>
);
