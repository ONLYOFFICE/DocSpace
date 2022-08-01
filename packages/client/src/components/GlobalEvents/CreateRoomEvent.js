import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import toastr from "client/toastr";

import { RoomsType } from "@docspace/common/constants";

import Dialog from "./sub-components/Dialog";

const CreateRoomEvent = ({ createRoom, updateCurrentFolder, id, onClose }) => {
  const options = [
    { key: RoomsType.CustomRoom, label: "Custom room" },
    { key: RoomsType.FillingFormsRoom, label: "Filling form room" },
    { key: RoomsType.EditingRoom, label: "Editing room" },
    { key: RoomsType.ReviewRoom, label: "Review room" },
    { key: RoomsType.ReadOnlyRoom, label: "View-only room" },
  ];

  const [selectedOption, setSelectedOption] = React.useState(options[0]);

  const { t } = useTranslation(["Translations", "Common"]);

  const onSelect = (item) => {
    setSelectedOption(item);
  };

  const onSave = (e, value) => {
    createRoom(value, selectedOption.key)
      .then(() => {
        updateCurrentFolder(null, id);
      })
      .finally(() => {
        onClose();
        toastr.success(`${value} success created`);
      });
  };

  return (
    <Dialog
      t={t}
      title={"Create room"}
      startValue={"New room"}
      visible={true}
      options={options}
      selectedOption={selectedOption}
      onSelect={onSelect}
      onSave={onSave}
      onCancel={onClose}
      onClose={onClose}
    />
  );
};

export default inject(
  ({ filesStore, filesActionsStore, selectedFolderStore }) => {
    const { createRoom } = filesStore;

    const { updateCurrentFolder } = filesActionsStore;

    const { id } = selectedFolderStore;

    return { createRoom, updateCurrentFolder, id };
  }
)(observer(CreateRoomEvent));
