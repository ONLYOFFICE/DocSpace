import React from "react";
import styled from "styled-components";

import { roomTypes } from "../data";
import RoomTypeDropdown from "./RoomTypeDropdown";
import ThirdPartyStorage from "./ThirdPartyStorage";
import TagInput from "./TagInput";
import RoomType from "./RoomType";
import IconEditor from "./IconEditor";
import PermanentSettings from "./PermanentSettings";
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
  setIsOauthWindowOpen,
  setRoomType,
  tagHandler,
  setIsScrollLocked,
  isEdit,
  connectItems,
  setConnectDialogVisible,
  setRoomCreation,
  saveThirdpartyResponse,
  openConnectWindow,
  setConnectItem,
  getOAuthToken,
}) => {
  const onChangeName = (e) => {
    // let value = e.target.value;
    // value = value.replace("/", "");
    // value = value.replace("\\", "");

    // const storageFolderPath = roomParams.storageLocation.storageFolderPath;
    // const pathArr = storageFolderPath.split("/");
    // const folderName = pathArr.pop();

    // if (roomParams.title === folderName)
    //   setRoomParams({
    //     ...roomParams,
    //     title: value,
    //     storageLocation: {
    //       ...roomParams.storageLocation,
    //       storageFolderPath:
    //         pathArr.join("/") + (!!pathArr.length ? "/" : "") + value,
    //     },
    //   });
    // else

    setRoomParams({ ...roomParams, title: e.target.value });
  };

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
      )}

      {!isEdit && (
        <ThirdPartyStorage
          t={t}
          connectItems={connectItems}
          setConnectDialogVisible={setConnectDialogVisible}
          setRoomCreation={setRoomCreation}
          saveThirdpartyResponse={saveThirdpartyResponse}
          openConnectWindow={openConnectWindow}
          setConnectItem={setConnectItem}
          getOAuthToken={getOAuthToken}
          roomParams={roomParams}
          isThirdparty={roomParams.isThirdparty}
          onChangeIsThirdparty={onChangeIsThirdparty}
          storageLocation={roomParams.storageLocation}
          setChangeStorageLocation={setChangeStorageLocation}
          rememberThirdpartyStorage={roomParams.rememberThirdpartyStorage}
          onChangeRememberThirdpartyStorage={onChangeRememberThirdpartyStorage}
          setIsScrollLocked={setIsScrollLocked}
          setIsOauthWindowOpen={setIsOauthWindowOpen}
        />
      )} */}

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
