import React from "react";
import Login from "./components/Login";
import { Switch, Route } from "react-router-dom";
import InvalidRoute from "./components/Invalid";
import CodeLogin from "./components/CodeLogin";
import initLoginStore from "../store";
import { Provider as MobxProvider } from "mobx-react";
import SimpleNav from "../client/components/sub-components/SimpleNav";
import { wrongPortalNameUrl } from "@docspace/common/constants";

interface ILoginProps extends IInitialState {
  isDesktopEditor?: boolean;
}
const App: React.FC<ILoginProps> = (props) => {
  const loginStore = initLoginStore(props?.currentColorScheme || {});

  React.useEffect(() => {
    if (window && props.error) {
      const { status, standalone, message } = props.error;

      if (status === 404 && !standalone) {
        window.location.replace(
          `${wrongPortalNameUrl}?url=${window.location.hostname}`
        );
      }

      throw new Error(message);
    }
  }, []);

  return (
    <MobxProvider {...loginStore}>
      <SimpleNav {...props} />
      <Switch>
        <Route path="/login/error">
          <InvalidRoute />
        </Route>
        <Route path="/login/code">
          <CodeLogin {...props} />
        </Route>
        <Route path="/login">
          <Login {...props} />
        </Route>
      </Switch>
    </MobxProvider>
  );
};

export default App;
