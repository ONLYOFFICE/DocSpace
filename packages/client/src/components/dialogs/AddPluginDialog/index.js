import React from "react";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import api from "@docspace/common/api";

import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Block from "./sub-components/Block";
import GeneralBlock from "./sub-components/GeneralBlock";
import ScopesBlock from "./sub-components/ScopesBlock";

const AddPluginDialog = ({ visible, displayName, onClose }) => {
  const { t } = useTranslation(["PluginsSettings", "Common"]);

  const [isRequestRunning, setIsRequestRunning] = React.useState(false);

  const [generalValue, setGeneralValue] = React.useState({
    name: "",
    version: "",
    author: "",
    description: "",
    image: null,
    plugin: null,
  });

  const [scopesValue, setScopesValue] = React.useState({
    api: false,
    settings: false,
    contextMenu: false,
    filesContextMenu: false,
    foldersContextMenu: false,
    roomsContextMenu: false,
    mainButton: false,
    profileMenu: false,
  });

  const onChangeName = (e) => {
    return setGeneralValue((v) => ({ ...v, name: e.target.value }));
  };

  const onChangeVersion = (e) => {
    return setGeneralValue((v) => ({ ...v, version: e.target.value }));
  };

  const onChangeAuthor = (e) => {
    return setGeneralValue((v) => ({ ...v, author: e.target.value }));
  };

  const onChangeDescription = (e) => {
    return setGeneralValue((v) => ({
      ...v,
      description: e.target.value,
    }));
  };

  const onChangeImage = (file) => {
    let formData = new FormData();

    formData.append("file", file);

    return setGeneralValue((v) => ({ ...v, image: formData }));
  };

  const onChangePlugin = (file) => {
    let formData = new FormData();

    formData.append("file", file);

    return setGeneralValue((v) => ({ ...v, plugin: formData }));
  };

  const onChangeApi = (e) => {
    return setScopesValue((value) => ({ ...value, api: !value.api }));
  };

  const onChangeSettings = (e) => {
    return setScopesValue((value) => ({ ...value, settings: !value.settings }));
  };

  const onChangeContextMenu = (e) => {
    return setScopesValue((value) => ({
      ...value,
      contextMenu: !value.contextMenu,
      filesContextMenu: !value.contextMenu,
      foldersContextMenu: !value.contextMenu,
      roomsContextMenu: !value.contextMenu,
    }));
  };

  const onChangeFilesContextMenu = (e) => {
    return setScopesValue((value) => ({
      ...value,
      contextMenu:
        !value.filesContextMenu ||
        value.roomsContextMenu ||
        value.foldersContextMenu,
      filesContextMenu: !value.filesContextMenu,
    }));
  };

  const onChangeFoldersContextMenu = (e) => {
    return setScopesValue((value) => ({
      ...value,
      contextMenu:
        value.filesContextMenu ||
        value.roomsContextMenu ||
        !value.foldersContextMenu,
      foldersContextMenu: !value.foldersContextMenu,
    }));
  };

  const onChangeRoomsContextMenu = (e) => {
    return setScopesValue((value) => ({
      ...value,
      contextMenu:
        value.filesContextMenu ||
        !value.roomsContextMenu ||
        value.foldersContextMenu,
      roomsContextMenu: !value.roomsContextMenu,
    }));
  };

  const onChangeMainButton = (e) => {
    return setScopesValue((value) => ({
      ...value,
      mainButton: !value.mainButton,
    }));
  };

  const onChangeProfileMenu = (e) => {
    return setScopesValue((value) => ({
      ...value,
      profileMenu: !value.profileMenu,
    }));
  };

  const onAddPlugin = async () => {
    setIsRequestRunning(true);
    const dto = {};

    dto.name = generalValue.name;
    dto.version = generalValue.version;
    dto.author = generalValue.author;
    dto.description = generalValue.description;
    dto.image = "";
    dto.plugin = "";

    dto.uploader = displayName;

    dto.apiScope = scopesValue.api;
    dto.settingsScope = scopesValue.settings;
    dto.contextMenuScope = scopesValue.contextMenu;
    dto.mainButtonScope = scopesValue.mainButton;
    dto.profileMenuScope = scopesValue.profileMenu;

    const plugin = await api.plugins.addPlugin(dto);

    const actions = [];

    actions.push(await api.plugins.uploadImage(plugin.id, generalValue.image));
    actions.push(
      await api.plugins.uploadPlugin(plugin.id, generalValue.plugin)
    );

    await Promise.all(actions);

    setIsRequestRunning(false);
    onClose();
  };

  return (
    <ModalDialog
      visible={visible}
      displayType="aside"
      onClose={onClose}
      withBodyScroll
    >
      <ModalDialog.Header>{t("AddPlugin")}</ModalDialog.Header>
      <ModalDialog.Body>
        <GeneralBlock
          startValue={generalValue}
          onChangeName={onChangeName}
          onChangeVersion={onChangeVersion}
          onChangeAuthor={onChangeAuthor}
          onChangeDescription={onChangeDescription}
          onChangeImage={onChangeImage}
          onChangePlugin={onChangePlugin}
        />

        <ScopesBlock
          startValue={scopesValue}
          onChangeApi={onChangeApi}
          onChangeSettings={onChangeSettings}
          onChangeContextMenu={onChangeContextMenu}
          onChangeFilesContextMenu={onChangeFilesContextMenu}
          onChangeFoldersContextMenu={onChangeFoldersContextMenu}
          onChangeRoomsContextMenu={onChangeRoomsContextMenu}
          onChangeMainButton={onChangeMainButton}
          onChangeProfileMenu={onChangeProfileMenu}
        />

        <Block />
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          id="add_plugin-submit"
          key="SaveButton"
          label={t("Common:SaveButton")}
          size="normal"
          primary
          onClick={onAddPlugin}
          isDisabled={
            isRequestRunning || !generalValue.name || !generalValue.plugin
          }
          scale
        />
        <Button
          id="add_plugin-cancel"
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onClose}
          scale
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default inject(({ auth }) => {
  const { displayName } = auth.userStore.user;

  return { displayName };
})(observer(AddPluginDialog));
