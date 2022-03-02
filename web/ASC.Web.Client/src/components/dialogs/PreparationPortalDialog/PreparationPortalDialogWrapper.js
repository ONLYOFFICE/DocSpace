import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import PreparationPortalDialog from ".";
import stores from "../../../store/index";

class PreparationPortalWrapper extends React.Component {
  render() {
    return (
      <MobxProvider {...stores}>
        <PreparationPortalDialog {...this.props} />
      </MobxProvider>
    );
  }
}

export default PreparationPortalWrapper;
