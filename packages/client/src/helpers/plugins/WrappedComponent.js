import React from "react";
import { inject, observer } from "mobx-react";

import RectangleLoader from "@docspace/common/components/Loaders/RectangleLoader";

import Box from "@docspace/components/box";
import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import TextArea from "@docspace/components/textarea";
import TextInput from "@docspace/components/text-input";
import Label from "@docspace/components/label";
import Button from "@docspace/components/button";
import ToggleButton from "@docspace/components/toggle-button";
import ComboBox from "@docspace/components/combobox";

import { PluginComponents } from "./constants";

import { messageActions } from "./utils";

const PropsContext = React.createContext({});

const ComponentPure = ({
  component,
  pluginId,
  setSettingsPluginDialogVisible,
  setCurrentSettingsDialogPlugin,
  updatePluginStatus,
  setPluginDialogVisible,
  setPluginDialogProps,
}) => {
  const [elementProps, setElementProps] = React.useState(component.props);

  const [isRequestRunning, setIsRequestRunning] = React.useState(false);

  const { contextProps, updatePropsContext } = React.useContext(PropsContext);

  React.useEffect(() => {
    if (!component.contextName || !contextProps[component.contextName]) return;

    setElementProps(contextProps[component.contextName]);
  }, [contextProps[component.contextName]]);

  React.useEffect(() => {
    setElementProps(component.props);
  }, [component.props]);

  const getElement = () => {
    const componentName = component.component;

    switch (componentName) {
      case PluginComponents.box: {
        const childrenComponents = elementProps?.children?.map(
          (item, index) => (
            <Component
              key={`box-${index}-${item.component}`}
              component={item}
              pluginId={pluginId}
            />
          )
        );

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
            updatePluginStatus,
            updatePropsContext,
            setPluginDialogVisible,
            setPluginDialogProps
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
            updatePluginStatus,
            updatePropsContext,
            setPluginDialogVisible,
            setPluginDialogProps
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
            updatePluginStatus,
            updatePropsContext,
            setPluginDialogVisible,
            setPluginDialogProps
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
            updatePluginStatus,
            updatePropsContext,
            setPluginDialogVisible,
            setPluginDialogProps
          );
        };

        return <TextInput {...elementProps} onChange={onChangeAction} />;
      }

      case PluginComponents.button: {
        const { withLoadingAfterClick, ...rest } = elementProps;

        const onClickAction = async () => {
          if (withLoadingAfterClick) {
            setIsRequestRunning(true);
          }
          const message = await elementProps.onClick();

          messageActions(
            message,
            setElementProps,
            null,
            pluginId,
            setSettingsPluginDialogVisible,
            setCurrentSettingsDialogPlugin,
            updatePluginStatus,
            updatePropsContext,
            setPluginDialogVisible,
            setPluginDialogProps
          );

          setIsRequestRunning(false);
        };

        return (
          <Button
            {...rest}
            isLoading={isRequestRunning}
            onClick={onClickAction}
          />
        );
      }

      case PluginComponents.comboBox: {
        const onSelectAction = (option) => {
          const message = elementProps.onSelect(option);

          messageActions(
            message,
            setElementProps,
            null,
            pluginId,
            setSettingsPluginDialogVisible,
            setCurrentSettingsDialogPlugin,
            updatePluginStatus,
            updatePropsContext,
            setPluginDialogVisible,
            setPluginDialogProps
          );
        };

        return <ComboBox {...elementProps} onSelect={onSelectAction} />;
      }

      case PluginComponents.iFrame: {
        return <iframe {...elementProps}></iframe>;
      }

      case PluginComponents.img: {
        return <img {...elementProps}></img>;
      }

      case PluginComponents.skeleton: {
        return <RectangleLoader {...elementProps} />;
      }
    }
  };

  const element = getElement();

  return element;
};

const Component = inject(({ pluginStore }) => {
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
})(observer(ComponentPure));

const WrappedComponent = ({ component, pluginId }) => {
  const [contextProps, setContextProps] = React.useState({});

  const updatePropsContext = (name, props) => {
    const newProps = { ...contextProps };
    newProps[name] = props;

    setContextProps(newProps);
  };

  return (
    <PropsContext.Provider value={{ contextProps, updatePropsContext }}>
      <Component component={component} pluginId={pluginId} />
    </PropsContext.Provider>
  );
};

export default WrappedComponent;
