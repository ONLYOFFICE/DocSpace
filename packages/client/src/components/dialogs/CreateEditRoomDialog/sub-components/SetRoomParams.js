import React from "react";
import styled from "styled-components";
import RoomTypeDropdown from "./RoomTypeDropdown";
import StorageLocation from "./StorageLocation";

import TextInput from "@docspace/components/text-input";
import Label from "@docspace/components/label";
import ToggleButton from "@docspace/components/toggle-button";
import Text from "@docspace/components/text";
import TagInput from "./TagInput";
import { StyledParam } from "./StyledParam";
import RoomType from "./RoomType";
import { roomTypes } from "../data";
import SecondaryInfoButton from "./SecondaryInfoButton";
import IconEditor from "./IconEditor";

const StyledSetRoomParams = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;
  gap: 20px;

  .set_room_params-input {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }
`;

const StyledIconEditorWrapper = styled.div`
  .use_modal-avatar_editor_body {
    margin: 0;
  }

  .use_modal-buttons_wrapper {
    display: none;
  }
`;

const SetRoomParams = ({
  t,
  roomParams,
  setRoomParams,
  setRoomType,
  tagHandler,
  setIsScrollLocked,
  isEdit,
}) => {
  const onChangeName = (e) =>
    setRoomParams({ ...roomParams, title: e.target.value });

  const onChangeIsPrivate = () =>
    setRoomParams({ ...roomParams, isPrivate: !roomParams.isPrivate });

  const onChangeThidpartyFolderName = (e) =>
    setRoomParams({ ...roomParams, thirdpartyFolderName: e.target.value });

  const [currentRoomTypeData] = roomTypes.filter(
    (room) => room.type === roomParams.type
  );

  return (
    <StyledSetRoomParams>
      {isEdit ? (
        <RoomType t={t} room={currentRoomTypeData} type="displayItem" />
      ) : (
        <RoomTypeDropdown
          t={t}
          currentRoom={currentRoomTypeData}
          setRoomType={setRoomType}
        />
      )}

      <div className="set_room_params-input">
        <Label
          display="display"
          htmlFor="room-name"
          text={`${t("Common:Name")}:`}
          title="Fill the first name field"
        />
        <TextInput
          id="room-name"
          value={roomParams.title}
          onChange={onChangeName}
          scale
          placeholder={t("NamePlaceholder")}
          tabIndex={1}
        />
      </div>

      <TagInput
        t={t}
        tagHandler={tagHandler}
        setIsScrollLocked={setIsScrollLocked}
      />

      <StyledParam isPrivate>
        <div className="set_room_params-info">
          <div className="set_room_params-info-title">
            <Text className="set_room_params-info-title-text">
              {t("MakeRoomPrivateTitle")}
            </Text>
            <SecondaryInfoButton content={t("MakeRoomPrivateSecondaryInfo")} />
          </div>
          <div className="set_room_params-info-description">
            {t("MakeRoomPrivateDescription")}
          </div>
        </div>
        <ToggleButton
          className="set_room_params-toggle"
          isChecked={roomParams.isPrivate}
          onChange={onChangeIsPrivate}
        />
      </StyledParam>

      <StorageLocation
        t={t}
        roomParams={roomParams}
        setRoomParams={setRoomParams}
        setIsScrollLocked={setIsScrollLocked}
      />

      <StyledParam folderName>
        <div className="set_room_params-info">
          <div className="set_room_params-info-title">
            <Text className="set_room_params-info-title-text">
              {`${t("FolderNameTitle")}:`}
            </Text>
          </div>
          <div className="set_room_params-info-description">
            {t("FolderNameDescription")}
          </div>
        </div>

        <div className="set_room_params-input">
          <TextInput
            id="room-folder-title"
            scale
            value={roomParams.thirdpartyFolderName}
            onChange={onChangeThidpartyFolderName}
            placeholder={`${
              roomParams.storageLocation
                ? roomParams.storageLocation.title + " - "
                : ""
            }${t("Home:NewRoom")}`}
            tabIndex={1}
          />
        </div>
      </StyledParam>

      <IconEditor />
    </StyledSetRoomParams>
  );
};

export default SetRoomParams;
