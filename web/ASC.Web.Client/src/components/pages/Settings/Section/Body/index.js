import React, { lazy } from "react";
import { Route, Switch } from "react-router-dom";
import { withRouter } from "react-router";

const CustomizationSettings = lazy(() => import("../../sub-components/common/customization"));
const NotImplementedSettings = lazy(() => import("../../sub-components/notImplementedSettings"));
class SectionBodyContent extends React.PureComponent {

  render() {
    return (
      <Switch>
        <Route
          exact
          path={[`${this.props.match.path}/common/customization`,`${this.props.match.path}/common`, this.props.match.path]}
          component={CustomizationSettings}
        />

        <Route component={NotImplementedSettings} />
      </Switch>
    );
  };
};

export default withRouter(SectionBodyContent);
