import React from "react";
import styled from "styled-components";

import RectangleLoader from "@docspace/common/components/Loaders/RectangleLoader/RectangleLoader";

import Button from "@docspace/components/button";

import ControlGroup from "SRC_DIR/helpers/plugins/components/ControlGroup";
import { messageActions } from "SRC_DIR/helpers/plugins/utils";

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

  getPluginSettings,

  updatePluginStatus,

  ...rest
}) => {
  const [groupsProps, setGroupsProps] = React.useState(groups);
  const [acceptButton, setAcceptButton] = React.useState({});
  const [isLoadingState, setILoadingState] = React.useState(isLoading);

  const onLoadAction = React.useCallback(async () => {
    const res = await onLoad();

    const settings = getPluginSettings();
    setGroupsProps(settings.groups);
    setILoadingState(res);
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
        null,
        null,
        updatePluginStatus
      );
    };

    return <Button {...acceptButton} onClick={onClick} />;
  };

  const element = getAcceptButtonElement();

  return (
    <StyledPluginSettings>
      {groupsProps?.map((group) => (
        <ControlGroup
          key={group.header}
          group={group}
          setAcceptButtonProps={setAcceptButton}
          isLoading={isLoadingState}
        />
      ))}
      {withAcceptButton && element}
    </StyledPluginSettings>
  );
};

export default PluginSettings;
