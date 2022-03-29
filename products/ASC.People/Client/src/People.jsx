import React, { useEffect } from "react";
import { Provider as PeopleProvider, inject, observer } from "mobx-react";
import { Redirect, Switch } from "react-router-dom";
import PeopleStore from "./store/PeopleStore";
import ErrorBoundary from "@appserver/common/components/ErrorBoundary";
import toastr from "studio/toastr";
import PrivateRoute from "@appserver/common/components/PrivateRoute";
import AppLoader from "@appserver/common/components/AppLoader";
import { combineUrl, updateTempContent } from "@appserver/common/utils";
import config from "../package.json";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import Home from "./pages/Home";
import Profile from "./pages/Profile";
import ProfileAction from "./pages/ProfileAction";
import GroupAction from "./pages/GroupAction";
import Filter from "@appserver/common/api/people/filter";
import { AppServerConfig } from "@appserver/common/constants";
import Article from "@appserver/common/components/Article";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent,
} from "./components/Article";

const { proxyURL } = AppServerConfig;
const homepage = config.homepage;

const PROXY_HOMEPAGE_URL = combineUrl(proxyURL, homepage);

const HOME_URL = combineUrl(PROXY_HOMEPAGE_URL, "/") || "/";
const HOME_FILTER_URL = combineUrl(PROXY_HOMEPAGE_URL, "/filter");
const PROFILE_URL = combineUrl(PROXY_HOMEPAGE_URL, "/view/:userId");
const PROFILE_EDIT_URL = combineUrl(PROXY_HOMEPAGE_URL, "/edit/:userId");
const PROFILE_CREATE_URL = combineUrl(PROXY_HOMEPAGE_URL, "/create/:type");
const GROUP_URLS = [
  combineUrl(PROXY_HOMEPAGE_URL, "/group/edit/:groupId"),
  combineUrl(PROXY_HOMEPAGE_URL, "/group/create"),
];
const REASSIGN_URL = combineUrl(PROXY_HOMEPAGE_URL, "/reassign/:userId");

const Reassign = React.lazy(() => import("./pages/Reassign"));
const Error404 = React.lazy(() => import("studio/Error404"));

const PeopleArticle = React.memo(() => {
  return (
    <Article>
      <Article.Header>
        <ArticleHeaderContent />
      </Article.Header>
      <Article.MainButton>
        <ArticleMainButtonContent />
      </Article.MainButton>
      <Article.Body>
        <ArticleBodyContent />
      </Article.Body>
    </Article>
  );
});

const PeopleSection = React.memo(() => {
  return (
    <Switch>
      <PrivateRoute exact path={PROFILE_URL} component={Profile} />
      <PrivateRoute
        path={PROFILE_EDIT_URL}
        restricted
        allowForMe
        component={ProfileAction}
      />
      <PrivateRoute
        path={PROFILE_CREATE_URL}
        restricted
        component={ProfileAction}
      />
      <PrivateRoute path={GROUP_URLS} restricted component={GroupAction} />
      <PrivateRoute path={REASSIGN_URL} restricted component={ReassignRoute} />
      <PrivateRoute exact path={HOME_URL} component={HomeRedirectToFilter} />
      <PrivateRoute path={HOME_FILTER_URL} component={Home} />
      <PrivateRoute component={Error404Route} />
    </Switch>
  );
});

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
  return (
    <Redirect to={combineUrl(PROXY_HOMEPAGE_URL, `/filter?${urlFilter}`)} />
  );
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
    <>
      <PeopleArticle />
      <PeopleSection />
    </>
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
