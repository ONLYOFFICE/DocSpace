import React from "react";
import styled from "styled-components";

import WrappedComponent from "../WrappedComponent";

import { PluginComponents } from "../constants";

const StyledPluginSettings = styled.div`
  .settings-header {
    margin: 0;
    margin-bottom: 16px;
  }

  margin-bottom: 32px;
`;

const PluginSettings = ({
  id,

  onLoad,

  customSettings,
}) => {
  const [customSettingsProps, setCustomSettingsProps] =
    React.useState(customSettings);

  const onLoadAction = React.useCallback(async () => {
    if (!onLoad) return;
    const res = await onLoad();

    setCustomSettingsProps(res.customSettings);
  }, [onLoad]);

  React.useEffect(() => {
    onLoadAction();
  }, [onLoadAction]);

  return (
    <StyledPluginSettings>
      <WrappedComponent
        pluginId={id}
        component={{
          component: PluginComponents.box,
          props: customSettingsProps,
        }}
      />
    </StyledPluginSettings>
  );
};

export default PluginSettings;
