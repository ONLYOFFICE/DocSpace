import React, { useState } from "react";

import TagHandler from "./handlers/tagHandler";
import SetRoomParams from "./sub-components/SetRoomParams";
import DialogHeader from "./sub-components/DialogHeader";

import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";

const EditRoomDialog = ({
  t,
  visible,
  onClose,
  onSave,
  isLoading,
  fetchedRoomParams,
  fetchedTags,
  folderFormValidation,
}) => {
  const [isScrollLocked, setIsScrollLocked] = useState(false);

  const [roomParams, setRoomParams] = useState({ ...fetchedRoomParams });

  const setRoomTags = (newTags) =>
    setRoomParams({ ...roomParams, tags: newTags });

  const tagHandler = new TagHandler(roomParams.tags, setRoomTags, fetchedTags);

  const setRoomType = (newRoomType) =>
    setRoomParams((prev) => ({
      ...prev,
      type: newRoomType,
    }));

  const onEditRoom = () => onSave(roomParams);

  // useEffect(async () => {
  //   console.log(fetchedRoomParams.uploadedFileSrc);
  //   if (fetchedRoomParams.uploadedFileSrc)
  //     await fetch(
  //       "http://192.168.0.100:8092/storage/room_logos/root/sbox9DOCSPACE%20CUSTOM%20ROOM%209_orig_887-339.jpeg"
  //     ).then((res) => {
  //       const buf = res.arrayBuffer();
  //       const file = new File([buf], "fetchedImage", { type: "image/png" });
  //       console.log(file);
  //       setRoomParams({
  //         ...roomParams,
  //         icon: { ...roomParams.icon, uploadedFile: file },
  //       });
  //     });
  // }, []);

  return (
    <ModalDialog
      displayType="aside"
      withBodyScroll
      visible={visible}
      onClose={onClose}
      isScrollLocked={isScrollLocked}
      withFooterBorder
    >
      <ModalDialog.Header>
        <DialogHeader isEdit />
      </ModalDialog.Header>

      <ModalDialog.Body>
        <SetRoomParams
          t={t}
          tagHandler={tagHandler}
          roomParams={roomParams}
          setRoomParams={setRoomParams}
          setRoomType={setRoomType}
          setIsScrollLocked={setIsScrollLocked}
          isEdit
        />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Button
          tabIndex={5}
          label={t("Common:SaveButton")}
          size="normal"
          primary
          scale
          onClick={onEditRoom}
          isLoading={isLoading}
        />
        <Button
          tabIndex={5}
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default EditRoomDialog;
