import React, { useState, useEffect, useCallback, memo } from "react";

import { FileAction } from "@docspace/common/constants";
import { Events } from "@docspace/common/constants";

import CreateEvent from "./CreateEvent";
import RenameEvent from "./RenameEvent";
import CreateRoomEvent from "./CreateRoomEvent";
import EditRoomEvent from "./EditRoomEvent";
import ChangeUserTypeEvent from "./ChangeUserTypeEvent";

const GlobalEvents = () => {
  const [createDialogProps, setCreateDialogProps] = useState({
    visible: false,
    id: null,
    type: null,
    extension: null,
    title: "",
    templateId: null,
    fromTemplate: null,
    onClose: null,
  });

  const [renameDialogProps, setRenameDialogProps] = useState({
    visible: false,
    item: null,
    onClose: null,
  });

  const [createRoomDialogProps, setCreateRoomDialogProps] = useState({
    visible: false,
    onClose: null,
  });

  const [editRoomDialogProps, setEditRoomDialogProps] = useState({
    visible: false,
    item: null,
    onClose: null,
  });

  const [changeUserTypeDialog, setChangeUserTypeDialogProps] = useState({
    visible: false,
    onClose: null,
  });

  const onCreate = useCallback((e) => {
    const { payload } = e;

    const visible = payload.id ? true : false;

    setCreateDialogProps({
      visible: visible,
      id: payload.id,
      type: FileAction.Create,
      extension: payload.extension,
      title: payload.title || null,
      templateId: payload.templateId || null,
      fromTemplate: payload.fromTemplate || null,
      onClose: () => {
        setCreateDialogProps({
          visible: false,
          id: null,
          type: null,
          extension: null,
          title: "",
          templateId: null,
          fromTemplate: null,
          onClose: null,
        });
      },
    });
  }, []);

  const onRename = useCallback((e) => {
    const visible = e.item ? true : false;

    setRenameDialogProps({
      visible: visible,
      type: FileAction.Rename,
      item: e.item,
      onClose: () => {
        setRenameDialogProps({
          visible: false,
          type: null,
          item: null,
        });
      },
    });
  }, []);

  const onCreateRoom = useCallback((e) => {
    setCreateRoomDialogProps({
      visible: true,
      onClose: () =>
        setCreateRoomDialogProps({ visible: false, onClose: null }),
    });
  }, []);

  const onEditRoom = useCallback((e) => {
    const visible = e.item ? true : false;

    setEditRoomDialogProps({
      visible: visible,
      item: e.item,
      onClose: () => {
        setEditRoomDialogProps({
          visible: false,
          item: null,
          onClose: null,
        });
      },
    });
  }, []);

  const onChangeUserType = useCallback((e) => {
    setChangeUserTypeDialogProps({
      visible: true,
      onClose: () =>
        setChangeUserTypeDialogProps({ visible: false, onClose: null }),
    });
  }, []);

  useEffect(() => {
    window.addEventListener(Events.CREATE, onCreate);
    window.addEventListener(Events.RENAME, onRename);
    window.addEventListener(Events.ROOM_CREATE, onCreateRoom);
    window.addEventListener(Events.ROOM_EDIT, onEditRoom);
    window.addEventListener(Events.CHANGE_USER_TYPE, onChangeUserType);

    return () => {
      window.removeEventListener(Events.CREATE, onCreate);
      window.removeEventListener(Events.RENAME, onRename);
      window.removeEventListener(Events.ROOM_CREATE, onCreateRoom);
      window.removeEventListener(Events.ROOM_EDIT, onEditRoom);
      window.removeEventListener(Events.CHANGE_USER_TYPE, onChangeUserType);
    };
  }, [onRename, onCreate, onCreateRoom, onEditRoom, onChangeUserType]);

  return [
    createDialogProps.visible && (
      <CreateEvent key={Events.CREATE} {...createDialogProps} />
    ),
    renameDialogProps.visible && (
      <RenameEvent key={Events.RENAME} {...renameDialogProps} />
    ),
    createRoomDialogProps.visible && (
      <CreateRoomEvent key={Events.ROOM_CREATE} {...createRoomDialogProps} />
    ),
    editRoomDialogProps.visible && (
      <EditRoomEvent key={Events.ROOM_EDIT} {...editRoomDialogProps} />
    ),
    changeUserTypeDialog.visible && (
      <ChangeUserTypeEvent
        key={Events.CHANGE_USER_TYPE}
        {...changeUserTypeDialog}
      />
    ),
  ];
};

export default memo(GlobalEvents);
