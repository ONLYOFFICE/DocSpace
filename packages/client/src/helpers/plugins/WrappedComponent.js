import React from "react";
import { inject, observer } from "mobx-react";

import Box from "@docspace/components/box";
import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import TextArea from "@docspace/components/textarea";
import TextInput from "@docspace/components/text-input";
import Label from "@docspace/components/label";
import Button from "@docspace/components/button";
import ToggleButton from "@docspace/components/toggle-button";

import { PluginComponents } from "./constants";

import { messageActions } from "./utils";

const WrappedComponent = ({
  component,
  pluginId,
  setSettingsPluginDialogVisible,
  setCurrentSettingsDialogPlugin,
  updatePluginStatus,
}) => {
  const [elementProps, setElementProps] = React.useState(component.props);

  const getElement = () => {
    const componentName = component.component;

    switch (componentName) {
      case PluginComponents.box: {
        const childrenComponents = elementProps.children.map((item, index) => (
          <WrappedComponent
            key={`box-${index}-${item.component}`}
            component={item}
            pluginId={pluginId}
          />
        ));

        return <Box {...elementProps}>{childrenComponents}</Box>;
      }

      case PluginComponents.text: {
        return <Text {...elementProps}>{elementProps.text}</Text>;
      }

      case PluginComponents.label: {
        return <Label {...elementProps} />;
      }

      case PluginComponents.checkbox: {
        const onChangeAction = () => {
          const message = elementProps.onChange();

          messageActions(
            message,
            setElementProps,
            null,
            pluginId,
            setSettingsPluginDialogVisible,
            setCurrentSettingsDialogPlugin,
            updatePluginStatus
          );
        };

        return <Checkbox {...elementProps} onChange={onChangeAction} />;
      }

      case PluginComponents.toggleButton: {
        const onChangeAction = () => {
          const message = elementProps.onChange();

          messageActions(
            message,
            setElementProps,
            null,
            pluginId,
            setSettingsPluginDialogVisible,
            setCurrentSettingsDialogPlugin,
            updatePluginStatus
          );
        };

        return <ToggleButton {...elementProps} onChange={onChangeAction} />;
      }

      case PluginComponents.textArea: {
        const onChangeAction = (e) => {
          const message = elementProps.onChange(e.target.value);

          messageActions(
            message,
            setElementProps,
            null,
            pluginId,
            setSettingsPluginDialogVisible,
            setCurrentSettingsDialogPlugin,
            updatePluginStatus
          );
        };

        return <TextArea {...elementProps} onChange={onChangeAction} />;
      }

      case PluginComponents.input: {
        const onChangeAction = (e) => {
          const message = elementProps.onChange(e.target.value);

          messageActions(
            message,
            setElementProps,
            null,
            pluginId,
            setSettingsPluginDialogVisible,
            setCurrentSettingsDialogPlugin,
            updatePluginStatus
          );
        };

        return <TextInput {...elementProps} onChange={onChangeAction} />;
      }

      case PluginComponents.button: {
        const onClickAction = async () => {
          const message = await elementProps.onClick();

          messageActions(
            message,
            setElementProps,
            null,
            pluginId,
            setSettingsPluginDialogVisible,
            setCurrentSettingsDialogPlugin,
            updatePluginStatus
          );
        };

        return <Button {...elementProps} onClick={onClickAction} />;
      }
    }
  };

  const element = getElement();

  return element;
};

export default inject(({ pluginStore }) => {
  const {
    updatePluginStatus,
    setCurrentSettingsDialogPlugin,
    setSettingsPluginDialogVisible,
  } = pluginStore;
  return {
    updatePluginStatus,
    setCurrentSettingsDialogPlugin,
    setSettingsPluginDialogVisible,
  };
})(observer(WrappedComponent));
