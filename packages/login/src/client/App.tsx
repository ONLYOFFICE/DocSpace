import React from "react";
import Login from "./components/Login";
import { Switch, Route } from "react-router-dom";
import InvalidRoute from "./components/Invalid";
import CodeLogin from "./components/CodeLogin";

interface ILoginProps extends IInitialState {
  isDesktopEditor?: boolean;
}
const App: React.FC<ILoginProps> = (props) => {
  return (
    <Switch>
      <Route path="/login" exact>
        <Login {...props} />
      </Route>
      <Route path="/login/error=:error">
        <InvalidRoute />
      </Route>
      <Route path="/login/code">
        <CodeLogin />
      </Route>
    </Switch>
  );
};

export default App;
