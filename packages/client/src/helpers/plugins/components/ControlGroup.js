import React from "react";
import styled from "styled-components";

import RectangleLoader from "@docspace/common/components/Loaders/RectangleLoader/RectangleLoader";

import Label from "@docspace/components/label";
import TextInput from "@docspace/components/text-input";
import Checkbox from "@docspace/components/checkbox";
import ToggleButton from "@docspace/components/toggle-button";

import { messageActions } from "../utils";

const StyledControlGroup = styled.div`
  display: flex;
  flex-direction: column;
  gap: 8px;

  margin-bottom: 16px;

  .loader-container {
    display: flex;
    gap: 8px;
    align-items: center;
  }

  .toggle-button {
    margin-bottom: 16px;
  }
`;

const ControlGroup = ({
  group,
  isLoading,
  setAcceptButtonProps,
  pluginId,
  updatePluginStatus,
  setCurrentSettingsDialogPlugin,
  setSettingsPluginDialogVisible,
  setPluginDialogVisible,
  setPluginDialogProps,
}) => {
  const [elementProps, setElementProps] = React.useState(null);

  React.useEffect(() => {
    setElementProps(group.elementProps);
  }, [group.elementProps]);

  const getElement = () => {
    if (!elementProps) return <></>;

    switch (group.element) {
      case "input":
        if (isLoading) return <RectangleLoader />;
        const onChange = async (e) => {
          if (!elementProps.onChange) return;
          const value = e.target.value;

          const message = await elementProps.onChange(value);

          messageActions(
            message,
            setElementProps,
            setAcceptButtonProps,
            pluginId,
            setSettingsPluginDialogVisible,
            setCurrentSettingsDialogPlugin,
            updatePluginStatus,
            null,
            setPluginDialogVisible,
            setPluginDialogProps
          );
        };

        const onBlur = async (e) => {
          if (!elementProps.onBlur) return;
          const value = e.target.value;

          const message = await elementProps.onBlur(value);

          messageActions(
            message,
            setElementProps,
            setAcceptButtonProps,
            pluginId,
            setSettingsPluginDialogVisible,
            setCurrentSettingsDialogPlugin,
            updatePluginStatus,
            null,
            setPluginDialogVisible,
            setPluginDialogProps
          );
        };

        const onFocus = async (e) => {
          if (!elementProps.onFocus) return;
          const value = e.target.value;

          const message = await elementProps.onFocus(value);

          messageActions(
            message,
            setElementProps,
            setAcceptButtonProps,
            pluginId,
            setSettingsPluginDialogVisible,
            setCurrentSettingsDialogPlugin,
            updatePluginStatus,
            null,
            setPluginDialogVisible,
            setPluginDialogProps
          );
        };

        return (
          <TextInput
            {...elementProps}
            onChange={onChange}
            onBlur={onBlur}
            onFocus={onFocus}
          />
        );
      case "checkbox":
        const cbElements = [];

        group.elementProps.forEach((value, index) => {
          const onChange = async () => {
            if (!value.onChange) return;

            const message = await elementProps[index].onChange();

            messageActions(
              message,
              setElementProps,
              setAcceptButtonProps,
              pluginId,
              setSettingsPluginDialogVisible,
              setCurrentSettingsDialogPlugin,
              updatePluginStatus,
              null,
              setPluginDialogVisible,
              setPluginDialogProps
            );
          };

          if (isLoading) {
            cbElements.push(
              <div className="loader-container" key={index}>
                <RectangleLoader width={"16px"} height={"16px"} />
                <RectangleLoader width={"150px"} height={"20px"} />
              </div>
            );
          } else {
            cbElements.push(
              <Checkbox
                key={`${value.label}-${index}`}
                {...elementProps[index]}
                onChange={onChange}
              />
            );
          }
        });

        return cbElements.map((element) => element);

      case "toggle-button":
        const tbElements = [];
        group.elementProps.forEach((value, index) => {
          const onChange = async () => {
            if (!value.onChange) return;

            const message = await elementProps[index].onChange();

            messageActions(
              message,
              setElementProps,
              setAcceptButtonProps,
              pluginId,
              setSettingsPluginDialogVisible,
              setCurrentSettingsDialogPlugin,
              updatePluginStatus,
              null,
              setPluginDialogVisible,
              setPluginDialogProps
            );
          };

          if (isLoading) {
            tbElements.push(
              <div className="loader-container" key={index}>
                <RectangleLoader
                  width={"32px"}
                  height={"16px"}
                  borderRadius={"6"}
                />
                <RectangleLoader width={"150px"} height={"20px"} />
              </div>
            );
          } else {
            tbElements.push(
              <ToggleButton
                className="toggle-button"
                key={`${value.label}-${index}`}
                {...elementProps[index]}
                onChange={onChange}
              />
            );
          }
        });

        return tbElements.map((element) => element);
    }
    return <></>;
  };

  const element = getElement();

  return (
    <StyledControlGroup>
      {!isLoading ? (
        <Label className="label" text={group.header} />
      ) : (
        <RectangleLoader width={"100px"} height={"18px"} />
      )}
      {element}
    </StyledControlGroup>
  );
};

export default ControlGroup;
