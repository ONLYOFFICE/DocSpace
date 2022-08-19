import React from "react";
import styled from "styled-components";
import RoomTypeDropdown from "./RoomTypeDropdown";
import ThirdPartyStorage from "./ThirdPartyStorage";

import TextInput from "@docspace/components/text-input";
import Label from "@docspace/components/label";
import TagInput from "./TagInput";
import RoomType from "./RoomType";
import { roomTypes } from "../data";
import IconEditor from "./IconEditor";
import PermanentSettings from "./PermanentSettings";
import ToggleParam from "./Params/ToggleParam";
import InputParam from "./Params/InputParam";
import IsPrivateParam from "./IsPrivateParam";

const StyledSetRoomParams = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;
  gap: 20px;
`;

const SetRoomParams = ({
  t,
  roomParams,
  setRoomParams,
  setRoomType,
  tagHandler,
  setIsScrollLocked,
  isEdit,
  providers,
}) => {
  const onChangeName = (e) =>
    setRoomParams({ ...roomParams, title: e.target.value });

  const onChangeIsPrivate = () =>
    setRoomParams({ ...roomParams, isPrivate: !roomParams.isPrivate });

  // const onChangeThidpartyFolderName = (e) =>
  //   setRoomParams({ ...roomParams, thirdpartyFolderName: e.target.value });

  const onChangeIcon = (icon) => setRoomParams({ ...roomParams, icon: icon });

  const onChangeIsThirdparty = () =>
    setRoomParams({ ...roomParams, isThirdparty: !roomParams.isThirdparty });

  const setChangeStorageLocation = (storageLocation) =>
    setRoomParams({ ...roomParams, storageLocation });

  const onChangeRememberThirdpartyStorage = () =>
    setRoomParams({
      ...roomParams,
      rememberThirdpartyStorage: !roomParams.rememberThirdpartyStorage,
    });

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
          setIsScrollLocked={setIsScrollLocked}
        />
      )}

      {isEdit && (
        <PermanentSettings
          t={t}
          title={roomParams.title}
          isThirdparty={roomParams.isThirdparty}
          storageLocation={roomParams.storageLocation}
          isPrivate={roomParams.isPrivate}
        />
      )}

      <InputParam
        id={"room-name"}
        title={`${t("Common:Name")}:`}
        placeholder={t("NamePlaceholder")}
        value={roomParams.title}
        onChange={onChangeName}
      />

      <TagInput
        t={t}
        tagHandler={tagHandler}
        currentRoomTypeData={currentRoomTypeData}
        setIsScrollLocked={setIsScrollLocked}
      />

      {/* {!isEdit && (
        <IsPrivateParam
          t={t}
          isPrivate={roomParams.isPrivate}
          onChangeIsPrivate={onChangeIsPrivate}
        />
      )} */}

      {!isEdit && (
        <ThirdPartyStorage
          t={t}
          providers={providers}
          roomParams={roomParams}
          isThirdparty={roomParams.isThirdparty}
          onChangeIsThirdparty={onChangeIsThirdparty}
          storageLocation={roomParams.storageLocation}
          setChangeStorageLocation={setChangeStorageLocation}
          rememberThirdpartyStorage={roomParams.rememberThirdpartyStorage}
          onChangeRememberThirdpartyStorage={onChangeRememberThirdpartyStorage}
          setIsScrollLocked={setIsScrollLocked}
        />
      )}

      <IconEditor
        t={t}
        title={roomParams.title}
        tags={roomParams.tags}
        currentRoomTypeData={currentRoomTypeData}
        icon={roomParams.icon}
        onChangeIcon={onChangeIcon}
      />
    </StyledSetRoomParams>
  );
};

export default SetRoomParams;
