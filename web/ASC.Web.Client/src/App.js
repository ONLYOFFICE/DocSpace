import React, { Suspense, lazy, useEffect } from "react";
import { Router, Route, Switch } from "react-router-dom";
import {
  history,
  PrivateRoute,
  PublicRoute,
  Login,
  Error404,
  Offline,
  NavMenu,
  Main,
  utils,
  toastr,
  Layout,
} from "asc-web-common";
import Home from "./components/pages/Home";
import { inject, observer } from "mobx-react";
import config from "../package.json";
import "./i18n";

const About = lazy(() => import("./components/pages/About"));
const Confirm = lazy(() => import("./components/pages/Confirm"));
const Settings = lazy(() => import("./components/pages/Settings"));
const Wizard = lazy(() => import("./components/pages/Wizard"));
const Payments = lazy(() => import("./components/pages/Payments"));
const ThirdPartyResponse = lazy(() => import("./components/pages/ThirdParty"));
const ComingSoon = lazy(() => import("./components/pages/ComingSoon"));

const App = (props) => {
  //constructor(props) {
  //super(props);

  //const pathname = window.location.pathname.toLowerCase();
  //this.isThirdPartyResponse = pathname.indexOf("thirdparty") !== -1;
  //}

  const { isLoaded, loadBaseInfo, isThirdPartyResponse, homepage } = props;

  useEffect(() => {
    try {
      loadBaseInfo();
    } catch (err) {
      toastr.error(err);
    }
  }, [loadBaseInfo]);

  useEffect(() => {
    console.log("App render", isLoaded);
    if (isLoaded) utils.updateTempContent();
  }, [isLoaded]);

  // useEffect(() => {
  //   debugger;
  //   utils.updateTempContent(isAuthenticated);
  // }, [isAuthenticated]);

  // if (this.isThirdPartyResponse) {
  //   //setIsLoaded();
  //   return;
  // }

  //utils.updateTempContent();
  //setIsLoaded();

  // const requests = [];
  // if (!isAuthenticated) {
  //   requests.push(getPortalSettings());
  // } else if (
  //   !window.location.pathname.includes("confirm/EmailActivation")
  // ) {
  //   requests.push(getUser());
  //   requests.push(getPortalSettings());
  //   //requests.push(getModules());
  // }

  // Promise.all(requests)
  //   .catch((e) => {
  //     toastr.error(e);
  //   })
  //   .finally(() => {
  //     utils.updateTempContent();
  //     setIsLoaded();
  //   });

  console.log("Client App render", props);
  return navigator.onLine ? (
    <Layout>
      <Router history={history}>
        {!isThirdPartyResponse && <NavMenu />}
        <Main>
          <Suspense fallback={null}>
            <Switch>
              <Route exact path={`${homepage}/wizard`} component={Wizard} />
              <PublicRoute
                exact
                path={[
                  `${homepage}/login`,
                  `${homepage}/login/error=:error`,
                  `${homepage}/login/confirmed-email=:confirmedEmail`,
                ]}
                component={Login}
              />
              <Route path={`${homepage}/confirm`} component={Confirm} />
              <PrivateRoute
                path={`${homepage}/thirdparty/:provider`}
                component={ThirdPartyResponse}
              />
              <PrivateRoute
                exact
                path={[`${homepage}/`, `${homepage}/error=:error`]}
                component={Home}
              />
              <PrivateRoute
                exact
                path={[
                  `${homepage}/coming-soon`,
                  `${homepage}/products/mail`,
                  `${homepage}/products/projects`,
                  `${homepage}/products/crm`,
                  `${homepage}/products/calendar`,
                  `${homepage}/products/talk/`,
                ]}
                component={ComingSoon}
              />
              <PrivateRoute
                path={`${homepage}/payments`}
                component={Payments}
              />
              <PrivateRoute component={Error404} />
            </Switch>
          </Suspense>
        </Main>
      </Router>
    </Layout>
  ) : (
    <Offline />
  );
};

export default inject(({ auth }) => {
  const { init, isLoaded } = auth;

  const pathname = window.location.pathname.toLowerCase();
  const isThirdPartyResponse = pathname.indexOf("thirdparty") !== -1;

  return {
    loadBaseInfo: () => {
      init();
      auth.setProductVersion(config.version);
    },
    isThirdPartyResponse,
    isLoaded,
    homepage: auth.settingsStore.homepage || config.homepage,
  };
})(observer(App));
