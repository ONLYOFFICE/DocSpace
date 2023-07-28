import React from "react";
import styled from "styled-components";

import { inject, observer } from "mobx-react";

import RectangleLoader from "@docspace/common/components/Loaders/RectangleLoader/RectangleLoader";

import Button from "@docspace/components/button";

import ControlGroup from "SRC_DIR/helpers/plugins/components/ControlGroup";
import { messageActions } from "SRC_DIR/helpers/plugins/utils";
import WrappedComponent from "../WrappedComponent";
import { PluginComponents } from "../constants";

const StyledPluginSettings = styled.div`
  .settings-header {
    margin: 0;
    margin-bottom: 16px;
  }
`;

const PluginSettings = ({
  id,
  groups,
  isLoading,
  onLoad,
  withAcceptButton,
  acceptButtonProps,

  withCustomSettings,
  customSettings,

  getPluginSettings,

  updatePluginStatus,
  setCurrentSettingsDialogPlugin,
  setSettingsPluginDialogVisible,
  setPluginDialogVisible,
  setPluginDialogProps,

  ...rest
}) => {
  const [groupsProps, setGroupsProps] = React.useState(groups);
  const [customSettingsProps, setCustomSettingsProps] =
    React.useState(customSettings);
  const [acceptButton, setAcceptButton] = React.useState({});
  const [isLoadingState, setIsLoading] = React.useState(isLoading);

  const onLoadAction = React.useCallback(async () => {
    if (!onLoad) return;
    const res = await onLoad();

    const settings = getPluginSettings();
    setGroupsProps(settings.groups);
    setCustomSettingsProps(settings.customSettings);
    setIsLoading(res);
  }, [onLoad]);

  React.useEffect(() => {
    onLoadAction();
  }, [onLoadAction]);

  React.useEffect(() => {
    if (withAcceptButton) setAcceptButton({ ...acceptButtonProps });
  }, [withAcceptButton, acceptButtonProps]);

  const getAcceptButtonElement = () => {
    if (isLoadingState)
      return <RectangleLoader width={"160px"} height={"28px"} />;
    const onClick = async () => {
      if (!acceptButton.onClick) return;

      const message = await acceptButton.onClick();

      messageActions(
        message,
        setAcceptButton,
        null,
        id,
        setSettingsPluginDialogVisible,
        setCurrentSettingsDialogPlugin,
        updatePluginStatus,
        null,
        setPluginDialogVisible,
        setPluginDialogProps
      );
    };

    return <Button {...acceptButton} onClick={onClick} />;
  };

  const element = getAcceptButtonElement();

  return (
    <StyledPluginSettings>
      {withCustomSettings ? (
        isLoadingState ? (
          <></>
        ) : (
          <WrappedComponent
            pluginId={id}
            component={{
              component: PluginComponents.box,
              props: customSettingsProps,
            }}
            isLoading={isLoadingState}
          />
        )
      ) : (
        <>
          {groupsProps?.map((group) => (
            <ControlGroup
              key={group.header}
              group={group}
              setAcceptButtonProps={setAcceptButton}
              isLoading={isLoadingState}
              pluginId={id}
              updatePluginStatus={updatePluginStatus}
              setCurrentSettingsDialogPlugin={setCurrentSettingsDialogPlugin}
              setSettingsPluginDialogVisible={setSettingsPluginDialogVisible}
              setPluginDialogVisible={setPluginDialogVisible}
              setPluginDialogProps={setPluginDialogProps}
            />
          ))}
          {withAcceptButton && element}
        </>
      )}
    </StyledPluginSettings>
  );
};

export default inject(({ pluginStore }) => {
  const {
    updatePluginStatus,
    setCurrentSettingsDialogPlugin,
    setSettingsPluginDialogVisible,
    setPluginDialogVisible,
    setPluginDialogProps,
  } = pluginStore;
  return {
    updatePluginStatus,
    setCurrentSettingsDialogPlugin,
    setSettingsPluginDialogVisible,
    setPluginDialogVisible,
    setPluginDialogProps,
  };
})(observer(PluginSettings));
